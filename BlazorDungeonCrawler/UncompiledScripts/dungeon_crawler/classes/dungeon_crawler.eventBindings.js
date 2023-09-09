﻿//
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

            this._eventBindings.push(eventBinding);

            this._eventBindingNames.push(name);

            return;
        }

        dungeon_crawler.core.outputError(`Event "${name}" already bound`);
    }

    getElementWithName(name) {
        if (typeof this._eventBindings != 'undefined' && this._eventBindings != null && this._eventBindings.length > 0) {
            let eventBinding
            for (let i = 0; i < this._eventBindings.length; i++) {
                eventBinding = this._eventBindings[i];

                if (eventBinding.getName() == name) {
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
                    eventBinding.setDispacterOn();
                    eventBinding.setBound(true);
                } else {
                    dungeon_crawler.core.outputError(`Event "${eventBinding.getName()}" already bound.`);
                }
            }
        }
    }

    unbindEvents() {
        if (typeof this._eventBindings != 'undefined' && this._eventBindings != null && this._eventBindings.length > 0) {
            let eventBinding
            for (let i = 0; i < this._eventBindings.length; i++) {
                eventBinding = this._eventBindings[i];
                eventBinding.setDispacterOff();
                eventBinding.setBound(false);
            }
        }
    }

    clearBoundEvents() {
        this._eventBindings = [];
        this._eventBindingNames = [];
    }
}