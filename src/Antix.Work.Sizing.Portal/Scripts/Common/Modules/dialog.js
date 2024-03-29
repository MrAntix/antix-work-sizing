﻿define("dialog", function(parent) {
    var $ = require("$"),
        window = require("window");

    var $cover =
            $("<div/>")
                .addClass("dialogCover")
                .css({ position: "fixed", zIndex: 1000, left: 0, top: 0, right: 0, bottom: 0 })
                .appendTo(parent),
        $container =
            $("<div/>")
                .addClass("dialogContainer")
                .css({ position: "fixed", zIndex: 1001, left: 0, right: 0 })
                .appendTo(parent),
        $dialog =
            $("<div class='dialogContainerInner'/>")
                .appendTo($container);

    return {
        show: function($el) {

            $dialog
                .empty()
                .append($el.show());

            $container.fadeIn();
            $cover.fadeIn();

            if (window.onorientationchange) {
                $(window).on("orientationchange", function() {
                    this.position();
                });
            }

            this.position();
        },
        position: function() {
            var top = ($cover.height() - $container.height()) / 2.75;
            $container.css({ top: top });
        },
        hide: function() {
            $container.hide();
            $cover.hide();
        }
    };
});