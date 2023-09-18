using System.Collections.Generic;

namespace BlazorDungeonCrawler.Shared.Models {
    //Response for Single monster
    public class MonsterResponse {
        public bool Success { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public Monster Monster { get; set; } = new Monster();
    }

}
