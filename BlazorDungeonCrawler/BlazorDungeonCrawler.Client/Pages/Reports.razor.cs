using Microsoft.AspNetCore.Components;

namespace BlazorDungeonCrawler.Client.Pages {
    public partial class Reports {

        [ParameterAttribute]
        public List<string> ErrorReports { get; set; }

        [ParameterAttribute]
        public List<string> InformationReports { get; set; }

        public Reports() {
            ErrorReports = new();
            InformationReports = new();
        }
    }
}