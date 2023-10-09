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

        public SharedAdventurer SharedModelMapper() {
            return new SharedAdventurer() {
                Id = this.Id,
                HealthInitial = this.HealthInitial,
                HealthBase = this.HealthBase,
                DamageBase = this.DamageBase,
                ProtectionBase = this.ProtectionBase,
                IsAlive = this.IsAlive
            };
        }

    }
}