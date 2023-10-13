using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public class Armour {
        public ArmourTypes Type { get; set; }
        public int TypeValue { get; set; }
        public ArmourConditions Condition { get; set; }
        public int ConditionValue { get; set; }

        public int ArmourValue { get; set; }

        public Armour(int depth, int typeValue, int conditionValue) {
            Type = GetType(depth, typeValue);
            TypeValue = GetTypeValue();

            Condition = GetCondition(depth, conditionValue);
            ConditionValue = GetConditionValue();

            ArmourValue = TypeValue * ConditionValue;
        }

        public string Description() {
            return $"{Condition} {Type}";
        }

        //  Type
        //      Level 1 - 2
        //          1 - 5:  Greave
        //          6:      Boots

        //      Level 3 - 4
        //          1:      Greave
        //          2 - 5:  Boots
        //          6:      Gauntlet

        //      Level 5 +
        //          1:      Boots
        //          2 - 3:  Gauntlet
        //          4 - 5:  Helmet
        //          6:      Breastplate
        private ArmourTypes GetType(int depth, int value) {
            if (depth > 4) {
                //Level 5 +
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
                //Level 3 - 4
                switch (value) {
                    case 1:
                        return ArmourTypes.Greave;
                        break;
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
                //      Level 1 - 2
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
