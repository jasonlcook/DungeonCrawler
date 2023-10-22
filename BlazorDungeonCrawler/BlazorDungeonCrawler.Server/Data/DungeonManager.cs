using BlazorDungeonCrawler.Server.Models;
using BlazorDungeonCrawler.Shared.Enumerators;

using BlazorDungeonCrawler.Database.Resources.Commands.Create;
using BlazorDungeonCrawler.Database.Resources.Commands.Delete;
using BlazorDungeonCrawler.Database.Resources.Commands.Update;

using BlazorDungeonCrawler.Database.Resources.Queries.Get;

using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;
using SharedAdventurer = BlazorDungeonCrawler.Shared.Models.Adventurer;
using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;
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
                int damage = Dice.RollDSix();
                int protection = Dice.RollDSix();

                List<int> adventurerRolls = new() { health, damage, protection };
                Message message = new Message($"ADVENTURER", adventurerRolls, null);

                message.AddChild(new Message($"ADVENTURER HEALTH {health}", health, null));
                message.AddChild(new Message($"ADVENTURER DAMAGE {damage}", damage, null));
                message.AddChild(new Message($"ADVENTURER PROTECTION {protection}", protection, null));

                messages.Add(message);

                Adventurer adventurer = new(health, damage, protection);

                //  Floors
                Floors floors = new();

                int depth = 1;
                Floor newFloor = new(depth);

                messages.Add(new Message($"DUNGEON DEPTH {depth}"));

                //  Tiles
                Tiles tiles = new(newFloor.Depth, newFloor.Rows, newFloor.Columns);
                newFloor.Tiles = tiles.GetTiles();

                floors.Add(newFloor);

                //  Dungon
                string apiVersion = new Version(0, 2, 0).ToString();
                SharedDungeon sharedDungeon = new() {
                    Id = Guid.NewGuid(),

                    Adventurer = adventurer.SharedModelMapper(),

                    Floors = floors.SharedModelMapper(),
                    Messages = messages.SharedModelMapper(),

                    Depth = depth,

                    ApiVersion = apiVersion
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
                if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }
                if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }

                //reset stairs
                dungeon.StairsDiscovered = false;

                Messages messages = new();
                Monsters monsters = new();

                Adventurer adventurer = new(dungeon.Adventurer);
                adventurer.DurationDecrement();

                SharedFloor? currentFloor = dungeon.Floors.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
                if (currentFloor == null) { throw new ArgumentNullException("Dungeon current Floor"); }

                Tiles currentFloorTiles = new(currentFloor.Tiles);

                Tile selectedTile = new();

                bool setSelectable = true;

                foreach (Tile tile in currentFloorTiles.GetTiles()) {
                    tile.Current = false;
                    tile.Selectable = false;

                    if (tile.Id == tileId) {
                        selectedTile = tile;

                        selectedTile.Hidden = false;
                        selectedTile.Current = true;
                    }
                }

                switch (selectedTile.Type) {
                    case DungeonEvents.DungeonEntrance:
                        if (!dungeon.MacGuffinFound) {
                            messages.Add(new Message("NO MACGUFFIN. GO FIND IT!"));
                        } else {
                            messages.Add(new Message("WELL DONE."));

                            currentFloorTiles.Unhide();
                            TilesUpdate.Update(currentFloorTiles.SharedModelMapper());

                            setSelectable = false;
                            //todo: show summary 
                        }
                        break;
                    case DungeonEvents.Fight:
                        if (!selectedTile.FightWon && selectedTile.Monsters.Count == 0) {
                            //todo: functionaise and merge with wandering monster generation
                            monsters = SetMonsters(selectedTile.Id, dungeon.Depth);

                            string monsterMessage;
                            if (monsters.Count() == 1) {
                                monsterMessage = $"MONSTER CAMP: A {monsters.GetName()}";
                            } else {
                                monsterMessage = $"MONSTER CAMP: {monsters.Count()} {monsters.GetName()}s";
                            }

                            string monsterDetails = string.Empty;
                            List<int> monsterDetailsRoll = new();
                            List<int> monsterDetailsRollCollection = new();
                            List<Message> monsterDetailsMessages = new();
                            foreach (Monster monster in monsters.Get()) {
                                monsterDetails = $"DAMAGE: {monster.Damage}.  HEALTH: {monster.Health}.  PROTECTION: {monster.Protection}.";
                                monsterDetailsRoll = new() { monster.Damage, monster.Health, monster.Protection };
                                monsterDetailsRollCollection.AddRange(monsterDetailsRoll);

                                monsterDetailsMessages.Add(new Message(monsterDetails, null, monsterDetailsRoll));
                            }

                            Message monsterGeneration = new(monsterMessage, null, monsterDetailsRollCollection);
                            foreach (Message monsterDetailsMessage in monsterDetailsMessages) {
                                monsterGeneration.AddChild(monsterDetailsMessage);
                            }

                            messages.Add(monsterGeneration);

                            selectedTile.Monsters = monsters.Get();
                        }

                        dungeon.InCombat = true;
                        dungeon.CombatTile = selectedTile.Id;

                        setSelectable = false;
                        break;
                    case DungeonEvents.StairsDescending:
                        //  Set surrounding selecteable tiles for when the user returns to this floor
                        currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                        TilesUpdate.Update(currentFloorTiles.SharedModelMapper());

                        int increasedDepth = dungeon.Depth + 1;
                        SharedFloor? deeperFloor = dungeon.Floors.Where(l => l.Depth == increasedDepth).FirstOrDefault();

                        //If deeper floor does not exist create it
                        if (deeperFloor == null || deeperFloor.Id == Guid.Empty) {
                            //Next floor
                            Floor newFloor = new(increasedDepth);
                            messages.Add(new Message($"DUNGEON DEPTH {increasedDepth} DISCOVERD"));

                            //  Tiles
                            Tiles newFloorTiles = new(newFloor.Depth, newFloor.Rows, newFloor.Columns);
                            newFloor.Tiles = newFloorTiles.GetTiles();

                            deeperFloor = newFloor.SharedModelMapper();

                            dungeon.Floors.Add(deeperFloor);

                            FloorCreate.Create(dungeon.Id, deeperFloor);

                            dungeon.StairsDiscovered = true;
                        } else {
                            dungeon.Depth = increasedDepth;
                            currentFloor = deeperFloor;
                            currentFloorTiles = new Tiles(deeperFloor.Tiles);
                        }

                        setSelectable = false;
                        break;
                    case DungeonEvents.StairsAscending:
                        //  Set surrounding selecteable tiles for when the user returns to this floor
                        currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                        TilesUpdate.Update(currentFloorTiles.SharedModelMapper());

                        int decreaseDepth = dungeon.Depth -= 1;
                        dungeon.Depth = decreaseDepth;

                        currentFloor = dungeon.Floors.Where(l => l.Depth == decreaseDepth).FirstOrDefault();
                        if (currentFloor == null || currentFloor.Id == Guid.Empty || currentFloor.Tiles == null) { throw new ArgumentNullException("Floor"); }

                        currentFloorTiles = new Tiles(currentFloor.Tiles);

                        setSelectable = false;
                        break;
                    case DungeonEvents.Chest:
                        int lootValue = Dice.RollDSix();
                        DungeonEvents lootTile = GetLootType(lootValue);
                        selectedTile.Type = lootTile;

                        switch (lootTile) {
                            case DungeonEvents.TakenWeapon:
                                int weaponsTypeValue = Dice.RollDSix();
                                int weaponsConditionValue = Dice.RollDSix();

                                Weapons weapons = new(dungeon.Depth, weaponsTypeValue, weaponsConditionValue);

                                int currentWeaponValue = adventurer.Weapon;

                                string weaponsMessage;
                                if (weapons.WeaponValue > currentWeaponValue) {
                                    adventurer.Weapon = weapons.WeaponValue;
                                    weaponsMessage = $"PICKUP A {weapons.Description()}";
                                } else {
                                    weaponsMessage = $"REJECTED A {weapons.Description()}";
                                }

                                Message weaponsMessages = new(weaponsMessage, new List<int> { weaponsTypeValue, weaponsConditionValue }, null);

                                weaponsMessages.AddChild(new Message($"Weapons condition: {weapons.Condition} (ROLL: {weaponsConditionValue})", weaponsConditionValue, null));
                                weaponsMessages.AddChild(new Message($"Weapons type: {weapons.Type} (ROLL: {weaponsTypeValue})", weaponsTypeValue, null));

                                weaponsMessages.AddChild(new Message($"Weapon value: {weapons.WeaponValue} ({weapons.TypeValue} * {weapons.ConditionValue})"));

                                messages.Add(weaponsMessages);
                                break;
                            case DungeonEvents.TakenProtection:
                                int armourTypeValue = Dice.RollDSix();
                                int armourConditionValue = Dice.RollDSix();

                                Armour armour = new(dungeon.Depth, armourTypeValue, armourConditionValue);

                                bool pickedUp = false;
                                switch (armour.Type) {
                                    case ArmourTypes.Helmet:
                                        if (armour.ArmourValue > adventurer.ArmourHelmet) {
                                            adventurer.ArmourHelmet = armour.ArmourValue;
                                            pickedUp = true;
                                        }
                                        break;
                                    case ArmourTypes.Breastplate:
                                        if (armour.ArmourValue > adventurer.ArmourBreastplate) {
                                            adventurer.ArmourBreastplate = armour.ArmourValue;
                                            pickedUp = true;
                                        }
                                        break;
                                    case ArmourTypes.Gauntlet:
                                        if (armour.ArmourValue > adventurer.ArmourGauntlet) {
                                            adventurer.ArmourGauntlet = armour.ArmourValue;
                                            pickedUp = true;
                                        }
                                        break;
                                    case ArmourTypes.Greave:
                                        if (armour.ArmourValue > adventurer.ArmourGreave) {
                                            adventurer.ArmourGreave = armour.ArmourValue;
                                            pickedUp = true;
                                        }
                                        break;
                                    case ArmourTypes.Boots:
                                        if (armour.ArmourValue > adventurer.ArmourBoots) {
                                            adventurer.ArmourBoots = armour.ArmourValue;
                                            pickedUp = true;
                                        }
                                        break;
                                    case ArmourTypes.Unknown:
                                    default:
                                        throw new ArgumentOutOfRangeException("Armour type selection");
                                }

                                string armourMessage;
                                if (pickedUp) {
                                    armourMessage = $"EQUIPT {armour.Description()}";
                                } else {
                                    armourMessage = $"REJECT {armour.Description()}";
                                }

                                Message armourMessages = new(armourMessage, new List<int> { armourConditionValue, armourTypeValue }, null);

                                armourMessages.AddChild(new Message($"Armour condition: {armour.Condition} (ROLL: {armourConditionValue})", armourConditionValue, null));
                                armourMessages.AddChild(new Message($"Armour type: {armour.Type} (ROLL: {armourTypeValue})", armourTypeValue, null));

                                armourMessages.AddChild(new Message($"Armour value: {armour.ArmourValue} ({armour.TypeValue} * {armour.ConditionValue})"));

                                messages.Add(armourMessages);
                                break;
                            case DungeonEvents.TakenPotion:
                                int potionTypeValue = Dice.RollDSix();
                                int potionSizeValue = Dice.RollDSix();
                                int potionDurationValue = Dice.RollDSix();

                                Potions potion = new(dungeon.Depth, potionTypeValue, potionSizeValue, potionDurationValue);

                                switch (potion.Type) {
                                    case PotionTypes.Aura:
                                        int regainedHealth = adventurer.SetAuraPotion(potion.SizeValue);
                                        if (adventurer.AuraPotion > 0) {
                                            adventurer.AuraPotionDuration += potion.DurationValue;
                                        }

                                        if (regainedHealth > 0) {
                                            messages.Add(new Message($"Regained {regainedHealth} health"));
                                        }
                                        break;
                                    case PotionTypes.Damage:
                                        adventurer.DamagePotion += potion.SizeValue;
                                        adventurer.DamagePotionDuration = +potion.DurationValue;
                                        break;
                                    case PotionTypes.Sheild:
                                        adventurer.ShieldPotion += potion.SizeValue;
                                        adventurer.ShieldPotionDuration += potion.DurationValue;
                                        break;
                                    case PotionTypes.Unknown:
                                        break;
                                }

                                Message potionMessages = new($"DRINK A {potion.Description()}", new List<int> { potionTypeValue, potionSizeValue, potionDurationValue }, null);

                                potionMessages.AddChild(new Message($"Potion type: {potion.Type} (ROLL: {potionTypeValue})", potionTypeValue, null));
                                potionMessages.AddChild(new Message($"Potion size: {potion.Size} (ROLL: {potionSizeValue})", potionSizeValue, null));
                                potionMessages.AddChild(new Message($"Potion duration: {potion.Duration} (ROLL: {potionDurationValue})", potionDurationValue, null));

                                messages.Add(potionMessages);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("Loot tile type.");
                        }
                        break;
                    case DungeonEvents.Macguffin:
                        monsters = SetMonsters(selectedTile.Id, 999);
                        messages.Add(new Message($"YOU HAVE UNCOVERED THE LAIR OF THE {monsters.GetName()}"));

                        selectedTile.Monsters = monsters.Get();

                        dungeon.InCombat = true;
                        dungeon.CombatTile = selectedTile.Id;

                        setSelectable = false;
                        break;
                    case DungeonEvents.Empty:
                    case DungeonEvents.FightWon:
                    case DungeonEvents.TakenWeapon:
                    case DungeonEvents.TakenProtection:
                    case DungeonEvents.TakenPotion:
                        int value = Dice.RollDSix();
                        if (value == 1) {
                            //todo: functionaise and merge with DungeonEvents.Fight monster generation
                            monsters = SetMonsters(selectedTile.Id, dungeon.Depth);

                            string monsterMessage;
                            if (monsters.Count() == 1) {
                                monsterMessage = $"WANDERING MONSTER: A {monsters.GetName()}";
                            } else {
                                monsterMessage = $"WANDERING MONSTER: {monsters.Count()} {monsters.GetName()}s";
                            }

                            string monsterDetails = string.Empty;
                            List<int> monsterDetailsRoll = new();
                            List<int> monsterDetailsRollCollection = new();
                            List<Message> monsterDetailsMessages = new();
                            foreach (Monster monster in monsters.Get()) {
                                monsterDetails = $"DAMAGE: {monster.Damage}.  HEALTH: {monster.Health}.  PROTECTION: {monster.Protection}.";
                                monsterDetailsRoll = new() { monster.Damage, monster.Health, monster.Protection };
                                monsterDetailsRollCollection.AddRange(monsterDetailsRoll);

                                monsterDetailsMessages.Add(new Message(monsterDetails, null, monsterDetailsRoll));
                            }

                            Message monsterGeneration = new(monsterMessage, null, monsterDetailsRollCollection);
                            foreach (Message monsterDetailsMessage in monsterDetailsMessages) {
                                monsterGeneration.AddChild(monsterDetailsMessage);
                            }

                            messages.Add(monsterGeneration);

                            selectedTile.Monsters = monsters.Get();

                            selectedTile.Type = DungeonEvents.Fight;

                            dungeon.InCombat = true;
                            dungeon.CombatTile = selectedTile.Id;

                            setSelectable = false;
                        }
                        break;
                    case DungeonEvents.FoundWeapon:
                    case DungeonEvents.FoundProtection:
                    case DungeonEvents.FoundPotion:
                    case DungeonEvents.Unknown:
                    default:
                        throw new ArgumentOutOfRangeException("Selected Dungeon Tiles Tile Type");
                }

                if (setSelectable) {
                    currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                }

                //Update Adventurer
                SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
                dungeon.Adventurer = sharedAdventurer;

                AdventurerUpdate.Update(sharedAdventurer);

                //Update Messages
                List<SharedMessage> sharedMessages = messages.SharedModelMapper();
                dungeon.Messages.AddRange(sharedMessages);
                MessagesCreate.Create(dungeon.Id, sharedMessages);

                List<SharedTile> sharedTiles = currentFloorTiles.SharedModelMapper();
                currentFloor.Tiles = sharedTiles;

                for (int i = 0; i < dungeon.Floors.Count; i++) {
                    if (dungeon.Floors[i].Id != currentFloor.Id) {
                        dungeon.Floors[i] = currentFloor;
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

        private Monsters SetMonsters(Guid tileId, int depth) {
            Monsters monsters = new();

            //generate new monsters
            monsters.Generate(depth);

            if (monsters.Count() > 0) {
                MonstersCreate.Create(tileId, monsters.SharedModelMapper());
            }

            return monsters;
        }

        //Loot
        //  1 - 2:  Potion
        //  3 - 4:  Weapon
        //  5 - 6:  Protection
        private DungeonEvents GetLootType(int value) {
            switch (value) {
                case 1:
                case 2:
                    return DungeonEvents.TakenPotion;
                case 3:
                case 4:
                    return DungeonEvents.TakenWeapon;
                case 5:
                case 6:
                    return DungeonEvents.TakenProtection;
                default:
                    throw new ArgumentOutOfRangeException("Loot type roll value");
            }
        }

        public async Task<SharedDungeon> AutomaticallyAdvanceDungeon(Guid dungeonId) {
            try {
                SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
                if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
                if (dungeon.Floors == null || dungeon.Floors.Count == 0) { throw new ArgumentNullException("Dungeon Floors"); }

                SharedAdventurer? adventurer = dungeon.Adventurer;
                if (adventurer == null || adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Adventurer"); }

                if (!adventurer.IsAlive) {
                    return dungeon;
                }

                SharedFloor? currentFloor = dungeon.Floors.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
                if (currentFloor == null || currentFloor.Tiles == null || currentFloor.Tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floors Tiles"); }

                if (dungeon.StairsDiscovered) {
                    return await DescendStairs(dungeonId);
                } else if (dungeon.InCombat) {
                    //fight the fight
                    SharedTile? currentTile = currentFloor.Tiles.Where(t => t.Current == true).FirstOrDefault();
                    if (currentTile == null || currentTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Floors current Tiles"); }

                    return await MonsterFight(dungeonId, currentTile.Id);
                } else {
                    //Select a tile to move to
                    Guid selectedTileId = Guid.Empty;
                    if (dungeon.MacGuffinFound) {
                        //plot a course between current position and stairs
                        SharedTile? current = currentFloor.Tiles.Where(t => t.Current == true).FirstOrDefault();
                        if (current != null && current.Id != Guid.Empty) {
                            int currentRow = current.Row;
                            int currentColumn = current.Column;

                            SharedTile? destination;
                            if (dungeon.Depth == 1) {
                                destination = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.DungeonEntrance).FirstOrDefault();
                            } else {
                                destination = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.StairsAscending).FirstOrDefault();
                            }

                            if (destination != null && destination.Id != Guid.Empty) {
                                int destinationRow = destination.Row;
                                int destinationColumn = destination.Column;

                                string directionOfTravel = getDirectionOfTravel(destinationRow, destinationColumn, currentRow, currentColumn);

                                List<SharedTile>? selectableTile = currentFloor.Tiles.Where(t => t.Selectable == true).Where(t => t.Type != DungeonEvents.StairsDescending).ToList();
                                foreach (SharedTile tile in selectableTile) {
                                    if (matchesDirectionOfTravel(directionOfTravel, tile.Row, tile.Column, currentRow, currentColumn)) {
                                        selectedTileId = tile.Id;
                                        break;
                                    }
                                }
                            }
                        }
                    } else {
                        //randomly selecte and hidden tile from the selectables
                        List<SharedTile>? hiddenSelectableTile = currentFloor.Tiles.Where(t => t.Selectable == true).Where(t => t.Hidden == true).ToList();
                        if (hiddenSelectableTile != null && hiddenSelectableTile.Count > 0) {
                            //if there are hidden tiles in the current selectable rnage
                            int tilesCount = hiddenSelectableTile.Count();
                            int randomTileIndex = Dice.RandomNumber(0, tilesCount - 1);

                            selectedTileId = hiddenSelectableTile[randomTileIndex].Id;
                        } else {
                            //if there are no hidden tiles in the selectable range
                            SharedTile? current = currentFloor.Tiles.Where(t => t.Current == true).FirstOrDefault();
                            if (current != null && current.Id != Guid.Empty) {
                                int currentRow = current.Row;
                                int currentColumn = current.Column;

                                List<SharedTile>? hiddenColumnTiles = currentFloor.Tiles.Where(t => t.Hidden == true).ToList();

                                string directionOfTravel = string.Empty;
                                SharedTile? targetTile = null;
                                List<SharedTile>? selectableTile = new();
                                if (hiddenColumnTiles != null && hiddenColumnTiles.Count > 0) {
                                    //create a distance score for each tile
                                    Dictionary<SharedTile, double> hiddenTiles = new();
                                    foreach (SharedTile tile in hiddenColumnTiles) {
                                        double distance = Math.Sqrt(Math.Pow((currentRow - tile.Row), 2) + Math.Pow((currentColumn - tile.Column), 2));
                                        hiddenTiles.Add(tile, distance);
                                    }

                                    targetTile = hiddenTiles.OrderBy(t => t.Value).First().Key;
                                    selectableTile = currentFloor.Tiles.Where(t => t.Selectable == true).Where(t => t.Type != DungeonEvents.StairsDescending).Where(t => t.Type != DungeonEvents.StairsAscending).Where(t => t.Type != DungeonEvents.DungeonEntrance).ToList();
                                } else {
                                    //if all hidden tiles have been uncoverd then make way to relivnt stairs 

                                    if (dungeon.MacGuffinFound) {
                                        if (dungeon.Depth == 1) {
                                            targetTile = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.DungeonEntrance).FirstOrDefault();
                                        } else {
                                            targetTile = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.StairsAscending).FirstOrDefault();
                                        }
                                    } else {
                                        targetTile = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.StairsDescending).FirstOrDefault();
                                    }

                                    selectableTile = currentFloor.Tiles.Where(t => t.Selectable == true).ToList();
                                }

                                if (selectableTile != null && selectableTile.Count > 0) {
                                    if (targetTile != null && targetTile.Id != Guid.Empty) {
                                        directionOfTravel = getDirectionOfTravel(targetTile.Row, targetTile.Column, currentRow, currentColumn);
                                    }

                                    foreach (SharedTile tile in selectableTile) {
                                        if (matchesDirectionOfTravel(directionOfTravel, tile.Row, tile.Column, currentRow, currentColumn)) {
                                            selectedTileId = tile.Id;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //todo: if no tiles were selected score the avalible ones

                    //go towards hidden
                    //go towards stairs decending
                    //away from fights


                    if (selectedTileId == Guid.Empty) {
                        List<SharedTile>? selectableTile = currentFloor.Tiles.Where(t => t.Selectable).ToList();
                        if (selectableTile != null && selectableTile.Count > 0) {
                            int tilesCount = selectableTile.Count();
                            int randomTileIndex = Dice.RandomNumber(0, tilesCount - 1);

                            selectedTileId = selectableTile[randomTileIndex].Id;
                        }
                    }

                    return await GetSelectedDungeonTiles(dungeonId, selectedTileId);
                }
            } catch (Exception ex) {
                throw;
            }
        }

        //todo: replace this with Manhattan distance and let the matchesDirectionOfTravel function chose the best tile for NE and SE conditions
        private string getDirectionOfTravel(int destinationRow, int destinationColumn, int currentRow, int currentColumn) {
            if (currentColumn == destinationColumn) {
                if (currentRow > destinationRow) {
                    return "N";
                } else {
                    return "S";
                }
            } else if (currentColumn < destinationColumn) {
                if ((currentColumn % 2) == 1) {
                    //long one
                    if (currentRow > destinationRow) {
                        return "NE";
                    } else {
                        return "SE";
                    }
                } else {
                    //short one
                    if (currentRow >= destinationRow) {
                        return "NE";
                    } else {
                        return "SE";
                    }
                }
            } else if (currentColumn > destinationColumn) {
                if ((currentColumn % 2) == 1) {
                    //long one
                    if (currentRow > destinationRow) {
                        return "NW";
                    } else {
                        return "SW";
                    }
                } else {
                    //short one
                    if (currentRow >= destinationRow) {
                        return "NW";
                    } else {
                        return "SW";
                    }
                }
            }

            return string.Empty;
        }

        private bool matchesDirectionOfTravel(string directionOfTravel, int tileRow, int tileColumn, int currentRow, int currentColumn) {
            switch (directionOfTravel) {
                case "N":
                    if (currentColumn == tileColumn && currentRow > tileRow) {
                        return true;
                    }
                    break;
                case "NE":
                    if (currentColumn < tileColumn) {
                        if ((currentColumn % 2) == 1) {
                            //long one
                            if (currentRow > tileRow) {
                                return true;
                            }
                        } else {
                            //short one
                            if (currentRow == tileRow) {
                                return true;
                            }
                        }
                    }
                    break;
                case "SE":
                    if (currentColumn < tileColumn) {
                        if ((currentColumn % 2) == 1) {
                            //long one
                            if (currentRow == tileRow) {
                                return true;
                            }

                        } else {
                            //short one
                            if (currentRow < tileRow) {
                                return true;
                            }
                        }
                    }
                    break;
                case "S":
                    if (currentColumn == tileColumn && currentRow < tileRow) {
                        return true;
                    }
                    break;
                case "SW":
                    if (currentColumn > tileColumn) {
                        if ((currentColumn % 2) == 1) {
                            //long one
                            if (currentRow == tileRow) {
                                return true;
                            }
                        } else {
                            //short one
                            if (currentRow < tileRow) {
                                return true;
                            }
                        }
                    }
                    break;
                case "NW":
                    if (currentColumn > tileColumn) {
                        if ((currentColumn % 2) == 1) {
                            //long one
                            if (currentRow > tileRow) {
                                return true;
                            }
                        } else {
                            //short one
                            if (currentRow == tileRow) {
                                return true;
                            }
                        }
                    }
                    break;
            }

            return false;
        }

        public async Task<SharedDungeon> DescendStairs(Guid dungeonId) {
            await Task.Delay(1);

            try {
                SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
                if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
                if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }

                int increasedDepth = dungeon.Depth += 1;
                SharedFloor? currentFloor = dungeon.Floors.Where(l => l.Depth == increasedDepth).FirstOrDefault();
                if (currentFloor == null || currentFloor.Id == Guid.Empty || currentFloor.Tiles == null) {
                    throw new Exception("Floor not found");
                }

                dungeon.StairsDiscovered = false;

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
                if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }
                if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }

                SharedFloor? currentFloor = dungeon.Floors.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
                if (currentFloor == null || currentFloor.Tiles == null || currentFloor.Tiles.Count == 0) { throw new ArgumentNullException("Dungeon Floor Tiles"); }

                SharedTile? selectedTile = currentFloor.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Floor selected Tile"); }

                Tiles currentFloorTiles = new(currentFloor.Tiles);

                Messages messages = new();

                Adventurer adventurer = new(dungeon.Adventurer);
                adventurer.DurationDecrement();

                if (AdventurerFleesCombat()) {
                    dungeon.InCombat = false;
                    dungeon.CombatTile = Guid.Empty;
                    dungeon.CombatInitiated = false;

                    messages.Add(new Message("ADVENTURER FLEES COMBAT"));

                    currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                } else {
                    Message monsterFlee = new("ADVENTURER FAILED TO FLEE");

                    //todo: replace this with monster attack round function 
                    if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Floor Tile Monsters"); }

                    List<SharedMonster> monsters = selectedTile.Monsters;
                    int monsterindex = Dice.RandomNumber(0, (monsters.Count() - 1));
                    Monster currentMonster = new(monsters[monsterindex]);

                    int adventurerProtection = adventurer.GetProtection();
                    int monsterDamage = currentMonster.Damage;

                    //Adventurer wounds
                    int adventurerWounds = monsterDamage - adventurerProtection;
                    if (adventurerWounds > 0) {
                        int currentHealth = adventurer.HealthBase - adventurerWounds;
                        if (currentHealth > 0) {
                            adventurer.HealthBase = currentHealth;

                            monsterFlee = new Message($"MONSTER ATTACK: ADVENTURER HIT FOR {adventurerWounds} WITH {adventurer.HealthBase} REMAINING");
                            monsterFlee.AddChild(new Message($"MONSTER DAMAGE {monsterDamage} ADVENTURER PROTECTION {adventurerProtection}"));
                        } else {
                            adventurer.HealthBase = 0;
                            adventurer.IsAlive = false;

                            selectedTile.Type = DungeonEvents.FightLost;

                            monsterFlee = new Message($"MONSTER ATTACK: ADVENTURER DIED WITH {adventurerWounds} WOUNDS");
                            monsterFlee.AddChild(new Message($"MONSTER DAMAGE {monsterDamage} ADVENTURER PROTECTION {adventurerProtection}"));

                            dungeon.InCombat = false;

                            //todo: unhide tiles for all floors upon death
                            //todo: remove all current and selectable from each floor
                            currentFloorTiles.Unhide();
                        }
                    } else {
                        monsterFlee = new Message("MONSTER ATTACK: NO DAMAGE DONE TO ADVENTURER");
                        monsterFlee.AddChild(new Message($"MONSTER DAMAGE {monsterDamage} ADVENTURER PROTECTION {adventurerProtection}"));
                    }

                    messages.Add(monsterFlee);
                }

                //Update Adventurer
                SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
                dungeon.Adventurer = sharedAdventurer;

                AdventurerUpdate.Update(sharedAdventurer);

                //Update Messages
                List<SharedMessage> sharedMessages = messages.SharedModelMapper();
                dungeon.Messages.AddRange(sharedMessages);
                MessagesCreate.Create(dungeon.Id, sharedMessages);

                //Update Tiles
                List<SharedTile> sharedTiles = currentFloorTiles.SharedModelMapper();

                //  Current Tile
                SharedTile currentSharedTiles;
                for (int i = 0; i < sharedTiles.Count; i++) {
                    currentSharedTiles = sharedTiles[i];
                    if (currentSharedTiles.Id == selectedTile.Id) {
                        sharedTiles[i] = selectedTile;
                    }
                }

                currentFloor.Tiles = sharedTiles;

                for (int i = 0; i < dungeon.Floors.Count; i++) {
                    if (dungeon.Floors[i].Id != currentFloor.Id) {
                        dungeon.Floors[i] = currentFloor;
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
                if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floor"); }

                SharedFloor? currentFloor = dungeon.Floors.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
                if (currentFloor == null) { throw new ArgumentNullException("Dungeon current Floor"); }

                Tiles currentFloorTiles = new(currentFloor.Tiles);

                //todo: get this from dungeon 
                SharedTile? selectedTile = currentFloor.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Floor selected Tile"); }
                if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Floor Tile Monsters"); }


                List<SharedMonster> monsters = selectedTile.Monsters.OrderBy(m => m.Index).ToList();

                //Adventurer details
                //todo: get this from dungeon 
                Adventurer adventurer = new(dungeon.Adventurer);
                adventurer.DurationDecrement();

                int adventurerDamage = adventurer.GetDamage();
                int adventurerProtection = adventurer.GetProtection();


                Message? summaryMessage = null;

                Message? combatInitiated = null;
                Message? adventurerCombatResult = null;
                Message? monsterCombatResult = null;

                bool adventurerInitiatesCombat = true;
                if (!dungeon.CombatInitiated) {
                    int adventurerRoll = Dice.RollDSix();
                    int monsterRoll = Dice.RollDSix();

                    if (adventurerRoll > monsterRoll) {
                        adventurerInitiatesCombat = true;
                        combatInitiated = new Message("ADVENTURER INITIATES COMBAT", adventurerRoll, monsterRoll);
                    } else {
                        adventurerInitiatesCombat = false;
                        combatInitiated = new Message("MONSTER INITIATES COMBAT", adventurerRoll, monsterRoll);
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
                        monsterWounds = adventurerDamage - monsterProtection;

                        if (monsterWounds > 0) {
                            int currentHealth = monsterHealth - monsterWounds;
                            if (currentHealth > 0) {
                                currentMonster.Health = currentHealth;
                                SharedMonster sharedMonster = currentMonster.SharedModelMapper();

                                MonsterUpdate.Update(sharedMonster);

                                monsters[monsterindex] = sharedMonster;

                                string importantMessage = $"ADVENTURER ATTACK: MONSTER HIT FOR {monsterWounds} WITH {currentHealth} REMAINING";
                                summaryMessage = new Message(importantMessage);
                                adventurerCombatResult = new Message(importantMessage);

                                if (combatInitiated != null) {
                                    adventurerCombatResult.AddChild(combatInitiated);
                                }

                                adventurerCombatResult.AddChild(new Message($"ADVENTURER'S ATTACK {adventurerAttackValue} WINS OVER MONSTER DODGE {monsterDodgeValue}", adventurerAttackDice, monsterDodgeRolls));
                            } else {
                                monsterHealth = 0;

                                adventurer.Experience += currentMonster.Experience;

                                //if the user kills the Beholder
                                string monsterKilled;
                                if (currentMonster.TypeName == "Beholder") {
                                    monsterKilled = $"ADVENTURER ATTACK: BOSS KILLED FIND YOUR WAY OUT";
                                    dungeon.MacGuffinFound = true;
                                } else {
                                    monsterKilled = $"ADVENTURER ATTACK: MONSTER KILLED WITH {monsterWounds}";
                                }

                                summaryMessage = new Message(monsterKilled);
                                adventurerCombatResult = new Message(monsterKilled);

                                if (combatInitiated != null) {
                                    adventurerCombatResult.AddChild(combatInitiated);
                                }

                                //remove monster at stack
                                MonsterDelete.Delete(currentMonster.Id);
                                monsters.RemoveAt(monsterindex);

                                //checked for remaining monsters
                                if (monsters.Count == 0) {
                                    //Update Adventurer
                                    int previousAdventurerLevel = adventurer.ExperienceLevel;
                                    int previousAdventurerHealth = adventurer.HealthBase;
                                    int previousAdventurerDamage = adventurer.DamageBase;
                                    int previousAdventurerProtection = adventurer.ProtectionBase;

                                    adventurer.LevelUp();
                                    if (adventurer.ExperienceLevel > previousAdventurerLevel) {
                                        int currentAdventurerLevel = adventurer.ExperienceLevel;
                                        int currentAdventurerHealth = adventurer.HealthBase;
                                        int currentAdventurerDamage = adventurer.DamageBase;
                                        int currentAdventurerProtection = adventurer.ProtectionBase;

                                        Message levelUp = new Message($"NEW LEVEL ({currentAdventurerLevel})");
                                        levelUp.AddChild(new Message($"NEW HEALTH {adventurer.HealthBase} (+ {currentAdventurerHealth - previousAdventurerHealth})"));
                                        levelUp.AddChild(new Message($"NEW DAMAGE {adventurer.DamageBase} (+ {currentAdventurerDamage - previousAdventurerDamage})"));
                                        levelUp.AddChild(new Message($"NEW PROTECTION {adventurer.ProtectionBase} (+ {currentAdventurerProtection - previousAdventurerProtection})"));

                                        adventurerCombatResult.AddChild(levelUp);
                                    }

                                    //Update Dungeon
                                    dungeon.InCombat = false;
                                    dungeon.CombatTile = Guid.Empty;
                                    dungeon.CombatInitiated = false;

                                    //Update Level
                                    selectedTile.FightWon = true;
                                    selectedTile.Type = DungeonEvents.FightWon;

                                    currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                                };
                            }
                        } else {
                            string adventurerCombatWiff = "ADVENTURER ATTACK: NO DAMAGE DONE TO MONSTER";

                            summaryMessage = new Message(adventurerCombatWiff);
                            adventurerCombatResult = new Message(adventurerCombatWiff);
                            adventurerCombatResult.AddChild(new Message($"ADVENTURER DAMAGE ({adventurerDamage}) MONSTER PROTECTION ({monsterProtection})"));
                        }

                        selectedTile.Monsters = monsters;
                    } else {
                        adventurerCombatResult = new Message($"ADVENTURER ATTACK: MONSTER DODGE {monsterDodgeValue} WINS OVER ADVENTURER ATTACK {adventurerAttackValue}", adventurerAttackDice, monsterDodgeRolls);
                    }
                }

                //Monster attack
                if (dungeon.InCombat) {
                    int monsterDamage, monsterProtection, monsterHealth;
                    foreach (SharedMonster monster in selectedTile.Monsters) {

                        //todo: replace this with monster attack round function

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
                            adventurerWounds = monsterDamage - adventurerProtection;

                            if (adventurerWounds > 0) {
                                int currentHealth = adventurer.HealthBase - adventurerWounds;
                                if (currentHealth > 0) {
                                    adventurer.HealthBase = currentHealth;

                                    string importantMessage = $"MONSTER ATTACK: ADVENTURER HIT FOR {adventurerWounds} WITH {adventurer.HealthBase} REMAINING";
                                    summaryMessage = new Message(importantMessage);
                                    monsterCombatResult = new Message(importantMessage);
                                    monsterCombatResult.AddChild(new Message($"MONSTER DAMAGE {monsterDamage} ADVENTURER PROTECTION {adventurerProtection}"));
                                } else {
                                    adventurer.HealthBase = 0;
                                    adventurer.IsAlive = false;

                                    selectedTile.Type = DungeonEvents.FightLost;

                                    string importantMessage = $"MONSTER ATTACK: ADVENTURER DIED WITH {adventurerWounds} WOUNDS";
                                    summaryMessage = new Message(importantMessage);
                                    monsterCombatResult = new Message(importantMessage);
                                    monsterCombatResult.AddChild(new Message($"MONSTER DAMAGE {monsterDamage} ADVENTURER PROTECTION {adventurerProtection}"));

                                    dungeon.InCombat = false;

                                    //todo: unhide tiles for all floors upon death
                                    //todo: remove all current and selectable from each floor
                                    currentFloorTiles.Unhide();

                                    break;
                                }

                                if (combatInitiated != null) {
                                    monsterCombatResult.AddChild(combatInitiated);
                                }

                                monsterCombatResult.AddChild(new Message($"MONSTER ATTACK {monsterAttackValue} WINS OVER ADVENTURER DODGE {adventurerDodgeValue}", adventurerDodgeRolls, monsterAttackDice));
                            } else {
                                string monsterCombatWiff = "MONSTER ATTACK: NO DAMAGE DONE TO ADVENTURER";

                                summaryMessage = new Message(monsterCombatWiff);
                                monsterCombatResult = new Message(monsterCombatWiff);
                                monsterCombatResult.AddChild(new Message($"MONSTER DAMAGE ({monsterDamage}) ADVENTURER PROTECTION ({adventurerProtection})"));
                            }
                        } else {
                            monsterCombatResult = new Message($"MONSTER ATTACK: ADVENTURER DODGE {monsterAttackValue} WINS OVER MONSTER ATTACK {adventurerDodgeValue}", adventurerDodgeRolls, monsterAttackDice);

                            if (combatInitiated != null) {
                                monsterCombatResult.AddChild(combatInitiated);
                            }
                        }
                    }
                }

                if (summaryMessage == null) {
                    summaryMessage = new Message("NO DAMAGE DONE");
                }

                if (adventurerCombatResult != null) {
                    summaryMessage.AddChild(adventurerCombatResult);
                }

                if (monsterCombatResult != null) {
                    summaryMessage.AddChild(monsterCombatResult);
                }

                Messages messages = new Messages();
                messages.Add(summaryMessage);

                SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
                dungeon.Adventurer = sharedAdventurer;

                AdventurerUpdate.Update(sharedAdventurer);

                //Update Messages
                List<SharedMessage> sharedMessages = messages.SharedModelMapper();
                dungeon.Messages.AddRange(sharedMessages);
                MessagesCreate.Create(dungeon.Id, sharedMessages);

                //Update Tiles
                List<SharedTile> sharedTiles = currentFloorTiles.SharedModelMapper();

                //  Current Tile
                SharedTile currentSharedTiles;
                for (int i = 0; i < sharedTiles.Count; i++) {
                    currentSharedTiles = sharedTiles[i];
                    if (currentSharedTiles.Id == selectedTile.Id) {
                        sharedTiles[i] = selectedTile;
                    }
                }

                currentFloor.Tiles = sharedTiles;

                for (int i = 0; i < dungeon.Floors.Count; i++) {
                    if (dungeon.Floors[i].Id != currentFloor.Id) {
                        dungeon.Floors[i] = currentFloor;
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
    }
}
