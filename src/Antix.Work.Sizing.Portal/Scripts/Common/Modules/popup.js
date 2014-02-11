define("popup", function () {
    var $ = require("$"),
        ui = require("ui"),
        window = require("window"),
        document = require("document"),
        register = require("registerComponent");

    var Popup = function (element, options) {
        var comp = this;
        
        comp.options = options;

        comp.$container = $("<div/>")
            .addClass(ui.css.popup)
            .css({
                position: "absolute"
            });

        comp.$container
            .after(element)
            .append(element);

        if (options.parentElement)
            comp.$container.appendTo(options.parentElement);
    };

    Popup.DEFAULTS = {
        parentElement: null
    };

    Popup.prototype.show = function() {
        var comp = this;
        if (comp.shown) return;

        var $el = this.options.$offsetElement;

        if ($el) {
            var elOffset = $el.offset(),
                elSize = {
                    width: $el.outerWidth(),
                    height: $el.outerHeight()
                },
                popupSize = {
                    width: comp.$container.width(),
                    height: comp.$container.height()
                };

            var position = setPosition.bottom(elOffset, elSize, popupSize)
                    || setPosition.top(elOffset, elSize, popupSize);

            comp.$container
                .css({
                    left: position.left,
                    top: position.top
                });
        }

        comp.$container.show();
        comp.shown = true;
        
        $(document).on(ui.touchClick, function() { comp.hide(); });
    };

    Popup.prototype.hide = function () {
        var comp = this;
        if (!comp.shown) return;

        comp.$container.hide();
        comp.shown = false;

        $(document).off(ui.touchClick, function () { comp.hide(); });
    };

    Popup.prototype.toggle = function() {
        var comp = this;
        if (comp.shown) comp.hide();
        else comp.show();
    };

    var setPosition = {
        top: function(elOffset, elSize, popupSize) {

            return {
                left: elOffset.left + elSize.width - popupSize.width,
                top: elOffset.top - popupSize.height - 2
            };
        },
        bottom: function(elOffset, elSize, popupSize) {
            var reach = elOffset.top + elSize.height + popupSize.height + 2;
            if (reach > $(window).height() + $(window).scrollTop()) return false;

            return {
                left: elOffset.left + elSize.width - popupSize.width,
                top: elOffset.top + elSize.height + 2
            };
        }
    };

    register("antix", "uiPopup", Popup);

    return Popup;
});