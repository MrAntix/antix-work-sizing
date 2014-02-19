define("effects", function() {
    var $ = require("$"),
        window = require("window"),
        register = require("registerComponent");

    var Flicker = function (el, options) {
        
        var $el = $(el),
            intervals = [100, 50, 200, 100, 50, 300, 100, 100, 10, 50, 10];
        
        
        var on = function() {
            $el.toggleClass(options.className);
            
            var interval = intervals.pop();
            if (interval) window.setTimeout(on, interval);
            else $el.data("antix.flickerEffect", null);
        };

        $el.removeClass(options.className);
        on();
    };

    register("antix", "flickerEffect", Flicker);
});