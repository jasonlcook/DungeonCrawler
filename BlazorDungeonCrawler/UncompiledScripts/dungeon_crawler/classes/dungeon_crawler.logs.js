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
        this.setLog(LogEntry.getId(), LogEntry.getTile(), LogEntry.getLogActions());
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

    setLog(id, message, logActions) {
        let logHtml = `<div data-identity="${id}" class="log-entry">`
        logHtml += `<span class='log-entry-message'>${message}</span>`

        if (logActions != null && logActions.length > 0) {
            logHtml += `<ol class="log-actions" hidden="hidden" style="display: none;">`

            let logAction, logActionIndex, logMessage;
            for (var i = 0; i < logActions.length; i++) {
                logAction = logActions[i];

                logMessage = '';

                logActionIndex = logAction.getIndex();
                if (logActionIndex > 0) {
                    logMessage = `${logAction.getIndex()} - `;
                }

                logMessage += logAction.getMessage();

                logHtml += `<li data-identity="${logAction.getId()}">`;
                logHtml += `<span class="log-action-message">${logMessage }</span>`;


                logHtml += `</li>`;

            }
            logHtml += `</ol>`
        }
        logHtml += `</div>`


        $('#log').append(logHtml).animate({ scrollTop: $("#log").offset().top }, 2000);
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