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
            Id = Guid.NewGuid();

            HealthInitial = health;
            HealthBase = health;
            DamageBase = damage;
            ProtectionBase = protection;
            IsAlive = true;
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