using Microsoft.JSInterop;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Client.Pages {
    public partial class Index {
        public string ApiVersion { get; set; } = "API V0.0.0";

        //Page elements
        List<string> errorMessages = new();
        List<string> infoMessages = new();        

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

        //Dungeon
        Dungeon dungeon = new Dungeon();
        Floor floor = new Floor();
        Adventurer adventurer = new Adventurer();

        protected override async void OnAfterRender(bool firstRender) {
            infoMessages.Add("INFO MESSAGE TEST");
            errorMessages.Add("INFO MESSAGE TEST");

            if (dungeon == null || dungeon.Id == Guid.Empty) {
                try {
                    //Get Dungeon
                    Dungeon? _dungeon = await DungeonManager.GenerateNewDungeon();

                    //Check and assign Dungeon
                    if (_dungeon == null || _dungeon.Id == Guid.Empty) { throw new ArgumentNullException("Dungon"); };
                    dungeon = _dungeon;

                    //Check and assign Dungeon Floor
                    if (_dungeon.Floors == null || _dungeon.Floors.Count == 0) { throw new ArgumentNullException("Dungon Floors"); };
                    Floor? _floor = _dungeon.Floors.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
                    if (_floor == null || _floor.Id == Guid.Empty) { throw new ArgumentNullException("Dungon Floor"); };
                    floor = _floor;

                    //Check and assign Dungeon Floor
                    if (_dungeon.Adventurer == null || _dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
                    adventurer = _dungeon.Adventurer;

                    ApiVersion = $"API V{dungeon.ApiVersion}";

                    await InvokeAsync(StateHasChanged);
                } catch (Exception ex) {
                    errorMessages.Add(ex.Message);
                }
            }
        }
    }
}