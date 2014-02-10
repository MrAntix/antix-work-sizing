$.cookie.json = true;

;
(function ($) {
    defineInstance("$", $);
    defineInstance("window", window);
    defineInstance("cookie", $.cookie);
    defineInstance("connection", $.connection);

    require("browser", document);
    require("logger", false);
    require("dialog", document.body, {});

    var app = require("Size")();

    app.init(window.location);

})(jQuery);