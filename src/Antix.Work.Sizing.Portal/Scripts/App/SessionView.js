define("SessionView", function() {
    var logger = require("logger");

    return function($el) {
        var $users = $el.find(".users .inputs"),
            $user = $el.find(".user .inputs"),
            $userPoints = $el.find(".user [name='points']"),
            $userPointsContainer = $el.find(".user .points"),
            $action = $el.find(".story .title .action"),
            $story = $el.find(".story [name='title']"),
            $storyPoints = $el.find(".story .points .input"),
            $storyPointsResult = $el.find(".story .points .results"),
            $storyPointsAction = $el.find(".story .points .action"),
            $demoButton = $el.find(".showDemo");

        var view = {
            model: null,
            render:
                function() {
                    logger.log("render: " + JSON.stringify(view.model));

                    location.hash = view.model.team.Id;
                    $(".story .share .input")
                        .empty()
                        .append($("<a/>")
                            .attr({ href: window.location })
                            .text(window.location)
                            .addClass("share")
                            .on("click", function() { return false; }));

                    renderStoryTitle();
                    renderStoryPoints();
                    renderUserPoints();
                    renderUsers();
                    renderTimer();
                    renderDemo();
                },
            $el: $el
        };

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
                $action.attr("disabled", true).off("click").removeClass("active");

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
                            .on("click", function() {
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
                $storyPointsResult.empty();
                if (view.model.team.CurrentStoryResult) {
                    $.each(view.model.team.CurrentStoryResults, function() {
                        $("<span>" + this.Value + "</span>")
                            .addClass("result")
                            .addClass("p" + this.Value)
                            .css({ width: this.Percentage + "%" })
                            .appendTo($storyPointsResult);
                    });
                } else {
                    $("<span>&nbsp</span>")
                        .addClass("result")
                        .appendTo($storyPointsResult);
                }

                $storyPoints.removeClass("withAction");
                $storyPointsAction
                    .hide()
                    .off("click");
                if (view.model.team.CurrentStory
                    && view.model.user.Name === view.model.team.CurrentStoryOwner) {
                    $storyPoints.addClass("withAction");
                    $storyPointsAction.show();

                    if (view.model.team.CurrentStoryResult) {
                        $storyPointsAction
                            .text("Clear")
                            .on("click", function() {
                                $el.trigger("clear-votes");
                            });
                    } else if (view.model.team.CurrentStory.VotingOpen) {
                        $storyPointsAction
                            .text("Close")
                            .on("click", function() {
                                $el.trigger("close-voting");
                            });
                    } else {
                        $storyPointsAction
                            .text("Open")
                            .on("click", function() {
                                $el.trigger("open-voting");
                            });

                        $storyPointsResult.empty();
                        $.each([2, 5, 10, 20], function() {
                            var mins = this,
                                $button = $("<span/>")
                                    .text(mins)
                                    .on("click", function() {
                                        $(this).addClass("active");
                                        $el.trigger("open-voting", mins);
                                    });

                            $storyPointsResult
                                .append($("<span class='time'/>").append($button));
                        });
                    }
                }
            },
            renderTimer = function() {
                var schedule = view.model.team.CurrentStory.VoteSchedule;
                if (!schedule
                    || !schedule.Seconds) return;

                var color = schedule.Seconds > 50 ? 255 : schedule.Seconds * 5,
                    time = formatSeconds(schedule.Seconds);

                $storyPointsResult.html(
                    $("<span/>")
                        .addClass("timer")
                        .width(schedule.Percent + "%")
                        .text(time)
                        .css({ backgroundColor: "rgb(" + Math.round(255 - color / 5) + "," + color + ",0)" })
                );

            },
            renderUserPoints = function() {
                $userPoints
                    .off("change click")
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
                $users.add($user).empty();
                $.each(view.model.team.Users, function() {
                    var userName = this.Name,
                        $row = $("<li class='input'></li>"),
                        $input = $("<input type='text' name='person.Name' />")
                            .val(userName);

                    if (userName === view.model.user.Name) {
                        $input
                            .on("click", function() { return false; })
                            .on("change", function() {
                                $el.trigger("user-change-name", $input.val());
                            });
                    } else {
                        $input.attr("readonly", true);
                        $row.addClass("readonly");
                    }

                    if (userName === view.model.user.Name
                        || view.model.user.Name === view.model.team.CurrentStoryOwner) {
                        $row
                            .on("click", function() {
                                $el.trigger("user-change-observer", [userName, !$row.hasClass("inactive")]);
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
                    .off("click");

                if (!view.model.team.CurrentStoryOwner
                    || (view.model.team.CurrentStory
                        && view.model.user.Name === view.model.team.CurrentStoryOwner)) {

                    $demoButton
                        .attr("disabled", false)
                        .on("click", function() {
                            $el.trigger("demo-toggle");
                        });
                }
            },
            formatSeconds = function(s) {
                if (s < 60) return s + "s";

                if (s < 110) {
                    var m = Math.floor(s / 60);
                    return "1m " + Math.ceil((s - 60)/10) + "0s";
                }

                s = Math.round(s / 60);
                if (s < 60) return s + "m";

                s = Math.round(s / 24);
                return s + "h";
            };

        return view;
    };
});