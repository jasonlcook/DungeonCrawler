using Microsoft.AspNetCore.Components;

namespace BlazorDungeonCrawler.Client.Pages {
    public class Attribute {
        public string Label { get; set; } 
        public string Value { get; set; } 
        public string? Duration { get; set; } 

        public Attribute() {
            Label = "0";
            Value = "0";
        }
    }

    public partial class AdventurerAttribute {
        [ParameterAttribute]
        public List<Attribute> Attributes { get; set; } = new();
    }
}