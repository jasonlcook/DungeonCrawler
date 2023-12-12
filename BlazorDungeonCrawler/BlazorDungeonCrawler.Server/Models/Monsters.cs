//**********************************************************************************************************************
//  Monsters
//  The pack of monsters generated to populate a fight tile.

using BlazorDungeonCrawler.Server.Data;

using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Models {
    public class Monsters {
        //****************************
        //***************** Attributes
        private readonly string _name;                      //Name of the type of monsters in the pack

        private readonly List<Monster> _monsters;           //Collection of monsters

        //****************************
        //*************** Constructors
        public Monsters() {
            _name = string.Empty;
            _monsters = new();
        }

        public Monsters(int depth) {
            _monsters = new();

            MonsterTypes monsterTypes = new();
            List<MonsterType> availableMonsters = monsterTypes.GetMonstersAtDepth(depth);

            if (availableMonsters.Count <= 0) { throw new Exception($"No available Monsters returned for floor {depth}."); }

            int currentMonsterTypeIndex = Dice.RandomNumber(0, availableMonsters.Count - 1);
            MonsterType currentMonsterType = availableMonsters[currentMonsterTypeIndex];

            if (currentMonsterType.Name == string.Empty) { throw new ArgumentNullException("Available Monsters"); }
            _name = currentMonsterType.Name;

            int packCount = 1;
            if (currentMonsterType.MaxPackNumber > 1) {
                packCount = Dice.RandomNumber(1, currentMonsterType.MaxPackNumber);
            }

            int dexterityValue, healthValue, damageValue, protectionValue, rollValue;
            List<int> dexterityDice = new(), healthDice = new(), damageDice = new(), protectionDice = new();

            for (int i = 0; i < packCount; i++) {
                dexterityValue = 0;
                healthValue = 0;
                damageValue = 0;
                protectionValue = 0;

                Monster monster = new Monster() {
                    Id = Guid.NewGuid(),
                    Index = i,
                    TypeName = currentMonsterType.Name
                };

                for (int dex = 0; dex < currentMonsterType.DexterityDiceCount; dex++) {
                    rollValue = Dice.RollDSix();
                    dexterityDice.Add(rollValue);
                    dexterityValue += rollValue;
                }
                monster.Dexterity = dexterityValue;

                for (int h = 0; h < currentMonsterType.HealthDiceCount; h++) {
                    rollValue = Dice.RollDSix();
                    healthDice.Add(rollValue);
                    healthValue += rollValue;
                }
                monster.Health = healthValue;

                for (int dam = 0; dam < currentMonsterType.DamageDiceCount; dam++) {
                    rollValue = Dice.RollDSix();
                    damageDice.Add(rollValue);
                    damageValue += rollValue;
                }
                monster.Damage = damageValue;

                for (int pro = 0; pro < currentMonsterType.ProtectionDiceCount; pro++) {
                    rollValue = Dice.RollDSix();
                    protectionDice.Add(rollValue);
                    protectionValue += rollValue;
                }
                monster.Protection = protectionValue;

                monster.Experience = (monster.Dexterity + monster.Health + monster.Damage + monster.Protection) / 2;

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

        //******************** Mapping
        public List<SharedMonster> SharedModelMapper() {
            List<SharedMonster> sharedMonsters = new();

            foreach (var monster in _monsters) {
                sharedMonsters.Add(monster.SharedModelMapper());
            }

            return sharedMonsters;
        }

        //****************************
        //****************** Operation
        //  Return current monsters pack
        public List<Monster> Get() {
            return _monsters;
        }

        //  Return the amount of monster in pack
        public int Count() {
            return _monsters.Count();
        }

        //  Return monster pack type name
        public string GetName() {
            return _name;
        }

        //  Randomly offest positions of monster counters
        public int randomisePlacement(int middle) {
            int randomisationFactor = 5;
            int min = middle - randomisationFactor;
            int max = middle + randomisationFactor;

            return Dice.RandomNumber(min, max);
        }
    }
}
