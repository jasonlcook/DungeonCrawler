//
class EventBinding {
    constructor() {
        this.Dispacter = null;
        this.Type = '';
        this.Handler = null;
        this.Bound = false;
        this.Name = '';
    }

    mapListener(dispacter, type, handler, name) {
        //Dispacter
        if (typeof dispacter == 'undefined' || dispacter == null) {
            dungeon_crawler.core.outputError('Unexpected error experienced whilst setting a dispacter for the event binding');
            return false;
        }
        this.Dispacter = dispacter;

        //Type
        if (typeof type == 'undefined' || type == null) {
            dungeon_crawler.core.outputError('Unexpected error experienced whilst setting a type for the event binding');
        }
        this.Type = type;

        //Handler
        if (typeof handler == 'undefined' || handler == null) {
            dungeon_crawler.core.outputError('Unexpected error experienced whilst setting a handler for the event binding');
        }
        this.Handler = handler;

        if (typeof name !== 'undefined' && name !== null) {
            this.Name = name;
        }
    }
}