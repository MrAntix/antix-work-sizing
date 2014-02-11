$.cookie.json = true;

;
(function ($) {
    defineInstance("$", $);
    defineInstance("window", window);
    defineInstance("document", document);
    defineInstance("cookie", $.cookie);
    defineInstance("connection", $.connection);

    require("ui", document);
    require("logger", false);
    require("dialog", document.body, {});

    var app = require("Size")();

    app.init(window.location);

})(jQuery);