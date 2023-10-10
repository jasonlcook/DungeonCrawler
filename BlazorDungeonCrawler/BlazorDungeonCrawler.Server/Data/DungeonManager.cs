﻿using BlazorDungeonCrawler.Server.Models;
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
            SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();

            //  Level
            int depth = 1;
            Level level = new(depth);
            SharedLevel sharedLevel = level.SharedModelMapper();

            messages.Add(new Message($"DUNGEON DEPTH {depth}"));

            //  Tiles
            Tiles tiles = new(level.Depth, level.Rows, level.Columns);
            List<SharedTile> sharedTiles = tiles.SharedModelMapper();

            sharedLevel.Tiles = sharedTiles;

            //  Messages
            List<SharedMessage> sharedMessages = messages.SharedModelMapper();

            //  Dungon
            string apiVersion = new Version(0, 2, 0).ToString();
            SharedDungeon sharedDungeon = new() {
                Id = Guid.NewGuid(),

                Adventurer = sharedAdventurer,
                Level = sharedLevel,
                Messages = sharedMessages,

                ApiVersion = apiVersion
            };

            DungeonCreate.Create(sharedDungeon);

            return sharedDungeon;
        }

        public async Task<SharedDungeon> RetrieveDungeon(Guid dungeonId) {
            await Task.Delay(1);

            SharedDungeon? sharedDungeon = DungeonQueries.Get(dungeonId);
            if (sharedDungeon == null || sharedDungeon.Id == Guid.Empty) {
                sharedDungeon = await Generate();
            }

            return sharedDungeon;
        }

        public async Task<SharedDungeon> GetSelectedDungeonTiles(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Level == null || dungeon.Level.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Level"); }

            Messages messages = new();
            Monsters monsters = new();

            List<SharedTile> sharedTiles = dungeon.Level.Tiles;
            Tiles tiles = new Tiles(sharedTiles);

            Tile selectedTile = new();
            foreach (Tile tile in tiles.GetTiles()) {
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
                        monsters.Generate(dungeon.Level.Depth);
                        selectedTile.Monsters = monsters.Get();
                        messages.Add(new Message($"{selectedTile.Monsters.Count} {monsters.GetName()}"));
                    }

                    dungeon.InCombat = true;
                    dungeon.CombatTile = selectedTile.Id;

                    break;
                case DungeonEvemts.Unknown:
                case DungeonEvemts.Empty:
                case DungeonEvemts.DungeonEntrance:
                case DungeonEvemts.StairsAscending:
                case DungeonEvemts.StairsDescending:
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
                default:
                    break;
            }

            if (!dungeon.InCombat) {
                tiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
            }

            if (dungeon.Messages != null && messages.Count() > 0) {
                List<SharedMessage> sharedMessages = messages.SharedModelMapper();
                dungeon.Messages.AddRange(sharedMessages);

                MessageCreate.Create(dungeon, sharedMessages);
            }

            sharedTiles = tiles.SharedModelMapper();
            dungeon.Level.Tiles = sharedTiles;

            TilesUpdate.Update(sharedTiles);

            if (monsters.Count() > 0) {
                MonsterCreate.Create(selectedTile.SharedModelMapper(), monsters.SharedModelMapper());
            }

            DungeonUpdate.Update(dungeon);

            return dungeon;
        }

        public async Task<SharedDungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Level == null || dungeon.Level.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Level"); }
            if (dungeon.Level.Tiles == null || dungeon.Level.Tiles.Count == 0) { throw new ArgumentNullException("Dungeon Level Tiles"); }

            SharedTile? selectedTile = dungeon.Level.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
            if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Level selected Tile"); }

            Tiles tiles = new(dungeon.Level.Tiles);

            Messages messages = new();

            if (AdventurerFleesCombat()) {
                dungeon.InCombat = false;
                dungeon.CombatTile = Guid.Empty;
                dungeon.CombatInitiated = false;

                messages.Add(new Message("ADVENTURER FLEES COMBAT"));

                tiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
            } else {
                messages.Add(new Message("ADVENTURER FAILED TO FLEE"));

                if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
                Adventurer adventurer = new(dungeon.Adventurer);


                if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Level Tile Monsters"); }
                Monster monster = new(selectedTile.Monsters.First());

                int adventurerProtection = adventurer.GetProtection();
                int monsterDamage = monster.Damage;

                int woundsReceived = monsterDamage - adventurerProtection;
                if (woundsReceived > 0) {
                    int currentHealth = adventurer.HealthBase - woundsReceived;
                    if (currentHealth > 0) {
                        adventurer.HealthBase = currentHealth;

                        messages.Add(new Message($"ADVENTURER HIT FOR {woundsReceived} WITH {monster.Health} REMAINING"));
                    } else {
                        adventurer.HealthBase = 0;
                        adventurer.IsAlive = false;

                        selectedTile.Type = DungeonEvemts.FightLost;

                        dungeon.InCombat = false;

                        tiles.Unhide();

                        messages.Add(new Message($"ADVENTURER KILLED WITH {woundsReceived}"));
                    }
                } else {
                    messages.Add(new Message("MONSTER WAS UNABLE TO CAUSE HARM"));
                }
            }

            if (dungeon.Messages != null && messages.Count() > 0) {
                List<SharedMessage> sharedMessages = messages.SharedModelMapper();
                dungeon.Messages.AddRange(sharedMessages);

                MessageCreate.Create(dungeon, sharedMessages);
            }

            List<SharedTile> sharedTiles = tiles.SharedModelMapper();
            dungeon.Level.Tiles = sharedTiles;

            TilesUpdate.Update(sharedTiles);

            DungeonUpdate.Update(dungeon);

            return dungeon;
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

            SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
            if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
            if (dungeon.Level == null || dungeon.Level.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Level"); }
            if (dungeon.Level.Tiles == null || dungeon.Level.Tiles.Count == 0) { throw new ArgumentNullException("Dungeon Level Tiles"); }

            Tiles tiles = new(dungeon.Level.Tiles);

            //todo: get this from dungeon 
            SharedTile? selectedTile = dungeon.Level.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
            if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Level selected Tile"); }
            if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Level Tile Monsters"); }

            Messages messages = new();

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
                int monsterindex = Dice.RandomNumber(0, (selectedTile.Monsters.Count() - 1));
                Monster currentMonster = new(selectedTile.Monsters[0]);

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

                        selectedTile.Monsters[monsterindex] = sharedMonster;

                        messages.Add(new Message($"MONSTER HIT FOR {monsterWounds} WITH {currentHealth} REMAINING"));
                    } else {
                        monsterHealth = 0;

                        messages.Add(new Message($"MONSTER KILLED WITH {monsterWounds}"));

                        //remove monster at stack
                        MonsterDelete.Delete(currentMonster.Id);
                        selectedTile.Monsters.RemoveAt(monsterindex);

                        //checked for remaining monsters
                        if (selectedTile.Monsters.Count == 0) {
                            dungeon.InCombat = false;
                            dungeon.CombatTile = Guid.Empty;
                            dungeon.CombatInitiated = false;

                            selectedTile.FightWon = true;
                            selectedTile.Type = DungeonEvemts.FightWon;

                            tiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
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

                            tiles.Unhide();

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
            MessageCreate.Create(dungeon, sharedMessages);

            //Update Tiles
            List<SharedTile> sharedTiles = tiles.SharedModelMapper();

            //  Current Tile
            SharedTile currentSharedTiles;
            for (int i = 0; i < sharedTiles.Count; i++) {
                currentSharedTiles = sharedTiles[i];
                if (currentSharedTiles.Id == selectedTile.Id) {
                    sharedTiles[i] = selectedTile;
                }
            }

            dungeon.Level.Tiles = sharedTiles;
            TilesUpdate.Update(sharedTiles);

            //Update Adventurer
            SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
            dungeon.Adventurer = sharedAdventurer;

            AdventurerUpdate.Update(sharedAdventurer);

            //Update Dungon
            DungeonUpdate.Update(dungeon);

            return dungeon;
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
