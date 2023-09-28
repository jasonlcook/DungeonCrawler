namespace BlazorDungeonCrawler.Database.Migrations {
    using BlazorDungeonCrawler.Shared.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DungeonCrawlerContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DungeonCrawlerContext context) {
            if (context.MonsterType.ToList().Count == 0) {
                List<MonsterType> monsterTypes = new List<MonsterType>() {
                    new MonsterType() {
                        Name = "Giant Leech",
                        LevelStart = 1,
                        LevelEnd = 1,
                        HealthDiceCount = 1,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.dandwiki.com/wiki/Giant_Leech_(5e_Creature)"
                    },
                    new MonsterType() {
                        Name = "Kobold",
                        LevelStart = 1,
                        LevelEnd = 2,
                        HealthDiceCount = 1,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Kobold"
                    },
                    new MonsterType() {
                        Name = "Skeleton",
                        LevelStart = 2,
                        LevelEnd = 6,
                        HealthDiceCount = 2,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Skeleton"
                    },
                    new MonsterType() {
                        Name = "Zombie",
                        LevelStart = 2,
                        LevelEnd = 6,
                        HealthDiceCount = 2,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Zombie"
                    },
                    new MonsterType() {
                        Name = "Kuo-Toa",
                        LevelStart = 3,
                        LevelEnd = 3,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Zombie"
                    },
                    new MonsterType() {
                        Name = "Flind",
                        LevelStart = 3,
                        LevelEnd = 6,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Flind"
                    },
                    new MonsterType() {
                        Name = "Giant Spider",
                        LevelStart = 4,
                        LevelEnd = 5,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Giant-Spider"
                    },
                    new MonsterType() {
                        Name = "Kenku",
                        LevelStart = 6,
                        LevelEnd = 6,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.dandwiki.com/wiki/Kenku_Blightwing_(5e_Creature)"
                    },
                    new MonsterType() {
                        Name = "Drow Elf",
                        LevelStart = 7,
                        LevelEnd = 9,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.dandwiki.com/wiki/Drow_Elf,_Variant_(3.5e_Race)"
                    },
                    new MonsterType() {
                        Name = "Skeletal Lord",
                        LevelStart = 7,
                        LevelEnd = 9,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.gmbinder.com/share/-LoH9Hgmov-rrNvVIKhh"
                    },
                    new MonsterType() {
                        Name = "Skeletal Lord",
                        LevelStart = 8,
                        LevelEnd = 9,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.gmbinder.com/share/-LoH9Hgmov-rrNvVIKhh"
                    },
                    new MonsterType() {
                        Name = "Drider",
                        LevelStart = 8,
                        LevelEnd = 9,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Drider"
                    },
                    new MonsterType() {
                        Name = "Hell Hound",
                        LevelStart = 8,
                        LevelEnd = 9,
                        HealthDiceCount = 3,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Hell-Hound"
                    },
                    new MonsterType() {
                        Name = "Displacer Beast",
                        LevelStart = 9,
                        LevelEnd = 9,
                        HealthDiceCount = 4,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Displacer-Beast"
                    },
                    new MonsterType() {
                        Name = "Rust Monster",
                        LevelStart = 9,
                        LevelEnd = 9,
                        HealthDiceCount = 4,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Rust-Monster"
                    },
                    new MonsterType() {
                        Name = "Mantis Warrior",
                        LevelStart = 10,
                        LevelEnd = 10,
                        HealthDiceCount = 4,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.dandwiki.com/wiki/Mantis_Warrior_(5e_Creature)"
                    },
                    new MonsterType() {
                        Name = "Mind Flayer",
                        LevelStart = 10,
                        LevelEnd = 10,
                        HealthDiceCount = 5,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=mind-flayer"
                    },
                    new MonsterType() {
                        Name = "Xorn",
                        LevelStart = 10,
                        LevelEnd = 10,
                        HealthDiceCount = 5,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Xorn"
                    },
                    new MonsterType() {
                        Name = "Stone Golem",
                        LevelStart = 10,
                        LevelEnd = 10,
                        HealthDiceCount = 6,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=stone-golem"
                    },
                    new MonsterType() {
                        Name = "Beholder",
                        LevelStart = 10,
                        LevelEnd = 10,
                        HealthDiceCount = 7,
                        DamageDiceCount = 1,
                        ProtectionDiceCount = 1,
                        Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Beholder"
                    }
                };

                context.MonsterType.AddRange(monsterTypes);
                context.SaveChanges();
            }
        }
    }
}
