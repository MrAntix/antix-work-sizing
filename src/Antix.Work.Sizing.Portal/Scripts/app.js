﻿$.cookie.json = true;

;
(function ($) {

    if (typeof Object.create !== "function")
        window.location = "sorry-browser-not-supported.html";

    defineInstance('$', $);
    defineInstance('window', window);
    defineInstance('document', document);
    defineInstance('cookie', $.cookie);
    defineInstance('connection', $.connection);

    require('logger', true);
    require('dialog', document.body, {});
    require("effects");
    
    var Sound = require('sound'),
        alarm = new Sound('Sounds/alarm.m4a').withLoop(5);

    defineInstance("alarmSound", alarm);

    var app = require('Size')();

    app.init(window.location);

})(jQuery);