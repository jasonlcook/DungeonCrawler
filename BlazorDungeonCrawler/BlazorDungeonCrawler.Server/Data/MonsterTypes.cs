namespace BlazorDungeonCrawler.Server.Data {
    public class MonsterTypes {
        private readonly List<MonsterType> _monsterTypes;

        public MonsterTypes() {
            _monsterTypes = new List<MonsterType>() {
                    new MonsterType() {
                        Name = "Giant Leech",
                        FloorStart = 1,
                        FloorEnd = 1,
                        MaxPackNumber = 1,
                        HealthDiceCount = 1,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.dandwiki.com/wiki/Giant_Leech_(5e_Creature)"
                    },
                    new MonsterType() {
                        Name = "Kobold",
                        FloorStart = 1,
                        FloorEnd = 2,
                        MaxPackNumber = 5,
                        HealthDiceCount = 1,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Kobold"
                    },
                    new MonsterType() {
                        Name = "Skeleton",
                        FloorStart = 2,
                        FloorEnd = 6,
                        MaxPackNumber = 2,
                        HealthDiceCount = 2,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Skeleton"
                    },
                    new MonsterType() {
                        Name = "Zombie",
                        FloorStart = 2,
                        FloorEnd = 6,
                        MaxPackNumber = 2,
                        HealthDiceCount = 2,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Zombie"
                    },
                    new MonsterType() {
                        Name = "Kuo-Toa",
                        FloorStart = 3,
                        FloorEnd = 3,
                        MaxPackNumber = 3,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Zombie"
                    },
                    new MonsterType() {
                        Name = "Flind",
                        FloorStart = 3,
                        FloorEnd = 6,
                        MaxPackNumber = 1,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Flind"
                    },
                    new MonsterType() {
                        Name = "Giant Spider",
                        FloorStart = 4,
                        FloorEnd = 5,
                        MaxPackNumber = 1,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Giant-Spider"
                    },
                    new MonsterType() {
                        Name = "Kenku",
                        FloorStart = 6,
                        FloorEnd = 6,
                        MaxPackNumber = 3,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.dandwiki.com/wiki/Kenku_Blightwing_(5e_Creature)"
                    },
                    new MonsterType() {
                        Name = "Drow Elf",
                        FloorStart = 7,
                        FloorEnd = 9,
                        MaxPackNumber = 3,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.dandwiki.com/wiki/Drow_Elf,_Variant_(3.5e_Race)"
                    },
                    new MonsterType() {
                        Name = "Skeletal Lord",
                        FloorStart = 7,
                        FloorEnd = 9,
                        MaxPackNumber = 1,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.gmbinder.com/share/-LoH9Hgmov-rrNvVIKhh"
                    },
                    new MonsterType() {
                        Name = "Drider",
                        FloorStart = 8,
                        FloorEnd = 9,
                        MaxPackNumber = 3,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Drider"
                    },
                    new MonsterType() {
                        Name = "Hell Hound",
                        FloorStart = 8,
                        FloorEnd = 9,
                        MaxPackNumber = 5,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Hell-Hound"
                    },
                    new MonsterType() {
                        Name = "Displacer Beast",
                        FloorStart = 9,
                        FloorEnd = 9,
                        MaxPackNumber = 1,
                        HealthDiceCount = 4,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Displacer-Beast"
                    },
                    new MonsterType() {
                        Name = "Rust Monster",
                        FloorStart = 9,
                        FloorEnd = 9,
                        MaxPackNumber = 1,
                        HealthDiceCount = 4,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Rust-Monster"
                    },
                    new MonsterType() {
                        Name = "Mantis Warrior",
                        FloorStart = 10,
                        FloorEnd = 10,
                        MaxPackNumber = 3,
                        HealthDiceCount = 4,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.dandwiki.com/wiki/Mantis_Warrior_(5e_Creature)"
                    },
                    new MonsterType() {
                        Name = "Mind Flayer",
                        FloorStart = 10,
                        FloorEnd = 10,
                        MaxPackNumber = 1,
                        HealthDiceCount = 5,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=mind-flayer"
                    },
                    new MonsterType() {
                        Name = "Xorn",
                        FloorStart = 10,
                        FloorEnd = 10,
                        HealthDiceCount = 5,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Xorn"
                    },
                    new MonsterType() {
                        Name = "Stone Golem",
                        FloorStart = 10,
                        FloorEnd = 10,
                        MaxPackNumber = 1,
                        HealthDiceCount = 6,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=stone-golem"
                    },
                    new MonsterType() {
                        Name = "Beholder",
                        FloorStart = 999,
                        FloorEnd = 999,
                        MaxPackNumber = 1,
                        HealthDiceCount = 7,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Beholder"
                    }
                };
        }

        public List<MonsterType> GetMonstersAtDepth(int depth) {
            return _monsterTypes.Where(mt => mt.FloorStart <= depth).Where(mt => mt.FloorEnd >= depth).ToList();
        }
    }
}
