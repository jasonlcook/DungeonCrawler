//rename enemies to monster list
dungeon_crawler.core.enemies = {
    //https://stanislavs.tripod.com/games/eob1mons.htm
    list: [
        {
            type: 'Kobold',
            levelStart: 1, levelEnd: 2,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Kobold'
        }, {
            type: 'Giant Leech',
            levelStart: 1, levelEnd: 1,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.dandwiki.com/wiki/Giant_Leech_(5e_Creature)'
        }, {
            type: 'Skeleton',
            levelStart: 2, levelEnd: 6,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Skeleton'
        }, {
            type: 'Zombie',
            levelStart: 2, levelEnd: 6,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Zombie'
        }, {
            type: 'Kuo-Toa',
            levelStart: 3, levelEnd: 3,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Kuo-Toa'
        }, {
            type: 'Flind',
            levelStart: 3, levelEnd: 6,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Flind'
        }, {
            type: 'Giant Spider',
            levelStart: 4, levelEnd: 5,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Giant-Spider'
        }, {
            type: 'Kenku',
            levelStart: 6, levelEnd: 6,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.dandwiki.com/wiki/Kenku_Blightwing_(5e_Creature)'
        }, {
            type: 'Drow Elf',
            levelStart: 7, levelEnd: 9,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.dandwiki.com/wiki/Drow_Elf,_Variant_(3.5e_Race)'
        }, {
            type: 'Skeletal Lord',
            levelStart: 7, levelEnd: 9,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.gmbinder.com/share/-LoH9Hgmov-rrNvVIKhh'
        }, {
            type: 'Drider',
            levelStart: 8, levelEnd: 9,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Drider'
        }, {
            type: 'Hell Hound',
            levelStart: 8, levelEnd: 9,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Hell-Hound'
        }, {
            type: 'Displacer Beast',
            levelStart: 9, levelEnd: 9,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Displacer-Beast'
        }, {
            type: 'Rust Monster',
            levelStart: 9, levelEnd: 9,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Rust-Monster'
        }, {
            type: 'Mantis Warrior',
            levelStart: 10, levelEnd: 11,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.dandwiki.com/wiki/Mantis_Warrior_(5e_Creature)'
        }, {
            type: 'Mind Flayer',
            levelStart: 11, levelEnd: 11,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=mind-flayer'
        }, {
            type: 'Xorn',
            levelStart: 11, levelEnd: 11,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Xorn'
        }, {
            type: 'Stone Golem',
            levelStart: 12, levelEnd: 12,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=stone-golem'
        }, {
            type: 'Beholder',
            levelStart: 12, levelEnd: 12,
            healthDice: 1, damageDice: 1, protectionDice: 1,
            link: 'https://www.aidedd.org/dnd/monstres.php?vo=Beholder'
        }
    ],

    getAvailableEnemies(dungeonLevel) {
        let enemy, enemies = dungeon_crawler.core.enemies.list;
        let availableEnemies = [];

        for (var i = 0; i < enemies.length; i++) {
            enemy = enemies[i];

            if (dungeonLevel >= enemy.levelStart && dungeonLevel <= enemy.levelEnd) {
                availableEnemies.push(enemy);
            }
        }

        return availableEnemies;
    },

    getEnemy(availableEnemies) {
        let enemyIndex = Math.floor(Math.random() * (availableEnemies.length));
        return availableEnemies[enemyIndex];
    }
}; 