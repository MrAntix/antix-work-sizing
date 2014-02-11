;
(function(undefined) {
    var modules = {},
        instances = {};

    window.define = function (name, module) {
        modules[name] = module;
    };

    window.defineInstance = function (name, instance) {
        instances[name] = instance;
    };

    window.require = function (name) {
        var instance = instances[name];
        if (instance !== undefined) return instance;

        var args = arguments && Array.prototype.slice.call(arguments, 1) || [];

        if (modules[name] === undefined) throw "unknown module name '" + name + "'";

        return (instances[name] = modules[name].apply(undefined, args));
    };
    
})();