﻿using System.Data.Entity;
using System.Text.RegularExpressions;

using BlazorDungeonCrawler.Shared.Models;
using BlazorDungeonCrawler.Database;

namespace BlazorDungeonCrawler.Server.Data {
    public class DungeonManager {
        private List<Message> newMessages = new List<Message>();

        //Dungon
        //  Set
        public async Task<Dungeon> Generate() {
            await Task.Delay(1);


            Dungeon dungeon = new Dungeon();

            try {
                dungeon = new Dungeon() {
                    Id = Guid.NewGuid(),
                    Adventurer = GenerateAdventurer(),
                    Level = GenerateLevel(),
                    ApiVersion = new Version(0, 2, 0).ToString()
                };

                foreach (Message message in newMessages) {
                    dungeon.Messages.Add(message);
                }
            } catch (Exception ex) {
                //todo add error handeing for error on model generation
            }

            try {
                if (dungeon.Id != Guid.Empty) {
                    using (var context = new DungeonCrawlerContext()) {
                        context.Dungeons.Add(dungeon);
                        context.SaveChanges();
                    }
                }
            } catch (Exception ex) {
                //todo add error handeing for error on model creation                
            };

            return dungeon;
        }

        //  Set
        public async Task<Dungeon> GetSelectedDungeonTile(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            using (DungeonCrawlerContext context = new DungeonCrawlerContext()) {
                Tile selectedTile = context.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (selectedTile != null && selectedTile.Selectable) {
                    Dungeon dungeon = context.Dungeons.Where(d => d.Id == dungeonId).Include(d => d.Adventurer).Include(d => d.Level).Include(d => d.Level.Tiles).Include(d => d.Messages).FirstOrDefault();
                    List<Tile> tiles = dungeon.Level.Tiles;

                    if (tiles.Count > 0) {
                        foreach (Tile tile in tiles) {
                            tile.Current = false;
                            tile.Selectable = false;
                        }

                        selectedTile.Hidden = false;
                        selectedTile.Current = true;

                        switch (selectedTile.Type) {
                            case DungeonEvemts.Fight:
                            ClearMessageQueue();

                            List<Monster> monsters = GetTileMonsters(dungeon.Level.Depth);
                            selectedTile.Monsters = monsters;

                            dungeon.InCombat = true;
                            dungeon.CombatTile = selectedTile.Id;

                            foreach (Message message in newMessages) {
                                dungeon.Messages.Add(message);
                            }

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
                            GetSelected(ref tiles, selectedTile.Row, selectedTile.Column);
                        }

                        context.SaveChanges();
                    } else {
                        //todo: log database error
                    }

                    return dungeon;
                }
            }

            return new Dungeon();
        }

        //Fight
        //  Flee
        public async Task<Dungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            try {
                using (DungeonCrawlerContext context = new DungeonCrawlerContext()) {
                    ClearMessageQueue();

                    Dungeon dungeon = context.Dungeons.Where(d => d.Id == dungeonId).Include(d => d.Adventurer).Include(d => d.Level).Include(d => d.Level.Tiles).Include(d => d.Messages).FirstOrDefault();
                    Tile selectedTile = context.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                    List<Monster> monsters = selectedTile.Monsters;





                    AddMessage("FLEE");

                    foreach (Message message in newMessages) {
                        dungeon.Messages.Add(message);
                    }

                    context.SaveChanges();

                    return dungeon;
                }
            } catch (Exception) {
                return new Dungeon();
            }
        }

        public bool AdventurerFleesCombat() {
            int adventurerRoll = RollDSix();
            int monsterRoll = RollDSix();

            if (adventurerRoll > monsterRoll) {
                AddMessage("ADVENTURER INITIATES COMBAT");
                return true;
            }

            AddMessage("MONSTER INITIATES COMBAT");

            return false;
        }

        //  Fight
        public async Task<Dungeon> MonsterFight(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            try {
                using (DungeonCrawlerContext context = new DungeonCrawlerContext()) {
                    ClearMessageQueue();

                    Dungeon dungeon = context.Dungeons.Where(d => d.Id == dungeonId).Include(d => d.Adventurer).Include(d => d.Level).Include(d => d.Level.Tiles).Include(d => d.Messages).FirstOrDefault();

                    Tile selectedTile = context.Tiles.Where(t => t.Id == tileId).Include(t => t.Monsters).FirstOrDefault();

                    //Adventurer details
                    Adventurer adventurer = dungeon.Adventurer;
                    int adventurerDamage = adventurer.DamageBase + adventurer.Weapon + adventurer.DamagePotion;
                    int adventurerProtection = adventurer.ProtectionBase + adventurer.ShieldPotion + adventurer.ArmourHelmet + adventurer.ArmourBreastplate + adventurer.ArmourGauntlet + adventurer.ArmourGreave + adventurer.ArmourBoots;

                    //Monster details
                    Monster currentMonster = selectedTile.Monsters.First();
                    int monsterDamage = currentMonster.Damage;
                    int monsterProtection = currentMonster.Protection;

                    bool adventurerInitiatesCombat = true;

                    //if firsst round check if Adventurer initiates combat
                    if (!dungeon.CombatInitiated) {
                        adventurerInitiatesCombat = AdventurerInitiatesCombat();
                        dungeon.CombatInitiated = true;
                    }

                    //Adventurer round
                    if (adventurerInitiatesCombat) {
                        int woundsInflicted = AttackRound(1, adventurerDamage, 1, monsterProtection);

                        if (woundsInflicted > 0) {
                            int currentHealth = currentMonster.Health - woundsInflicted;
                            if (currentHealth > 0) {
                                currentMonster.Health = currentHealth;

                                AddMessage($"MONSTER HIT FOR {woundsInflicted} WITH {currentMonster.Health} REMAINING");
                            } else {
                                currentMonster.Health = 0;
                                currentMonster = null;

                                AddMessage($"MONSTER KILLED WITH {woundsInflicted}");

                                //remove monster at stack
                                selectedTile.Monsters.RemoveAt(0);

                                //checked for remaining monsters
                                if (selectedTile.Monsters.Count == 0) {
                                    dungeon.InCombat = false;
                                    dungeon.CombatTile = Guid.Empty;
                                    dungeon.CombatInitiated = false;

                                    selectedTile.Type = DungeonEvemts.FightWon;

                                    List<Tile> tiles = dungeon.Level.Tiles;

                                    GetSelected(ref tiles, selectedTile.Row, selectedTile.Column);
                                };
                            }
                        } else {
                            AddMessage($"MONSTER AVOIDED DAMAGE");
                        }
                    }

                    //Monster round
                    if (currentMonster != null) {
                        int woundsReceived = AttackRound(1, monsterDamage, 1, adventurerProtection);

                        if (woundsReceived > 0) {
                            int currentHealth = adventurer.HealthBase - woundsReceived;
                            if (currentHealth > 0) {
                                adventurer.HealthBase = currentHealth;

                                AddMessage($"ADVENTURER HIT FOR {woundsReceived} WITH {currentMonster.Health} REMAINING");
                            } else {
                                adventurer.HealthBase = 0;
                                adventurer.IsAlive = false;

                                selectedTile.Type = DungeonEvemts.FightLost;

                                AddMessage($"ADVENTURER KILLED WITH {woundsReceived}");

                                dungeon.InCombat = false;

                                foreach (Tile tile in dungeon.Level.Tiles) {
                                    tile.Hidden = false;
                                }
                            }
                        } else {
                            AddMessage($"ADVENTURER AVOIDED DAMAGE");
                        }
                    }

                    //Update messages
                    foreach (Message message in newMessages) {
                        dungeon.Messages.Add(message);
                    }

                    context.SaveChanges();

                    return dungeon;
                }
            } catch (Exception ex) {
                return new Dungeon();
            }
        }

        public bool AdventurerInitiatesCombat() {
            int adventurerRoll = RollDSix();
            int monsterRoll = RollDSix();

            if (adventurerRoll > monsterRoll) {
                AddMessage("ADVENTURER INITIATES COMBAT");
                return true;
            }

            AddMessage("MONSTER INITIATES COMBAT");
            return false;
        }

        public int AttackRound(int attackDiceAmount, int damage, int dodgeDiceAmount, int protection) {
            int roll;

            int attack = 0;
            List<int> attackRolls = new List<int>();
            for (int i = 0; i < attackDiceAmount; i++) {
                roll = RollDSix();

                attackRolls.Add(roll);
                attack += roll;
            }

            AddMessage($"ATTACK ROLL {attack}", attackRolls);

            int dodge = 0;
            List<int> dodgeRolls = new List<int>();
            for (int i = 0; i < dodgeDiceAmount; i++) {
                roll = RollDSix();

                dodgeRolls.Add(roll);
                dodge += roll;
            }

            AddMessage($"DODGE ROLL {dodge}", dodgeRolls);

            if (attack > dodge) {
                int wounds = damage - protection;

                AddMessage($"WOUNDS {wounds} (DAMAGE: {damage} - PROTECTION: {protection})");

                if (wounds < 0) {
                    wounds = 0;
                }

                return wounds;
            }

            return 0;
        }

        //Random numbers
        public int RandomNumber(int min, int max) {
            Guid guid = Guid.NewGuid();
            string guidDigits = Regex.Replace(guid.ToString(), @"\D0*", "");
            int number = int.Parse(guidDigits.Substring(0, 8));
            Random rnd = new Random(number);

            return rnd.Next(min, max + 1);
        }

        //Dice
        public int RollDSix() {
            return RandomNumber(1, 6);
        }

        //Adventurer
        public Adventurer GenerateAdventurer() {
            int health = RollDSix();
            AddMessage($"ADVENTURER HEALTH {health}", health);

            int damage = RollDSix();
            AddMessage($"ADVENTURER DAMAGE {damage}", damage);

            int protection = RollDSix();
            AddMessage($"ADVENTURER PROTECTION {protection}", protection);

            return new Adventurer() {
                Id = Guid.NewGuid(),
                HealthInitial = health,
                HealthBase = health,
                DamageBase = damage,
                ProtectionBase = protection,
                IsAlive = true
            };
        }

        //Level
        public Level GenerateLevel() {
            Level level = GenerateLevel(1);
            return level;
        }

        public Level GenerateLevel(int depth) {
            Level level = new Level() {
                Id = Guid.NewGuid(),
                Depth = depth
            };

            int rows = 0;
            int columns = 0;

            switch (depth) {
                case 1:
                rows = 3;
                columns = 3;
                break;
                case 2:
                case 3:
                rows = 5;
                columns = 5;
                break;
                case 4:
                case 5:
                rows = 7;
                columns = 7;
                break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                rows = 7;
                columns = 9;
                break;
            }

            level.Rows = rows;
            level.Columns = columns;

            level.Tiles = GenerateTiles(depth, rows, columns);

            AddMessage($"DUNGEON DEPTH {depth}");

            return level;
        }

        //  Tiles
        public List<Tile> GenerateTiles(int depth, int rows, int cols) {
            List<Tile> tiles = new List<Tile>();

            //Generate 
            float overflow = (cols + 1) / 2;
            int tileCount = (rows * cols) - (int)Math.Ceiling(overflow);

            List<int> avalibleTileIndexes = new List<int>();

            int row = -1, column = 0;
            for (int i = 0; i < tileCount; i++) {
                row += 1;
                if ((column % 2) == 1) {
                    //long
                    if (row > rows - 1) {
                        row = 0;
                        column += 1;
                    }
                } else {
                    //short
                    if (row > rows - 2) {
                        row = 0;
                        column += 1;
                    }
                }

                tiles.Add(new Tile() {
                    Id = Guid.NewGuid(),
                    Row = row,
                    Column = column,
                    Type = GetTileType()
                });

                avalibleTileIndexes.Add(i);
            }

            //Insert level elements
            List<DungeonEvemts> additionalEvents = new List<DungeonEvemts>();
            switch (depth) {
                case 1:
                additionalEvents.Add(DungeonEvemts.DungeonEntrance);
                additionalEvents.Add(DungeonEvemts.StairsDescending);
                break;
            }

            Tile randomSelectedTile;
            int avalibleTileIndex;
            int currentRow = int.MinValue, currentColumn = int.MinValue;
            for (int i = 0; i < additionalEvents.Count; i++) {
                //get random tile 
                avalibleTileIndex = RandomNumber(0, avalibleTileIndexes.Count - 1);
                randomSelectedTile = tiles.ElementAt(avalibleTileIndexes.ElementAt(avalibleTileIndex));

                //once selected remove from the avalible list
                avalibleTileIndexes.RemoveAt(avalibleTileIndex);

                //retrive first event
                randomSelectedTile.Type = additionalEvents.ElementAt(i);

                //if it is a starting tile set it as current un visible
                if (randomSelectedTile.Type == DungeonEvemts.DungeonEntrance || randomSelectedTile.Type == DungeonEvemts.StairsAscending) {
                    randomSelectedTile.Hidden = false;
                    randomSelectedTile.Current = true;

                    currentRow = randomSelectedTile.Row;
                    currentColumn = randomSelectedTile.Column;
                }
            }

            //Check for current
            if (currentRow == int.MinValue || currentColumn == int.MinValue) {
                //todo return error
            };

            //Get Selected
            GetSelected(ref tiles, currentRow, currentColumn);

            return tiles;
        }

        public void GetSelected(ref List<Tile> tiles, int currentRow, int currentColumn) {
            int previousTileRow, currentTileRow, nextTileRow, previousTileColumn, currentTileColumn, nextTileColumn;
            foreach (Tile tile in tiles) {
                tile.Selectable = false;

                currentTileRow = tile.Row;
                previousTileRow = currentTileRow - 1;
                nextTileRow = currentTileRow + 1;

                currentTileColumn = tile.Column;
                previousTileColumn = currentTileColumn - 1;
                nextTileColumn = currentTileColumn + 1;

                if ((currentTileColumn % 2) == 1) {
                    //previous and next column
                    if (previousTileColumn == currentColumn || nextTileColumn == currentColumn) {
                        if (previousTileRow == currentRow || currentTileRow == currentRow) {
                            tile.Selectable = true;
                        }
                    }

                    //curent column
                    if (currentTileColumn == currentColumn) {
                        if (previousTileRow == currentRow || nextTileRow == currentRow) {
                            tile.Selectable = true;
                        }
                    }
                } else {
                    //previous and next column
                    if (previousTileColumn == currentColumn || nextTileColumn == currentColumn) {
                        if (currentTileRow == currentRow || nextTileRow == currentRow) {
                            tile.Selectable = true;
                        }
                    }

                    //curent column
                    if (currentTileColumn == currentColumn) {
                        if (previousTileRow == currentRow || nextTileRow == currentRow) {
                            tile.Selectable = true;
                        }
                    }
                }
            }
        }

        public DungeonEvemts GetTileType() {
            DungeonEvemts dungeonEvemts = DungeonEvemts.Unknown;

            int value = RollDSix();
            switch (value) {
                case 1:
                case 2:
                dungeonEvemts = DungeonEvemts.Fight;
                break;
                case 3:
                case 4:
                case 5:
                dungeonEvemts = DungeonEvemts.Empty;
                break;
                case 6:
                dungeonEvemts = DungeonEvemts.Chest;
                break;
            }

            return dungeonEvemts;
        }

        public List<Monster> GetTileMonsters(int depth) {
            List<Monster> monsters = new List<Monster>();

            List<MonsterType> availableMonsters;
            using (var context = new DungeonCrawlerContext()) {
                availableMonsters = context.MonsterType.Where(mt => mt.LevelStart <= depth).Where(mt => mt.LevelEnd >= depth).ToList();
            }

            if (availableMonsters.Count > 0) {
                int currentMonsterTypeIndex = RandomNumber(0, availableMonsters.Count - 1);
                MonsterType currentMonsterType = availableMonsters[currentMonsterTypeIndex];

                if (currentMonsterType.Id != Guid.Empty) {
                    int monsterGroup = 1, rollValue = 0, health = 0, damage = 0, protection = 0;
                    List<int> healthDice = new(), damageDice = new(), protectionDice = new();

                    for (int i = 0; i < monsterGroup; i++) {
                        Monster monster = new Monster() {
                            Id = Guid.NewGuid(),
                            TypeName = currentMonsterType.Name
                        };

                        for (int h = 0; h < currentMonsterType.HealthDiceCount; h++) {
                            rollValue = RollDSix();
                            healthDice.Add(rollValue);
                            health += rollValue;
                        }
                        monster.Health = health;
                        AddMessage($"MONSTER HEALTH {health}", healthDice);

                        for (int d = 0; d < currentMonsterType.DamageDiceCount; d++) {
                            rollValue = RollDSix();
                            damageDice.Add(rollValue);
                            damage += rollValue;
                        }
                        monster.Damage = damage;
                        AddMessage($"MONSTER DAMAGE {damage}", damageDice);

                        for (int p = 0; p < currentMonsterType.DamageDiceCount; p++) {
                            rollValue = RollDSix();
                            protectionDice.Add(rollValue);
                            protection += rollValue;
                        }
                        monster.Protection = protection;
                        AddMessage($"MONSTER PROTECTION {protection}", protectionDice);

                        monster.ClientX = RandomNumber(20, 50);
                        monster.ClientY = RandomNumber(20, 50);

                        monsters.Add(monster);
                    }
                }

            } else {
                //todo log error
            }

            return monsters;
        }

        //Message
        public void ClearMessageQueue() {
            newMessages.Clear();
        }

        public void AddMessage(string message) {
            AddMessage(message, new List<int>());
        }
        public void AddMessage(string message, int die) {
            AddMessage(message, new List<int>() { die });
        }

        public void AddMessage(string message, List<int> dice) {
            newMessages.Add(new Message() {
                Id = Guid.NewGuid(),
                Index = newMessages.Count + 1,
                Text = message,
                Dice = dice
            });
        }
    }
}
