define("Size", function() {
    var $ = require("$");
    var SessionService = require("SessionService"),
        SessionView = require("SessionView"),
        UserNameView = require("UserNameView"),
        HelpView = require("HelpView"),
        ShareView = require("ShareView");

    return function() {
        var logger = require("logger"),
            connection = require("connection"),
            hub = connection.sizeHub,
            sessionService;

        hub.client.teamUpdate = function(data) {
            logger.log("size.teamUpdate");
            sessionService.teamUpdate(data);
        };

        return {
            init: function(location) {
                logger.log("size.init");

                sessionService = new SessionService(
                    hub,
                    new SessionView($("html")),
                    new UserNameView($("#UserNameView")),
                    new HelpView($("#HelpContainer"), ".help"),
                    new ShareView($(".story .share"))
                );

                var start = function () {
                    connection.hub.start();
                }, restart = function () {
                    logger.log("size.init restarting");
                    window.setTimeout(start, 1000);
                    connection.hub.stop();
                };

                connection.hub.reconnected(sessionService.connect);
                connection.hub.stateChanged(function (change) {
                    switch (change.newState) {
                        case $.connection.connectionState.connecting: //0
                            logger.log("size.stateChanged connecting");

                            break;
                        case $.connection.connectionState.connected: //1
                            logger.log("size.stateChanged connected");
                            sessionService.connect(location.hash);

                            break;
                        case $.connection.connectionState.reconnecting: //2
                            logger.log("size.stateChanged reconnecting");

                            break;
                        case $.connection.connectionState.disconnected: //4
                            logger.log("size.stateChanged disconnected");
                            //restart();

                            break;
                    }
                });
                
                start();

                //$(window).on("beforeunload", function () {
                //    logger.log("unloading: stopping hub");
                //    connection.hub.stop();
                //});
                window.onerror = function (message) {
                    logger.log("size.error: " + message);
                    restart();
                };

                $(".body")
                    .hide()
                    .css({ visibility: "visible" })
                    .fadeIn();
            }
        };
    };
});
