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
                $story.attr("readonly", true).off("keyup");
                $action.attr("disabled", true).off("click").removeClass("active");

                view.model.user.isOwner = false;
                if (!view.model.team
                    || !view.model.team.CurrentStoryOwner
                    || view.model.team.CurrentStoryOwner === view.model.user.Name) {

                    $story.attr("readonly", false)
                        .on("keyup", lock);

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
                    $.each(view.model.team.CurrentStoryResultQuantities, function() {
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
                            .text("Stop")
                            .on("click", function() {
                                $el.trigger("close-voting");
                            });
                    } else {
                        $storyPointsAction
                            .text("Start")
                            .on("click", function() {
                                $el.trigger("open-voting");
                            });
                    }
                }
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
                        .on("change", function () {
                            pointsValue =
                                $userPoints.filter(":checked").val();

                            $el.trigger("user-points", pointsValue);
                        })
                        .on("click", function() {
                            var $this = $(this);
                            if ($this.val() == pointsValue) {
                                pointsValue = 0;
                                $el.trigger("user-points", pointsValue);
                            }
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
            };

        return view;
    };
});