define("HelpView", function() {
    var logger = require("logger"),
        window = require("window"),
        ui = require("ui"),
        namespace = "showHelp",
        clickEvent = ui.getClickEvent(namespace);

    return function ($el, itemsSelector) {

        var $help = $(itemsSelector),
            $nextButton = $el.find(".next"),
            index = -1;

        var $highlighter = $("<div>")
            .css({
                position: "absolute",
                zIndex:1,
                left: 0,
                top: 0,
                right: 0,
                bottom: 0,
                boxSizing: "border-box",
                border: "solid 10px rgba(0,0,0,.5)"
            })
            .appendTo("body")
            .hide();

        $el
            .hide()
            .css({
                position: "absolute"
            });

        $(".showHelp").on(clickEvent, function () {
            index = -1;
            $el.show();
            view.showNext();
            return false;
        });

        var view = {
            render:
                function() {
                    logger.log("render help");

                },
            $el: $el,
            
            hide: function() {
                $el.hide();
                $highlighter
                    .animate({
                        borderTopWidth: 0,
                        borderLeftWidth: 0,
                        borderRightWidth: 0,
                        borderBottomWidth: 0
                    })
                    .hide();

                $(window).off('.' + namespace);
            },
            
            showNext: function() {

                index++;
                if (index >= $help.length) {
                    view.hide();
                    return;
                } else if (index == $help.length - 1) {
                    $nextButton.text("done");
                } else {
                    $nextButton.text("next");
                }

                $help.hide();
                var $item = $help.eq(index).show();

                if (!$item.data("parent")) {
                    $item.data("parent", $item.parents(".row"));
                    $el.find(".helpContainerInner").append($item);
                }

                view.resize(true);

                $(window).on("resize." + namespace, view.resize);
            },
            resize:function(animate) {
                var $item = $help.eq(index).show();

                var $parent = $item.data("parent"),
                    parentOffset = $parent.offset(),
                    parentHeight = $parent.outerHeight(),
                    windowScrollTop = $(window).scrollTop(),
                    windowHeight = $(window).innerHeight();
                
                var height = $el.outerHeight(),
                    left = parentOffset.left + 15,
                    top = parentOffset.top - height - 5;
                logger.log("top:" + top);
                logger.log("windowScrollTop:" + windowScrollTop);

                $el.removeClass("below");
                if (top < 0) {
                    logger.log("top<0");

                    top = parentOffset.top + $parent.outerHeight() + 5;
                    $el.addClass("below");

                    if (top + height > windowScrollTop + windowHeight) {
                        windowScrollTop = top + height - windowHeight;
                    }
                } else {
                    if (parentOffset.top + parentHeight > windowScrollTop + windowHeight) {
                        windowScrollTop = parentOffset.top + parentHeight - windowHeight;
                    }
                }
                
                if (windowScrollTop > top) {
                    windowScrollTop = top;
                }

                $("html,body").animate({ scrollTop: windowScrollTop+"px" });

                $el
                    .offset({ left: left, top: top })
                    .show();

                var maxHeight = Math.max(windowHeight, $("html,body").height(), top + height);

                var css = {
                    borderTopWidth: parentOffset.top - 3,
                    borderLeftWidth: parentOffset.left - 3,
                    borderRightWidth: $(window).innerWidth() - parentOffset.left - $parent.width() - 3,
                    borderBottomWidth: maxHeight - parentOffset.top - parentHeight - 3
                };

                $highlighter
                    .show()
                    .css({
                        top: 0,
                        height: maxHeight
                    });

                if (animate===true)
                    $highlighter.animate(css);
                else
                    $highlighter.css(css);

            }
        };

        $el.find(".next")
            .on(clickEvent, function () {
                view.showNext();

                this.blur();
                return false;
            });

        $(window).on(clickEvent, view.hide);

        return view;
    };
});
