//**********************************************************************************************************************
//  Adventurer
//  Server version of the Database and Client class containing mothods for updating state and mapping between Shared
//  version
using BlazorDungeonCrawler.Shared.Models;
using SharedAdventurer = BlazorDungeonCrawler.Shared.Models.Adventurer;

namespace BlazorDungeonCrawler.Server.Models {
    public class Adventurer {
        //****************************
        //***************** Attributes
        public Guid Id { get; private set; }                //Database Id

        public bool IsAlive { get; set; }                   //Player status

        //  Experience
        public int ExperienceLevel { get; set; }            //Current experience level
        public int Experience { get; set; }                 //Amount of experience points
        public int NextExperienceLevelCost { get; set; }    //Required experience points for next level

        //  Dexterity
        public int DexterityBase { get; set; }              //Dexterity

        //  Health
        public int HealthBase { get; set; }                 //Current health points
        public int HealthInitial { get; set; }              //Initial health roll  
        public int AuraPotion { get; set; }                 //Amount of protection potion affords
        public int AuraPotionDuration { get; set; }         //How many steps the Aura potion will last

        //  Dammage
        public int DamageBase { get; set; }                 //Initial damage dealt to monsters
        public int DamagePotion { get; set; }               //Amount of additional damage from potion effect
        public int DamagePotionDuration { get; set; }       //How many steps the potion will last

        //  Protection
        public int ProtectionBase { get; set; }             //Initial protection from monster attacks
        public int ShieldPotion { get; set; }               //Amount of additional protection the potion affords
        public int ShieldPotionDuration { get; set; }       //How many steps the shield potion will last

        //      Armour
        //      If any of the below values are above zero that piece of armour is being worn providing that amount of
        //      additional protection
        public int ArmourHelmet { get; set; }
        public int ArmourBreastplate { get; set; }
        public int ArmourGauntlet { get; set; }
        public int ArmourGreave { get; set; }
        public int ArmourBoots { get; set; }

        //  Weapon
        public int Weapon { get; set; }                     //Current weapon attack value

        //****************************
        //*************** Constructors
        public Adventurer() {
            this.Id = Guid.NewGuid();
        }

        public Adventurer(int dexterity, int health, int damage, int protection) : base() {
            this.IsAlive = true;

            this.ExperienceLevel = 1;
            this.Experience = 0;
            this.NextExperienceLevelCost = GetLevelCost(this.ExperienceLevel + 1);

            this.DexterityBase = dexterity;
            this.HealthInitial = health;
            this.HealthBase = health;
            this.DamageBase = damage;
            this.ProtectionBase = protection;
        }

        //******************** Mapping

        //  DB > Class
        public Adventurer(SharedAdventurer adventurer) {
            this.Id = adventurer.Id;
            this.IsAlive = adventurer.IsAlive;
            this.ExperienceLevel = adventurer.ExperienceLevel;
            this.Experience = adventurer.Experience;
            this.DexterityBase = adventurer.DexterityBase;
            this.NextExperienceLevelCost = adventurer.NextExperienceLevelCost;
            this.HealthBase = adventurer.HealthBase;
            this.HealthInitial = adventurer.HealthInitial;
            this.AuraPotion = adventurer.AuraPotion;
            this.AuraPotionDuration = adventurer.AuraPotionDuration;
            this.DamageBase = adventurer.DamageBase;
            this.DamagePotion = adventurer.DamagePotion;
            this.DamagePotionDuration = adventurer.DamagePotionDuration;
            this.ProtectionBase = adventurer.ProtectionBase;
            this.ShieldPotion = adventurer.ShieldPotion;
            this.ShieldPotionDuration = adventurer.ShieldPotionDuration;
            this.Weapon = adventurer.Weapon;
            this.ArmourHelmet = adventurer.ArmourHelmet;
            this.ArmourBreastplate = adventurer.ArmourBreastplate;
            this.ArmourGauntlet = adventurer.ArmourGauntlet;
            this.ArmourGreave = adventurer.ArmourGreave;
            this.ArmourBoots = adventurer.ArmourBoots;
        }

        //  Class > DB
        public SharedAdventurer SharedModelMapper() {
            return new SharedAdventurer() {
                Id = this.Id,
                IsAlive = this.IsAlive,
                ExperienceLevel = this.ExperienceLevel,
                Experience = this.Experience,
                DexterityBase = this.DexterityBase,
                NextExperienceLevelCost = this.NextExperienceLevelCost,
                HealthBase = this.HealthBase,
                HealthInitial = this.HealthInitial,
                AuraPotion = this.AuraPotion,
                AuraPotionDuration = this.AuraPotionDuration,
                DamageBase = this.DamageBase,
                DamagePotion = this.DamagePotion,
                DamagePotionDuration = this.DamagePotionDuration,
                ProtectionBase = this.ProtectionBase,
                ShieldPotion = this.ShieldPotion,
                ShieldPotionDuration = this.ShieldPotionDuration,
                Weapon = this.Weapon,
                ArmourHelmet = this.ArmourHelmet,
                ArmourBreastplate = this.ArmourBreastplate,
                ArmourGauntlet = this.ArmourGauntlet,
                ArmourGreave = this.ArmourGreave,
                ArmourBoots = this.ArmourBoots
            };
        }

        //****************************
        //****************** Operation

        //  Leveling
        public int GetLevelCost(int level) {
            return GeometricSeries(1d, 2d, level);
        }

        public int GeometricSeries(double a, double r, int n) {
            double series = a * (1 - Math.Pow(r, n)) / (1 - r);
            return Convert.ToInt32(series);
        }

        public void LevelUp() {
            if (Experience >= NextExperienceLevelCost) {
                this.ExperienceLevel += 1;
                NextExperienceLevelCost = GetLevelCost(ExperienceLevel);

                int dexterityUpgrade = DexterityBase / 2;
                if (dexterityUpgrade < 1) {
                    dexterityUpgrade = 1;
                }

                DexterityBase += dexterityUpgrade;

                int healthUpgrade = HealthInitial / 2;
                if (healthUpgrade < 1) {
                    healthUpgrade = 1;
                }

                HealthBase += healthUpgrade;
                HealthInitial += healthUpgrade;

                int damageUpgrade = DamageBase / 2;
                if (damageUpgrade < 1) {
                    damageUpgrade = 1;
                }

                DamageBase += damageUpgrade;

                int protectionUpgrade = ProtectionBase / 2;
                if (protectionUpgrade < 1) {
                    protectionUpgrade = 1;
                }

                ProtectionBase += protectionUpgrade;

                LevelUp();
            }
        }

        //  Dexterity
        public int GetDexterity() {
            return DexterityBase;
        }


        //  Damage
        public int GetDamage() {
            return DamageBase + Weapon + DamagePotion;
        }

        //  Protection
        public int GetProtection() {
            return ProtectionBase + ShieldPotion + ArmourHelmet + ArmourBreastplate + ArmourGauntlet + ArmourGreave + ArmourBoots;
        }

        //  Potion
        public void DurationDecrement() {
            if (AuraPotionDuration > 0) {
                AuraPotionDuration -= 1;
                if (AuraPotionDuration == 0) {
                    AuraPotion = 0;
                }
            }

            if (DamagePotionDuration > 0) {
                DamagePotionDuration -= 1;
                if (DamagePotionDuration == 0) {
                    DamagePotion = 0;
                }
            }

            if (ShieldPotionDuration > 0) {
                ShieldPotionDuration -= 1;
                if (ShieldPotionDuration == 0) {
                    ShieldPotion = 0;
                }
            }
        }

        //      Aura
        //      If health has been lost then use the potion point to heal (up to initial rolled value) and add remaining points as Aura
        public int SetAuraPotion(int sizeValue) {
            int regainedHealth = 0;

            //check if damage has been taken
            if (HealthBase < HealthInitial) {
                int damageTaken = HealthInitial - HealthBase;

                if (damageTaken >= sizeValue) {
                    //if damage taken is more (than the potion value) add potion value to the current health
                    HealthBase += sizeValue;
                    regainedHealth = sizeValue;
                } else {
                    //if damage taken is less (than the potion value) heal the damage and use the remaining points as Aura
                    AuraPotion += sizeValue - damageTaken;
                    HealthBase = HealthInitial;
                    regainedHealth = damageTaken;
                }
            } else {
                AuraPotion += sizeValue;
            }

            return regainedHealth;
        }
    }
}