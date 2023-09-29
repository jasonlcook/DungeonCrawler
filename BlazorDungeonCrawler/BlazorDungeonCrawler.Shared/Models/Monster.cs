using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Monster {
        [Key]
        public Guid Id { get; set; } = Guid.Empty;


        public string TypeName { get; set; } = "";


        public int Health { get; set; } = 0;
        public int Damage { get; set; } = 0;
        public int Protection { get; set; } = 0;

        public int ClientX { get; set; } = 0;
        public int ClientY { get; set; } = 0;
    }
}