define("select", function () {
    var $ = require("$"), 
        register = require("registerComponent"),
        ui = require("ui"),
        Popup = require("popup");

    var Select = function(element, options) {
        var comp = this;

        comp.values = {};

        comp.$element = $(element);
        comp.$list = $("<ol/>").addClass(ui.css.selectList);
        comp.$input = $("<span/>").addClass(ui.css.input);
        comp.$button = $("<span>&#9660;</span>").addClass(ui.css.inputButton);

        comp.$element
            .after(comp.$input.append(comp.$button))
            .hide();

        if (!options.$offsetElement) {
            options.$offsetElement = comp.$input;
        }
        if (options.$button) {
            comp.$input.prepend(options.$button);
            comp.buttonClass = options.$button.attr("class") || "";
        }

        Popup.call(comp, comp.$list, options);

        comp.$container
            .addClass(ui.css.select)
            .hide();
        
        comp.$element
            .find("option")
            .each(function() {
                var $option = $(this);
                comp.add($option.val(), $option.text(), $option.attr("class"));
            });

        comp.$button.on(ui.touchClick, function () {
            comp.toggle();
            return false;
        });

        comp.showButton();
    };

    Select.prototype = Object.create(Popup.prototype);
    Select.prototype.constructor = Select;

    Select.prototype.add = function(value, text, cssClass) {
        var comp = this;

        var $item = $("<li/>")
            .addClass(ui.css.selectItem)
            .addClass(cssClass)
            .text(text)
            .on(ui.touchClick, function() {
                comp.select(value);
                comp.hide();

                return false;
            });
        
        comp.values[value] = $item;

        comp.$list.append($item);
    };

    Select.prototype.select = function(value) {
        var comp = this,
            selected = comp.values[value],
            text = selected.text();
        
        comp.$element.val(value);
        comp.$element.trigger("select", [value, text]);
        if (comp.options.$button) {
            comp.options.$button
                .text(text)
                .attr("class", comp.buttonClass)
                .addClass(selected.attr("class"));
        }
        
        return comp;
    };

    Select.prototype.showButton = function () {
        var comp = this;

        comp.$input.css({ paddingRight: "1.5em" });
        comp.$button.show();

        return comp;
    };

    Select.prototype.hideButton = function () {
        var comp = this;

        comp.$input.css({ paddingRight: "0" });
        comp.$button.hide();

        return comp;
    };

    Select.DEFAULTS = {
    };

    register("antix", "uiSelect", Select);

    return Select;
});