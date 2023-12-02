using Microsoft.Extensions.Localization;

using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Data {
    public class MessageManager {
        private IStringLocalizer<DungeonManager> _localiser;

        public MessageManager(IStringLocalizer<DungeonManager> localiser) {
            _localiser = localiser;
        }

        //Error
        //  No Dungeon found
        public string ErrorDungeonNoFound() {
            return _localiser["ErrorDungeonNoFound"];
        }

        //Dungeon
        //  Exit
        //      Fail
        public string DungeonExitFail() {
            return _localiser["MessageDungeonExitFail"];
        }

        //      Success
        public string DungeonExitSuccess() {
            return _localiser["MessageDungeonExitSuccess"];
        }

        //  Depth
        public string DungeonIncreasedDepth(int increasedDepth) {
            string messageDungeonDepth = _localiser["MessageDungeonDepth"];
            return messageDungeonDepth.Replace("[INCREASED_DEPTH]", increasedDepth.ToString());
        }

        //Adventure
        //  Generation
        public string AdventureGeneration(int health, int damage, int protection) {
            return _localiser["MessageAdventureGeneration"];
        }

        //      Health
        public string AdventureHealth(int health) {
            string messageAdventureHealth = _localiser["MessageAdventureHealth"];
            return messageAdventureHealth.Replace("[ADVENTURER_HEALTH]", health.ToString());
        }

        //      Damage
        public string AdventureDamage(int damage) {
            string messageAdventureDamage = _localiser["MessageAdventureDamage"];
            return messageAdventureDamage.Replace("[ADVENTURER_DAMAGE]", damage.ToString());
        }

        //      Protection
        public string AdventureProtection(int protection) {
            string messageAdventureProtection = _localiser["MessageAdventureProtection"];
            return messageAdventureProtection.Replace("[ADVENTURER_PROTECTION]", protection.ToString());
        }

        //  Levels
        public string AdventurerLevelUp(int currentAdventurerLevel) {
            string messageAdventurerLevelUp = _localiser["MessageAdventurerLevelUp"];
            return messageAdventurerLevelUp.Replace("[ADVENTURER_CURRENT_DAMAGE]", currentAdventurerLevel.ToString());
        }

        //      Health
        public string AdventurerLevelUpHealth(int adventurerHealthBase, int levelUpHealth) {
            string messageAdventurerLevelUpHealth = _localiser["MessageAdventurerLevelUpHealth"];
            return messageAdventurerLevelUpHealth.Replace("[ADVENTURER_HEALTH]", adventurerHealthBase.ToString()).Replace("[ADVENTURER_HEALTH_ADDITION]", levelUpHealth.ToString());
        }

        //      Damage
        public string AdventurerLevelUpDamage(int adventurerDamageBase, int levelUpDamage) {
            string messageAdventurerLevelUpDamage = _localiser["MessageAdventurerLevelUpDamage"];
            return messageAdventurerLevelUpDamage.Replace("[ADVENTURER_DAMAGE]", adventurerDamageBase.ToString()).Replace("[ADVENTURER_DAMAGE_ADDITION]", levelUpDamage.ToString());
        }

        //      Damage
        public string AdventurerLevelUpProtection(int adventurerProtectionBase, int levelUpProtection) {
            string messageAdventurerLevelUpProtection = _localiser["MessageAdventurerLevelUpProtection"];
            return messageAdventurerLevelUpProtection.Replace("[ADVENTURER_PROTECTION]", adventurerProtectionBase.ToString()).Replace("[ADVENTURER_PROTECTION_ADDITION]", levelUpProtection.ToString());
        }

        //  Weapons
        //      Equipped
        public string AdventurerWeaponEquipped(string weaponsDescription) {
            string messageWeaponEquipped = _localiser["MessageWeaponEquipped"];
            return messageWeaponEquipped.Replace("[WEAPON_DESCRIPTION]", weaponsDescription);
        }

        //      Rejected
        public string AdventurerWeaponRejected(string weaponsDescription) {
            string messageWeaponRejected = _localiser["MessageWeaponRejected"];
            return messageWeaponRejected.Replace("[WEAPON_DESCRIPTION]", weaponsDescription);
        }

        //      Condition
        public string AdventurerWeaponCondition(WeaponConditions weaponCondition, int weaponsConditionValue) {
            string messageWeaponCondition = _localiser["MessageWeaponCondition"];
            return messageWeaponCondition.Replace("[WEAPON_CONDITION]", weaponCondition.ToString()).Replace("[WEAPON_CONDITION_VALUE]", weaponsConditionValue.ToString());
        }

        //      Type
        public string AdventurerWeaponType(WeaponTypes weaponTypes, int weaponsTypeValue) {
            string messageWeaponType = _localiser["MessageWeaponType"];
            return messageWeaponType.Replace("[WEAPON_TYPE]", weaponTypes.ToString()).Replace("[WEAPON_TYPE_VALUE]", weaponsTypeValue.ToString());
        }

        //      Value
        public string AdventurerWeaponValue(int weaponValue, int weaponTypeValue, int weaponsConditionValue) {
            string messageWeaponValue = _localiser["MessageWeaponValue"];
            return messageWeaponValue.Replace("[WEAPON_VALUE]", weaponValue.ToString()).Replace("[WEAPON_TYPE_VALUE]", weaponTypeValue.ToString()).Replace("[WEAPON_CONDITION_VALUE]", weaponsConditionValue.ToString());
        }

        //  Armours
        //      Equipped
        public string AdventurerArmourEquipped(string armourDescription) {
            string messageArmourEquipped = _localiser["MessageArmourEquipped"];
            return messageArmourEquipped.Replace("[ARMOUR_DESCRIPTION]", armourDescription);
        }

        //      Rejected
        public string AdventurerArmourRejected(string weaponsDescription) {
            string messageWeaponRejected = _localiser["MessageArmourRejected"];
            return messageWeaponRejected.Replace("[ARMOUR_DESCRIPTION]", weaponsDescription);
        }

        //      Condition

        public string AdventurerArmourCondition(ArmourConditions armourCondition, int armourConditionValue) {
            string messageArmourCondition = _localiser["MessageArmourCondition"];
            return messageArmourCondition.Replace("[ARMOUR_CONDITION]", armourCondition.ToString()).Replace("[ARMOUR_CONDITION_VALUE]", armourConditionValue.ToString());
        }

        //      Type
        public string AdventurerArmourType(ArmourTypes armourType, int armourTypeValue) {
            string messageArmourType = _localiser["MessageArmourType"];
            return messageArmourType.Replace("[ARMOUR_TYPE]", armourType.ToString()).Replace("[ARMOUR_TYPE_VALUE]", armourTypeValue.ToString());
        }

        //      Value
        public string AdventurerArmourValue(int armourValue, int armourTypeValue, int armourConditionValue) {
            string messageArmourValue = _localiser["MessageArmourValue"];
            return messageArmourValue.Replace("ARMOUR_VALUE", armourValue.ToString()).Replace("[ARMOUR_TYPE_VALUE]", armourTypeValue.ToString()).Replace("[ARMOUR_CONDITION_VALUE]", armourConditionValue.ToString());
        }

        //  Potions
        public string AdventurerPotionDrink(string potionDescription) {
            string messagePotionDrink = _localiser["MessagePotionDrink"];
            return messagePotionDrink.Replace("[POTION_DESCRIPTION]", potionDescription);
        }

        //      type
        public string AdventurerPotionType(PotionTypes potionType, int potionTypeValue) {
            string messagePotionType = _localiser["MessagePotionType"];
            return messagePotionType.Replace("[POTION_TYPE]", potionType.ToString()).Replace("[POTION_TYPE_VALUE]", potionTypeValue.ToString());
        }

        //      Size
        public string AdventurerPotionSize(PotionSizes potionSize, int potionSizeValue) {
            string messagePotionSize = _localiser["MessagePotionSize"];
            return messagePotionSize.Replace("[POTION_SIZE]", potionSize.ToString()).Replace("[POTION_SIZE_VALUE]", potionSizeValue.ToString());
        }

        //      Duration
        public string AdventurerPotionDuration(PotionDurations potionDuration, int potionDurationValue) {
            string messagePotionDuration = _localiser["MessagePotionDuration"];
            return messagePotionDuration.Replace("[POTION_DURATION]", potionDuration.ToString()).Replace("[POTION_DURATION_VALUE]", potionDurationValue.ToString());
        }

        //  Flee
        //      Success
        public string AdventurerFleeSuccess() {
            return _localiser["MessageAdventurerFleeSuccess"];
        }

        //      Fail
        public string AdventurerFleeFail() {
            return _localiser["MessageAdventurerFleeFail"];
        }

        //Monster
        //  Monster Camp
        //      Single
        public string MonsterCampSingle(string monstersName) {
            string messageMonsterCampSingle = _localiser["MessageMonsterCampSingle"];
            return messageMonsterCampSingle.Replace("[MONSTER_NAME]", monstersName);
        }

        //      Multiple
        public string MonsterCampMultiple(string monstersName, string monstersCount) {
            string messageMonsterCampMultiple = _localiser["MessageMonsterCampMultiple"];
            return messageMonsterCampMultiple.Replace("[MONSTER_COUNT]", monstersCount).Replace("[MONSTER_NAME]", monstersName);
        }

        //  Wandering monsters
        //      Single
        public string WanderingMonsterSingle(string monstersName) {
            string messageMonsterCampSingle = _localiser["MessageWanderingMonsterSingle"];
            return messageMonsterCampSingle.Replace("[MONSTER_NAME]", monstersName);
        }

        //      Multiple
        public string WanderingMonsterMultiple(string monstersName, int monstersCount) {
            string messageMonsterCampMultiple = _localiser["MessageWanderingMonsterMultiple"];
            return messageMonsterCampMultiple.Replace("[MONSTER_COUNT]", monstersCount.ToString()).Replace("[MONSTER_NAME]", monstersName);
        }

        //  Generation
        public string MonsterGeneration(string monstersType) {
            string messageMonsterGeneration = _localiser["MessageMonsterGeneration"];
            return messageMonsterGeneration.Replace("[MONSTER_TYPE]", monstersType);
        }

        //      Health
        public string MonsterHealth(int health) {
            string messageMonsterHealth = _localiser["MessageMonsterHealth"];
            return messageMonsterHealth.Replace("[MONSTER_HEALTH]", health.ToString());
        }

        //      Damage
        public string MonsterDamage(int damage) {
            string messageMonsterDamage = _localiser["MessageMonsterDamage"];
            return messageMonsterDamage.Replace("[MONSTER_DAMAGE]", damage.ToString());
        }

        //      Protection
        public string MonsterProtection(int protection) {
            string messageMonsterProtection = _localiser["MessageMonsterProtection"];
            return messageMonsterProtection.Replace("[MONSTER_PROTECTION]", protection.ToString());
        }

        //  FinalBoss
        //      Generation
        public string FinalBossGeneration(string monsterName) {
            string messageFinalBossGeneration = _localiser["MessageFinalBossGeneration"];
            return messageFinalBossGeneration.Replace("[BOSS_NAME]", monsterName);
        }

        //      Fight
        //          won
        public string FinalBossFightWon() {
            return _localiser["MessageFinalBossFightWon"];
        }

        //          Lost
        public string FinalBossFightWon(int monsterWounds) {
            string messageFinalBossFightLost = _localiser["MessageFinalBossFightLost"];
            return messageFinalBossFightLost.Replace("[MONSTER_WOUNDS_RECEIVED]", monsterWounds.ToString());
        }

        //Combat
        //  Initiates
        //      Adventurer 
        public string AdventurerInitiatesCombat() {
            return _localiser["MessageAdventurerInitiatesCombat"];
        }

        //          Roll
        public string AdventurerCombatInitiation(int adventurerCombatInitiationRoll) {
            string messageAdventurerCombatInitiation = _localiser["MessageAdventurerCombatInitiation"];
            return messageAdventurerCombatInitiation.Replace("[ADVENTURER_COMBAT_INITIATION]", adventurerCombatInitiationRoll.ToString());
        }        

        //      Monster 
        public string MonsterInitiatesCombats() {
            return _localiser["MessageMonsterInitiatesCombats"];
        }

        //          Roll
        public string MonsterCombatInitiation(int monsterCombatInitiationRoll) {
            string messageMonsterCombatInitiation = _localiser["MessageMonsterCombatInitiation"];
            return messageMonsterCombatInitiation.Replace("[MONSTER_COMBAT_INITIATION]", monsterCombatInitiationRoll.ToString());
        }

        //  Adventurer
        //      Hit
        public string AdventurerAttackHits(int monsterWounds, int adventurerCurrentHealth) {
            string messageAdventurerAttackHits = _localiser["MessageAdventurerAttackHits"];
            return messageAdventurerAttackHits.Replace("[MONSTER_WOUNDS_RECEIVED]", monsterWounds.ToString()).Replace("[MONSTER_HEALTH_REMAINING]", adventurerCurrentHealth.ToString());
        }

        //      Details
        public string AdventurerAttackDetails(int adventurerAttackValue, int monsterDodgeValue) {
            string messageAdventurerAttackDetails = _localiser["MessageAdventurerAttackDetails"];
            return messageAdventurerAttackDetails.Replace("[ADVENTURER_ATTACK]", adventurerAttackValue.ToString()).Replace("[MONSTER_DODGE]", monsterDodgeValue.ToString());
        }

        //      Dodged
        public string AdventurerAttackDodged(int monsterDodgeValue, int adventurerAttackValue) {
            string messageAdventurerAttackDodged = _localiser["MessageAdventurerAttackDodged"];
            return messageAdventurerAttackDodged.Replace("[MONSTER_DODGE]", monsterDodgeValue.ToString()).Replace("[ADVENTURER_ATTACK]", adventurerAttackValue.ToString());
        }

        //      Miss
        public string AttacksMiss() {
            return _localiser["MessageAttacksMiss"];
        }

        //          details
        public string MonsterAttackMissDetails(int adventurerDodgeValue, int monsterAttackValue) {
            string messageMonsterAttackMissDetails = _localiser["MessageMonsterAttackMissDetails"];
            return messageMonsterAttackMissDetails.Replace("[ADVENTURER_DODGE]", adventurerDodgeValue.ToString()).Replace("[MONSTER_ATTACK]", monsterAttackValue.ToString());
        }

        //      Wiff
        public string AdventurerWiff() {
            return _localiser["MessageAdventurerWiff"];
        }

        //          Details
        public string AdventurerWiffDetails(int adventurerDamage, int monsterProtection) {
            string MessageAdventurerWiffDetails = _localiser["MessageAdventurerWiffDetails"];
            return MessageAdventurerWiffDetails.Replace("[ADVENTURER_DAMAGE]", adventurerDamage.ToString()).Replace("[MONSTER_PROTECTION]", monsterProtection.ToString());
        }

        //  Monster
        //      Hit
        public string MonsterAttackHit(int adventurerWounds, int adventurerHealthBase) {
            string messageMonsterAttackHit = _localiser["MessageMonsterAttackHit"];
            return messageMonsterAttackHit.Replace("[ADVENTURER_WOUNDS]", adventurerWounds.ToString()).Replace("[ADVENTURER_HEALTH]", adventurerHealthBase.ToString());
        }

        //      Details
        public string MonsterAttackHitDetails(int monsterAttackValue, int adventurerDodgeValue) {
            string messageMonsterAttackHitDetails = _localiser["MessageMonsterAttackHitDetails"];
            return messageMonsterAttackHitDetails.Replace("[MONSTER_ATTACK]", monsterAttackValue.ToString()).Replace("[ADVENTURER_DODGE]", adventurerDodgeValue.ToString());
        }

        //      Damage
        public string MonsterAttackHitDamage(int monsterDamage, int adventurerProtection) {
            string messageMonsterAttackHitDamage = _localiser["MessageMonsterAttackHitDamage"];
            return messageMonsterAttackHitDamage.Replace("[MONSTER_DAMAGE]", monsterDamage.ToString()).Replace("[ADVENTURER_PROTECTION]", adventurerProtection.ToString());
        }

        //      Won
        public string MonsterKilledAdventure(int adventurerWounds) {
            string MmessageMonsterKilledAdventure = _localiser["MessageMonsterKilledAdventure"];
            return MmessageMonsterKilledAdventure.Replace("[ADVENTURER_WOUNDS]", adventurerWounds.ToString());
        }

        //      No damage
        public string MonsterAttackNoDamage() {
            return _localiser["MessageMonsterAttackNoDamage"];
        }

        //End of Game
        //  Floors
        public string EndOfGameFloors(int floorsDiscovered) {
            string messageEndOfGameFloors = _localiser["MessageEndOfGameFloors"];
            return messageEndOfGameFloors.Replace("[FLOORS_DISCOVERED]", floorsDiscovered.ToString());
        }

        //  Tiles
        public string EndOfGameFloorDetails(int floorDepth, int tilesFound, int tilesTotal) {
            string messageEndOfGameFloorDetails = _localiser["MessageEndOfGameFloorDetails"];
            return messageEndOfGameFloorDetails.Replace("[FLOOR_DEPTH]", floorDepth.ToString()).Replace("[FLOOR_TILES_FOUND]", tilesFound.ToString()).Replace("[FLOOR_TILES_TOTAL]", tilesTotal.ToString());
        }

        //  Loot
        public string EndOfGameLootedChests(int floorDepth, int chestsLooted) {
            string messageEndOfGameLootedChests = _localiser["MessageEndOfGameLootedChests"];
            return messageEndOfGameLootedChests.Replace("[FLOOR_DEPTH]", floorDepth.ToString()).Replace("[CHESTS_LOOTED]", chestsLooted.ToString());
        }

        //      Wepons
        public string EndOfGameLootedWepons(int takenWeapon) {
            string messageEndOfGameLootedWepons = _localiser["MessageEndOfGameLootedWepons"];
            return messageEndOfGameLootedWepons.Replace("[WEAPONS_LOOTED]", takenWeapon.ToString());
        }

        //      Protection
        public string EndOfGameLootedProtection(int takenProtection) {
            string messageEndOfGameLootedProtection = _localiser["MessageEndOfGameLootedProtection"];
            return messageEndOfGameLootedProtection.Replace("[PROTECTION_LOOTED]", takenProtection.ToString());
        }

        //      Protection
        public string EndOfGameLootedPotions(int takenPotion) {
            string messageEndOfGameLootedPotions = _localiser["MessageEndOfGameLootedPotions"];
            return messageEndOfGameLootedPotions.Replace("[POTIONS_LOOTED]", takenPotion.ToString());
        }

        //  Monsters
        //      Fights
        //          Total
        public string EndOfGameFightsTotal(int floorDepth, int fights) {
            string messageEndOfGameFightsTotal = _localiser["MessageEndOfGameFightsTotal"];
            return messageEndOfGameFightsTotal.Replace("[FLOOR_DEPTH]", floorDepth.ToString()).Replace("[FIGHTS_TOTAL]", fights.ToString());
        }

        //          Fleed
        public string EndOfGameFightsFleed(int fightsFleed) {
            string messageEndOfGameFightsFleed = _localiser["MessageEndOfGameFightsFleed"];
            return messageEndOfGameFightsFleed.Replace("[FIGHTS_FLED]", fightsFleed.ToString());
        }

        //          Fleed
        public string EndOfGameFightsWon(int fightsWon) {
            string messageEndOfGameFightsWon = _localiser["MessageEndOfGameFightsWon"];
            return messageEndOfGameFightsWon.Replace("[FIGHTS_WON]", fightsWon.ToString());
        }
    }
}
