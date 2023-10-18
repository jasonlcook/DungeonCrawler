using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public class Potions {
        public PotionTypes Type { get; set; }
        public PotionSizes Size { get; set; }
        public int SizeValue { get; set; }
        public PotionDurations Duration { get; set; }
        public int DurationValue { get; set; }

        public Potions(int depth, int typeValue, int sizeValue, int durationValue) {
            Type = GetType(typeValue);

            Size = GetSize(depth, sizeValue);
            SizeValue = GetSizeValue();

            Duration = GetDuration(durationValue);
            DurationValue = GetDurationValue();
        }

        public string Description() {
            return $"Drink a {Size} of {Duration} duration {Type} potion.";
        }

        //  Type
        //      1 - 2:  Sheild (Protection)
        //      3 - 4:  Damage
        //      5 - 6:  Aura (Health)
        private PotionTypes GetType(int value) {
            switch (value) {
                case 1:
                case 2:
                    return PotionTypes.Sheild;
                case 3:
                case 4:
                    return PotionTypes.Damage;
                case 5:
                case 6:
                    return PotionTypes.Aura;
                default:
                    throw new ArgumentOutOfRangeException("Potion type selection");
            }
        }

        //  Size
        //      Floor 1 - 4
        //          1 - 5:  Vial
        //          6:      Flask

        //      Floor 5 +
        //          1:      Vial
        //          2 - 5:  Flask
        //          6:      Bottle
        private PotionSizes GetSize(int depth, int value) {
            if (depth > 4) {
                switch (value) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return PotionSizes.Vial;
                    case 6:
                        return PotionSizes.Flask;
                    default:
                        throw new ArgumentOutOfRangeException("Potion size selection");
                }
            } else {
                switch (value) {
                    case 1:
                        return PotionSizes.Vial;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return PotionSizes.Flask;
                    case 6:
                        return PotionSizes.Bottle;
                    default:
                        throw new ArgumentOutOfRangeException("Potion size selection");
                }
            }
        }

        private int GetSizeValue() {
            switch (Size) {
                case PotionSizes.Vial:
                    return 6;
                case PotionSizes.Flask:
                    return 12;
                case PotionSizes.Bottle:
                    return 18;
                case PotionSizes.Unknown:
                default:
                    throw new ArgumentOutOfRangeException("Potion size value selection");
            }
        }

        //  Duration
        //      1 - 2:  Short
        //      3 - 4:  Medium
        //      5 - 6:  Long
        private PotionDurations GetDuration(int value) {
            switch (value) {
                case 1:
                case 2:
                    return PotionDurations.Short;
                case 3:
                case 4:
                    return PotionDurations.Medium;
                case 5:
                case 6:
                    return PotionDurations.Long;
                default:
                    throw new ArgumentOutOfRangeException("Potion duration selection");
            }
        }

        private int GetDurationValue() {
            switch (Duration) {
                case PotionDurations.Short:
                    return 10;
                case PotionDurations.Medium:
                    return 20;
                case PotionDurations.Long:
                    return 30;
                case PotionDurations.Unknown:
                default:
                    throw new ArgumentOutOfRangeException("Potion duration value selection");
            }
        }
    }
}
