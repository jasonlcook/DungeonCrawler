namespace BlazorDungeonCrawler.Shared.Models {
    public  class Adventurer {        
        public int HealthBase { get; set; } = 0;
        public int HealthInitial { get; set; } = 0;
        public int AuraPotion { get; set; } = 0;
        public int AuraPotionDuration { get; set; } = 0;

        public int DamageBase { get; set; } = 0;
        public int DamagePotion { get; set; } = 0;
        public int DamagePotionDuration { get; set; } = 0;
        
        public int ProtectionBase { get; set; } = 0;
        public int ShieldPotion { get; set; } = 0;
        public int ShieldPotionDuration { get; set; } = 0;
        
        public int Weapon { get; set; } = 0;
        
        public int ArmourHelmet { get; set; } = 0;
        public int ArmourBreastplate { get; set; } = 0;
        public int ArmourGauntlet { get; set; } = 0;
        public int ArmourGreave { get; set; } = 0;
        public int ArmourBoots { get; set; } = 0;
        
        public bool IsAlive { get; set; } = true;
    }
}
