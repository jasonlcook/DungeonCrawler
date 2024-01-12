using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorDungeonCrawler.Shared.Models {
    public class AccessTokenLog {

        [Key]
        public Guid Id { get; set; }

        public DateTime DateAccessed { get; set; }

        [ForeignKey("AccessToken")]
        public Guid AccessTokenId { get; set; }

        public AccessTokenLog() {
            Id = Guid.NewGuid();
            DateAccessed = DateTime.Now;
        }
    }
}
