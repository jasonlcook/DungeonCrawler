using Microsoft.AspNetCore.Components;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Client.Pages {
    public partial class Logs {

        [ParameterAttribute]
        public List<Message> Messages { get; set; }

        [Parameter]
        public EventCallback<(string, string)> OnMouseEnterCallback { get; set; }

        public Logs() {
            Messages = new();
        }

        private async Task CallParentUpdateDiceFunction(string safeDice, string dangerDice) {
            await OnMouseEnterCallback.InvokeAsync((safeDice, dangerDice));
        }
    }
}