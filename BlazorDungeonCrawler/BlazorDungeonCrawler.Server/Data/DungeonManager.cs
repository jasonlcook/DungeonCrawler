using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using BlazorDungeonCrawler.Shared.Enumerators;

using BlazorDungeonCrawler.Server.Models;

using BlazorDungeonCrawler.Server.Database;
using BlazorDungeonCrawler.Server.Database.Resources.Commands.Create;
using BlazorDungeonCrawler.Server.Database.Resources.Commands.Delete;
using BlazorDungeonCrawler.Server.Database.Resources.Commands.Update;
using BlazorDungeonCrawler.Server.Database.Resources.Queries.Get;

using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;
using SharedAdventurer = BlazorDungeonCrawler.Shared.Models.Adventurer;
using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;
using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedMessage = BlazorDungeonCrawler.Shared.Models.Message;
using SharedMonster = BlazorDungeonCrawler.Shared.Models.Monster;

namespace BlazorDungeonCrawler.Server.Data {
    public class DungeonManager {
        private readonly IStringLocalizer<DungeonManager> _localiser;
        private readonly ILogger _logger;
        private readonly IDbContextFactory<DungeonDbContext> _contextFactory;
        public DungeonManager(IDbContextFactory<DungeonDbContext> contextFactory, ILogger<DungeonManager> logger, IStringLocalizer<DungeonManager> localiser) {
            this._localiser = localiser;
            this._contextFactory = contextFactory;
            this._logger = logger;

            _logger.LogInformation("Dungeon manager initiated. {DT}", DateTime.UtcNow.ToLongTimeString());
        }

        public async Task<SharedDungeon> Generate() {
            Messages messages = new();

            //Create
            //  Adventurer

            int health = Dice.RollDSix();
            int damage = Dice.RollDSix();
            int protection = Dice.RollDSix();

            List<int> adventurerRolls = new() { health, damage, protection };

            Message message = new(_localiser["MessageAdventureGeneration"], adventurerRolls, null);

            string messageAdventureHealth = _localiser["MessageAdventureHealth"];
            message.AddChild(new(messageAdventureHealth.Replace("[ADVENTURER_HEALTH]", health.ToString()), health, null));

            string messageAdventureDamage = _localiser["MessageAdventureDamage"];
            message.AddChild(new(messageAdventureDamage.Replace("[ADVENTURER_DAMAGE]", damage.ToString()), damage, null));

            string messageAdventureProtection = _localiser["MessageAdventureProtection"];
            message.AddChild(new(messageAdventureProtection.Replace("[ADVENTURER_PROTECTION]", protection.ToString()), protection, null));

            messages.Add(message);

            Adventurer adventurer = new(health, damage, protection);

            //  Floors
            Floors floors = new();

            int depth = 1;
            Floor newFloor = new(depth);

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

            DungeonCreate dungeonCreate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonCreate.Create(sharedDungeon);

            _logger.LogInformation($"Dungeon {sharedDungeon.Id} generated.");

            return sharedDungeon;
        }

        public async Task<SharedDungeon> RetrieveDungeon(Guid dungeonId) {
            _logger.LogInformation($"Dungeon {dungeonId} retrieve.");

            DungeonQueries dungeonQueries = new(_contextFactory.CreateDbContext(), _logger);
            SharedDungeon? sharedDungeon = await dungeonQueries.Get(dungeonId);

            if (sharedDungeon == null || sharedDungeon.Id == Guid.Empty) { throw new Exception(_localiser["ErrorDungeonNoFound"]); }

            return sharedDungeon;
        }

        public async Task<SharedDungeon> GetSelectedDungeonTiles(Guid dungeonId, Guid tileId) {
            _logger.LogInformation($"Dungeon {dungeonId} tile {tileId} selected.");

            SharedDungeon? dungeon = await RetrieveDungeon(dungeonId);
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
                        messages.Add(new(_localiser["MessageDungeonExitFail"]));
                    } else {
                        dungeon.GameOver = true;

                        Message gameOverMessage = GenerateGameOverMessage(_localiser["MessageDungeonExitSuccess"], dungeon);
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
                            string messageMonsterCampSingle = _localiser["MessageMonsterCampSingle"];
                            monsterMessage = messageMonsterCampSingle.Replace("[MONSTER_NAME]", monsters.GetName());
                        } else {
                            string messageMonsterCampMultiple = _localiser["MessageMonsterCampMultiple"];
                            monsterMessage = messageMonsterCampMultiple.Replace("[MONSTER_COUNT]]", monsters.Count().ToString()).Replace("[MONSTER_NAME]", monsters.GetName());
                        }

                        string monsterDetails = string.Empty;
                        List<int> monsterDetailsRoll = new();
                        List<int> monsterDetailsRollCollection = new();
                        List<Message> monsterDetailsMessages = new();
                        foreach (Monster monster in monsters.Get()) {
                            string messageMonsterDamage = _localiser["MessageMonsterDamage"];
                            monsterDetails = string.Format(messageMonsterDamage.Replace("[MONSTER_DAMAGE]", monster.Damage.ToString()));

                            string messageMonsterHealth = _localiser["MessageMonsterHealth"];
                            monsterDetails += string.Format(messageMonsterDamage.Replace("[MONSTER_HEALTH]", monster.Health.ToString()));

                            string messageMonsterProtection = _localiser["MessageMonsterProtection"];
                            monsterDetails += string.Format(messageMonsterProtection.Replace("[MONSTER_PROTECTION]", monster.Protection.ToString()));

                            monsterDetailsRoll = new() { monster.Damage, monster.Health, monster.Protection };
                            monsterDetailsRollCollection.AddRange(monsterDetailsRoll);

                            monsterDetailsMessages.Add(new(monsterDetails, null, monsterDetailsRoll));
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

                    int increasedDepth = dungeon.Depth + 1;
                    SharedFloor? deeperFloor = dungeon.Floors.Where(l => l.Depth == increasedDepth).FirstOrDefault();

                    //If deeper floor does not exist create it
                    if (deeperFloor == null || deeperFloor.Id == Guid.Empty) {
                        //Next floor
                        Floor newFloor = new(increasedDepth);

                        string messageDungeonDepth = _localiser["MessageDungeonDepth"];
                        messages.Add(new(messageDungeonDepth.Replace("[INCREASED_DEPTH]", increasedDepth.ToString())));

                        //  Tiles
                        Tiles newFloorTiles = new(newFloor.Depth, newFloor.Rows, newFloor.Columns);
                        newFloor.Tiles = newFloorTiles.GetTiles();

                        deeperFloor = newFloor.SharedModelMapper();

                        dungeon.Floors.Add(deeperFloor);

                        FloorCreate floorCreate = new(_contextFactory.CreateDbContext(), _logger);
                        await floorCreate.Create(dungeon.Id, deeperFloor);
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

                                string messageWeaponEquipped = _localiser["MessageWeaponEquipped"];
                                weaponsMessage = messageWeaponEquipped.Replace("[WEAPON_DESCRIPTION]", weapons.Description());
                            } else {
                                string messageWeaponRejected = _localiser["MessageWeaponRejected"];
                                weaponsMessage = messageWeaponRejected.Replace("[WEAPON_DESCRIPTION]", weapons.Description());
                            }
                            Message weaponsMessages = new(weaponsMessage, new List<int> { weaponsTypeValue, weaponsConditionValue }, null);

                            string messageWeaponCondition = _localiser["MessageWeaponCondition"];
                            weaponsMessages.AddChild(new(messageWeaponCondition.Replace("[WEAPON_CONDITION]", weapons.Condition.ToString()).Replace("[WEAPON_CONDITION_VALUE]", weaponsConditionValue.ToString()), weaponsConditionValue, null));

                            string messageWeaponType = _localiser["MessageWeaponType"];
                            weaponsMessages.AddChild(new(messageWeaponType.Replace("[WEAPON_TYPE]", weapons.Type.ToString()).Replace("[WEAPON_TYPE_VALUE]", weaponsTypeValue.ToString()), weaponsTypeValue, null));

                            string messageWeaponValue = _localiser["MessageWeaponValue"];
                            weaponsMessages.AddChild(new(messageWeaponValue.Replace("WEAPON_VALUE", weapons.WeaponValue.ToString()).Replace("[WEAPON_TYPE_VALUE]", weapons.TypeValue.ToString()).Replace("[WEAPON_CONDITION_VALUE]", weapons.ConditionValue.ToString())));

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
                                string messageArmourEquipped = _localiser["MessageArmourEquipped"];
                                armourMessage = messageArmourEquipped.Replace("[ARMOUR_DESCRIPTION]", armour.Description().ToString());
                            } else {
                                string messageArmourRejected = _localiser["MessageArmourRejected"];
                                armourMessage = messageArmourRejected.Replace("[ARMOUR_DESCRIPTION]", armour.Description().ToString());
                            }
                            Message armourMessages = new(armourMessage, new List<int> { armourConditionValue, armourTypeValue }, null);

                            string messageArmourCondition = _localiser["MessageArmourCondition"];
                            armourMessages.AddChild(new(messageArmourCondition.Replace("[ARMOUR_CONDITION]", armour.Condition.ToString()).Replace("[ARMOUR_CONDITION_VALUE]", armourConditionValue.ToString()), armourConditionValue, null));

                            string messageArmourType = _localiser["MessageArmourType"];
                            armourMessages.AddChild(new(messageArmourType.Replace("[ARMOUR_TYPE]", armour.Type.ToString()).Replace("[ARMOUR_TYPE_VALUE]", armourTypeValue.ToString()), armourTypeValue, null));

                            string messageArmourValue = _localiser["MessageArmourValue"];
                            armourMessages.AddChild(new(messageArmourValue.Replace("ARMOUR_VALUE", armour.ArmourValue.ToString()).Replace("[ARMOUR_TYPE_VALUE]", armour.TypeValue.ToString()).Replace("[ARMOUR_CONDITION_VALUE]", armour.ConditionValue.ToString())));

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
                                        messages.Add(new($"Regained {regainedHealth} health"));
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

                            string messagePotionDrink = _localiser["MessagePotionDrink"];
                            Message potionMessages = new(messagePotionDrink.Replace("[POTION_DESCRIPTION]", potion.Description().ToString()), new List<int> { potionTypeValue, potionSizeValue, potionDurationValue }, null);

                            string messagePotionType = _localiser["MessagePotionType"];
                            potionMessages.AddChild(new(messagePotionType.Replace("[POTION_TYPE]", potion.Type.ToString()).Replace("[POTION_TYPE_VALUE]", potionTypeValue.ToString()), potionTypeValue, null));

                            string messagePotionSize = _localiser["MessagePotionSize"];
                            potionMessages.AddChild(new(messagePotionSize.Replace("[POTION_SIZE]", potion.Size.ToString()).Replace("[POTION_SIZE_VALUE]", potionSizeValue.ToString()), potionSizeValue, null));

                            string messagePotionDuration = _localiser["MessagePotionDuration"];
                            potionMessages.AddChild(new(messagePotionDuration.Replace("[POTION_DURATION]", potion.Duration.ToString()).Replace("[POTION_DURATION_VALUE]", potionDurationValue.ToString()), potionDurationValue, null));

                            messages.Add(potionMessages);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Loot tile type.");
                    }
                    break;
                case DungeonEvents.Macguffin:
                    monsters = await SetMonsters(selectedTile.Id, 999);

                    string messageFinalBossGeneration = _localiser["MessageFinalBossGeneration"];
                    messages.Add(new(messageFinalBossGeneration.Replace("[BOSS_NAME]", monsters.GetName().ToString())));

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
                            string messageWanderingMonsterSingle = _localiser["MessageWanderingMonsterSingle"];
                            monsterMessage = messageWanderingMonsterSingle.Replace("[MONSTER_NAME]", monsters.GetName());
                        } else {
                            string messageWanderingMonsterMultiple = _localiser["MessageWanderingMonsterMultiple"];
                            monsterMessage = messageWanderingMonsterMultiple.Replace("[MONSTER_COUNT]]", monsters.Count().ToString()).Replace("[MONSTER_NAME]", monsters.GetName());
                        }

                        string monsterDetails = string.Empty;
                        List<int> monsterDetailsRoll = new();
                        List<int> monsterDetailsRollCollection = new();
                        List<Message> monsterDetailsMessages = new();
                        foreach (Monster monster in monsters.Get()) {
                            string messageMonsterDamage = _localiser["MessageMonsterDamage"];
                            monsterDetails = string.Format(messageMonsterDamage.Replace("[MONSTER_DAMAGE]", monster.Damage.ToString()));

                            string messageMonsterHealth = _localiser["MessageMonsterHealth"];
                            monsterDetails += string.Format(messageMonsterDamage.Replace("[MONSTER_HEALTH]", monster.Health.ToString()));

                            string messageMonsterProtection = _localiser["MessageMonsterProtection"];
                            monsterDetails += string.Format(messageMonsterProtection.Replace("[MONSTER_PROTECTION]", monster.Protection.ToString()));

                            monsterDetailsRoll = new() { monster.Damage, monster.Health, monster.Protection };
                            monsterDetailsRollCollection.AddRange(monsterDetailsRoll);

                            monsterDetailsMessages.Add(new(monsterDetails, null, monsterDetailsRoll));
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

            AdventurerUpdate adventurerUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await adventurerUpdate.Update(sharedAdventurer);

            //Update Messages
            List<SharedMessage> sharedMessages = messages.SharedModelMapper();
            dungeon.Messages.AddRange(sharedMessages);

            MessagesCreate messagesCreate = new(_contextFactory.CreateDbContext(), _logger);
            await messagesCreate.Create(dungeon.Id, sharedMessages);

            //Update Tiles
            List<SharedTile> sharedTiles = currentFloorTiles.SharedModelMapper();
            currentFloor.Tiles = sharedTiles;

            for (int i = 0; i < dungeon.Floors.Count; i++) {
                if (dungeon.Floors[i].Id != currentFloor.Id) {
                    dungeon.Floors[i] = currentFloor;
                }
            }

            TilesUpdate tilesUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await tilesUpdate.Update(sharedTiles);

            //Update Dungon
            DungeonUpdate dungeonUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonUpdate.Update(dungeon);

            return dungeon;
        }

        private async Task<Monsters> SetMonsters(Guid tileId, int depth) {
            Monsters monsters = new();

            //generate new monsters
            monsters.Generate(depth);

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

        public async Task<SharedDungeon> AutomaticallyAdvanceDungeon(Guid dungeonId) {
            _logger.LogInformation($"Dungeon {dungeonId} automatically advanced.");

            SharedDungeon? dungeon = await RetrieveDungeon(dungeonId);
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Floors == null || dungeon.Floors.Count == 0) { throw new ArgumentNullException("Dungeon Floors"); }

            if (dungeon.GameOver) {
                return dungeon;
            }

            SharedAdventurer? adventurer = dungeon.Adventurer;
            if (adventurer == null || adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Adventurer"); }

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
                                //if all hidden tiles have been uncoverd then make way to relevant tile
                                if (dungeon.MacGuffinFound) {
                                    if (dungeon.Depth == 1) {
                                        //if first floor then get to the Dungeon entrance
                                        targetTile = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.DungeonEntrance).FirstOrDefault();
                                    } else {
                                        //if lower floor then get to the ascending stairs
                                        targetTile = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.StairsAscending).FirstOrDefault();
                                    }
                                } else {
                                    if (dungeon.Depth == 10) {
                                        //if lowest floor then get to the Macguffin
                                        targetTile = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.Macguffin).FirstOrDefault();
                                    } else {
                                        //if higher floor then get to the descending stairs
                                        targetTile = currentFloor.Tiles.Where(t => t.Type == DungeonEvents.StairsDescending).FirstOrDefault();
                                    }
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

        public async Task<SharedDungeon> DescendStairs(Guid dungeonId) {
            _logger.LogInformation($"Descend Dungeon {dungeonId} stairs.");

            SharedDungeon? dungeon = await RetrieveDungeon(dungeonId);
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }

            int increasedDepth = dungeon.Depth += 1;
            SharedFloor? currentFloor = dungeon.Floors.Where(l => l.Depth == increasedDepth).FirstOrDefault();
            if (currentFloor == null || currentFloor.Id == Guid.Empty || currentFloor.Tiles == null) {
                throw new Exception("Floor not found");
            }

            dungeon.StairsDiscovered = false;

            //Update Dungon            
            DungeonUpdate dungeonUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonUpdate.Update(dungeon);

            return dungeon;
        }

        public async Task<SharedDungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            _logger.LogInformation($"Flee Dungeon {dungeonId} monster at tile {tileId}.");

            SharedDungeon? dungeon = await RetrieveDungeon(dungeonId);
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

                string messageAdventurerFleeSuccess = _localiser["MessageAdventurerFleeSuccess"];
                messages.Add(new(messageAdventurerFleeSuccess));

                currentFloorTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
            } else {
                string messageAdventurerFleeFail = _localiser["MessageAdventurerFleeFail"];
                Message monsterFlee = new(messageAdventurerFleeFail);
                                
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

                        string messageMonsterAttackHit = _localiser["MessageMonsterAttackHit"];
                        monsterFlee = new(messageMonsterAttackHit.Replace("[ADVENTURER_WOUNDS]", adventurerWounds.ToString()).Replace("[ADVENTURER_HEALTH]", adventurer.HealthBase.ToString()));

                        string messageMonsterAttackHitDamage = _localiser["MessageMonsterAttackHitDamage"];
                        monsterFlee.AddChild(new(messageMonsterAttackHitDamage.Replace("[MONSTER_DAMAGE]", monsterDamage.ToString()).Replace("[ADVENTURER_PROTECTION]", adventurerProtection.ToString())));
                    } else {
                        adventurer.HealthBase = 0;
                        adventurer.IsAlive = false;

                        dungeon.GameOver = true;

                        string MmessageMonsterKilledAdventure = _localiser["MessageMonsterKilledAdventure"];
                        Message gameOverMessage = GenerateGameOverMessage(MmessageMonsterKilledAdventure.Replace("[ADVENTURER_WOUNDS]", adventurerWounds.ToString()), dungeon);

                        monsterFlee = gameOverMessage;

                        selectedTile.Type = DungeonEvents.FightLost;

                        string messageMonsterAttackHitDamage = _localiser["MessageMonsterAttackHitDamage"];
                        monsterFlee.AddChild(new(messageMonsterAttackHitDamage.Replace("[MONSTER_DAMAGE]", monsterDamage.ToString()).Replace("[ADVENTURER_PROTECTION]", adventurerProtection.ToString())));

                        dungeon.InCombat = false;

                        currentFloorTiles.Unhide();
                    }
                } else {
                    string messageMonsterAttackNoDamage = _localiser["MessageMonsterAttackNoDamage"];
                    monsterFlee = new(messageMonsterAttackNoDamage);

                    string messageMonsterAttackHitDamage = _localiser["MessageMonsterAttackHitDamage"];
                    monsterFlee.AddChild(new(messageMonsterAttackHitDamage.Replace("[MONSTER_DAMAGE]", monsterDamage.ToString()).Replace("[ADVENTURER_PROTECTION]", adventurerProtection.ToString())));
                }

                messages.Add(monsterFlee);
            }

            //Update Adventurer
            SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
            dungeon.Adventurer = sharedAdventurer;

            AdventurerUpdate adventurerUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await adventurerUpdate.Update(sharedAdventurer);

            //Update Messages
            List<SharedMessage> sharedMessages = messages.SharedModelMapper();
            dungeon.Messages.AddRange(sharedMessages);

            MessagesCreate messagesCreate = new(_contextFactory.CreateDbContext(), _logger);
            await messagesCreate.Create(dungeon.Id, sharedMessages);

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

            TilesUpdate tilesUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await tilesUpdate.Update(sharedTiles);

            //Update Dungon
            DungeonUpdate dungeonUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonUpdate.Update(dungeon);

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
            _logger.LogInformation($"Fight Dungeon {dungeonId} monster at tile {tileId}.");

            SharedDungeon? dungeon = await RetrieveDungeon(dungeonId);
            if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
            if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
            if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floor"); }

            SharedFloor? currentFloor = dungeon.Floors.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
            if (currentFloor == null) { throw new ArgumentNullException("Dungeon current Floor"); }

            Tiles currentFloorTiles = new(currentFloor.Tiles);

            SharedTile? selectedTile = currentFloor.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
            if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Floor selected Tile"); }
            if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Floor Tile Monsters"); }


            List<SharedMonster> monsters = selectedTile.Monsters.OrderBy(m => m.Index).ToList();

            //Adventurer details
            Adventurer adventurer = new(dungeon.Adventurer);
            adventurer.DurationDecrement();

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
                    combatInitiated = new(_localiser["MessageAdventurerInitiatesCombat"], adventurerRoll, monsterRoll);
                } else {
                    adventurerInitiatesCombat = false;
                    combatInitiated = new(_localiser["MessageMonsterInitiatesCombats"], adventurerRoll, monsterRoll);
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

                    if (monsterWounds < 1) {
                        monsterWounds = 1;
                    }

                    if (monsterWounds > 0) {
                        int currentHealth = monsterHealth - monsterWounds;
                        if (currentHealth > 0) {
                            currentMonster.Health = currentHealth;

                            SharedMonster sharedMonster = currentMonster.SharedModelMapper();

                            MonsterUpdate monsterUpdate = new(_contextFactory.CreateDbContext(), _logger);
                            await monsterUpdate.Update(sharedMonster);

                            monsters[monsterindex] = sharedMonster;

                            string messageAdventurerAttackHits = _localiser["MessageAdventurerAttackHits"];
                            string importantMessage = messageAdventurerAttackHits.Replace("[MONSTER_WOUNDS_RECEIVED]", monsterWounds.ToString()).Replace("[MONSTER_HEALTH_REMAINING]", currentHealth.ToString());

                            summaryMessage = new(importantMessage);
                            adventurerCombatResult = new(importantMessage);

                            if (combatInitiated != null) {
                                adventurerCombatResult.AddChild(combatInitiated);
                            }

                            string messageAdventurerAttackDetails = _localiser["MessageAdventurerAttackDetails"];
                            adventurerCombatResult.AddChild(new(messageAdventurerAttackHits.Replace("[ADVENTURER_ATTACK]", adventurerAttackDice.ToString()).Replace("[MONSTER_DODGE]", monsterDodgeRolls.ToString()), adventurerAttackDice, monsterDodgeRolls));
                        } else {
                            monsterHealth = 0;

                            adventurer.Experience += currentMonster.Experience;

                            //if the user kills the Beholder
                            string monsterKilled;
                            if (currentMonster.TypeName == "Beholder") {
                                monsterKilled = _localiser["MessageFinalBossFightWon"];
                                dungeon.MacGuffinFound = true;
                            } else {
                                string messageFinalBossFightLost = _localiser["MessageFinalBossFightLost"];
                                monsterKilled = messageFinalBossFightLost.Replace("[MONSTER_WOUNDS_RECEIVED]", monsterWounds.ToString());
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

                                    string messageAdventurerLevelUp = _localiser["MessageAdventurerLevelUp"];
                                    Message levelUp = new(messageAdventurerLevelUp.Replace("[ADVENTURER_CURRENT_DAMAGE]", currentAdventurerLevel.ToString()));

                                    string messageAdventurerLevelUpHealth = _localiser["MessageAdventurerLevelUpHealth"];
                                    int levelUpHealth = currentAdventurerHealth - previousAdventurerHealth;
                                    levelUp.AddChild(new(messageAdventurerLevelUpHealth.Replace("[ADVENTURER_HEALTH]", adventurer.HealthBase.ToString()).Replace("[ADVENTURER_HEALTH_ADDITION]", levelUpHealth.ToString())));

                                    string messageAdventurerLevelUpDamage = _localiser["MessageAdventurerLevelUpDamage"];
                                    int levelUpDamage = currentAdventurerDamage - previousAdventurerDamage;
                                    levelUp.AddChild(new(messageAdventurerLevelUpDamage.Replace("[ADVENTURER_DAMAGE]", adventurer.DamageBase.ToString()).Replace("[ADVENTURER_DAMAGE_ADDITION]", levelUpDamage.ToString())));

                                    string messageAdventurerLevelUpProtection = _localiser["MessageAdventurerLevelUpProtection"];
                                    int levelUpProtection = currentAdventurerProtection - previousAdventurerProtection;
                                    levelUp.AddChild(new(messageAdventurerLevelUpProtection.Replace("[ADVENTURER_PROTECTION]", adventurer.ProtectionBase.ToString()).Replace("[ADVENTURER_PROTECTION_ADDITION]", levelUpProtection.ToString())));

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
                        string adventurerCombatWiff = _localiser["MessageAdventurerWiff"];

                        summaryMessage = new(adventurerCombatWiff);
                        adventurerCombatResult = new(adventurerCombatWiff);

                        string MessageAdventurerWiffDetails = _localiser["MessageAdventurerWiffDetails"];
                        adventurerCombatResult.AddChild(new(MessageAdventurerWiffDetails.Replace("[ADVENTURER_DAMAGE]", adventurerDamage.ToString()).Replace("[MONSTER_PROTECTION]", monsterProtection.ToString())));
                    }

                    selectedTile.Monsters = monsters;
                } else {
                    string messageAdventurerAttackDodged = _localiser["MessageAdventurerAttackDodged"];
                    adventurerCombatResult = new(messageAdventurerAttackDodged.Replace("[MONSTER_DODGE]", monsterDodgeValue.ToString()).Replace("[ADVENTURER_ATTACK]", adventurerAttackValue.ToString()), adventurerAttackDice, monsterDodgeRolls);
                }
            }

            //Monster attack
            if (dungeon.InCombat) {
                int monsterDamage, monsterProtection, monsterHealth;
                foreach (SharedMonster monster in selectedTile.Monsters) {
                    //Monster details
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

                                string messageMonsterAttackHit = _localiser["MessageMonsterAttackHit"];
                                string importantMessage = messageMonsterAttackHit.Replace("[ADVENTURER_WOUNDS]", adventurerWounds.ToString()).Replace("[ADVENTURER_HEALTH]", adventurer.HealthBase.ToString());

                                summaryMessage = new(importantMessage);
                                monsterCombatResult = new(importantMessage);

                                string messageMonsterAttackHitDamage = _localiser["MessageMonsterAttackHitDamage"];
                                monsterCombatResult.AddChild(new(messageMonsterAttackHitDamage.Replace("[MONSTER_DAMAGE]", monsterDamage.ToString()).Replace("[ADVENTURER_PROTECTION]", adventurerProtection.ToString())));
                            } else {
                                adventurer.HealthBase = 0;
                                adventurer.IsAlive = false;

                                dungeon.GameOver = true;

                                string messageMonsterKilledAdventure = _localiser["MessageMonsterKilledAdventure"];
                                string importantMessage = messageMonsterKilledAdventure.Replace("[ADVENTURER_WOUNDS]", adventurerWounds.ToString());
                                monsterCombatResult = new(importantMessage);

                                selectedTile.Type = DungeonEvents.FightLost;

                                string messageMonsterAttackHitDamage = _localiser["MessageMonsterAttackHitDamage"];
                                monsterCombatResult.AddChild(new(messageMonsterAttackHitDamage.Replace("[MONSTER_DAMAGE]", monsterDamage.ToString()).Replace("[ADVENTURER_PROTECTION]", adventurerProtection.ToString())));

                                dungeon.InCombat = false;

                                currentFloorTiles.Unhide();

                                gameOverMessage = GenerateGameOverMessage(importantMessage, dungeon);

                                break;
                            }

                            if (combatInitiated != null) {
                                monsterCombatResult.AddChild(combatInitiated);
                            }

                            string messageMonsterAttackHitDetails = _localiser["MessageMonsterAttackHitDetails"];
                            monsterCombatResult.AddChild(new(messageMonsterAttackHitDetails.Replace("[MONSTER_ATTACK]", monsterAttackValue.ToString()).Replace("[ADVENTURER_DODGE]", adventurerDodgeValue.ToString()), adventurerDodgeRolls, monsterAttackDice));
                        } else {
                            string monsterCombatWiff = _localiser["MessageMonsterAttackNoDamage"];

                            summaryMessage = new(monsterCombatWiff);
                            monsterCombatResult = new(monsterCombatWiff);

                            string messageMonsterAttackHitDamage = _localiser["MessageMonsterAttackHitDamage"];
                            monsterCombatResult.AddChild(new(messageMonsterAttackHitDamage.Replace("[MONSTER_DAMAGE]", monsterDamage.ToString()).Replace("[ADVENTURER_PROTECTION]", adventurerProtection.ToString())));
                        }
                    } else {
                        string messageMonsterAttackMissDetails = _localiser["MessageMonsterAttackMissDetails"];
                        monsterCombatResult = new(messageMonsterAttackMissDetails.Replace("[ADVENTURER_DODGE]", adventurerDodgeValue.ToString()).Replace("[MONSTER_ATTACK]", monsterAttackValue.ToString()), adventurerDodgeRolls, monsterAttackDice);

                        if (combatInitiated != null) {
                            monsterCombatResult.AddChild(combatInitiated);
                        }
                    }
                }
            }

            if (summaryMessage == null) {
                summaryMessage = new(_localiser["MessageAttacksMiss"]);
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

            //Update Adventurer
            SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
            dungeon.Adventurer = sharedAdventurer;

            AdventurerUpdate adventurerUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await adventurerUpdate.Update(sharedAdventurer);

            //Update Messages
            List<SharedMessage> sharedMessages = messages.SharedModelMapper();
            dungeon.Messages.AddRange(sharedMessages);

            MessagesCreate messagesCreate = new(_contextFactory.CreateDbContext(), _logger);
            await messagesCreate.Create(dungeon.Id, sharedMessages);

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

            TilesUpdate tilesUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await tilesUpdate.Update(sharedTiles);

            //Update Dungon
            DungeonUpdate dungeonUpdate = new(_contextFactory.CreateDbContext(), _logger);
            await dungeonUpdate.Update(dungeon);

            return dungeon;
        }

        public Message GenerateGameOverMessage(string message, SharedDungeon dungeon) {
            Message endOfGameMessage = new(message);

            //Floor            
            if (dungeon.Floors == null) { throw new ArgumentNullException("Dungeon Floors"); }
            int floorsDiscovered = dungeon.Floors.Count();

            string messageEndOfGameFloors = _localiser["MessageEndOfGameFloors"];
            endOfGameMessage.AddChild(new(messageEndOfGameFloors.Replace("[FLOORS_DISCOVERED]", floorsDiscovered.ToString())));

            //Tiles
            string messageEndOfGameFloorDetails = _localiser["MessageEndOfGameFloorDetails"];
            foreach (var floor in dungeon.Floors.OrderBy(f => f.Depth).ToList()) {
                int tilesTotal = floor.Tiles.Count();
                int tilesFound = floor.Tiles.Where(t => t.Hidden == false).ToList().Count();
                endOfGameMessage.AddChild(new(messageEndOfGameFloorDetails.Replace("[FLOOR_DEPTH]", floor.Depth.ToString()).Replace("[FLOOR_TILES_FOUND]", tilesFound.ToString()).Replace("[FLOOR_TILES_TOTAL]", tilesTotal.ToString())));
            }

            //Loot
            string messageEndOfGameLootedChests = _localiser["MessageEndOfGameLootedChests"];
            string messageEndOfGameLootedWepons = _localiser["MessageEndOfGameLootedWepons"];
            string messageEndOfGameLootedProtection = _localiser["MessageEndOfGameLootedProtection"];
            string messageEndOfGameLootedPotions = _localiser["MessageEndOfGameLootedPotions"];

            foreach (var floor in dungeon.Floors.OrderBy(f => f.Depth).ToList()) {
                int takenWeapon = floor.Tiles.Where(t => t.Type == DungeonEvents.TakenWeapon).ToList().Count();
                int takenPotion = floor.Tiles.Where(t => t.Type == DungeonEvents.TakenPotion).ToList().Count();
                int takenProtection = floor.Tiles.Where(t => t.Type == DungeonEvents.TakenProtection).ToList().Count();

                int chestsLooted = takenWeapon + takenPotion + takenProtection;
                Message lootMessage = new(messageEndOfGameLootedChests.Replace("[FLOOR_DEPTH]", floor.Depth.ToString()).Replace("[CHESTS_LOOTED]", chestsLooted.ToString()));              
                lootMessage.AddChild(new(messageEndOfGameLootedWepons.Replace("[WEAPONS_LOOTED]", takenWeapon.ToString())));
                lootMessage.AddChild(new(messageEndOfGameLootedProtection.Replace("[PROTECTION_LOOTED]", takenProtection.ToString())));
                lootMessage.AddChild(new(messageEndOfGameLootedPotions.Replace("[POTIONS_LOOTED]", takenPotion.ToString())));

                endOfGameMessage.AddChild(lootMessage);
            }

            //Monsters
            string messageEndOfGameFightsTotal = _localiser["MessageEndOfGameFightsTotal"];
            string messageEndOfGameFightsFleed = _localiser["MessageEndOfGameFightsFleed"];
            string messageEndOfGameFightsWon = _localiser["MessageEndOfGameFightsWon"];

            foreach (var floor in dungeon.Floors.OrderBy(f => f.Depth).ToList()) {
                //var monsters = floor.Tiles.Where(t => t.Monsters.Count > 0).ToList();
                int fightsFleed = floor.Tiles.Where(t => t.Type == DungeonEvents.Fight).Where(t => t.Hidden == false).ToList().Count;
                int fightsWon = floor.Tiles.Where(t => t.Type == DungeonEvents.FightWon).Where(t => t.Hidden == false).ToList().Count;

                int fights = fightsFleed + fightsWon;
                Message monstersMessage = new(messageEndOfGameFightsTotal.Replace("[FLOOR_DEPTH]", floor.Depth.ToString()).Replace("[FIGHTS_TOTAL]", fights.ToString()));
                
                monstersMessage.AddChild(new(messageEndOfGameFightsFleed.Replace("[FIGHTS_FLED]", fightsFleed.ToString())));
                monstersMessage.AddChild(new(messageEndOfGameFightsWon.Replace("[FIGHTS_WON]", fightsWon.ToString())));

                endOfGameMessage.AddChild(monstersMessage);
            }

            return endOfGameMessage;
        }
    }
}
