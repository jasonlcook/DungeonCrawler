using BlazorDungeonCrawler.Server.Data;

using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Monsters {
        private List<Monster> _monsters = new List<Monster>();

        public Monsters() { }

        public void Generate(int depth) {
            MonsterTypes monsterTypes = new();
            List<MonsterType> availableMonsters = monsterTypes.GetMonstersAtDepth(depth);

            if (availableMonsters.Count > 0) {
                int currentMonsterTypeIndex = Dice.RandomNumber(0, availableMonsters.Count - 1);
                MonsterType currentMonsterType = availableMonsters[currentMonsterTypeIndex];

                if (currentMonsterType.Name != string.Empty) {
                    int monsterGroup = 1, health = 0, damage = 0, protection = 0, rollValue;
                    List<int> healthDice = new(), damageDice = new(), protectionDice = new();

                    for (int i = 0; i < monsterGroup; i++) {
                        Monster monster = new Monster() {
                            Id = Guid.NewGuid(),
                            TypeName = currentMonsterType.Name
                        };

                        for (int h = 0; h < currentMonsterType.HealthDiceCount; h++) {
                            rollValue = Dice.RollDSix();
                            healthDice.Add(rollValue);
                            health += rollValue;
                        }
                        monster.Health = health;

                        for (int d = 0; d < currentMonsterType.DamageDiceCount; d++) {
                            rollValue = Dice.RollDSix();
                            damageDice.Add(rollValue);
                            damage += rollValue;
                        }
                        monster.Damage = damage;

                        for (int p = 0; p < currentMonsterType.DamageDiceCount; p++) {
                            rollValue = Dice.RollDSix();
                            protectionDice.Add(rollValue);
                            protection += rollValue;
                        }
                        monster.Protection = protection;

                        monster.ClientX = Dice.RandomNumber(20, 50);
                        monster.ClientY = Dice.RandomNumber(20, 50);

                        _monsters.Add(monster);
                    }
                }
            } else {
                //todo log error
            }
        }

        public int Count() {
            return _monsters.Count();
        }

        public List<SharedMonster> SharedModelMapper() {
            List<SharedMonster> sharedMonsters = new();

            foreach (var monster in _monsters) {
                SharedMonster sharedMonster = new() {
                    Id = monster.Id,
                    TypeName = monster.TypeName,
                    Health = monster.Health,
                    Damage = monster.Damage,
                    Protection = monster.Protection,
                    ClientX = monster.ClientX,
                    ClientY = monster.ClientY
                };

                sharedMonsters.Add(sharedMonster);
            }

            return sharedMonsters;
        }

        public List<Monster> Get() {
            return _monsters;
        }
    }
}
