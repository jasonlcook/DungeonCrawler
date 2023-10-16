using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Monster {
        [Key]
        public Guid Id { get; set; }

        public int Index { get; set; }

        public string TypeName { get; set; }


        public int Health { get; set; }
        public int Damage { get; set; }
        public int Protection { get; set; }


        public int Experience { get; set; }


        public int ClientX { get; set; }
        public int ClientY { get; set; }

        public Monster() {
            Id = Guid.Empty;
            TypeName = string.Empty;
        }
    }
}