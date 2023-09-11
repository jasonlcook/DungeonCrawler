//  logs *
//  -   log entry
//      -   log action
//      -   log action
//  -   log entry
//      -   log action
//      -   log action
//      -   log action

class Logs {
    constructor() {
        this._logs = [];
    }

    addEntry(LogEntry) {
        this._logs.push(LogEntry);
        this.setLog(LogEntry.getId(), LogEntry.getTile())
    }

    getLogEntryFromId(id) {
        let log;
        for (var i = 0; i < this._logs.length; i++) {
            log = this._logs[i];

            if (log.getId() == id) {
                return log;
            }
        }
    }

    getLogAction() {
        this._logs.getLogActions();
    }

    setLog(id, message) {
        $('#log').append(`<div data-identity="${id}" class="entry">${message}</div>`).animate({ scrollTop: $("#log").offset().top }, 2000);
    }
}; 