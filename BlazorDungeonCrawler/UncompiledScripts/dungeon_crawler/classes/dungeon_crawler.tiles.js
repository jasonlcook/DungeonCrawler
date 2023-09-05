class Tiles {
    constructor() {
        this.tiles = [];
    }

    get length() {
        return this.tiles.length;
    }

    add(tile) {
        this.tiles.push(tile);
    }

    get(index) {
        if (index < this.tiles.length) {
            return this.tiles[index];
        } else {
            console.log(`Index "${index}" not found`);
        }
    }
}; 