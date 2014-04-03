define("SessionView", function() {
    var logger = require("logger"),
        ui = require("ui"),
        clickEvent = ui.getClickEvent();

    require("select");

    return function($el) {
        var $users = $el.find(".users .inputs"),
            $userPoints = $el.find(".user [name='points']"),
            $userPointsContainer = $el.find(".user .points"),
            $action = $el.find(".story .title .action"),
            $story = $el.find(".story [name='title']"),
            $storyPoints = $el.find(".story .points .input"),
            $storyPointsResults = $el.find(".story .points .results"),
            $storyPointsAction = $el.find(".story .points .action").hide(),
            $storyPointsActionButton = $el.find(".story .points .action button"),
            $storyPointsActionList = $el.find(".story .points .action select"),
            $demoButton = $el.find(".showDemo");

        var view = {
            delay: 0,
            model: null,
            render:
                function() {
                    logger.log("render: " + JSON.stringify(view.model));

                    renderTimer();
                    renderStoryTitle();
                    renderStoryPoints();
                    renderUserPoints();
                    renderUsers();
                    renderDemo();
                },
            $el: $el
        };

        $storyPointsActionList
            .uiSelect({
                parentElement: "body",
                $offsetElement: $storyPointsActionButton.parent(),
                $button: $storyPointsActionButton
            })
            .on("select", function(e, value) {
                view.delay = value;
            });

        var lock = function() {
            $el.trigger("story-lock", { Title: $story.val() });
        },
            release = function() {
                $el.trigger("story-release");
            },
            renderStoryTitle = function() {
                $story.off("keyup")
                    .attr("readonly", true)
                    .parent().addClass("readonly");
                $action.attr("disabled", true).off(clickEvent).removeClass("active");

                view.model.user.isOwner = false;
                if (!view.model.team
                    || !view.model.team.CurrentStoryOwner
                    || view.model.team.CurrentStoryOwner === view.model.user.Name) {

                    $story
                        .on("keyup", lock)
                        .attr("readonly", false)
                        .parent().removeClass("readonly");

                    $action.attr("disabled", false);
                    if (view.model.team.CurrentStoryOwner) {
                        $action.text("")
                            .addClass("active")
                            .on("click", function() {
                                release();
                                this.blur();
                                return false;
                            });
                        view.model.user.isOwner = true;

                    } else {
                        $action.text("")
                            .on(clickEvent, function () {
                                lock();
                                this.blur();
                                return false;
                            });
                    }
                } else {

                    $action
                        .addClass("active")
                        .text(view.model.team.CurrentStoryOwner);
                }

                if (!view.model.user.isOwner
                    && view.model.team.CurrentStory) {
                    $story.val(view.model.team.CurrentStory.Title);
                }
            },
            renderStoryPoints = function() {
                if (view.model.team.CurrentStoryResult) {
                    $storyPointsResults.empty();

                    $.each(view.model.team.CurrentStoryResults, function () {
                        $("<span>" + this.Value + "</span>")
                            .addClass("result")
                            .addClass("p" + this.Value)
                            .css({ width: this.Percentage + "%" })
                            .appendTo($storyPointsResults);
                    });
                }

                $storyPoints.removeClass("withAction");
                $storyPointsAction.hide();
                $storyPointsActionList.data("antix.uiSelect").hideButton();
                $storyPointsActionButton.off(clickEvent);
              
                if (view.model.team.CurrentStory
                    && view.model.user.Name === view.model.team.CurrentStoryOwner) {
                    $storyPoints.addClass("withAction");
                    $storyPointsAction.show();

                    if (view.model.team.CurrentStoryResult) {
                        $storyPointsActionButton
                            .text("Clear")
                            .on(clickEvent, function () {
                                $el.trigger("clear-votes");
                            });
                    } else if (view.model.team.CurrentStory.VotingOpen) {
                        $storyPointsActionButton
                            .text("Close")
                            .on(clickEvent, function () {
                                $el.trigger("close-voting");
                            });
                    } else {
                        if (view.model.team.CurrentStory.VoteSchedule) {

                            $storyPointsActionButton
                                .text("Open")
                                .attr("class", "")
                                .on(clickEvent, function() {
                                    $el.trigger("open-voting");
                                });
                        } else {

                            $storyPointsActionList
                                .data("antix.uiSelect")
                                .select(view.delay)
                                .showButton();
                            
                            $storyPointsActionButton
                                .on(clickEvent, function () {
                                    $el.trigger("open-voting", view.delay);
                                });
                        }
                    }
                }
            },
            renderTimer = function() {

                var schedule = view.model.team.CurrentStory.VoteSchedule;
                if (!schedule
                    || !schedule.Seconds) {

                    view.scheduleSeconds = null;
                    clearTimer();
                    $storyPointsResults.empty();

                    return;
                }

                if (view.scheduleSeconds &&
                    schedule.Seconds > view.scheduleSeconds) {

                    return;
                }

                clearTimer();
                view.scheduleSeconds = schedule.Seconds;

                var percent = schedule.Percent,
                    percentDrop = percent / view.scheduleSeconds;

                var show = function() {

                    var color = view.scheduleSeconds > 50 ? 200 : view.scheduleSeconds * 4,
                        time = formatSeconds(view.scheduleSeconds);

                    $storyPointsResults
                        .html(
                            $("<span/>")
                                .addClass("timer")
                                .width(percent + "%")
                                .css({ backgroundColor: "rgb(" + Math.round(200 - color / 5) + "," + color + ",0)" })
                                .append($("<span/>")
                                    .addClass("timerText icon-timer")
                                    .text(time)
                                ));

                    view.scheduleSeconds -= 1;
                    percent -= percentDrop;
                    if (view.scheduleSeconds > 0) view.renderTimerId = window.setTimeout(show, 1000);
                };

                show();

            },
            clearTimer = function() {
                if (view.renderTimerId) {
                    window.clearTimeout(view.renderTimerId);
                    view.renderTimerId = null;
                }
            },
            renderUserPoints = function() {
                $userPoints
                    .off("change " + clickEvent)
                    .attr({ disabled: true });
                $userPointsContainer
                    .find("label")
                    .removeClass("active");

                var pointsValue = false, show = false;
                if (view.model.team.CurrentStory
                    && !view.model.user.IsObserver) {

                    $.each(view.model.team.CurrentStory.Points, function() {
                        if (this.Name === view.model.user.Name) {
                            pointsValue = this.Value;
                            var id = $userPoints
                                .filter("#Points" + this.Value)
                                .attr({ checked: true })
                                .attr("id");
                            $userPointsContainer
                                .find("label[for='" + id + "']")
                                .addClass("active");
                        }
                    });

                    $userPoints
                        .attr({ disabled: false })
                        .on("change", function() {
                            pointsValue =
                                $userPoints.filter(":checked").val();

                            $el.trigger("user-points", pointsValue);
                        });

                    show = view.model.team.CurrentStory.VotingOpen;
                }
                if (!pointsValue) $userPoints.attr({ checked: false });
                if ($userPointsContainer.is(":visible")) {
                    if (!show) $userPointsContainer.slideUp();
                } else {
                    if (show) $userPointsContainer.slideDown();
                }
            },
            renderUsers = function() {
                $users.empty();
                $.each(view.model.team.Users, function() {
                    var userName = this.Name,
                        $row = $("<li class='input withIcon'><span class='activate icon icon-user'/></li>"),
                        $input = $("<input type='text' name='person.Name'/>")
                            .val(userName);

                    if (userName === view.model.user.Name) {
                        $input
                            .on(clickEvent, function () { return false; })
                            .on("change", function() {
                                $el.trigger("user-change-name", $input.val());
                            });
                    } else {
                        $input.attr("readonly", true);
                        $row.addClass("readonly");
                    }

                    if (userName === view.model.user.Name
                        || view.model.user.Name === view.model.team.CurrentStoryOwner) {
                        $row.find(".activate")
                            .on(clickEvent, function () {
                                $el.trigger("user-change-observer", [userName, !$row.hasClass("inactive")]);
                                return false;
                            });
                    }

                    $users.append($row.append($input));

                    $row.toggleClass("inactive", this.IsObserver);
                    if (!this.IsObserver) {
                        $row.toggleClass("voted", this.hasVoted);

                        if (this.hasVoted)
                            $row.addClass("p" + this.vote);
                    }
                });
            },
            renderDemo = function() {

                $demoButton
                    .attr("disabled", true)
                    .off(clickEvent);

                if (!view.model.team.CurrentStoryOwner
                    || (view.model.team.CurrentStory
                        && view.model.user.Name === view.model.team.CurrentStoryOwner)) {

                    $demoButton
                        .attr("disabled", false)
                        .on(clickEvent, function () {
                            $el.trigger("demo-toggle");
                        });
                }
            },
            formatSeconds = function(s) {
                var mLeft = Math.floor(s / 60),
                    sLeft = Math.ceil(s % 60);
                
                return (mLeft > 0 ? mLeft + "m " : "")
                    + sLeft + "s";
            };

        return view;
    };
});