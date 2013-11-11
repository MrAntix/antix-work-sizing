define("callout", function(target) {
    var $ = require("$"),
        $target = $(target);

    var $container =
            $("<div/>")
                .addClass("calloutContainer")
                .css({ position: "absolute", left: 0, right: 0 })
                .appendTo($target.offsetParent()),
        $callout =
            $("<div class='calloutContainerInner'/>")
                .appendTo($container);

    return {
        show: function($el) {
            $el.show();

            $callout.append($el);
            
            $container.show()
                .css({ top: top });
        },
        hide: function() {
            $container.hide();
        }
    };
});
