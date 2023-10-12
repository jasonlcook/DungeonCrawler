using BlazorDungeonCrawler.Shared.Enumerators;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Models {
    public class Weapons {
        public WeaponTypes Type { get; set; }
        public int TypeValue { get; set; }
        public WeaponConditions Condition { get; set; }
        public int ConditionValue { get; set; }

        public int WeaponValue { get; set; }

        public Weapons(int depth, int typeValue, int conditionValue) {
            Type = GetType(depth, typeValue);
            TypeValue = GetTypeValue();

            ConditionValue = 1;
            if (Type != WeaponTypes.Rock || Type != WeaponTypes.Club) {
                Condition = GetCondition(depth, conditionValue);
                ConditionValue = GetConditionValue();
            } 

            WeaponValue = TypeValue * ConditionValue;
        }

        public string Description() {
            if (Type == WeaponTypes.Rock || Type == WeaponTypes.Club) {
                return Type.ToString();
            } else {
                return  $"{Condition} {Type}";
            }
        }

        //  Type
        //      Level 1 - 2
        //          1 - 5:  Rock
        //          6:      Club

        //      Level 3 - 4
        //          1:      Rock
        //          2 - 5:  Club
        //          6:      Mace

        //      Level 5 +
        //          1:      Club
        //          2 - 3:  Mace
        //          4 - 5:  Axe
        //          6:      Sword
        private WeaponTypes GetType(int depth, int value) {
            if (depth > 4) {
                //Level 5 +
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
                //Level 3 - 4
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
                //      Level 1 - 2
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

        //  Type
        //      Level 1 - 2
        //          1 - 5:  Broken
        //          6:      Rusty

        //      Level 3 - 4
        //          1:      Broken
        //          2 - 5:  Rusty
        //          6:      Chipped

        //      Level 5 +
        //          1:      Rusty
        //          2 - 3:  Chipped
        //          4 - 5:  Sharp
        //          6:      Flaming
        private WeaponConditions GetCondition(int depth, int value) {
            if (depth > 4) {
                //Level 5 +
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
                //Level 3 - 4
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
                //      Level 1 - 2
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
