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

                bool setSelectable = true;

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
                    case DungeonEvemts.DungeonEntrance:
                        if (!dungeon.MacGuffinFound) {
                            messages.Add(new Message("NO MACGUFFIN. GO FIND IT!"));
                        } else {
                            messages.Add(new Message("WELL DONE."));

                            currentLevelTiles.Unhide();
                            TilesUpdate.Update(currentLevelTiles.SharedModelMapper());

                            setSelectable = false;
                            //todo: show summary 
                        }
                        break;
                    case DungeonEvemts.Fight:
                        if (!selectedTile.FightWon && selectedTile.Monsters.Count == 0) {
                            //generate new monsters
                            monsters.Generate(dungeon.CurrentLevel);
                            selectedTile.Monsters = monsters.Get();

                            if (selectedTile.Monsters.Count == 1) {
                                messages.Add(new Message($"A {monsters.GetName()}"));
                            } else {
                                messages.Add(new Message($"{selectedTile.Monsters.Count} {monsters.GetName()}s"));
                            }
                        }

                        dungeon.InCombat = true;
                        dungeon.CombatTile = selectedTile.Id;

                        setSelectable = false;
                        break;
                    case DungeonEvemts.StairsDescending:
                        //  Set surrounding selecteable tiles for when the user returns to this level
                        currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                        TilesUpdate.Update(currentLevelTiles.SharedModelMapper());

                        int increasedDepth = dungeon.CurrentLevel += 1;
                        dungeon.CurrentLevel = increasedDepth;

                        currentLevel = dungeon.Levels.Where(l => l.Depth == increasedDepth).FirstOrDefault();
                        if (currentLevel == null || currentLevel.Id == Guid.Empty) {
                            //Next level
                            Level newLevel = new(increasedDepth);
                            messages.Add(new Message($"DUNGEON DEPTH {increasedDepth}"));

                            //  Tiles
                            Tiles tiles = new(newLevel.Depth, newLevel.Rows, newLevel.Columns);
                            newLevel.Tiles = tiles.GetTiles();

                            currentLevelTiles = tiles;
                            currentLevel = newLevel.SharedModelMapper();

                            dungeon.Levels.Add(currentLevel);

                            LevelCreate.Create(dungeon.Id, currentLevel);
                        } else {
                            currentLevelTiles = new Tiles(currentLevel.Tiles);
                        }

                        dungeon.RefreshRequired = true;
                        setSelectable = false;
                        break;
                    case DungeonEvemts.StairsAscending:
                        //  Set surrounding selecteable tiles for when the user returns to this level
                        currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                        TilesUpdate.Update(currentLevelTiles.SharedModelMapper());

                        int decreaseDepth = dungeon.CurrentLevel -= 1;
                        dungeon.CurrentLevel = decreaseDepth;

                        currentLevel = dungeon.Levels.Where(l => l.Depth == decreaseDepth).FirstOrDefault();
                        if (currentLevel == null || currentLevel.Id == Guid.Empty || currentLevel.Tiles == null) {
                            throw new Exception("Level not found");
                        }

                        currentLevelTiles = new Tiles(currentLevel.Tiles);

                        dungeon.RefreshRequired = true;
                        setSelectable = false;
                        break;
                    case DungeonEvemts.Chest:
                        int lootValue = Dice.RollDSix();
                        DungeonEvemts lootTile = GetLootType(lootValue);

                        switch (lootTile) {
                            case DungeonEvemts.TakenWeapon:
                                messages.Add(new Message("Taken weapon"));
                                break;
                            case DungeonEvemts.TakenProtection:
                                messages.Add(new Message("Taken protection"));
                                break;
                            case DungeonEvemts.TakenPotion:
                                int typeValue = Dice.RollDSix();
                                int sizeValue = Dice.RollDSix();
                                int durationValue = Dice.RollDSix();

                                Potion potion = new(dungeon.CurrentLevel, typeValue, sizeValue, durationValue);

                                messages.Add(new Message($"Potion type: {potion.Type} ({typeValue})", typeValue));
                                messages.Add(new Message($"Potion size: {potion.Size} ({sizeValue})", sizeValue));
                                messages.Add(new Message($"Potion duration: {potion.Duration} ({durationValue})", durationValue));

                                Adventurer adventurer = new(dungeon.Adventurer);

                                switch (potion.Type) {
                                    case PotionType.Aura:
                                        int regainedHealth = adventurer.SetAuraPotion(potion.SizeValue);
                                        adventurer.AuraPotionDuration = potion.DurationValue;

                                        if (regainedHealth > 0) {
                                            messages.Add(new Message($"Regained {regainedHealth} health"));
                                        }                                        
                                        break;
                                    case PotionType.Damage:
                                        adventurer.DamagePotion = potion.SizeValue;
                                        adventurer.DamagePotionDuration = potion.DurationValue;
                                        break;
                                    case PotionType.Sheild:
                                        adventurer.ShieldPotion = potion.SizeValue;
                                        adventurer.ShieldPotionDuration = potion.DurationValue;
                                        break;
                                    case PotionType.Unknown:
                                        break;
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException("Loot tile type.");
                        }
                        break;
                    case DungeonEvemts.Macguffin:

                        break;
                    case DungeonEvemts.FoundWeapon:
                    case DungeonEvemts.FoundProtection:
                    case DungeonEvemts.FoundPotion:
                    case DungeonEvemts.TakenWeapon:
                    case DungeonEvemts.TakenProtection:
                    case DungeonEvemts.TakenPotion:
                    case DungeonEvemts.Empty:
                    case DungeonEvemts.FightWon:

                        break;
                    case DungeonEvemts.Unknown:
                    default:
                        throw new ArgumentOutOfRangeException("Selected Dungeon Tiles Tile Type");
                }

                if (setSelectable) {
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

        //Loot
        //  1 - 2:  Potion
        //  3 - 4:  Weapon
        //  5 - 6:  Protection
        private DungeonEvemts GetLootType(int value) {
            switch (value) {
                case 1:
                case 2:
                    return DungeonEvemts.TakenPotion;
                case 3:
                case 4:
                    return DungeonEvemts.TakenWeapon;
                case 5:
                case 6:
                    return DungeonEvemts.TakenProtection;
                default:
                    throw new ArgumentOutOfRangeException("Loot type roll value");
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

                        selectedTile.Monsters = monsters;
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
