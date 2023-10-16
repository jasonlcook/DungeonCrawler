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

                    Depth = depth,

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
                if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }

                Messages messages = new();
                Monsters monsters = new();

                Adventurer adventurer = new(dungeon.Adventurer);
                adventurer.DurationDecrement();

                SharedLevel? currentLevel = dungeon.Levels.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
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
                    case DungeonEvents.DungeonEntrance:
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
                    case DungeonEvents.Fight:
                        if (!selectedTile.FightWon && selectedTile.Monsters.Count == 0) {
                            //generate new monsters
                            monsters.Generate(dungeon.Depth);
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
                    case DungeonEvents.StairsDescending:
                        //  Set surrounding selecteable tiles for when the user returns to this level
                        currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                        TilesUpdate.Update(currentLevelTiles.SharedModelMapper());

                        int increasedDepth = dungeon.Depth += 1;
                        dungeon.Depth = increasedDepth;

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
                    case DungeonEvents.StairsAscending:
                        //  Set surrounding selecteable tiles for when the user returns to this level
                        currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                        TilesUpdate.Update(currentLevelTiles.SharedModelMapper());

                        int decreaseDepth = dungeon.Depth -= 1;
                        dungeon.Depth = decreaseDepth;

                        currentLevel = dungeon.Levels.Where(l => l.Depth == decreaseDepth).FirstOrDefault();
                        if (currentLevel == null || currentLevel.Id == Guid.Empty || currentLevel.Tiles == null) {
                            throw new Exception("Level not found");
                        }

                        currentLevelTiles = new Tiles(currentLevel.Tiles);

                        dungeon.RefreshRequired = true;
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

                                messages.Add(new Message($"Weapons condition: {weapons.Condition} (ROLL: {weaponsConditionValue})", weaponsConditionValue));
                                messages.Add(new Message($"Weapons type: {weapons.Type} (ROLL: {weaponsTypeValue})", weaponsTypeValue));

                                messages.Add(new Message($"Weapon value: {weapons.WeaponValue} ({weapons.TypeValue} * {weapons.ConditionValue})"));

                                int currentWeaponValue = adventurer.Weapon;

                                if (weapons.WeaponValue > currentWeaponValue) {
                                    adventurer.Weapon = weapons.WeaponValue;
                                    messages.Add(new Message($"PICKUP A {weapons.Description()}"));
                                } else {
                                    messages.Add(new Message($"REJECT A {weapons.Description()}"));
                                }

                                break;
                            case DungeonEvents.TakenProtection:
                                int armourTypeValue = Dice.RollDSix();
                                int armourConditionValue = Dice.RollDSix();

                                Armour armour = new(dungeon.Depth, armourTypeValue, armourConditionValue);

                                messages.Add(new Message($"Armour condition: {armour.Condition} (ROLL: {armourConditionValue})", armourConditionValue));
                                messages.Add(new Message($"Armour type: {armour.Type} (ROLL: {armourTypeValue})", armourTypeValue));

                                messages.Add(new Message($"Armour value: {armour.ArmourValue} ({armour.TypeValue} * {armour.ConditionValue})"));

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

                                if (pickedUp) {
                                    messages.Add(new Message($"EQUIPT {armour.Description()}"));
                                } else {
                                    messages.Add(new Message($"REJECT {armour.Description()}"));
                                }

                                break;
                            case DungeonEvents.TakenPotion:
                                int potionTypeValue = Dice.RollDSix();
                                int potionSizeValue = Dice.RollDSix();
                                int potionDurationValue = Dice.RollDSix();

                                Potions potion = new(dungeon.Depth, potionTypeValue, potionSizeValue, potionDurationValue);

                                messages.Add(new Message($"Potion type: {potion.Type} (ROLL: {potionTypeValue})", potionTypeValue));
                                messages.Add(new Message($"Potion size: {potion.Size} (ROLL: {potionSizeValue})", potionSizeValue));
                                messages.Add(new Message($"Potion duration: {potion.Duration} (ROLL: {potionDurationValue})", potionDurationValue));

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

                                messages.Add(new Message($"DRINK A {potion.Description()}"));

                                break;
                            default:
                                throw new ArgumentOutOfRangeException("Loot tile type.");
                        }
                        break;
                    case DungeonEvents.Macguffin:
                        dungeon.MacGuffinFound = true;
                        selectedTile.Type = DungeonEvents.Empty;
                        messages.Add(new Message("MACGUFFIN FOUND.  NOW GET OUT."));
                        break;
                    case DungeonEvents.FoundWeapon:
                    case DungeonEvents.FoundProtection:
                    case DungeonEvents.FoundPotion:
                    case DungeonEvents.TakenWeapon:
                    case DungeonEvents.TakenProtection:
                    case DungeonEvents.TakenPotion:
                    case DungeonEvents.Empty:
                    case DungeonEvents.FightWon:

                        break;
                    case DungeonEvents.Unknown:
                    default:
                        throw new ArgumentOutOfRangeException("Selected Dungeon Tiles Tile Type");
                }

                if (setSelectable) {
                    currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                }

                //Update Adventurer
                SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
                dungeon.Adventurer = sharedAdventurer;

                AdventurerUpdate.Update(sharedAdventurer);

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

        public async Task<SharedDungeon> MonsterFlee(Guid dungeonId, Guid tileId) {
            await Task.Delay(1);

            try {
                SharedDungeon? dungeon = DungeonQueries.Get(dungeonId);
                if (dungeon == null || dungeon.Id != dungeonId) { throw new ArgumentNullException("Dungeon"); }
                if (dungeon.Messages == null) { throw new ArgumentNullException("Dungeon Messages"); }
                if (dungeon.Levels == null) { throw new ArgumentNullException("Dungeon Level"); }
                if (dungeon.Adventurer == null || dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }

                SharedLevel? currentLevel = dungeon.Levels.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
                if (currentLevel == null || currentLevel.Tiles == null || currentLevel.Tiles.Count == 0) { throw new ArgumentNullException("Dungeon Level Tiles"); }

                SharedTile? selectedTile = currentLevel.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
                if (selectedTile == null || selectedTile.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Level selected Tile"); }

                Tiles currentLevelTiles = new(currentLevel.Tiles);

                Messages messages = new();

                Adventurer adventurer = new(dungeon.Adventurer);
                adventurer.DurationDecrement();

                if (AdventurerFleesCombat()) {
                    dungeon.InCombat = false;
                    dungeon.CombatTile = Guid.Empty;
                    dungeon.CombatInitiated = false;

                    messages.Add(new Message("ADVENTURER FLEES COMBAT"));

                    currentLevelTiles.SetSelectableTiles(selectedTile.Row, selectedTile.Column);
                } else {
                    messages.Add(new Message("ADVENTURER FAILED TO FLEE"));

                    if (selectedTile.Monsters == null || selectedTile.Monsters.Count == 0) { throw new ArgumentNullException("Dungeon Level Tile Monsters"); }

                    List<SharedMonster> monsters = selectedTile.Monsters;
                    int monsterindex = Dice.RandomNumber(0, (monsters.Count() - 1));
                    Monster currentMonster = new(monsters[monsterindex]);

                    int adventurerProtection = adventurer.GetProtection();
                    int monsterDamage = currentMonster.Damage;

                    int woundsReceived = monsterDamage - adventurerProtection;
                    if (woundsReceived > 0) {
                        int adventurerDammage = adventurer.reciveWounds(woundsReceived);

                        if (!adventurer.IsAlive) {
                            selectedTile.Type = DungeonEvents.FightLost;

                            dungeon.InCombat = false;

                            currentLevelTiles.Unhide();

                            messages.Add(new Message($"ADVENTURER KILLED WITH {adventurerDammage}"));
                        } else {
                            messages.Add(new Message($"ADVENTURER HIT FOR {adventurerDammage} WITH {adventurer.HealthBase} REMAINING"));
                        }
                    } else {
                        messages.Add(new Message("MONSTER WAS UNABLE TO CAUSE HARM"));
                    }
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

                SharedLevel? currentLevel = dungeon.Levels.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
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
                adventurer.DurationDecrement();

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

                        int currentHealth = monsterHealth - monsterWounds;
                        if (currentHealth > 0) {
                            currentMonster.Health = currentHealth;
                            SharedMonster sharedMonster = currentMonster.SharedModelMapper();

                            MonsterUpdate.Update(sharedMonster);

                            monsters[monsterindex] = sharedMonster;

                            messages.Add(new Message($"MONSTER HIT FOR {monsterWounds} WITH {currentHealth} REMAINING"));
                        } else {
                            monsterHealth = 0;

                            adventurer.Experience += currentMonster.Experience;

                            messages.Add(new Message($"MONSTER KILLED WITH {monsterWounds}"));

                            //remove monster at stack
                            MonsterDelete.Delete(currentMonster.Id);
                            monsters.RemoveAt(monsterindex);

                            //checked for remaining monsters
                            if (monsters.Count == 0) {
                                //Update Adventurer
                                int previousAdventurerLevel = adventurer.Level;
                                int previousAdventurerHealth = adventurer.HealthBase;
                                int previousAdventurerDamage = adventurer.DamageBase;
                                int previousAdventurerProtection = adventurer.ProtectionBase;

                                adventurer.LevelUp();
                                if (adventurer.Level > previousAdventurerLevel) {

                                    int currentAdventurerLevel = adventurer.Level;
                                    int currentAdventurerHealth = adventurer.HealthBase;
                                    int currentAdventurerDamage = adventurer.DamageBase;
                                    int currentAdventurerProtection = adventurer.ProtectionBase;

                                    messages.Add(new Message($"NEW LEVEL ({currentAdventurerLevel})"));
                                    messages.Add(new Message($"NEW HEALTH {adventurer.HealthBase} (+ {currentAdventurerHealth - previousAdventurerHealth})"));
                                    messages.Add(new Message($"NEW DAMAGE {adventurer.DamageBase} (+ {currentAdventurerDamage - previousAdventurerDamage})"));
                                    messages.Add(new Message($"NEW PROTECTION {adventurer.ProtectionBase} (+ {currentAdventurerProtection - previousAdventurerProtection})"));
                                }

                                //Update Dungeon
                                dungeon.InCombat = false;
                                dungeon.CombatTile = Guid.Empty;
                                dungeon.CombatInitiated = false;

                                //Update Level
                                selectedTile.FightWon = true;
                                selectedTile.Type = DungeonEvents.FightWon;

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

                            //do at least one damage to monsters to prevent deadlocking 
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

                                selectedTile.Type = DungeonEvents.FightLost;

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

                SharedAdventurer sharedAdventurer = adventurer.SharedModelMapper();
                dungeon.Adventurer = sharedAdventurer;

                AdventurerUpdate.Update(sharedAdventurer);

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
