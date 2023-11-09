//**********************************************************************************************************************
//  Potion
//  There are three potion types each effecting one of the adventurer's base stats.  Each potion will have a maximum
//  value and will last a given duration. all three values are decided by dice rolls some are affected by the dungeon
//  depth of where they are found.
//  Can be found as part of the loot table gained by discovering a chest

using BlazorDungeonCrawler.Shared.Enumerators;

namespace BlazorDungeonCrawler.Server.Models {
    public class Potion {
        //****************************
        //***************** Attributes
        public PotionTypes Type { get; set; }               //Potion type (which stat is effected)

        public PotionSizes Size { get; set; }               //Potion size (value of the potion)
        public int SizeValue { get; set; }                  //Value of the potion size

        public PotionDurations Duration { get; set; }       //Potion duration (how many steps the potion lasts)
        public int DurationValue { get; set; }              //Value of the potion duration

        //****************************
        //*************** Constructors
        public Potion() {
            Type = PotionTypes.Unknown;
            Size = PotionSizes.Unknown;
            Duration = PotionDurations.Unknown;
        }

        public Potion(int depth, int typeValue, int sizeValue, int durationValue) {
            Type = GetType(typeValue);

            Size = GetSize(depth, sizeValue);
            SizeValue = GetSizeValue();

            Duration = GetDuration(durationValue);
            DurationValue = GetDurationValue();
        }

        //****************************
        //****************** Operation

        //  return the current potion details description as text  
        public string Description() {
            return $"Drink a {Size} of {Duration} duration {Type} potion.";
        }

        //  Type
        //  The type of the potion obtainable depends on the value of a dice roll.

        //      Get the potion type from the dice roll
        //          1 - 2:  Shield  (Protection)
        //          3 - 4:  Damage
        //          5 - 6:  Aura (Health)
        private PotionTypes GetType(int value) {
            switch (value) {
                case 1:
                case 2:
                    return PotionTypes.Shield;
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
        //  The size of the obtainable potion depends on the current floor depth and the value of the dice roll.
        //  The larger the size the higher the possible effect value and the deeper the floor the larger the possible size.

        //      Get the potion size from the floor depth and dice roll
        //          Floor 1 - 4
        //              1 - 5:  Vial
        //              6:      Flask

        //          Floor 5 +
        //              1:      Vial
        //              2 - 5:  Flask
        //              6:      Bottle
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

        //      Get the potion's effect value from the potion size
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
        //  Each potion will last a set amount of steps and once the potion is exhasted the effects will stop.
        //  The larger die value the longer the potion's duration

        //      Get the potion duration from the dice roll
        //          1 - 2:  Short
        //          3 - 4:  Medium
        //          5 - 6:  Long
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

        //      Get the potion duration value from the potion duration type.
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
