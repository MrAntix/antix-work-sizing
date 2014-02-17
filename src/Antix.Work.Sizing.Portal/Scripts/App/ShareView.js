define("ShareView", function () {
    var window = require("window"),
        document = require("document"),
        logger = require("logger"),
        dialog = require("dialog"),
        ui = require("ui"),
        qrCode = require("qrCode");

    return function ($el) {

        var $locationEl = $el.find(".location"),
            $showQRCodeEl = $el.find(".show-qrCode");

        var view = {
            showQRCodeDialog :function() {

                dialog.show(
                    $("<div class='qrCode'/>")
                        .append(qrCode(window.location.href))
                        
                );

                $(document).on(ui.touchClick, dialog.hide);
            },

            render:
                function(model) {
                    logger.log("render share view");

                    window.location.hash = model.hash;
                    $locationEl
                        .attr({ href: window.location.href })
                        .text(window.location.href.substr(7))
                        .on(ui.touchClick, function () { return false; });
                    
                    $showQRCodeEl
                        .on(ui.touchClick, function() {
                            view.showQRCodeDialog();
                            return false;
                        });
                        
                },
            $el: $el
        };


        return view;
    };
});
