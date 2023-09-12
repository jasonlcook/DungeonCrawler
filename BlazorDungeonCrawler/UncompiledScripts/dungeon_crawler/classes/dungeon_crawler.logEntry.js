//  logs
//  -   log entry *
//      -   log action
//      -   log action
//  -   log entry *
//      -   log action
//      -   log action
//      -   log action

class LogEntry {
    constructor(title) {
        this._id = dungeon_crawler.core.generateGuid();
        this._title = title;
        this._logActions = [];
    }

    setTitle(title) {
        this._title = title;
    }

    getTile() {
        return this._title;
    }

    getId() {
        return this._id;
    }

    getLogActions() {
        return this._logActions;
    }

    addLogAction(logAction) {
        if (logAction !== null) {
            this._logActions.push(logAction);
        }
    }

    addLogActions(logActions) {
        if (logActions !== null && logActions.length > 0) {
            this._logActions.concat(logActions);
        }
    }

    getLogActionFromId(logActionId) {
        let logAction;
        for (var i = 0; i < this._logActions.length; i++) {
            logAction = this._logActions[i];

            if (logAction.getId() == logActionId) {
                return logAction;
            }
        }
    }
}; 