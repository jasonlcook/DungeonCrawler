using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Monster {
        public Guid Id { get; set; }
        public string TypeName { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public int Protection { get; set; }
        public int ClientX { get; set; }
        public int ClientY { get; set; }

        public Monster() {
            Id = Guid.NewGuid();
            TypeName = string.Empty;
        }

        public SharedMonster SharedModelMapper() {
            return new SharedMonster() {
                Id = this.Id,
                TypeName = this.TypeName,
                Health = this.Health,
                Damage = this.Damage,
                Protection = this.Protection,
                ClientX = this.ClientX,
                ClientY = this.ClientY
            };
        }

        public Monster ServerModelMapper(SharedMonster monster) {
            return new Monster() {
                Id = monster.Id,
                TypeName = monster.TypeName,
                Health = monster.Health,
                Damage = monster.Damage,
                Protection = monster.Protection,
                ClientX = monster.ClientX,
                ClientY = monster.ClientY
            };
        }
    }
}