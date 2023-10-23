using Microsoft.AspNetCore.Components;

namespace BlazorDungeonCrawler.Client.Pages
{
    public partial class AdvanceDungeon {
        [Parameter]
        public bool ButtonState { get; set; }        

        [Parameter]
        public EventCallback OnCallClickFunction { get; set; }

        public AdvanceDungeon() {
            ButtonState = false;

            OnCallClickFunction = new();
        }
    }
}