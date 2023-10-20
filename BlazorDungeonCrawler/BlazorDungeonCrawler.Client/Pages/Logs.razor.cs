using Microsoft.AspNetCore.Components;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Client.Pages
{
    public partial class Logs {

        [ParameterAttribute]
        public List<Message> Messages { get; set; } = new();

        private void ShowLogActions(List<int> dice) {
            
        }

        private void HideLogActions() {

        }
    }
}