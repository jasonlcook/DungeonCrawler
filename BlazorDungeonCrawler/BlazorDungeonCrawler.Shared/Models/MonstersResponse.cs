using System.Collections.Generic;

namespace BlazorDungeonCrawler.Shared.Models {
    //Response for list of monsters
    public class MonstersResponse {
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public List<Monster> Monsters { get; set; } = new List<Monster>();
    }
}
