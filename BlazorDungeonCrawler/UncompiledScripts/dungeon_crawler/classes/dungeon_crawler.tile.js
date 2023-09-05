class Tile {
    constructor(index, type, x, y) {
        this.Id = dungeon_crawler.core.generateGuid();
        this.Index = index;
        this.Type = type;
        this.Hidden = true;
        this.X = x;
        this.Y = y;
    }
}; 