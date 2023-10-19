using Microsoft.AspNetCore.Components;

namespace BlazorDungeonCrawler.Client.Pages
{
    public partial class Message {

        [ParameterAttribute]
        public List<string> ErrorMessages { get; set; } = new();

        [ParameterAttribute]
        public List<string> InfoMessages { get; set; } = new();
    }
}