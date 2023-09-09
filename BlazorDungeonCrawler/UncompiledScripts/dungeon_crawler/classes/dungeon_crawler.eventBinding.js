//
class EventBinding {
    constructor() {
        this._dispacter = null;
        this._type = '';
        this._handler = null;
        this._bound = false;
        this._name = '';
    }

    setDispacterOn() {
        this._dispacter.on(this._type, this._handler);
    }

    setDispacterOff() {
        this._dispacter.off(this._type, this._handler);
    }

    setBound(value) {
        this._bound = value;
    }

    getName() {
        return this._name;
    }


    mapListener(dispacter, type, handler, name) {
        //Dispacter
        if (typeof dispacter == 'undefined' || dispacter == null) {
            dungeon_crawler.core.outputError('Unexpected error experienced whilst setting a dispacter for the event binding');
            return false;
        }
        this._dispacter = dispacter;

        //Type
        if (typeof type == 'undefined' || type == null) {
            dungeon_crawler.core.outputError('Unexpected error experienced whilst setting a type for the event binding');
        }
        this._type = type;

        //Handler
        if (typeof handler == 'undefined' || handler == null) {
            dungeon_crawler.core.outputError('Unexpected error experienced whilst setting a handler for the event binding');
        }
        this._handler = handler;

        if (typeof name !== 'undefined' && name !== null) {
            this._name = name;
        }
    }
}