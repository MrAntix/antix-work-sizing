define("ui", function () {
    var document = require("document"),
        pointer = "onpointerdown" in document;
    
    return {
        pointer: pointer,
        getEvent: function (eventNames, namespace) {
            if (namespace) namespace = '.' + namespace;

            return $.grep(eventNames, function (eventName) {
                return eventName + namespace;
            }).join(" ");
        },
        getClickEvent: function (namespace) {
            return this.getEvent(
                pointer ? ["pointerdown"] : ["click"],
                namespace);
        },
        css: {
            input: "ui-input",
            inputButton: "ui-input-button",
            popup: "ui-popup",
            select: "ui-select",
            selectList: "ui-select-list",
            selectItem: "ui-select-item",
        }
    };
});