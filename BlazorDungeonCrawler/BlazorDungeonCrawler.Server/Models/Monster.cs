using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Monster {
        //attributes
        public Guid Id { get; set; }
        public int Index { get; set; }
        public string TypeName { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public int Protection { get; set; }
        public int ClientX { get; set; }
        public int ClientY { get; set; }

        //constructors
        public Monster() {
            Id = Guid.NewGuid();
            TypeName = string.Empty;
        }

        public Monster(SharedMonster monster) {
            Id = monster.Id;
            Index = monster.Index;
            TypeName = monster.TypeName;
            Health = monster.Health;
            Damage = monster.Damage;
            Protection = monster.Protection;
            Experience = monster.Experience;
            ClientX = monster.ClientX;
            ClientY = monster.ClientY;
        }

        //mapping
        public SharedMonster SharedModelMapper() {
            return new SharedMonster() {
                Id = this.Id,
                Index = this.Index,
                TypeName = this.TypeName,
                Health = this.Health,
                Damage = this.Damage,
                Protection = this.Protection,
                Experience = this.Experience,
                ClientX = this.ClientX,
                ClientY = this.ClientY
            };
        }

        //operation
        public Monster ServerModelMapper(SharedMonster monster) {
            return new Monster(monster);
        }
    }
}