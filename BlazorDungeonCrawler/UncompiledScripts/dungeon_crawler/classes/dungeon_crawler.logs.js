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
        this._logEntries = [];
    }

    //Log entry
    addEntry(LogEntry) {
        this._logEntries.push(LogEntry);
        this.setLog(LogEntry.getId(), LogEntry.getTile())
    }

    getLogEntryFromId(id) {
        let logEntry;
        for (var i = 0; i < this._logEntries.length; i++) {
            logEntry = this._logEntries[i];

            if (logEntry.getId() == id) {
                return logEntry;
            }
        }
    }

    //log action
    getLogAction() {
        this._logEntries.getLogActions();
    }

    setLog(id, message) {
        $('#log').append(`<div data-identity="${id}" class="entry">${message}</div>`).animate({ scrollTop: $("#log").offset().top }, 2000);
    }

    getLogEntryActionFromId(logEntryId, logEntryActionId) {
        let logEntry;
        for (var i = 0; i < this._logEntries.length; i++) {
            logEntry = this._logEntries[i];

            if (logEntry.getId() == logEntryId) {
                return logEntry.getLogActionFromId(logEntryActionId);
            }
        }
    }
}; 