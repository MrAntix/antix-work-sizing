define("UserNameView", function () {
    var logger = require("logger"),
        dialog = require("dialog");

    return function ($el) {

        var $userNameInput = $el.find("[name='UserName']"),
            $ok = $el.find("button"),
            ok = function() {
                var name = $userNameInput.val();

                if (name) {
                    $ok.trigger("ok", $userNameInput.val());

                    dialog.hide();
                }
            };

        $ok.on("click", ok);

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
