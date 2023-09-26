﻿using System.Text.RegularExpressions;

using BlazorDungeonCrawler.Shared.Models;
using BlazorDungeonCrawler.Database;
using System.Data.Entity;

namespace BlazorDungeonCrawler.Server.Data {
    public class DungeonManager {
        DungeonCrawlerContext dungeonCrawlerContext = new DungeonCrawlerContext();

        private List<Message> messages = new List<Message>();

        //Dungon
        //  Set
        public async Task<Dungeon> Generate() {
            await Task.Delay(1);

            Dungeon dungeon = new Dungeon() {
                Id = Guid.NewGuid(),
                Adventurer = GenerateAdventurer(),
                Level = GenerateLevel(),
                ApiVersion = new Version(0, 2, 0)
            };

            dungeon.Messages.AddRange(messages);

            dungeonCrawlerContext.Dungeons.Add(dungeon);
            dungeonCrawlerContext.SaveChanges();

            return dungeon;
        }

        //Random numbers
        public int randomNumber(int min, int max) {
            Guid guid = Guid.NewGuid();
            string guidDigits = Regex.Replace(guid.ToString(), @"\D0*", "");
            int number = int.Parse(guidDigits.Substring(0, 8));
            Random rnd = new Random(number);

            return rnd.Next(min, max + 1);
        }

        //Dice
        public int rollDSix() {
            return randomNumber(1, 6);
        }

        //Adventurer
        public Adventurer GenerateAdventurer() {
            int health = rollDSix();
            AddMessage($"ADVENTURER HEALTH {health}", health);

            int damage = rollDSix();
            AddMessage($"ADVENTURER DAMAGE {damage}", damage);

            int protection = rollDSix();
            AddMessage($"ADVENTURER PROTECTION {protection}", protection);

            return new Adventurer() {
                Id = Guid.NewGuid(),
                HealthBase = health,
                DamageBase = damage,
                ProtectionBase = protection,
                IsAlive = true
            };
        }

        //Level
        public Level GenerateLevel() {
            return GenerateLevel(1);
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
                avalibleTileIndex = randomNumber(0, avalibleTileIndexes.Count);
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
            if (currentRow == int.MinValue || currentColumn == int.MinValue ) { 
            //todo return error
            };

            //Get Selected
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

            return tiles;
        }

        public DungeonEvemts GetTileType() {
            DungeonEvemts dungeonEvemts = DungeonEvemts.Unknown;

            int value = rollDSix();
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

            MonsterManager monsterManager = new MonsterManager();
            List<MonsterType> availableMonsters = monsterManager.GetMonsters(depth);

            int currentMonsterTypeIndex = randomNumber(0, availableMonsters.Count - 1);
            MonsterType currentMonsterType = availableMonsters[currentMonsterTypeIndex];

            int monsterGroup = 1, rollValue = 0, health = 0, damage = 0, protection = 0;
            List<int> healthDice = new(), damageDice = new(), protectionDice = new();

            for (int i = 0; i < monsterGroup; i++) {
                Monster monster = new Monster() {
                    Id = Guid.NewGuid(),
                    TypeName = currentMonsterType.Name
                };

                for (int h = 0; h < currentMonsterType.HealthDiceCount; h++) {
                    rollValue = rollDSix();
                    healthDice.Add(rollValue);
                    health += rollValue;
                }
                monster.Health = health;
                AddMessage($"MONSTER HEALTH {health}", healthDice);

                for (int d = 0; d < currentMonsterType.DamageDiceCount; d++) {
                    rollValue = rollDSix();
                    damageDice.Add(rollValue);
                    damage += rollValue;
                }
                monster.Damage = 0;
                AddMessage($"MONSTER DAMAGE {damage}", damageDice);

                for (int p = 0; p < currentMonsterType.DamageDiceCount; p++) {
                    rollValue = rollDSix();
                    protectionDice.Add(rollValue);
                    protection += rollValue;
                }
                monster.Protection = 0;
                AddMessage($"MONSTER PROTECTION {protection}", protectionDice);

                monsters.Add(monster);
            }

            return monsters;
        }

        //Message
        public void AddMessage(string message) {
            AddMessage(message, new List<int>());
        }
        public void AddMessage(string message, int die) {
            AddMessage(message, new List<int>() { die });
        }

        public void AddMessage(string message, List<int> dice) {
            messages.Add(new Message() {
                Id = Guid.NewGuid(),
                Index = messages.Count + 1,
                Text = message,
                Dice = dice
            });
        }
    }
}
