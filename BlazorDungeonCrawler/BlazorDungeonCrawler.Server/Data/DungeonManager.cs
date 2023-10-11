using BlazorDungeonCrawler.Server.Models;
using BlazorDungeonCrawler.Shared.Enumerators;

using BlazorDungeonCrawler.Database.Resources.Commands.Create;
using BlazorDungeonCrawler.Database.Resources.Commands.Delete;
using BlazorDungeonCrawler.Database.Resources.Commands.Update;

using BlazorDungeonCrawler.Database.Resources.Queries.Get;

using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;
using SharedAdventurer = BlazorDungeonCrawler.Shared.Models.Adventurer;
using SharedLevel = BlazorDungeonCrawler.Shared.Models.Level;
using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;
using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Data {
    public class DungeonManager {

        public async Task<SharedDungeon> Generate() {
            await Task.Delay(1);

            try {
                Messages messages = new();

                //Create
                //  Adventurer
                int health = Dice.RollDSix();
                messages.Add(new Message($"ADVENTURER HEALTH {health}", health));

                int damage = Dice.RollDSix();
                messages.Add(new Message($"ADVENTURER DAMAGE {damage}", damage));

                int protection = Dice.RollDSix();
                messages.Add(new Message($"ADVENTURER PROTECTION {protection}", protection));

                Adventurer adventurer = new(health, damage, protection);

                //  Level
                Levels levels = new();

                int depth = 1;
                Level newLevel = new(depth);

                messages.Add(new Message($"DUNGEON DEPTH {depth}"));

                //  Tiles
                Tiles tiles = new(newLevel.Depth, newLevel.Rows, newLevel.Columns);
                newLevel.Tiles = tiles.GetTiles();

                levels.Add(newLevel);

                //  Dungon
                string apiVersion = new Version(0, 2, 0).ToString();
                SharedDungeon sharedDungeon = new() {
                    Id = Guid.NewGuid(),

                    Adventurer = adventurer.SharedModelMapper(),

                    Levels = levels.SharedModelMapper(),
                    Messages = messages.SharedModelMapper(),

                    CurrentLevel = depth,

                    ApiVersion = apiVersion,

                    RefreshRequired = true
                };

                DungeonCreate.Create(sharedDungeon);

                return sharedDungeon;
            } catch (Exception ex) {
                throw;
            }
        }

        public async Task<SharedDungeon> RetrieveDungeon(Guid dungeonId) {
            await Task.Delay(1);

            try {
                SharedDungeon? sharedDungeon = DungeonQueries.Get(dungeonId);
                if (sharedDungeon == null || sharedDungeon.Id == Guid.Empty) {
                    sharedDungeon = await Generate();
                }

                return sharedDungeon;
            } catch (Exception ex) {
                throw;
            }
        }

        public async Task<SharedDungeon> GetSelectedDungeonTiles(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            try {
                SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
                if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
                if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
                if (dungeon.Levels == null) { throw new ArgumentNullException("Dungeon Level"); }

                Messages messages = new();
                Monsters monsters = new();

                SharedLevel? currentLevel = dungeon.Levels.Where(l => l.Depth == dungeon.CurrentLevel).FirstOrDefault();
                if (currentLevel == null) { throw new ArgumentNullException("Dungeon current Level"); }

                Tiles currentLevelTiles = new(currentLevel.Tiles);

                Tile selectedTile = new();

                foreach (Tile tile in currentLevelTiles.GetTiles()) {
                    tile.Current = false;
                    tile.Selectable = false;

                    if (tile.Id == tileId) {
                        selectedTile = tile;

                        selectedTile.Hidden = false;
                        selectedTile.Current = true;
                    }
                }

                switch (selectedTile.Type) {
                    case DungeonEvemts.Fight:
                        if (!selectedTile.FightWon && selectedTile.Monsters.Count == 0) {
                            //generate new monsters
                            monsters.Generate(dungeon.CurrentLevel);
                            selectedTile.Monsters = monsters.Get();
                            messages.Add(new Message($"{selectedTile.Monsters.Count} {monsters.GetName()}"));
                        }

                        dungeon.InCombat = true;
                        dungeon.CombatTile = selectedTile.Id;
                        break;
                    case DungeonEvemts.StairsDescending:
                        int depth = dungeon.CurrentLevel += 1;
                        dungeon.CurrentLevel = depth;

                        Level newLevel = new(depth);

                        messages.Add(new Message($"DUNGEON DEPTH {depth}"));

                        //  Tiles
                        Tiles tiles = new(newLevel.Depth, newLevel.Rows, newLevel.Columns);
                        newLevel.Tiles = tiles.GetTiles();

                        currentLevelTiles = tiles;
                        currentLevel = newLevel.SharedModelMapper();

                        dungeon.Levels.Add(currentLevel);

                        LevelCreate.Create(dungeon.Id, currentLevel);

                        dungeon.RefreshRequired = true;

                        break;
                    case DungeonEvemts.Empty:
                    case DungeonEvemts.DungeonEntrance:
                    case DungeonEvemts.StairsAscending:
                    case DungeonEvemts.FightWon:
                    case DungeonEvemts.FightLost:
                    case DungeonEvemts.Chest:
                    case DungeonEvemts.FoundWeapon:
                    case DungeonEvemts.FoundProtection:
                    case DungeonEvemts.FoundPotion:
                    case DungeonEvemts.TakenWeapon:
                    case DungeonEvemts.TakenProtection:
                    case DungeonEvemts.TakenPotion:
                    case DungeonEvemts.Macguffin:

                        break;
                    case DungeonEvemts.Unknown:
                    default:
                        throw new ArgumentOutOfRangeException("Tile Type");
                }

                if (!dungeon.InCombat && !dungeon.RefreshRequired) {
                    currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                }

                //Update Messages
                List<SharedMessage> sharedMessages = messages.SharedModelMapper();
                dungeon.Messages.AddRange(sharedMessages);
                MessagesCreate.Create(dungeon.Id, sharedMessages);

                List<SharedTile> sharedTiles = currentLevelTiles.SharedModelMapper();
                currentLevel.Tiles = sharedTiles;

                for (int i = 0; i < dungeon.Levels.Count; i++) {
                    if (dungeon.Levels[i].Id != currentLevel.Id) {
                        dungeon.Levels[i] = currentLevel;
                    }
                }

                TilesUpdate.Update(sharedTiles);

                if (monsters.Count() > 0) {
                    MonstersCreate.Create(selectedTile.Id, monsters.SharedModelMapper());
                }

                //Update Dungon
                DungeonUpdate.Update(dungeon);

                return dungeon;
            } catch (Exception ex) {
                throw;
            }
        }

        public async Task<SharedDungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            try {
                SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
                if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
                if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
                if (dungeon.Levels == null) { throw new ArgumentNullException("Dungeon Level"); }

                SharedLevel? currentLevel = dungeon.Levels.Where(l => l.Depth == dungeon.CurrentLevel).FirstOrDefault();
                if (currentLevel == null || currentLevel.Tiles == null || currentLevel.Tiles.Count == 0) { throw new ArgumentNullException("Dungeon Level Tiles"); }

                SharedTile? selectedTile = currentLevel.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Level selected Tile"); }

                Tiles currentLevelTiles = new(currentLevel.Tiles);

                Messages messages = new();

                if (AdventurerFleesCombat()) {
                    dungeon.InCombat = false;
                    dungeon.CombatTile = Guid.Empty;
                    dungeon.CombatInitiated = false;

                    messages.Add(new Message("ADVENTURER FLEES COMBAT"));

                    currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                } else {
                    messages.Add(new Message("ADVENTURER FAILED TO FLEE"));

                    if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
                    Adventurer adventurer = new(dungeon.Adventurer);

                    if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Level Tile Monsters"); }

                    List<SharedMonster> monsters = selectedTile.Monsters;
                    int monsterindex = Dice.RandomNumber(0, (monsters.Count() - 1));
                    Monster currentMonster = new(monsters[monsterindex]);

                    int adventurerProtection = adventurer.GetProtection();
                    int monsterDamage = currentMonster.Damage;

                    int woundsReceived = monsterDamage - adventurerProtection;
                    if (woundsReceived > 0) {
                        int currentHealth = adventurer.HealthBase - woundsReceived;
                        if (currentHealth > 0) {
                            adventurer.HealthBase = currentHealth;

                            messages.Add(new Message($"ADVENTURER HIT FOR {woundsReceived} WITH {currentMonster.Health} REMAINING"));
                        } else {
                            adventurer.HealthBase = 0;
                            adventurer.IsAlive = false;

                            selectedTile.Type = DungeonEvemts.FightLost;

                            dungeon.InCombat = false;

                            currentLevelTiles.Unhide();

                            messages.Add(new Message($"ADVENTURER KILLED WITH {woundsReceived}"));
                        }
                    } else {
                        messages.Add(new Message("MONSTER WAS UNABLE TO CAUSE HARM"));
                    }
                }

                //Update Messages
                List<SharedMessage> sharedMessages = messages.SharedModelMapper();
                dungeon.Messages.AddRange(sharedMessages);
                MessagesCreate.Create(dungeon.Id, sharedMessages);

                //Update Tiles
                List<SharedTile> sharedTiles = currentLevelTiles.SharedModelMapper();

                //  Current Tile
                SharedTile currentSharedTiles;
                for (int i = 0; i < sharedTiles.Count; i++) {
                    currentSharedTiles = sharedTiles[i];
                    if (currentSharedTiles.Id == selectedTile.Id) {
                        sharedTiles[i] = selectedTile;
                    }
                }

                currentLevel.Tiles = sharedTiles;

                for (int i = 0; i < dungeon.Levels.Count; i++) {
                    if (dungeon.Levels[i].Id != currentLevel.Id) {
                        dungeon.Levels[i] = currentLevel;
                    }
                }

                TilesUpdate.Update(sharedTiles);

                //Update Dungon
                DungeonUpdate.Update(dungeon);

                return dungeon;
            } catch (Exception ex) {
                throw;
            }
        }

        public bool AdventurerFleesCombat() {
            int adventurerRoll = Dice.RollDSix();
            int monsterRoll = Dice.RollDSix();

            if (adventurerRoll > monsterRoll) {
                return true;
            }

            return false;
        }

        public async Task<SharedDungeon> MonsterFight(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            try {
                SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
                if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
                if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
                if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
                if (dungeon.Levels == null) { throw new ArgumentNullException("Dungeon Level"); }

                SharedLevel? currentLevel = dungeon.Levels.Where(l => l.Depth == dungeon.CurrentLevel).FirstOrDefault();
                if (currentLevel == null) { throw new ArgumentNullException("Dungeon current Level"); }

                Tiles currentLevelTiles = new(currentLevel.Tiles);

                //todo: get this from dungeon 
                SharedTile? selectedTile = currentLevel.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Level selected Tile"); }
                if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Level Tile Monsters"); }

                Messages messages = new();
                List<SharedMonster> monsters = selectedTile.Monsters.OrderBy(m => m.Index).ToList();

                //Adventurer details
                //todo: get this from dungeon 
                Adventurer adventurer = new(dungeon.Adventurer);
                int adventurerDamage = adventurer.GetDamage();
                int adventurerProtection = adventurer.GetProtection();

                bool adventurerInitiatesCombat = true;
                if (!dungeon.CombatInitiated) {
                    adventurerInitiatesCombat = AdventurerInitiatesCombat();

                    if (adventurerInitiatesCombat) {
                        messages.Add(new Message("ADVENTURER INITIATES COMBAT"));
                    } else {
                        messages.Add(new Message("MONSTER INITIATES COMBAT"));
                    }

                    dungeon.CombatInitiated = true;
                }

                if (adventurerInitiatesCombat) {
                    //Monster defend
                    int monsterindex = Dice.RandomNumber(0, (monsters.Count() - 1));
                    Monster currentMonster = new(monsters[monsterindex]);

                    int monsterProtection = currentMonster.Protection;
                    int monsterHealth = currentMonster.Health;

                    //Adventurer attack
                    List<int> adventurerAttackDice = Dice.RollMiltipleDSixs(1);
                    int adventurerAttackValue = Dice.AddRollValues(adventurerAttackDice);

                    //Monster dodge
                    List<int> monsterDodgeRolls = Dice.RollMiltipleDSixs(1);
                    int monsterDodgeValue = Dice.AddRollValues(monsterDodgeRolls);

                    //Monster wounds
                    int monsterWounds = 0;
                    if (adventurerAttackValue > monsterDodgeValue) {
                        messages.Add(new Message($"ADVENTURER ATTACK ({adventurerAttackValue}) WINS OVER MONSTER DODGE ({monsterDodgeValue})"));

                        monsterWounds = adventurerDamage - monsterProtection;

                        if (monsterWounds < 1) {
                            monsterWounds = 1;
                        }

                        int currentHealth = monsterHealth - monsterWounds;
                        if (currentHealth > 0) {
                            currentMonster.Health = currentHealth;
                            SharedMonster sharedMonster = currentMonster.SharedModelMapper();

                            MonsterUpdate.Update(sharedMonster);

                            monsters[monsterindex] = sharedMonster;

                            messages.Add(new Message($"MONSTER HIT FOR {monsterWounds} WITH {currentHealth} REMAINING"));
                        } else {
                            monsterHealth = 0;

                            messages.Add(new Message($"MONSTER KILLED WITH {monsterWounds}"));

                            //remove monster at stack
                            MonsterDelete.Delete(currentMonster.Id);
                            monsters.RemoveAt(monsterindex);

                            //checked for remaining monsters
                            if (monsters.Count == 0) {
                                dungeon.InCombat = false;
                                dungeon.CombatTile = Guid.Empty;
                                dungeon.CombatInitiated = false;

                                selectedTile.FightWon = true;
                                selectedTile.Type = DungeonEvemts.FightWon;

                                currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                            };
                        }
                    } else {
                        messages.Add(new Message($"MONSTER DODGE ({monsterDodgeValue}) WINS OVER ADVENTURER ATTACK ({adventurerAttackValue})"));
                    }
                }

                //Monster attack
                if (dungeon.InCombat) {
                    int monsterDamage, monsterProtection, monsterHealth;
                    foreach (SharedMonster monster in selectedTile.Monsters) {
                        //Monster details
                        //todo: get this from dungeon 
                        monsterDamage = monster.Damage;
                        monsterProtection = monster.Protection;
                        monsterHealth = monster.Health;

                        List<int> monsterAttackDice = Dice.RollMiltipleDSixs(1);
                        int monsterAttackValue = Dice.AddRollValues(monsterAttackDice);

                        //Adventurer dodge
                        List<int> adventurerDodgeRolls = Dice.RollMiltipleDSixs(1);
                        int adventurerDodgeValue = Dice.AddRollValues(adventurerDodgeRolls);

                        //Adventurer wounds
                        int adventurerWounds = 0;
                        if (monsterAttackValue > adventurerDodgeValue) {
                            messages.Add(new Message($"MONSTER ATTACK ({monsterAttackValue}) WINS OVER ADVENTURER DODGE ({adventurerDodgeValue})"));

                            adventurerWounds = monsterDamage - adventurerProtection;

                            if (adventurerWounds < 1) {
                                adventurerWounds = 1;
                            }

                            int currentHealth = adventurer.HealthBase - adventurerWounds;
                            if (currentHealth > 0) {
                                adventurer.HealthBase = currentHealth;

                                messages.Add(new Message($"ADVENTURER HIT FOR {adventurerWounds} WITH {adventurer.HealthBase} REMAINING"));
                            } else {
                                adventurer.HealthBase = 0;
                                adventurer.IsAlive = false;

                                selectedTile.Type = DungeonEvemts.FightLost;

                                messages.Add(new Message($"ADVENTURER DIED WITH {adventurerWounds} WOUNDS"));

                                dungeon.InCombat = false;

                                currentLevelTiles.Unhide();

                                break;
                            }
                        } else {
                            messages.Add(new Message($"ADVENTURER DODGE ({monsterAttackValue}) WINS OVER MONSTER ATTACK ({adventurerDodgeValue})"));
                        }
                    }
                }

                //Update Messages
                List<SharedMessage> sharedMessages = messages.SharedModelMapper();
                dungeon.Messages.AddRange(sharedMessages);
                MessagesCreate.Create(dungeon.Id, sharedMessages);

                //Update Tiles
                List<SharedTile> sharedTiles = currentLevelTiles.SharedModelMapper();

                //  Current Tile
                SharedTile currentSharedTiles;
                for (int i = 0; i < sharedTiles.Count; i++) {
                    currentSharedTiles = sharedTiles[i];
                    if (currentSharedTiles.Id == selectedTile.Id) {
                        sharedTiles[i] = selectedTile;
                    }
                }

                currentLevel.Tiles = sharedTiles;

                for (int i = 0; i < dungeon.Levels.Count; i++) {
                    if (dungeon.Levels[i].Id != currentLevel.Id) {
                        dungeon.Levels[i] = currentLevel;
                    }
                }

                TilesUpdate.Update(sharedTiles);

                //Update Adventurer
                SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
                dungeon.Adventurer = sharedAdventurer;

                AdventurerUpdate.Update(sharedAdventurer);

                //Update Dungon
                DungeonUpdate.Update(dungeon);

                return dungeon;
            } catch (Exception ex) {
                throw;
            }
        }

        public bool AdventurerInitiatesCombat() {
            int adventurerRoll = Dice.RollDSix();
            int monsterRoll = Dice.RollDSix();

            if (adventurerRoll > monsterRoll) {
                return true;
            }

            return false;
        }

        public int GetWounds(int attack, int dodge, int damage, int protection) {


            return 0;
        }
    }
}
