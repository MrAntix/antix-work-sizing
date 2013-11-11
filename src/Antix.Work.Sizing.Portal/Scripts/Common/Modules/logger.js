define("logger", function (debug) {
    
    return {
        log: function (text, e) {
            if (e) text = text + " e:" + e.toString();

            if (console
                && console.log
                && debug)
                console.log(text);
        }
    };
});
