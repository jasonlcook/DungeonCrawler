using BlazorDungeonCrawler.Shared.Models;
using SharedAdventurer = BlazorDungeonCrawler.Shared.Models.Adventurer;

namespace BlazorDungeonCrawler.Server.Models {
    public class Adventurer {
        public Guid Id { get; private set; }
        public int HealthBase { get; set; }
        public int HealthInitial { get; set; }
        public int AuraPotion { get; set; }
        public int AuraPotionDuration { get; set; }
        public int DamageBase { get; set; }
        public int DamagePotion { get; set; }
        public int DamagePotionDuration { get; set; }
        public int ProtectionBase { get; set; }
        public int ShieldPotion { get; set; }
        public int ShieldPotionDuration { get; set; }
        public int Weapon { get; set; }
        public int ArmourHelmet { get; set; }
        public int ArmourBreastplate { get; set; }
        public int ArmourGauntlet { get; set; }
        public int ArmourGreave { get; set; }
        public int ArmourBoots { get; set; }
        public bool IsAlive { get; set; }

        public Adventurer(int health, int damage, int protection) {
            this.Id = Guid.NewGuid();

            this.HealthInitial = health;
            this.HealthBase = health;
            this.DamageBase = damage;
            this.ProtectionBase = protection;

            this.IsAlive = true;
        }

        public Adventurer(SharedAdventurer adventurer) {
            this.Id = adventurer.Id;
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
            this.IsAlive = adventurer.IsAlive;
        }

        public int GetDamage() {
            return DamageBase + Weapon + DamagePotion;
        }

        public int GetProtection() {
            return ProtectionBase + ShieldPotion + ArmourHelmet + ArmourBreastplate + ArmourGauntlet + ArmourGreave + ArmourBoots;
        }              

        public int reciveWounds(int woundsReceived) {
            int remainingDammagePoints, adventurerDammage = 0;

            //take dammage to shield potion
            if (ShieldPotion > 0) {
                if (woundsReceived < ShieldPotion) {
                    remainingDammagePoints = 0;

                    //shield potion took all damage points
                    ShieldPotion -= woundsReceived;
                } else {
                    //shield potion took some damage points
                    remainingDammagePoints = woundsReceived - ShieldPotion;
                    ShieldPotion = 0;
                }

                woundsReceived = remainingDammagePoints;
            }

            //take dammage to aura potion
            if (woundsReceived > 0 && AuraPotion > 0) {
                if (woundsReceived < AuraPotion) {
                    remainingDammagePoints = 0;

                    //aura potion took all damage points
                    AuraPotion -= woundsReceived;
                } else {

                    //aura potion took some damage points
                    remainingDammagePoints = woundsReceived - AuraPotion;

                    AuraPotion = 0;
                }

                woundsReceived = remainingDammagePoints;
            }

            //take dammage to Adventurer
            if (woundsReceived > 0) {
                adventurerDammage = woundsReceived;

                int updatedHealth = HealthBase - woundsReceived;

                if (updatedHealth > 0) {
                    HealthBase = updatedHealth;
                } else {
                    HealthBase = 0;
                    IsAlive = false;
                }
            }

            return adventurerDammage;
        }

        //Potion
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

        //  Aura
        //  if health has been lost then use the potion point to heal (up to inital rolled value) and add remaining points as Aura
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

        public SharedAdventurer SharedModelMapper() {
            return new SharedAdventurer() {
                Id = this.Id,
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
                ArmourBoots = this.ArmourBoots,
                IsAlive = this.IsAlive
            };
        }
    }
}