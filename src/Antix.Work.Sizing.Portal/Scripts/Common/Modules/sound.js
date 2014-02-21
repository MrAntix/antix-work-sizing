define('sound', function () {

    var Sound = function (mp3) {

        var audio,
            loops = 1;

        try {

            audio = new Audio(mp3);
        } catch (ex) {
            audio = false;
        }

        this.init = function () {
            if (audio) {

                audio.play();
                audio.pause();
            };

            return this;
        };

        this.play = function () {
            if (audio) {
                audio.play();
            }

            return this;
        };

        this.withLoop = function (count) {
            loops = count;
            return this;
        };

    };

    return Sound;
});
