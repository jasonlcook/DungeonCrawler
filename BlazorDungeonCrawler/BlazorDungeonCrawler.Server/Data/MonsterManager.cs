using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Data {
    public class MonsterManager {

        private List<MonsterType> monsterTypes = new List<MonsterType>();

        public MonsterManager() {
            monsterTypes.Add(new MonsterType {
                Id = 1,
                Name = "Giant Leech",
                LevelStart = 1,
                LevelEnd = 1,
                HealthDiceCount = 1,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.dandwiki.com/wiki/Giant_Leech_(5e_Creature)"
            });

            monsterTypes.Add(new MonsterType {
                Id = 2,
                Name = "Kobold",
                LevelStart = 1,
                LevelEnd = 2,
                HealthDiceCount = 1,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Kobold"
            });

            monsterTypes.Add(new MonsterType {
                Id = 3,
                Name = "Skeleton",
                LevelStart = 2,
                LevelEnd = 6,
                HealthDiceCount = 2,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Skeleton"
            });

            monsterTypes.Add(new MonsterType {
                Id = 4,
                Name = "Zombie",
                LevelStart = 2,
                LevelEnd = 6,
                HealthDiceCount = 2,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Zombie"
            });

            monsterTypes.Add(new MonsterType {
                Id = 5,
                Name = "Kuo-Toa",
                LevelStart = 3,
                LevelEnd = 3,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Zombie"
            });

            monsterTypes.Add(new MonsterType {
                Id = 6,
                Name = "Flind",
                LevelStart = 3,
                LevelEnd = 6,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Flind"
            });

            monsterTypes.Add(new MonsterType {
                Id = 7,
                Name = "Giant Spider",
                LevelStart = 4,
                LevelEnd = 5,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Giant-Spider"
            });

            monsterTypes.Add(new MonsterType {
                Id = 8,
                Name = "Kenku",
                LevelStart = 6,
                LevelEnd = 6,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.dandwiki.com/wiki/Kenku_Blightwing_(5e_Creature)"
            });

            monsterTypes.Add(new MonsterType {
                Id = 9,
                Name = "Drow Elf",
                LevelStart = 7,
                LevelEnd = 9,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.dandwiki.com/wiki/Drow_Elf,_Variant_(3.5e_Race)"
            });

            monsterTypes.Add(new MonsterType {
                Id = 10,
                Name = "Skeletal Lord",
                LevelStart = 7,
                LevelEnd = 9,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.gmbinder.com/share/-LoH9Hgmov-rrNvVIKhh"
            });

            monsterTypes.Add(new MonsterType {
                Id = 11,
                Name = "Skeletal Lord",
                LevelStart = 8,
                LevelEnd = 9,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.gmbinder.com/share/-LoH9Hgmov-rrNvVIKhh"
            });

            monsterTypes.Add(new MonsterType {
                Id = 12,
                Name = "Drider",
                LevelStart = 8,
                LevelEnd = 9,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Drider"
            });

            monsterTypes.Add(new MonsterType {
                Id = 13,
                Name = "Hell Hound",
                LevelStart = 8,
                LevelEnd = 9,
                HealthDiceCount = 3,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Hell-Hound"
            });

            monsterTypes.Add(new MonsterType {
                Id = 14,
                Name = "Displacer Beast",
                LevelStart = 9,
                LevelEnd = 9,
                HealthDiceCount = 4,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Displacer-Beast"
            });

            monsterTypes.Add(new MonsterType {
                Id = 15,
                Name = "Rust Monster",
                LevelStart = 9,
                LevelEnd = 9,
                HealthDiceCount = 4,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Rust-Monster"
            });

            monsterTypes.Add(new MonsterType {
                Id = 16,
                Name = "Mantis Warrior",
                LevelStart = 10,
                LevelEnd = 10,
                HealthDiceCount = 4,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.dandwiki.com/wiki/Mantis_Warrior_(5e_Creature)"
            });

            monsterTypes.Add(new MonsterType {
                Id = 17,
                Name = "Mind Flayer",
                LevelStart = 10,
                LevelEnd = 10,
                HealthDiceCount = 5,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=mind-flayer"
            });

            monsterTypes.Add(new MonsterType {
                Id = 18,
                Name = "Xorn",
                LevelStart = 10,
                LevelEnd = 10,
                HealthDiceCount = 5,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Xorn"
            });

            monsterTypes.Add(new MonsterType {
                Id = 19,
                Name = "Stone Golem",
                LevelStart = 10,
                LevelEnd = 10,
                HealthDiceCount = 6,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=stone-golem"
            });

            monsterTypes.Add(new MonsterType {
                Id = 20,
                Name = "Beholder",
                LevelStart = 10,
                LevelEnd = 10,
                HealthDiceCount = 7,
                DamageDiceCount = 1,
                ProtectionDiceCount = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Beholder"
            });
        }

        public List<MonsterType> GetMonsters(int depth) {
            return (from x in monsterTypes
                    where x.LevelStart >= depth 
                    where x.LevelEnd <= depth
                    select x).ToList();
        }
    }
}
