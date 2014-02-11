define("ui", function(document) {
    var touch = "ontouchend" in document;
    return {
        touch: touch,
        touchClick: touch ? "touchstart" : "click",
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