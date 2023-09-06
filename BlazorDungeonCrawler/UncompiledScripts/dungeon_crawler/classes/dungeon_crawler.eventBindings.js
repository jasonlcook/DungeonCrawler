//
class EventBindings {
    constructor() {
        this._eventBindings = [];
        this._eventBindingNames = [];
    }

    isEmpty() {
        if (this._eventBindings.length == 0) {
            return true;
        }

        return false;
    }

    getElement(index) {
        return this._eventBindings[index];
    }

    getLength() {
        return this._eventBindings.length;
    }

    addEventBinding(dispacter, type, handler, name) {
        if (!this._eventBindingNames.includes(name)) {
            let eventBinding = new EventBinding();
            eventBinding.mapListener(dispacter, type, handler, name);

            dungeon_crawler.core.outputToConsole(`Registered event "${eventBinding.Name}"`);

            this._eventBindings.push(eventBinding);

            this._eventBindingNames.push(name);
        } else {
            dungeon_crawler.core.outputToConsole(`Event "${name}" already bound`);
        }
    }

    getElementWithName(name) {
        if (typeof this._eventBindings != 'undefined' && this._eventBindings != null && this._eventBindings.length > 0) {
            let eventBinding
            for (let i = 0; i < this._eventBindings.length; i++) {
                eventBinding = this._eventBindings[i];

                if (eventBinding.Name == name) {
                    return eventBinding;
                }
            }
        }
    }

    bindEvents() {
        if (typeof this._eventBindings != 'undefined' && this._eventBindings != null && this._eventBindings.length > 0) {
            let eventBinding
            for (let i = 0; i < this._eventBindings.length; i++) {
                eventBinding = this._eventBindings[i];

                if (!eventBinding.Bound) {
                    eventBinding.Dispacter.on(eventBinding.Type, eventBinding.Handler);
                    eventBinding.Bound = true;

                    dungeon_crawler.core.outputToConsole(`Event bound: "${eventBinding.Name}"`);
                } else {
                    dungeon_crawler.core.outputToConsole(`Event "${eventBinding.Name}" already bound.`);
                }
            }
        }
    }

    unbindEvents() {
        if (typeof this._eventBindings != 'undefined' && this._eventBindings != null && this._eventBindings.length > 0) {
            let eventBinding
            for (let i = 0; i < this._eventBindings.length; i++) {
                eventBinding = this._eventBindings[i];
                eventBinding.Dispacter.off(eventBinding.Type, eventBinding.Handler);
                eventBinding.Bound = false;

                dungeon_crawler.core.outputToConsole(`Event unbound: "${eventBinding.Name}"`);
            }
        }
    }

    clearBoundEvents() {
        this._eventBindings = [];
        this._eventBindingNames = [];
    }
}