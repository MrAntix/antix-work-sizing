define('sound', function () {

    var Sound = function (mp3) {

        var audio = new Audio(mp3),
            loops = 1;

        this.init = function () {
            audio.play();
            audio.pause();
            return this;
        };

        this.play = function () {
            audio.play();
            return this;
        };

        this.withLoop = function(count) {
            loops = count;
            return this;
        };

    };
    
    return Sound;
});
