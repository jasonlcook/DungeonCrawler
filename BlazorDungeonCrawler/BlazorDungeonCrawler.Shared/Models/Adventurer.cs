using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Adventurer {

        [Key]
        public Guid Id { get; set; }


        public bool IsAlive { get; set; }


        public int ExperienceLevel { get; set; }
        public int Experience { get; set; }
        public int NextExperienceLevelCost { get; set; }


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


        public int ArmourHelmet { get; set; }
        public int ArmourBreastplate { get; set; }
        public int ArmourGauntlet { get; set; }
        public int ArmourGreave { get; set; }
        public int ArmourBoots { get; set; }


        public int Weapon { get; set; }


        public Adventurer() {
            Id = Guid.Empty;
        }
    }
}
