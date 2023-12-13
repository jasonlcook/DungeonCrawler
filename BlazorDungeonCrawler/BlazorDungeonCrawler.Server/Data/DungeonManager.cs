//**********************************************************************************************************************
//  DungeonManager
//  The DungeonManager class contains the functionality to progress, parse, save and return the Dungeon state

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using BlazorDungeonCrawler.Shared.Enumerators;
using BlazorDungeonCrawler.Server.Models;

//  Database methods
using BlazorDungeonCrawler.Server.Database;
using BlazorDungeonCrawler.Server.Database.Resources.Commands.Create;
using BlazorDungeonCrawler.Server.Database.Resources.Commands.Delete;
using BlazorDungeonCrawler.Server.Database.Resources.Commands.Update;
using BlazorDungeonCrawler.Server.Database.Resources.Queries.Get;

//  Namespace aliasing
//  The database is built using the shared models, however the related models methods are limited in scope to the server project.
using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;
using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;
using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Data {
    public class DungeonManager {
        private readonly ILogger _logger;                                       //  Logger.               Errors and important information is returned to Azure application insights and the debug console.
        private readonly IStringLocalizer<DungeonManager> _localiser;           //  Localiser.            All messages returned to the user from the localiser via the DungeonManager resource file. 
        private readonly IDbContextFactory<DungeonDbContext> _contextFactory;   //	Database Context.     Reference to the database context.

        private readonly MessageManager _messageManager;

        private readonly string _apiVersion = new Version(0, 2, 1).ToString();   //	API version.

        //***********************************************************
        //*********************************************** Constructor
        public DungeonManager(IDbContextFactory<DungeonDbContext> contextFactory, ILogger<DungeonManager> logger, IStringLocalizer<DungeonManager> localiser) {
            this._localiser = localiser;
            this._messageManager = new MessageManager(_localiser);

            this._contextFactory = contextFactory;
            this._logger = logger;

            _logger.LogInformation("Dungeon manager initiated. {DT}", DateTime.UtcNow.ToLongTimeString());
        }

        //***********************************************************
        //************************************************** Generate

        //	Generate new Dungeon and child elements 

        public async Task<SharedDungeon> Generate() {
            //	Create
            //	  Adventurer
            int health = Dice.RollDSix();
            int dexterity = Dice.RollDSix();
            int damage = Dice.RollDSix();
            int protection = Dice.RollDSix();

            List<int> adventurerRolls = new() { health, damage, protection };

            Message message = new(_messageManager.AdventureGeneration(health, damage, protection), adventurerRolls, null);

            message.AddChild(new(_messageManager.AdventureHealth(health), health, null));
            message.AddChild(new(_messageManager.AdventureDexterity(dexterity), dexterity, null));
            message.AddChild(new(_messageManager.AdventureDamage(damage), damage, null));
            message.AddChild(new(_messageManager.AdventureProtection(protection), protection, null));

            Messages messages = new();
            messages.Add(message);

            Adventurer adventurer = new(dexterity, health, damage, protection);

            //	  Floors
            Floors floors = new();

            int depth = 1;
            Floor newFloor = new(depth);
            newFloor.IsCurrent = true;

            //	  Tiles
            newFloor.Tiles = new(newFloor.Depth, newFloor.Rows, newFloor.Columns);

            floors.Add(newFloor);

            //	  Dungon
            Dungeon dungeon = new() {
                Id = Guid.NewGuid(),

                Adventurer = adventurer,

                Floors = floors,
                Messages = messages,

                Depth = depth,

                ApiVersion = _apiVersion
            };

            DungeonCreate dungeonCreate = new(_contextFactory.CreateDbContext(), _logger);

            SharedDungeon sharedDungeon = dungeon.SharedModelMapper();
            sharedDungeon.NewDungeon = true;

            await dungeonCreate.Create(sharedDungeon);

            _logger.LogInformation($"Dungeon {dungeon.Id} generated.");

            return sharedDungeon;
        }

        //***********************************************************
        //******************************************* RetrieveDungeon

        //	Retrieve Dungeon and child elements from database

        public async Task<SharedDungeon> RetrieveDungeon(Guid dungeonId) {
            if (dungeonId == Guid.Empty) { throw new ArgumentNullException("Dungeon Id"); }

            _logger.LogInformation($"Dungeon {dungeonId.ToString()} retrieve.");

            DungeonQueries dungeonQueries = new(_contextFactory.CreateDbContext(), _logger);
            Dungeon dungeon = new(await dungeonQueries.Get(dungeonId));

            if (dungeon == null || dungeon.Id == Guid.Empty) {
                throw new Exception(_messageManager.ErrorDungeonNoFound());
            }

            return dungeon.SharedModelMapper();
        }

        //***********************************************************
        //*********************************** GetSelectedDungeonTiles

        //	Process current Dungeon Tile

        public async Task<SharedDungeon> GetSelectedDungeonTiles(Guid dungeonId, Guid tileId) {
            if (dungeonId == Guid.Empty) { throw new ArgumentNullException("Dungeon Id"); }
            if (tileId == Guid.Empty) { throw new ArgumentNullException("Tile Id"); }

            _logger.LogInformation($"Dungeon {dungeonId} tile {tileId} selected.");

            Dungeon dungeon = new(await RetrieveDungeon(dungeonId));

            if (dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }
            if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }

            //reset stairs
            dungeon.StairsDiscovered = false;

            Messages messages = new();
            Monsters monsters = new();

            Adventurer adventurer = dungeon.Adventurer;
            adventurer.DurationDecrement();

            Floor currentFloor = dungeon.Floors.Get(dungeon.Depth);
            if (currentFloor == null) { throw new ArgumentNullException("Dungeon current Floor"); }

            Tiles currentFloorTiles = currentFloor.Tiles;

            Tile selectedTile = new();

            bool setSelectable = true;

            foreach (Tile tile in currentFloorTiles.Get()) {
                tile.Current = false;
                tile.Selectable = false;

                if (tile.Id == tileId) {
                    selectedTile = tile;

                    selectedTile.Visited = true;
                    selectedTile.Hidden = false;
                    selectedTile.Current = true;
                }
            }

            switch (selectedTile.Type) {
                case DungeonEvents.DungeonEntrance:
                    if (!dungeon.MacGuffinFound) {
                        messages.Add(new(_messageManager.DungeonExitFail()));
                    } else {
                        dungeon.GameOver = true;

                        Message gameOverMessage = await GenerateGameOverMessage(_messageManager.DungeonExitSuccess(), dungeon.SharedModelMapper());
                        messages.Add(gameOverMessage);

                        currentFloorTiles.Unhide();

                        setSelectable = false;
                    }
                    break;
                case DungeonEvents.Fight:
                    if (!selectedTile.FightWon && selectedTile.Monsters.Count == 0) {
                        monsters = await SetMonsters(selectedTile.Id, dungeon.Depth);

                        string monsterMessage;
                        if (monsters.Count() == 1) {
                            monsterMessage = _messageManager.MonsterCampSingle(monsters.GetName());
                        } else {
                            monsterMessage = _messageManager.MonsterCampMultiple(monsters.GetName(), monsters.Count().ToString());
                        }

                        string monsterDetails = string.Empty;
                        List<int> monsterDetailsRollCollection = new();

                        List<Message> monsterDetailsMessages = new();
                        foreach (Monster monster in monsters.Get()) {
                            List<int> monsterDetailsRoll = new() { monster.Health, monster.Damage, monster.Protection };
                            monsterDetailsRollCollection.AddRange(monsterDetailsRoll);

                            Message monsterDetailsMessage = new(_messageManager.MonsterGeneration(monster.TypeName), null, monsterDetailsRoll);

                            monsterDetailsMessage.AddChild(new(_messageManager.MonsterHealth(monster.Health), null, monster.Health));
                            monsterDetailsMessage.AddChild(new(_messageManager.MonsterDamage(monster.Damage), null, monster.Damage));
                            monsterDetailsMessage.AddChild(new(_messageManager.MonsterProtection(monster.Protection), null, monster.Protection));

                            monsterDetailsMessages.Add(monsterDetailsMessage);
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
                    //Set surrounding selecteable tiles for when the user returns to this floor
                    currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);

                    int increasedDepth = dungeon.Depth + 1;

                    FloorQueries floorDescendingQueries = new(_contextFactory.CreateDbContext(), _logger);
                    SharedFloor sharedFloor = await floorDescendingQueries.Get(dungeonId, increasedDepth);

                    //If deeper floor does not exist create it
                    if (sharedFloor == null || sharedFloor.Id == Guid.Empty) {
                        //Next floor
                        Floor newFloor = new(increasedDepth);
                        messages.Add(new(_messageManager.DungeonIncreasedDepth(increasedDepth)));

                        //  Tiles
                        Tiles newFloorTiles = new(newFloor.Depth, newFloor.Rows, newFloor.Columns);
                        newFloor.Tiles = newFloorTiles;

                        //  Create
                        FloorCreate floorCreate = new(_contextFactory.CreateDbContext(), _logger);
                        await floorCreate.Create(dungeon.Id, newFloor.SharedModelMapper());

                        //Set stiars discover prompt 
                        dungeon.StairsDiscovered = true;
                    } else {
                        FloorUpdate floorDescendingUpdate = new(_contextFactory.CreateDbContext(), _logger);

                        currentFloor.IsCurrent = false;
                        await floorDescendingUpdate.Update(currentFloor.SharedModelMapper());

                        sharedFloor.IsCurrent = true;
                        await floorDescendingUpdate.Update(sharedFloor);

                        Floor deeperFloor = new(sharedFloor);

                        dungeon.Depth = deeperFloor.Depth;

                        dungeon.Floors = new(deeperFloor);

                        currentFloor = deeperFloor;
                        currentFloorTiles = deeperFloor.Tiles;
                    }

                    setSelectable = false;
                    break;
                case DungeonEvents.StairsAscending:
                    //  Set surrounding selecteable tiles for when the user returns to this floor
                    currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);

                    //update floor left
                    Floor floorLeft = dungeon.Floors.Get(dungeon.Depth);
                    floorLeft.IsCurrent = false;

                    FloorUpdate floorUpdate = new(_contextFactory.CreateDbContext(), _logger);
                    await floorUpdate.Update(floorLeft.SharedModelMapper());

                    //get floor enterd
                    int decreaseDepth = dungeon.Depth -= 1;

                    FloorQueries floorAscendingQueries = new(_contextFactory.CreateDbContext(), _logger);
                    Floor floorEntered = new(await floorAscendingQueries.Get(dungeonId, decreaseDepth));

                    if (floorEntered == null || floorEntered.Id == Guid.Empty || floorEntered.Tiles == null) {
                        throw new Exception("Floor not found");
                    }

                    floorEntered.IsCurrent = true;
                    dungeon.Floors = new(floorEntered);

                    if (currentFloor == null || currentFloor.Id == Guid.Empty || currentFloor.Tiles == null) { throw new ArgumentNullException("Floor"); }

                    currentFloorTiles = floorEntered.Tiles;

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

                            Weapon weapons = new(dungeon.Depth, weaponsTypeValue, weaponsConditionValue);

                            int currentWeaponValue = adventurer.Weapon;

                            string weaponsMessage;
                            if (weapons.WeaponValue > currentWeaponValue) {
                                adventurer.Weapon = weapons.WeaponValue;
                                weaponsMessage = _messageManager.AdventurerWeaponEquipped(weapons.Description());
                            } else {
                                weaponsMessage = _messageManager.AdventurerWeaponRejected(weapons.Description());
                            }

                            Message weaponsMessages = new(weaponsMessage, new List<int> { weaponsTypeValue, weaponsConditionValue }, null);

                            weaponsMessages.AddChild(new(_messageManager.AdventurerWeaponCondition(weapons.Condition, weaponsConditionValue), weaponsConditionValue, null));
                            weaponsMessages.AddChild(new(_messageManager.AdventurerWeaponType(weapons.Type, weaponsTypeValue), weaponsTypeValue, null));
                            weaponsMessages.AddChild(new(_messageManager.AdventurerWeaponValue(weapons.WeaponValue, weapons.TypeValue, weapons.ConditionValue), weaponsTypeValue, null));

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
                                armourMessage = _messageManager.AdventurerArmourEquipped(armour.Description());
                            } else {
                                armourMessage = _messageManager.AdventurerArmourRejected(armour.Description());
                            }

                            Message armourMessages = new(armourMessage, new List<int> { armourConditionValue, armourTypeValue }, null);

                            armourMessages.AddChild(new(_messageManager.AdventurerArmourCondition(armour.Condition, armourConditionValue), armourConditionValue, null));
                            armourMessages.AddChild(new(_messageManager.AdventurerArmourType(armour.Type, armourTypeValue), armourTypeValue, null));
                            armourMessages.AddChild(new(_messageManager.AdventurerArmourValue(armour.ArmourValue, armour.TypeValue, armour.ConditionValue), armourConditionValue, null));

                            messages.Add(armourMessages);
                            break;
                        case DungeonEvents.TakenPotion:
                            int potionTypeValue = Dice.RollDSix();
                            int potionSizeValue = Dice.RollDSix();
                            int potionDurationValue = Dice.RollDSix();

                            Potion potion = new(dungeon.Depth, potionTypeValue, potionSizeValue, potionDurationValue);

                            switch (potion.Type) {
                                case PotionTypes.Aura:
                                    int regainedHealth = adventurer.SetAuraPotion(potion.SizeValue);
                                    if (adventurer.AuraPotion > 0) {
                                        adventurer.AuraPotionDuration += potion.DurationValue;
                                    }

                                    if (regainedHealth > 0) {
                                        messages.Add(new($"Regained {regainedHealth} health"));
                                    }
                                    break;
                                case PotionTypes.Damage:
                                    adventurer.DamagePotion += potion.SizeValue;
                                    adventurer.DamagePotionDuration = +potion.DurationValue;
                                    break;
                                case PotionTypes.Shield:
                                    adventurer.ShieldPotion += potion.SizeValue;
                                    adventurer.ShieldPotionDuration += potion.DurationValue;
                                    break;
                                case PotionTypes.Unknown:
                                    break;
                            }

                            Message potionMessages = new(_messageManager.AdventurerPotionDrink(potion.Description()), new List<int> { potionTypeValue, potionSizeValue, potionDurationValue }, null);

                            potionMessages.AddChild(new(_messageManager.AdventurerPotionType(potion.Type, potionTypeValue), potionTypeValue, null));
                            potionMessages.AddChild(new(_messageManager.AdventurerPotionSize(potion.Size, potionSizeValue), potionSizeValue, null));
                            potionMessages.AddChild(new(_messageManager.AdventurerPotionDuration(potion.Duration, potionDurationValue), potionDurationValue, null));

                            messages.Add(potionMessages);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Loot tile type.");
                    }
                    break;
                case DungeonEvents.Macguffin:
                    monsters = await SetMonsters(selectedTile.Id, 999);

                    messages.Add(new(_messageManager.FinalBossGeneration(monsters.GetName())));

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
                        monsters = await SetMonsters(selectedTile.Id, dungeon.Depth);

                        string monsterMessage;
                        if (monsters.Count() == 1) {
                            monsterMessage = _messageManager.WanderingMonsterSingle(monsters.GetName());
                        } else {
                            monsterMessage = _messageManager.WanderingMonsterMultiple(monsters.GetName(), monsters.Count());
                        }

                        string monsterDetails = string.Empty;
                        List<int> monsterDetailsRollCollection = new();

                        List<Message> monsterDetailsMessages = new();
                        foreach (Monster monster in monsters.Get()) {
                            List<int> monsterDetailsRoll = new() { monster.Health, monster.Damage, monster.Protection };
                            monsterDetailsRollCollection.AddRange(monsterDetailsRoll);

                            Message monsterDetailsMessage = new(_messageManager.MonsterGeneration(monster.TypeName), null, monsterDetailsRoll);

                            monsterDetailsMessage.AddChild(new(_messageManager.MonsterHealth(monster.Health), null, monster.Health));
                            monsterDetailsMessage.AddChild(new(_messageManager.MonsterDamage(monster.Damage), null, monster.Damage));
                            monsterDetailsMessage.AddChild(new(_messageManager.MonsterProtection(monster.Protection), null, monster.Protection));

                            monsterDetailsMessages.Add(monsterDetailsMessage);
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
            dungeon.Adventurer = adventurer;

            //Update Messages
            MessagesCreate messagesCreate = new(_contextFactory.CreateDbContext(), _logger);
            await messagesCreate.Create(dungeon.Id, messages.SharedModelMapper());

            dungeon.Messages.AddRange(messages);

            ////Update Tiles
            currentFloor.Tiles = currentFloorTiles;

            dungeon.Floors.Update(currentFloor);

            //Update Dungon
            DungeonUpdate dungeonUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonUpdate.Update(dungeon.SharedModelMapper());

            return dungeon.SharedModelMapper();
        }

        private async Task<Monsters> SetMonsters(Guid tileId, int depth) {
            Monsters monsters = new(depth);

            if (monsters.Count() > 0) {
                MonstersCreate monstersCreate = new(_contextFactory.CreateDbContext(), _logger);
                await monstersCreate.Create(tileId, monsters.SharedModelMapper());
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


        //***********************************************************
        //******************************* AutomaticallyAdvanceDungeon

        //	Automatically progress the Adventurer by selecting, the most desirable from the available tiles

        public async Task<SharedDungeon> AutomaticallyAdvanceDungeon(Guid dungeonId) {
            _logger.LogInformation($"Dungeon {dungeonId} automatically advanced.");

            Dungeon dungeon = new(await RetrieveDungeon(dungeonId));
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }

            if (dungeon.GameOver) {
                return dungeon.SharedModelMapper();
            }

            Adventurer adventurer = dungeon.Adventurer;
            if (adventurer == null || adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Adventurer"); }

            Floor currentFloor = dungeon.Floors.Get(dungeon.Depth);
            if (currentFloor == null || currentFloor.Tiles == null || currentFloor.Tiles.Count() == 0) { throw new ArgumentNullException("Dungeon Floors Tiles"); }

            if (dungeon.StairsDiscovered) {
                return await DescendStairs(dungeonId);
            } else if (dungeon.InCombat) {
                //fight the fight
                Tile? currentTile = currentFloor.Tiles.GetCurrent();
                if (currentTile == null || currentTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Floors current Tiles"); }

                return await MonsterFight(dungeonId, currentTile.Id);
            } else {
                //Select a tile to move to
                Guid selectedTileId = Guid.Empty;
                if (dungeon.MacGuffinFound) {
                    //plot a course between current position and stairs
                    Tile current = currentFloor.Tiles.GetCurrent();
                    if (current != null && current.Id != Guid.Empty) {
                        int currentRow = current.Row;
                        int currentColumn = current.Column;

                        Tile destination;
                        if (dungeon.Depth == 1) {
                            destination = currentFloor.Tiles.GetDungeonEntrance();
                        } else {
                            destination = currentFloor.Tiles.GetStairsAscending();
                        }

                        if (destination != null && destination.Id != Guid.Empty) {
                            int destinationRow = destination.Row;
                            int destinationColumn = destination.Column;

                            string directionOfTravel = getDirectionOfTravel(destinationRow, destinationColumn, currentRow, currentColumn);

                            List<Tile> selectableTile = currentFloor.Tiles.GetSelectableCurrentFloor();
                            foreach (Tile tile in selectableTile) {
                                if (matchesDirectionOfTravel(directionOfTravel, tile.Row, tile.Column, currentRow, currentColumn)) {
                                    selectedTileId = tile.Id;
                                    break;
                                }
                            }
                        }
                    }
                } else {
                    //randomly selecte and hidden tile from the selectables
                    List<Tile> hiddenSelectableTile = currentFloor.Tiles.GetHiddenSelectable();
                    if (hiddenSelectableTile != null && hiddenSelectableTile.Count > 0) {
                        //if there are hidden tiles in the current selectable rnage
                        int tilesCount = hiddenSelectableTile.Count();
                        int randomTileIndex = Dice.RandomNumber(0, tilesCount - 1);

                        selectedTileId = hiddenSelectableTile[randomTileIndex].Id;
                    } else {
                        //if there are no hidden tiles in the selectable range
                        Tile current = currentFloor.Tiles.GetCurrent();
                        if (current != null && current.Id != Guid.Empty) {
                            int currentRow = current.Row;
                            int currentColumn = current.Column;

                            List<Tile> hiddenColumnTiles = currentFloor.Tiles.GetHidden();

                            string directionOfTravel = string.Empty;
                            Tile targetTile = new();
                            List<Tile> selectableTile = new();
                            if (hiddenColumnTiles != null && hiddenColumnTiles.Count > 0) {
                                //create a distance score for each tile
                                Dictionary<Tile, double> hiddenTiles = new();
                                foreach (Tile tile in hiddenColumnTiles) {
                                    double distance = Math.Sqrt(Math.Pow((currentRow - tile.Row), 2) + Math.Pow((currentColumn - tile.Column), 2));
                                    hiddenTiles.Add(tile, distance);
                                }

                                targetTile = hiddenTiles.OrderBy(t => t.Value).First().Key;
                                selectableTile = currentFloor.Tiles.GetSelectableUnhighlightable();
                            } else {
                                //if all hidden tiles have been uncoverd then make way to relevant tile
                                if (dungeon.MacGuffinFound) {
                                    if (dungeon.Depth == 1) {
                                        //if first floor then get to the Dungeon entrance
                                        targetTile = currentFloor.Tiles.GetDungeonEntrance();
                                    } else {
                                        //if lower floor then get to the ascending stairs
                                        targetTile = currentFloor.Tiles.GetStairsAscending();
                                    }
                                } else {
                                    if (dungeon.Depth == 10) {
                                        //if lowest floor then get to the Macguffin
                                        targetTile = currentFloor.Tiles.GetMacguffin();
                                    } else {
                                        //if higher floor then get to the descending stairs
                                        targetTile = currentFloor.Tiles.GetStairsDescending();
                                    }
                                }

                                selectableTile = currentFloor.Tiles.GetSelectable();
                            }

                            if (selectableTile.Count > 0) {
                                if (targetTile.Id != Guid.Empty) {
                                    directionOfTravel = getDirectionOfTravel(targetTile.Row, targetTile.Column, currentRow, currentColumn);
                                }

                                foreach (Tile tile in selectableTile) {
                                    if (matchesDirectionOfTravel(directionOfTravel, tile.Row, tile.Column, currentRow, currentColumn)) {
                                        selectedTileId = tile.Id;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (selectedTileId == Guid.Empty) {
                    List<Tile> selectableTile = currentFloor.Tiles.GetSelectable();
                    if (selectableTile != null && selectableTile.Count > 0) {
                        int tilesCount = selectableTile.Count();
                        int randomTileIndex = Dice.RandomNumber(0, tilesCount - 1);

                        selectedTileId = selectableTile[randomTileIndex].Id;
                    }
                }

                return await GetSelectedDungeonTiles(dungeonId, selectedTileId);
            }
        }

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

        //***********************************************************
        //********************************************* DescendStairs

        //	Process confirmation of changing to lower floor

        public async Task<SharedDungeon> DescendStairs(Guid dungeonId) {
            _logger.LogInformation($"Descend Dungeon {dungeonId} stairs.");

            Dungeon dungeon = new(await RetrieveDungeon(dungeonId));
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }

            //update floor left
            Floor floorLeft = dungeon.Floors.Get(dungeon.Depth);
            floorLeft.IsCurrent = false;

            FloorUpdate floorUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await floorUpdate.Update(floorLeft.SharedModelMapper());

            //get floor enterd
            int increasedDepth = dungeon.Depth += 1;

            FloorQueries floorQueries = new(_contextFactory.CreateDbContext(), _logger);
            Floor floorEntered = new(await floorQueries.Get(dungeonId, increasedDepth));

            if (floorEntered == null || floorEntered.Id == Guid.Empty || floorEntered.Tiles == null) {
                throw new Exception("Floor not found");
            }

            floorEntered.IsCurrent = true;

            dungeon.Floors = new(floorEntered);

            dungeon.StairsDiscovered = false;

            //Update Dungon            
            SharedDungeon sharedDungeon = dungeon.SharedModelMapper();

            DungeonUpdate dungeonUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonUpdate.Update(sharedDungeon);

            return sharedDungeon;
        }

        //***********************************************************
        //*********************************************** MonsterFlee

        //	Adventurer flees from fight

        public async Task<SharedDungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            _logger.LogInformation($"Flee Dungeon {dungeonId} monster at tile {tileId}.");

            Dungeon dungeon = new(await RetrieveDungeon(dungeonId));
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }
            if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }

            Floor currentFloor = dungeon.Floors.Get(dungeon.Depth);
            if (currentFloor == null || currentFloor.Tiles == null || currentFloor.Tiles.Count() == 0) { throw new ArgumentNullException("Dungeon Floor Tiles"); }

            Tile selectedTile = currentFloor.Tiles.Get(tileId);
            if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Floor selected Tile"); }

            Tiles currentFloorTiles = currentFloor.Tiles;

            Messages messages = new();

            Adventurer adventurer = dungeon.Adventurer;
            adventurer.DurationDecrement();

            if (AdventurerFleesCombat()) {
                dungeon.InCombat = false;
                dungeon.CombatTile = Guid.Empty;
                dungeon.CombatInitiated = false;

                messages.Add(new(_messageManager.AdventurerFleeSuccess()));

                currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
            } else {
                Message monsterFlee = new(_messageManager.AdventurerFleeFail());

                if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Floor Tile Monsters"); }

                List<Monster> monsters = selectedTile.GetMonsters();
                int monsterindex = Dice.RandomNumber(0, (monsters.Count() - 1));
                Monster currentMonster = monsters[monsterindex];

                int adventurerProtection = adventurer.GetProtection();
                int monsterDamage = currentMonster.Damage;

                //Adventurer wounds
                int adventurerWounds = monsterDamage - adventurerProtection;
                if (adventurerWounds > 0) {
                    int currentHealth = adventurer.HealthBase - adventurerWounds;
                    if (currentHealth > 0) {
                        adventurer.HealthBase = currentHealth;

                        monsterFlee = new(_messageManager.MonsterAttackHit(adventurerWounds, adventurer.HealthBase));
                        monsterFlee.AddChild(new(_messageManager.MonsterAttackHitDamage(monsterDamage, adventurerProtection)));
                    } else {
                        adventurer.HealthBase = 0;
                        adventurer.IsAlive = false;

                        dungeon.GameOver = true;
                        Message gameOverMessage = await GenerateGameOverMessage(_messageManager.MonsterKilledAdventure(adventurerWounds), dungeon.SharedModelMapper());
                        monsterFlee = gameOverMessage;

                        selectedTile.Type = DungeonEvents.FightLost;

                        monsterFlee.AddChild(new(_messageManager.MonsterAttackHitDamage(monsterDamage, adventurerProtection)));

                        dungeon.InCombat = false;

                        currentFloorTiles.Unhide();
                    }
                } else {
                    monsterFlee = new(_messageManager.MonsterAttackNoDamage());

                    monsterFlee.AddChild(new(_messageManager.MonsterAttackHitDamage(monsterDamage, adventurerProtection)));
                }

                messages.Add(monsterFlee);
            }

            //Update Adventurer
            dungeon.Adventurer = adventurer;

            AdventurerUpdate adventurerUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await adventurerUpdate.Update(adventurer.SharedModelMapper());

            //Update Messages
            MessagesCreate messagesCreate = new(_contextFactory.CreateDbContext(), _logger);
            await messagesCreate.Create(dungeon.Id, messages.SharedModelMapper());

            dungeon.Messages.AddRange(messages);

            //Update Tiles
            List<SharedTile> sharedTiles = currentFloorTiles.SharedModelMapper();

            //  Current Tile
            currentFloor.Tiles = currentFloorTiles;

            SharedTile currentSharedTiles;
            for (int i = 0; i < sharedTiles.Count; i++) {
                currentSharedTiles = sharedTiles[i];
                if (currentSharedTiles.Id == selectedTile.Id) {
                    sharedTiles[i] = selectedTile.SharedModelMapper();
                }
            }

            //Update Dungon
            SharedDungeon sharedDungeon = dungeon.SharedModelMapper();

            DungeonUpdate dungeonUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonUpdate.Update(sharedDungeon);

            return sharedDungeon;
        }

        public bool AdventurerFleesCombat() {
            int adventurerRoll = Dice.RollDSix();
            int monsterRoll = Dice.RollDSix();

            if (adventurerRoll > monsterRoll) {
                return true;
            }

            return false;
        }

        //***********************************************************
        //********************************************** MonsterFight

        //	Adventurer fights monsters on current tile

        public async Task<SharedDungeon> MonsterFight(Guid dungeonId, Guid tileId) {
            _logger.LogInformation($"Fight Dungeon {dungeonId} monster at tile {tileId}.");

            Dungeon dungeon = new(await RetrieveDungeon(dungeonId));
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
            if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floor"); }

            Floor currentFloor = dungeon.Floors.Get(dungeon.Depth);
            if (currentFloor == null) { throw new ArgumentNullException("Dungeon current Floor"); }

            Tiles currentFloorTiles = currentFloor.Tiles;
            Tile selectedTile = currentFloor.Tiles.Get(tileId);
            List<Monster> monsters = selectedTile.GetMonsters();

            //Adventurer details
            Adventurer adventurer = dungeon.Adventurer;
            adventurer.DurationDecrement();

            int adventurerDexterity = adventurer.GetDexterity();
            int adventurerDamage = adventurer.GetDamage();
            int adventurerProtection = adventurer.GetProtection();

            Message? summaryMessage = null;

            Message? combatInitiated = null;
            Message? adventurerCombatResult = null;
            Message? monsterCombatResult = null;

            Message? gameOverMessage = null;

            bool adventurerInitiatesCombat = true;
            if (!dungeon.CombatInitiated) {
                int adventurerRoll = Dice.RollDSix();
                int monsterRoll = Dice.RollDSix();

                if (adventurerRoll > monsterRoll) {
                    adventurerInitiatesCombat = true;
                    combatInitiated = new(_messageManager.AdventurerInitiatesCombat(), adventurerRoll, monsterRoll);
                    combatInitiated.AddChild(new(_messageManager.AdventurerCombatInitiation(adventurerRoll), adventurerRoll, null));
                    combatInitiated.AddChild(new(_messageManager.MonsterCombatInitiation(monsterRoll), null, monsterRoll));
                } else {
                    adventurerInitiatesCombat = false;
                    combatInitiated = new(_messageManager.MonsterInitiatesCombats(), adventurerRoll, monsterRoll);
                    combatInitiated.AddChild(new(_messageManager.MonsterCombatInitiation(monsterRoll), null, monsterRoll));
                    combatInitiated.AddChild(new(_messageManager.AdventurerCombatInitiation(adventurerRoll), adventurerRoll, null));
                }

                dungeon.CombatInitiated = true;
            }

            if (adventurerInitiatesCombat) {
                //Monster defend
                int monsterindex = Dice.RandomNumber(0, (monsters.Count() - 1));
                Monster currentMonster = monsters[monsterindex];

                int monsterDexterity = currentMonster.Dexterity;
                int monsterProtection = currentMonster.Protection;
                int monsterHealth = currentMonster.Health;

                //Monster wounds
                int monsterWounds = 0;
                if (adventurerDexterity > monsterDexterity) {
                    monsterWounds = adventurerDamage - monsterProtection;

                    if (monsterWounds < 1) {
                        monsterWounds = 1;
                    }

                    if (monsterWounds > 0) {
                        int currentHealth = monsterHealth - monsterWounds;
                        if (currentHealth > 0) {
                            currentMonster.Health = currentHealth;

                            monsters[monsterindex] = currentMonster;

                            SharedMonster sharedMonster = currentMonster.SharedModelMapper();

                            MonsterUpdate monsterUpdate = new(_contextFactory.CreateDbContext(), _logger);
                            await monsterUpdate.Update(sharedMonster);

                            string importantMessage = _messageManager.AdventurerAttackHits(monsterWounds, currentHealth);

                            summaryMessage = new(importantMessage);
                            adventurerCombatResult = new(importantMessage);

                            if (combatInitiated != null) {
                                adventurerCombatResult.AddChild(combatInitiated);
                            }

                            adventurerCombatResult.AddChild(new(_messageManager.AdventurerAttackDetails(adventurerDexterity, monsterDexterity), adventurerDexterity, monsterDexterity));
                        } else {
                            monsterHealth = 0;

                            adventurer.Experience += currentMonster.Experience;

                            //if the user kills the Beholder
                            string monsterKilled;
                            if (currentMonster.TypeName == "Beholder") {
                                monsterKilled = _messageManager.FinalBossFightWon();
                                dungeon.MacGuffinFound = true;
                            } else {
                                monsterKilled = _messageManager.FinalBossFightWon(monsterWounds);
                            }

                            summaryMessage = new(monsterKilled);
                            adventurerCombatResult = new(monsterKilled);

                            if (combatInitiated != null) {
                                adventurerCombatResult.AddChild(combatInitiated);
                            }

                            //remove monster at stack
                            MonsterDelete monsterDelete = new(_contextFactory.CreateDbContext(), _logger);
                            await monsterDelete.Delete(currentMonster.Id);

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

                                    Message levelUp = new(_messageManager.AdventurerLevelUp(currentAdventurerLevel));
                                    int levelUpHealth = currentAdventurerHealth - previousAdventurerHealth;
                                    levelUp.AddChild(new(_messageManager.AdventurerLevelUpHealth(adventurer.HealthBase, levelUpHealth)));

                                    int levelUpDamage = currentAdventurerDamage - previousAdventurerDamage;
                                    levelUp.AddChild(new(_messageManager.AdventurerLevelUpDamage(adventurer.DamageBase, levelUpDamage)));

                                    int levelUpProtection = currentAdventurerProtection - previousAdventurerProtection;
                                    levelUp.AddChild(new(_messageManager.AdventurerLevelUpProtection(adventurer.ProtectionBase, levelUpProtection)));

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
                        string adventurerCombatWiff = _messageManager.AdventurerWiff();

                        summaryMessage = new(adventurerCombatWiff);
                        adventurerCombatResult = new(adventurerCombatWiff);

                        adventurerCombatResult.AddChild(new(_messageManager.AdventurerWiffDetails(adventurerDamage, monsterProtection)));
                    }

                    selectedTile.Monsters = monsters;
                } else {
                    adventurerCombatResult = new(_messageManager.AdventurerAttackDodged(monsterDexterity, adventurerDexterity), adventurerDexterity, monsterDexterity);
                }
            }

            //Monster attack
            if (dungeon.InCombat) {
                int monsterDexterity, monsterDamage, monsterProtection, monsterHealth;
                foreach (Monster monster in monsters) {
                    //Monster details
                    monsterDexterity = monster.Dexterity;
                    monsterDamage = monster.Damage;
                    monsterProtection = monster.Protection;
                    monsterHealth = monster.Health;
                                        
                    //Adventurer wounds
                    int adventurerWounds = 0;
                    if (monsterDexterity > adventurerDexterity) {
                        adventurerWounds = monsterDamage - adventurerProtection;

                        if (adventurerWounds > 0) {
                            int currentHealth = adventurer.HealthBase - adventurerWounds;
                            if (currentHealth > 0) {
                                adventurer.HealthBase = currentHealth;

                                string importantMessage = _messageManager.MonsterAttackHit(adventurerWounds, adventurer.HealthBase);

                                summaryMessage = new(importantMessage);
                                monsterCombatResult = new(importantMessage);

                                monsterCombatResult.AddChild(new(_messageManager.MonsterAttackHitDamage(monsterDamage, adventurerProtection)));
                            } else {
                                adventurer.HealthBase = 0;
                                adventurer.IsAlive = false;

                                dungeon.GameOver = true;

                                string importantMessage = _messageManager.MonsterKilledAdventure(adventurerWounds);
                                monsterCombatResult = new(importantMessage);

                                selectedTile.Type = DungeonEvents.FightLost;

                                monsterCombatResult.AddChild(new(_messageManager.MonsterAttackHitDamage(monsterDamage, adventurerProtection)));

                                dungeon.InCombat = false;

                                currentFloorTiles.Unhide();

                                gameOverMessage = await GenerateGameOverMessage(importantMessage, dungeon.SharedModelMapper());

                                break;
                            }

                            if (combatInitiated != null) {
                                monsterCombatResult.AddChild(combatInitiated);
                            }

                            monsterCombatResult.AddChild(new(_messageManager.MonsterAttackHitDetails(monsterDexterity, adventurerDexterity), adventurerDexterity, monsterDexterity));
                        } else {
                            string monsterCombatWiff = _messageManager.MonsterAttackNoDamage();

                            summaryMessage = new(monsterCombatWiff);
                            monsterCombatResult = new(monsterCombatWiff);

                            monsterCombatResult.AddChild(new(_messageManager.MonsterAttackHitDamage(monsterDamage, adventurerProtection)));
                        }
                    } else {

                        monsterCombatResult = new(_messageManager.MonsterAttackMissDetails(adventurerDexterity, monsterDexterity), adventurerDexterity, monsterDexterity);

                        if (combatInitiated != null) {
                            monsterCombatResult.AddChild(combatInitiated);
                        }
                    }
                }
            }

            if (summaryMessage == null) {
                summaryMessage = new(_messageManager.AttacksMiss());
            }

            if (adventurerCombatResult != null) {
                summaryMessage.AddChild(adventurerCombatResult);
            }

            if (monsterCombatResult != null) {
                summaryMessage.AddChild(monsterCombatResult);
            }

            Messages messages = new();
            messages.Add(summaryMessage);

            if (gameOverMessage != null) {
                messages.Add(gameOverMessage);
            }

            ////Update Adventurer
            dungeon.Adventurer = adventurer;

            //Update Messages
            MessagesCreate messagesCreate = new(_contextFactory.CreateDbContext(), _logger);
            await messagesCreate.Create(dungeon.Id, messages.SharedModelMapper());


            dungeon.Messages.AddRange(messages);

            //Update Tiles
            currentFloor.Tiles = currentFloorTiles;


            //Update Dungon
            SharedDungeon sharedDungeon = dungeon.SharedModelMapper();

            DungeonUpdate dungeonUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonUpdate.Update(sharedDungeon);

            return sharedDungeon;
        }

        //***********************************************************
        //*********************************** GenerateGameOverMessage

        //	End of game summary
        //	Once the Adventurer dies, or returns victorious to the Dungeon entrance, a summery of the game will be generated from the database.

        public async Task<Message> GenerateGameOverMessage(string message, SharedDungeon dungeon) {
            Message endOfGameMessage = new(message);

            //Floor            
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }
            int floorsDiscovered = dungeon.Floors.Count();

            FloorQueries floorDescendingQueries = new(_contextFactory.CreateDbContext(), _logger);
            List<SharedFloor> sharedFloor = await floorDescendingQueries.GetUnhiddenFloors(dungeon.Id);

            dungeon.Floors = new(sharedFloor);

            endOfGameMessage.AddChild(new(_messageManager.EndOfGameFloors(floorsDiscovered)));

            //Tiles
            foreach (var floor in dungeon.Floors.OrderBy(f => f.Depth).ToList()) {
                int tilesTotal = floor.Tiles.Count();
                int tilesFound = floor.Tiles.Where(t => t.Visited == true).ToList().Count();
                endOfGameMessage.AddChild(new(_messageManager.EndOfGameFloorDetails(floor.Depth, tilesFound, tilesTotal)));
            }

            //Loot
            foreach (var floor in dungeon.Floors.OrderBy(f => f.Depth).ToList()) {
                int takenWeapon = floor.Tiles.Where(t => t.Type == DungeonEvents.TakenWeapon).ToList().Count();
                int takenPotion = floor.Tiles.Where(t => t.Type == DungeonEvents.TakenPotion).ToList().Count();
                int takenProtection = floor.Tiles.Where(t => t.Type == DungeonEvents.TakenProtection).ToList().Count();

                int chestsLooted = takenWeapon + takenPotion + takenProtection;
                Message lootMessage = new(_messageManager.EndOfGameLootedChests(floor.Depth, chestsLooted));
                lootMessage.AddChild(new(_messageManager.EndOfGameLootedWepons(takenWeapon)));
                lootMessage.AddChild(new(_messageManager.EndOfGameLootedProtection(takenProtection)));
                lootMessage.AddChild(new(_messageManager.EndOfGameLootedPotions(takenPotion)));

                endOfGameMessage.AddChild(lootMessage);
            }

            //Monsters
            foreach (var floor in dungeon.Floors.OrderBy(f => f.Depth).ToList()) {
                //var monsters = floor.Tiles.Where(t => t.Monsters.Count > 0).ToList();
                int fightsFleed = floor.Tiles.Where(t => t.Type == DungeonEvents.Fight).Where(t => t.Hidden == false).ToList().Count;
                int fightsWon = floor.Tiles.Where(t => t.Type == DungeonEvents.FightWon).Where(t => t.Hidden == false).ToList().Count;

                int fights = fightsFleed + fightsWon;
                Message monstersMessage = new(_messageManager.EndOfGameFightsTotal(floor.Depth, fights));

                monstersMessage.AddChild(new(_messageManager.EndOfGameFightsFleed(fightsFleed)));
                monstersMessage.AddChild(new(_messageManager.EndOfGameFightsWon(fightsWon)));

                endOfGameMessage.AddChild(monstersMessage);
            }

            return endOfGameMessage;
        }
    }
}
