class Tile {
    constructor(index, type, row, column, x, y) {
        this._id = dungeon_crawler.core.generateGuid();
        this._index = index;
        this._type = type;
        this._hidden = true;
        this._row = row;
        this._column = column;
        this._x = x;
        this._y = y;
        this._current = false;
        this._selectable = false;
    }

    getId() {
        return this._id;
    }

    getIndex() {
        return this._index;
    }

    setType(type) {
        return this._type = type;
    }

    getType() {
        return this._type;
    }

    setHidden(value) {
        this._hidden = value;
    }

    getHidden() {
        return this._hidden;
    }

    getRow() {
        return this._row;
    }

    getColumn() {
        return this._column;
    }

    getX() {
        return this._x;
    }

    getY() {
        return this._y;
    }

    getCurrent() {
        return this._current;
    }

    setCurrent(value) {
        this._current = value;
    }

    setSelectable(value) {
         this._selectable = value;
    }

    getSelectable() {
        return this._selectable;
    }
}; 