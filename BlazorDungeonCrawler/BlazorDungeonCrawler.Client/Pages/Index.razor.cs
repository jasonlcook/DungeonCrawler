using BlazorDungeonCrawler.Shared.Responses;

namespace BlazorDungeonCrawler.Client.Pages {

    public partial class Index {

        public List<string> ErrorReports { get; set; }
        public List<string> InformationReports { get; set; }

        private string UserToken { get; set; }

        public bool ButtonDisabled { get; set; }

        public Index() {
            ErrorReports = new();
            InformationReports = new();

            UserToken = string.Empty;
            ButtonDisabled = false;
        }

        private async Task SubmitToken() {
            ErrorReports = new();
            InformationReports = new();

            try {
                ButtonDisabled = true;

                try {
                    TokenResponse tokenResponse = await AccessToken.RetrieveAccessToken(UserToken);
                    if (tokenResponse.Success) {
                        InformationReports.Add("Yes, thats the one");
                    } else {
                        ErrorReports.Add("Token not found");
                    }

                    ButtonDisabled = false;
                } catch (Exception ex) {
                    ErrorReports.Add(ex.Message);
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }

        }
    }
}