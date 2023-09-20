﻿var dungeon_crawler = dungeon_crawler || {};

dungeon_crawler.core = {
    createEnum(values) {
        const enumObject = {};

        for (const val of values) {
            enumObject[val] = val;
        }

        return Object.freeze(enumObject);
    },

    generateGuid() {
        return URL.createObjectURL(new Blob()).substr(-36);
    },

    outputToConsole(message) {
        console.log(message);
    },

    outputError(message) {
        let title = 'Error';
        dungeon_crawler.core.outputToConsole(`${title}: ${message}`);
    },
}