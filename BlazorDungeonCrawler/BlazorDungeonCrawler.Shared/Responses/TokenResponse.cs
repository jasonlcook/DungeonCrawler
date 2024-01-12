namespace BlazorDungeonCrawler.Shared.Responses {
    public class TokenResponse {
        public bool Success { get; set; }
        public string Name { get; set; }
        public string ErrorMessages { get; set; }

        public TokenResponse() {
            Success = false;
            Name = string.Empty;
            ErrorMessages = string.Empty;
        }
    }
}
