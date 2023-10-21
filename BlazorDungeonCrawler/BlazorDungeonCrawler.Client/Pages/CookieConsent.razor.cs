using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorDungeonCrawler.Client.Pages {
    public partial class CookieConsent {
        [ParameterAttribute]
        public bool? FoundCookie { get; set; }

        [ParameterAttribute]
        public bool RejectedCookie { get; set; }

        [ParameterAttribute]
        public Guid DungeonId { get; set; }

        private readonly string cookieKeyId = "BlazorWebAppCookies-Id";

        public CookieConsent() {
            FoundCookie = null;
            RejectedCookie = false;

            DungeonId = Guid.Empty;
        }

        private async void AcceptCookies() {
            Guid dungeonId = Guid.Empty;

            if (DungeonId != null && DungeonId != Guid.Empty) {
                dungeonId = DungeonId;
            }

            StoreCookie(dungeonId);

            await InvokeAsync(StateHasChanged);
        }

        private async void RejectCookies() {
            RejectedCookie = true;

            await InvokeAsync(StateHasChanged);
        }

        private async void StoreCookie(Guid dungeonId) {
            string safeDungeonId = dungeonId.ToString();
            string cookie = BakeCookie(cookieKeyId, safeDungeonId, 7);
            await SetCookie(cookie);
        }

        private string BakeCookie(string key, string value, double days) {
            string dateStamp = "";
            if (days > 0) {
                dateStamp = DateTime.Now.AddDays(days).ToUniversalTime().ToString("R");
            }

            return $"{key}={value}; expires={dateStamp}; path=/";
        }

        private async Task SetCookie(string cookie) {
            await JS.InvokeVoidAsync("eval", $"document.cookie = \"{cookie}\"");
        }
    }
}