//**********************************************************************************************************************
//  Weapon
//  The value of each weapon is calculated from the value of its type multiplied by the value of its condition.  Both
//  the type and condition values are calculated from dice rolls.
//  Can be found as part of the loot table gained by discovering a chest

using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public class Weapon {
        //****************************
        //***************** Attributes
        public int WeaponValue { get; set; }                //Numeric value of the weapon, calculated from the type value multiplied by the condition
                                                            
        //  Type                                            
        public WeaponTypes Type { get; set; }               //Type of weapon
        public int TypeValue { get; set; }                  //Damage value of weapon type
                                                            
                                                            
        //  Condition                                       
        public WeaponConditions Condition { get; set; }     //Condition type
        public int ConditionValue { get; set; }             //Value of the condition multiplier


        //****************************
        //*************** Constructors
        public Weapon(int depth, int typeValue, int conditionValue) {
            Type = GetType(depth, typeValue);
            TypeValue = GetTypeValue();

            ConditionValue = 1;
            if (Type != WeaponTypes.Rock || Type != WeaponTypes.Club) {
                Condition = GetCondition(depth, conditionValue);
                ConditionValue = GetConditionValue();
            }

            WeaponValue = TypeValue * ConditionValue;
        }

        //****************************
        //****************** Operation        

        //  Return the current armour details description as text  
        public string Description() {
            if (Type == WeaponTypes.Rock || Type == WeaponTypes.Club) {
                return Type.ToString();
            } else {
                return $"{Condition} {Type}";
            }
        }

        //  Type
        //  The type of obtainable weapon depends on the current floor depth and the value of the dice roll.
        //  The deeper the floor the greater the possible damage is available from the weapon.

        //      Get the weapon type from the floor depth and dice roll
        //          Floor 1 - 2
        //              1 - 5:  Rock
        //              6:      Club

        //          Floor 3 - 4
        //              1:      Rock
        //              2 - 5:  Club
        //              6:      Mace

        //          Floor 5 +
        //              1:      Club
        //              2 - 3:  Mace
        //              4 - 5:  Axe
        //              6:      Sword
        private WeaponTypes GetType(int depth, int value) {
            if (depth > 4) {
                //Floor 5 +
                switch (value) {
                    case 1:
                        return WeaponTypes.Club;
                    case 2:
                    case 3:
                        return WeaponTypes.Mace;
                    case 4:
                    case 5:
                        return WeaponTypes.Axe;
                    case 6:
                        return WeaponTypes.Sword;
                    default:
                        throw new ArgumentOutOfRangeException("Weapon type selection");
                }
            } else if (depth > 2) {
                //Floor 3 - 4
                switch (value) {
                    case 1:
                        return WeaponTypes.Rock;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return WeaponTypes.Club;
                    case 6:
                        return WeaponTypes.Mace;
                    default:
                        throw new ArgumentOutOfRangeException("Weapon type selection");
                }
            } else {
                //      Floor 1 - 2
                switch (value) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return WeaponTypes.Rock;
                    case 6:
                        return WeaponTypes.Club;
                    default:
                        throw new ArgumentOutOfRangeException("Weapon type selection");
                }
            }
        }

        private int GetTypeValue() {
            switch (Type) {
                case WeaponTypes.Rock:
                    return 1;
                case WeaponTypes.Club:
                    return 2;
                case WeaponTypes.Mace:
                    return 3;
                case WeaponTypes.Axe:
                    return 4;
                case WeaponTypes.Sword:
                    return 5;
                case WeaponTypes.Unknown:
                default:
                    throw new ArgumentOutOfRangeException("Weapon type value selection");
            }
        }

        //  Condition
        //      Floor 1 - 2
        //          1 - 5:  Broken
        //          6:      Rusty

        //      Floor 3 - 4
        //          1:      Broken
        //          2 - 5:  Rusty
        //          6:      Chipped

        //      Floor 5 +
        //          1:      Rusty
        //          2 - 3:  Chipped
        //          4 - 5:  Sharp
        //          6:      Flaming
        private WeaponConditions GetCondition(int depth, int value) {
            if (depth > 4) {
                //Floor 5 +
                switch (value) {
                    case 1:
                        return WeaponConditions.Rusty;
                    case 2:
                    case 3:
                        return WeaponConditions.Chipped;
                    case 4:
                    case 5:
                        return WeaponConditions.Sharp;
                    case 6:
                        return WeaponConditions.Flaming;
                    default:
                        throw new ArgumentOutOfRangeException("Weapon conditions selection");
                }
            } else if (depth > 2) {
                //Floor 3 - 4
                switch (value) {
                    case 1:
                        return WeaponConditions.Broken;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return WeaponConditions.Rusty;
                    case 6:
                        return WeaponConditions.Chipped;
                    default:
                        throw new ArgumentOutOfRangeException("Weapon conditions selection");
                }
            } else {
                //      Floor 1 - 2
                switch (value) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return WeaponConditions.Broken;
                    case 6:
                        return WeaponConditions.Rusty;
                    default:
                        throw new ArgumentOutOfRangeException("Weapon conditions selection");
                }
            }
        }

        private int GetConditionValue() {
            switch (Condition) {
                case WeaponConditions.Broken:
                    return 1;
                case WeaponConditions.Rusty:
                    return 2;
                case WeaponConditions.Chipped:
                    return 3;
                case WeaponConditions.Sharp:
                    return 4;
                case WeaponConditions.Flaming:
                    return 10;
                case WeaponConditions.Unknown:
                default:
                    throw new ArgumentOutOfRangeException("Weapon conditions value selection");
            }
        }
    }
}
