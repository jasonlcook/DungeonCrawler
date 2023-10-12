using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public class Potion {
        public PotionType Type { get; set; }
        public PotionSize Size { get; set; }
        public int SizeValue { get; set; }
        public PotionDuration Duration { get; set; }
        public int DurationValue { get; set; }

        public Potion(int depth, int typeValue, int sizeValue, int durationValue) {
            Type = GetType(typeValue);

            Size = GetSize(depth, sizeValue);
            sizeValue = GetSizeValue();

            Duration = GetDuration(durationValue);
            durationValue = GetDurationValue();
        }

        //  Type
        //      1 - 2:  Sheild (Protection)
        //      3 - 4:  Damage
        //      5 - 6:  Aura (Health)
        private PotionType GetType(int value) {
            switch (value) {
                case 1:
                case 2:
                    return PotionType.Sheild;
                case 3:
                case 4:
                    return PotionType.Damage;
                case 5:
                case 6:
                    return PotionType.Aura;
                default:
                    throw new ArgumentOutOfRangeException("Potion type selection");
            }
        }

        //  Size
        //      Level 1 - 4
        //          1 - 5:  Vial
        //          6:      Flask

        //      Level 5 +
        //          1:      Vial
        //          2 - 5:  Flask
        //          6:      Bottle
        private PotionSize GetSize(int depth, int value) {
            if (depth > 4) {
                switch (value) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return PotionSize.Vial;
                    case 6:
                        return PotionSize.Flask;
                    default:
                        throw new ArgumentOutOfRangeException("Potion size selection");
                }
            } else {
                switch (value) {
                    case 1:
                        return PotionSize.Vial;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return PotionSize.Flask;
                    case 6:
                        return PotionSize.Bottle;
                    default:
                        throw new ArgumentOutOfRangeException("Potion size selection");
                }
            }
        }

        private int GetSizeValue() {
            switch (Size) {
                case PotionSize.Vial:
                    return 6;
                case PotionSize.Flask:
                    return 12;
                case PotionSize.Bottle:
                    return 18;
                case PotionSize.Unknown:
                default:
                    throw new ArgumentOutOfRangeException("Potion size value selection");
            }
        }

        //  Duration
        //      1 - 2:  Short
        //      3 - 4:  Medium
        //      5 - 6:  Long
        private PotionDuration GetDuration(int value) {
            switch (value) {
                case 1:
                case 2:
                    return PotionDuration.Short;
                case 3:
                case 4:
                    return PotionDuration.Medium;
                case 5:
                case 6:
                    return PotionDuration.Long;
                default:
                    throw new ArgumentOutOfRangeException("Potion duration selection");
            }
        }

        private int GetDurationValue() {
            switch (Duration) {
                case PotionDuration.Short:
                    return 10;
                case PotionDuration.Medium:
                    return 20;
                case PotionDuration.Long:
                    return 30;
                case PotionDuration.Unknown:
                default:
                    throw new ArgumentOutOfRangeException("Potion duration value selection");
            }
        }
    }
}
