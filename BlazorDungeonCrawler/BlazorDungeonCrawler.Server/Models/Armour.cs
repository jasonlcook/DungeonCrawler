//**********************************************************************************************************************
//  Armour
//  All armour slots add additional protection on top of the base protection value.  Only one piece of armour can be used
//  in each slot so if a (numerically) superior piece of armour is acquired it will replace the current piece in that slot.
//  Can be found as part of the loot table gained by discovering a chest

using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public class Armour {
        //****************************
        //***************** Attributes
        public ArmourTypes Type { get; set; }               //Armour slot used
        public int TypeValue { get; set; }                  //Armour type value
        public ArmourConditions Condition { get; set; }     //Armour value multiplier
        public int ConditionValue { get; set; }             //Armour condition value

        public int ArmourValue { get; set; }                //Armour value (TypeValue * ConditionValue)

        //****************************
        //*************** Constructors
        public Armour(int depth, int typeValue, int conditionValue) {
            Type = GetType(depth, typeValue);
            TypeValue = GetTypeValue();

            Condition = GetCondition(depth, conditionValue);
            ConditionValue = GetConditionValue();

            ArmourValue = TypeValue * ConditionValue;
        }

        //****************************
        //****************** Operation

        //  Return the current armour details description as text  
        public string Description() {
            return $"{Condition} {Type}";
        }

        //  Type
        //  The type of obtainable armour depends on the current floor depth and the value of the dice roll.
        //  The deeper the floor the more protection is available from the armour.

        //      Get the armour type from the floor depth and dice roll
        //          Floor 1 - 2
        //              1 - 5:  Greave
        //              6:      Boots

        //          Floor 3 - 4
        //              1:      Greave
        //              2 - 5:  Boots
        //              6:      Gauntlet

        //          Floor 5 +
        //              1:      Boots
        //              2 - 3:  Gauntlet
        //              4 - 5:  Helmet
        //              6:      Breastplate
        private ArmourTypes GetType(int depth, int value) {
            if (depth > 4) {
                //Floor 5 +
                switch (value) {
                    case 1:
                        return ArmourTypes.Boots;
                    case 2:
                    case 3:
                        return ArmourTypes.Gauntlet;
                    case 4:
                    case 5:
                        return ArmourTypes.Helmet;
                    case 6:
                        return ArmourTypes.Breastplate;
                    default:
                        throw new ArgumentOutOfRangeException("Armour type selection");
                }
            } else if (depth > 2) {
                //Floor 3 - 4
                switch (value) {
                    case 1:
                        return ArmourTypes.Greave;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return ArmourTypes.Helmet;
                    case 6:
                        return ArmourTypes.Gauntlet;
                    default:
                        throw new ArgumentOutOfRangeException("Armour type selection");
                }
            } else {
                //      Floor 1 - 2
                switch (value) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return ArmourTypes.Greave;
                    case 6:
                        return ArmourTypes.Boots;
                    default:
                        throw new ArgumentOutOfRangeException("Armour type selection");
                }
            }
        }

        //      Get the protection value from the armour type
        private int GetTypeValue() {
            switch (Type) {
                case ArmourTypes.Boots:
                case ArmourTypes.Greave:
                    return 1;
                case ArmourTypes.Gauntlet:
                    return 3;
                case ArmourTypes.Helmet:
                case ArmourTypes.Breastplate:
                    return 4;
                case ArmourTypes.Unknown:
                default:
                    throw new ArgumentOutOfRangeException("Armour type value selection");
            }
        }

        //  Condition
        //  The condition is a multiplier applied to the base value of the protection type
        //  As with the armour type the deeper the floor the greater the available multiplier.

        //      Get the armour condition from the floor and dice roll
        //          Floor 1 - 4
        //              1 - 5:  Rusty
        //              6:      Tarnished

        //          Floor 5 +
        //              1:      Rusty
        //              2 - 5:  Tarnished
        //              6:      Shiny

        private ArmourConditions GetCondition(int depth, int value) {
            if (depth > 4) {
                switch (value) {
                    case 1:
                        return ArmourConditions.Rusty;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return ArmourConditions.Tarnished;
                    case 6:
                        return ArmourConditions.Shiny;
                    default:
                        throw new ArgumentOutOfRangeException("Armour condition selection");
                }
            } else {
                switch (value) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return ArmourConditions.Rusty;
                    case 6:
                        return ArmourConditions.Tarnished;
                    default:
                        throw new ArgumentOutOfRangeException("Armour condition selection");
                }
            }
        }

        //      Get the condition multiplier value from the armour condition
        private int GetConditionValue() {
            switch (Condition) {
                case ArmourConditions.Rusty:
                    return 1;
                case ArmourConditions.Tarnished:
                    return 2;
                case ArmourConditions.Shiny:
                    return 3;
                case ArmourConditions.Unknown:
                default:
                    throw new ArgumentOutOfRangeException("Armour condition value selection");
            }
        }
    }
}
