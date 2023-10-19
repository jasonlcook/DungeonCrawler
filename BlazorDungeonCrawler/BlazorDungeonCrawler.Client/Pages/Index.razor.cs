using Microsoft.JSInterop;

namespace BlazorDungeonCrawler.Client.Pages
{
    public partial class Index
    {
        //Generic
        public async Task WriteLog(string message) {
            await this.JS.InvokeVoidAsync("console.log", message);
        }

        //Cookies
        bool? foundCookie = null;
        string cookieKeyId = "BlazorWebAppCookies-Id";

        Guid cookieId = Guid.Empty;

        protected override void OnInitialized() {
            if (foundCookie == null) {
                CheckCookies();
            }
        }

        //  Retrieve
        private async void CheckCookies() {
            string response = GetCookie();
            Dictionary<string, string> cookies = ParseCookieResponse(response);

            if (cookies.ContainsKey(cookieKeyId)) {
                foundCookie = true;

                if (Guid.TryParse(cookies[cookieKeyId], out cookieId)) {
                    await WriteLog($"COOKIEID: {cookieId}");
                }
            } else {
                foundCookie = false;
            }
        }

        private string GetCookie() {
            return ((IJSInProcessRuntime)JS).Invoke<string>("eval", $"document.cookie");
        }

        private Dictionary<string, string> ParseCookieResponse(string value) {
            Dictionary<string, string> cookies = new();

            if (!string.IsNullOrEmpty(value)) {
                string[] values = value.Split(';');

                string cookieKey, cookieValue;
                foreach (string val in values) {
                    cookieKey = val.Substring(0, val.IndexOf('=')).Trim();
                    cookieValue = val.Substring(val.IndexOf('=') + 1);
                    cookies.Add(cookieKey, cookieValue);
                }
            }

            return cookies;
        }
    }
}