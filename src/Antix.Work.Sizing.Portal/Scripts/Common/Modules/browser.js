define("browser", function(document) {
    var touch = "ontouchend" in document;
    return {
        touch: touch,
        touchClick: touch ? "touchstart" : "click"
    };
});
