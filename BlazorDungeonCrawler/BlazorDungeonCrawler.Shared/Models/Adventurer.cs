using System.ComponentModel.DataAnnotations;

namespace BlazorDungeonCrawler.Shared.Models {
    public class Adventurer {

        [Key]
        public Guid Id { get; set; }


        public int Level { get; set; }
        public int Experience { get; set; }

        //Level      XP
        //1           0
        //2           1
        //3           3
        //4           7
        //5          15
        //6          31
        //7          63
        //8         127
        public int NextLevelCost { get; set; }


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


        public Adventurer() {
            Id = Guid.Empty;
        }
    }
}
