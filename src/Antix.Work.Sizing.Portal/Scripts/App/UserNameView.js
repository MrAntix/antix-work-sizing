define("UserNameView", function () {
    var logger = require("logger"),
        dialog = require("dialog"),
        ui = require("ui"),
        clickEvent = ui.getClickEvent();

    return function ($el) {

        var $userNameInput = $el.find("[name='UserName']"),
            $isObserver = $el.find("#IsObserver"),
            $ok = $el.find("button"),
            ok = function() {
                var name = $userNameInput.val();

                if (name) {
                    $ok.trigger("ok", [$userNameInput.val(), $isObserver.is(":checked")]);

                    dialog.hide();
                }
            };

        $ok.on(clickEvent, ok);

        $userNameInput.on("keypress", function(e) {
            if (e.keyCode == 13) ok();
        });

        var view = {
            render:
                function() {
                    logger.log("render user name dialog:");

                    dialog.show($el);

                    $userNameInput.focus();
                },
            $el: $el
        };


        return view;
    };
});
