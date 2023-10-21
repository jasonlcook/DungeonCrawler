using Microsoft.AspNetCore.Components;

namespace BlazorDungeonCrawler.Client.Pages
{
    public partial class ActionDialog {
        [Parameter]
        public string DialogText { get; set; }        


        [Parameter]
        public string AcceptLabel { get; set; }

        [Parameter]
        public string RejectLabel { get; set; }


        [Parameter]
        public EventCallback OnCallAcceptFunction { get; set; }

        [Parameter]
        public EventCallback OnCallRejectFunction { get; set; }

        public ActionDialog() {
            DialogText = string.Empty;

            AcceptLabel = string.Empty;
            RejectLabel = string.Empty;

            OnCallAcceptFunction = new();
            OnCallRejectFunction = new();
        }
    }
}