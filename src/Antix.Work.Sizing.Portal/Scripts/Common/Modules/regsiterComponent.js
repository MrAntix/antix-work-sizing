define("registerComponent", function() {
    var $ = require("$");

    return function(namespace, name, component) {
        
        $.fn[name] = function (option) {
            
            return this.each(function () {
                
                var $el = $(this),
                    options = $.extend({},
                        component.DEFAULTS, $el.data(),
                        typeof option == 'object' && option);
                
                var path = namespace + '.' + name,
                    data = $el.data(path);

                if (!data) $el.data(path, (data = new component(this, options)));

                if (typeof option == 'string') data[option].call($el);
            });
        };
    };

});

