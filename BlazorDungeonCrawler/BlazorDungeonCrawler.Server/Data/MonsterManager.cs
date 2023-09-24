using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Data {
    public class MonsterManager {

        private List<Monster> monsters = new List<Monster>();
        public MonsterManager() {
            monsters.Add(new Monster {
                Id = 1,
                Name = "Giant Leech",
                LevelStart = 1,
                LevelEnd = 1,
                DiceHealth = 1,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.dandwiki.com/wiki/Giant_Leech_(5e_Creature)"
            });

            monsters.Add(new Monster {
                Id = 2,
                Name = "Kobold",
                LevelStart = 1,
                LevelEnd = 2,
                DiceHealth = 1,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Kobold"
            });

            monsters.Add(new Monster {
                Id = 3,
                Name = "Skeleton",
                LevelStart = 2,
                LevelEnd = 6,
                DiceHealth = 2,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Skeleton"
            });

            monsters.Add(new Monster {
                Id = 4,
                Name = "Zombie",
                LevelStart = 2,
                LevelEnd = 6,
                DiceHealth = 2,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Zombie"
            });

            monsters.Add(new Monster {
                Id = 5,
                Name = "Kuo-Toa",
                LevelStart = 3,
                LevelEnd = 3,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Zombie"
            });

            monsters.Add(new Monster {
                Id = 6,
                Name = "Flind",
                LevelStart = 3,
                LevelEnd = 6,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Flind"
            });

            monsters.Add(new Monster {
                Id = 7,
                Name = "Giant Spider",
                LevelStart = 4,
                LevelEnd = 5,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Giant-Spider"
            });

            monsters.Add(new Monster {
                Id = 8,
                Name = "Kenku",
                LevelStart = 6,
                LevelEnd = 6,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.dandwiki.com/wiki/Kenku_Blightwing_(5e_Creature)"
            });

            monsters.Add(new Monster {
                Id = 9,
                Name = "Drow Elf",
                LevelStart = 7,
                LevelEnd = 9,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.dandwiki.com/wiki/Drow_Elf,_Variant_(3.5e_Race)"
            });

            monsters.Add(new Monster {
                Id = 10,
                Name = "Skeletal Lord",
                LevelStart = 7,
                LevelEnd = 9,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.gmbinder.com/share/-LoH9Hgmov-rrNvVIKhh"
            });

            monsters.Add(new Monster {
                Id = 11,
                Name = "Skeletal Lord",
                LevelStart = 8,
                LevelEnd = 9,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.gmbinder.com/share/-LoH9Hgmov-rrNvVIKhh"
            });

            monsters.Add(new Monster {
                Id = 12,
                Name = "Drider",
                LevelStart = 8,
                LevelEnd = 9,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Drider"
            });

            monsters.Add(new Monster {
                Id = 13,
                Name = "Hell Hound",
                LevelStart = 8,
                LevelEnd = 9,
                DiceHealth = 3,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Hell-Hound"
            });

            monsters.Add(new Monster {
                Id = 14,
                Name = "Displacer Beast",
                LevelStart = 9,
                LevelEnd = 9,
                DiceHealth = 4,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Displacer-Beast"
            });

            monsters.Add(new Monster {
                Id = 15,
                Name = "Rust Monster",
                LevelStart = 9,
                LevelEnd = 9,
                DiceHealth = 4,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Rust-Monster"
            });

            monsters.Add(new Monster {
                Id = 16,
                Name = "Mantis Warrior",
                LevelStart = 10,
                LevelEnd = 10,
                DiceHealth = 4,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.dandwiki.com/wiki/Mantis_Warrior_(5e_Creature)"
            });

            monsters.Add(new Monster {
                Id = 17,
                Name = "Mind Flayer",
                LevelStart = 10,
                LevelEnd = 10,
                DiceHealth = 5,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=mind-flayer"
            });

            monsters.Add(new Monster {
                Id = 18,
                Name = "Xorn",
                LevelStart = 10,
                LevelEnd = 10,
                DiceHealth = 5,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Xorn"
            });

            monsters.Add(new Monster {
                Id = 19,
                Name = "Stone Golem",
                LevelStart = 10,
                LevelEnd = 10,
                DiceHealth = 6,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=stone-golem"
            });

            monsters.Add(new Monster {
                Id = 20,
                Name = "Beholder",
                LevelStart = 10,
                LevelEnd = 10,
                DiceHealth = 7,
                DiceDamage = 1,
                DiceProtection = 1,
                Documentation = "https://www.aidedd.org/dnd/monstres.php?vo=Beholder"
            });
        }

        public async Task<List<Monster>> GetAllMonsters() {
            await Task.Delay(1);
            return monsters;
        }

        public async Task<Monster> GetMonster(int Id) {
            await Task.Delay(1);

            return (from x in monsters
                    where x.Id == Id
                    select x).FirstOrDefault();
        }

        public async Task<List<Monster>> SearchMonsters(string Name) {
            await Task.Delay(1);
            return (from x in monsters
                    where x.Name.ToLower().Contains(Name.ToLower())
                    select x).ToList();
        }
    }
}
