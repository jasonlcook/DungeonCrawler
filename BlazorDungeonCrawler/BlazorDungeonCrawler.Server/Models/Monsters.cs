using BlazorDungeonCrawler.Server.Data;

using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Monsters {
        private string _name = string.Empty;
        private List<Monster> _monsters = new List<Monster>();

        public Monsters() { }

        public void Generate(int depth) {
            MonsterTypes monsterTypes = new();
            List<MonsterType> availableMonsters = monsterTypes.GetMonstersAtDepth(depth);

            if (availableMonsters.Count > 0) {
                int currentMonsterTypeIndex = Dice.RandomNumber(0, availableMonsters.Count - 1);
                MonsterType currentMonsterType = availableMonsters[currentMonsterTypeIndex];

                if (currentMonsterType.Name != string.Empty) {
                    _name = currentMonsterType.Name;

                    int packCount = 1;
                    if (currentMonsterType.MaxPackNumber > 1) {
                        packCount = Dice.RandomNumber(1, currentMonsterType.MaxPackNumber);
                    }

                    int health, damage, protection, rollValue;
                    List<int> healthDice = new(), damageDice = new(), protectionDice = new();
                    
                    for (int i = 0; i < packCount; i++) {
                        health = 0;
                        damage = 0;
                        protection = 0;
                        Monster monster = new Monster() {
                            Id = Guid.NewGuid(),
                            Index = i,
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

                        _monsters.Add(monster);
                    }

                    switch (packCount) {
                        case 1:
                            _monsters[0].ClientX = randomisePlacement(38);
                            _monsters[0].ClientY = randomisePlacement(34);
                            break;
                        case 2:
                            _monsters[0].ClientX = randomisePlacement(20);
                            _monsters[0].ClientY = randomisePlacement(32);

                            _monsters[1].ClientX = randomisePlacement(56);
                            _monsters[1].ClientY = randomisePlacement(32);
                            break;
                        case 3:
                            _monsters[0].ClientX = randomisePlacement(38);
                            _monsters[0].ClientY = randomisePlacement(18);

                            _monsters[1].ClientX = randomisePlacement(20);
                            _monsters[1].ClientY = randomisePlacement(44);

                            _monsters[2].ClientX = randomisePlacement(55);
                            _monsters[2].ClientY = randomisePlacement(44);
                            break;
                        case 4:
                            _monsters[0].ClientX = randomisePlacement(20);
                            _monsters[0].ClientY = randomisePlacement(18);

                            _monsters[1].ClientX = randomisePlacement(56);
                            _monsters[1].ClientY = randomisePlacement(18);

                            _monsters[2].ClientX = randomisePlacement(20);
                            _monsters[2].ClientY = randomisePlacement(44);

                            _monsters[3].ClientX = randomisePlacement(56);
                            _monsters[3].ClientY = randomisePlacement(44);
                            break;
                        case 5:
                            _monsters[0].ClientX = randomisePlacement(38);
                            _monsters[0].ClientY = randomisePlacement(34);

                            _monsters[1].ClientX = randomisePlacement(18);
                            _monsters[1].ClientY = randomisePlacement(16);

                            _monsters[2].ClientX = randomisePlacement(58);
                            _monsters[2].ClientY = randomisePlacement(16);

                            _monsters[3].ClientX = randomisePlacement(18);
                            _monsters[3].ClientY = randomisePlacement(50);

                            _monsters[4].ClientX = randomisePlacement(57);
                            _monsters[4].ClientY = randomisePlacement(50);
                            break;
                        default:
                            foreach (var monster in _monsters) {
                                monster.ClientX = Dice.RandomNumber(20, 50);
                                monster.ClientY = Dice.RandomNumber(20, 50);
                            }
                            break;
                    }
                }
            } else {
                //todo log error
            }
        }

        public int randomisePlacement(int middle) {
            int randomisationFactor = 5;
            int min = middle - randomisationFactor;
            int max = middle + randomisationFactor;
            
            return Dice.RandomNumber(min, max);
        }

        public int Count() {
            return _monsters.Count();
        }

        public List<SharedMonster> SharedModelMapper() {
            List<SharedMonster> sharedMonsters = new();

            foreach (var monster in _monsters) {
                sharedMonsters.Add(monster.SharedModelMapper());
            }

            return sharedMonsters;
        }

        public List<Monster> Get() {
            return _monsters;
        }

        public string GetName() {
            return _name;
        }
    }
}
