using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class AccessToken {

        [Key]
        public Guid Id { get; set; }

        public string Token { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public AccessToken() {
            Id = Guid.Empty;

            Token = string.Empty;
            Name = string.Empty;

            CreateDate = DateTime.MinValue;
        }

        public AccessToken(string accessToken, string name) {
            Id = Guid.NewGuid();

            Token = accessToken;
            Name = name;

            CreateDate = DateTime.Now;
        }
    }
}
