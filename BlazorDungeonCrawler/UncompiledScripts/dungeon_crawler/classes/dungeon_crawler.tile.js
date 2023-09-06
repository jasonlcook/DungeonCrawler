class Tile {
    constructor(index, type, row, column, x, y) {
        this.Id = dungeon_crawler.core.generateGuid();
        this.Index = index;
        this.Type = type;
        this.Hidden = true;
        this.Row = row;
        this.Column = column;
        this.X = x;
        this.Y = y;
        this.Current = false;
        this.Selectable = false;
    }
}; 