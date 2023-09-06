dungeon_crawler.log = {
    generateStartingAdventurerText(healthValue, strengthValue, armourValue) {
        let healthLevel = 'unknown';
        let strengthLevel = 'unknown';
        let armourLevel = 'unknown';

        switch (healthValue) {
            case 1:
            case 2:
            case 3:
                healthLevel = 'poor';
                break;
            case 4:
            case 5:
                healthLevel = 'good';
                break;
            case 6:
                healthLevel = 'excellent';
                break;
        }

        switch (strengthValue) {
            case 1:
            case 2:
            case 3:
                strengthLevel = 'vulnerable';
                break;
            case 4:
            case 5:
                strengthLevel = 'fine';
                break;
            case 6:
                strengthLevel = 'powerful';
                break;
        }

        switch (armourValue) {
            case 1:
            case 2:
            case 3:
                armourLevel = 'rags';
                break;
            case 4:
            case 5:
                armourLevel = 'clothes';
                break;
            case 6:
                armourLevel = 'armour';
                break;
        }

        return `You have been offerd as a sacrifices to the dungon and are in ${healthLevel} health and feel ${strengthLevel} wearing your ${armourLevel}.`;
    }
}; 