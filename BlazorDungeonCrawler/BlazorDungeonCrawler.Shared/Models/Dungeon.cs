using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Dungeon {
        //Id for the Dungeon session 
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Adventurer Adventurer { get; set; } = new Adventurer();

        public Level Level { get; set; } = new Level();

        public bool MacGuffinFound { get; set; } = false;

        [ForeignKey("Adventurer")]
        public Guid AdventurerId { get; set; }

        [ForeignKey("Level")]
        public Guid LevelId { get; set; }
    }
}