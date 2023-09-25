using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Dungeon {
        //If Id is an empty guid, then class has not been set
        [Key]
        public Guid Id { get; set; } = new Guid();

        public Adventurer Adventurer { get; set; } = new Adventurer();

        public Level Level { get; set; } = new Level();

        public bool MacGuffinFound { get; set; } = false;
        
        [NotMapped]
        public Version ApiVersion { get; set; } = new Version(0, 0, 0);

        [ForeignKey("Adventurer")]
        public Guid AdventurerId { get; set; }

        [ForeignKey("Level")]
        public Guid LevelId { get; set; }
    }
}