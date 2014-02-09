define("SessionService", function() {
    var userCookieName = "User";

    return function(hub, view, userNameView, helpView) {
        var logger = require("logger"),
            cookie = require("cookie");

        var service = {
            saveUser: function (user) {
                if (user !== undefined) service.user = user;
                
                cookie(userCookieName, service.user, { expires: 7 });
            },
            connect: function (teamId) {
                logger.log("SessionService.connect");

                if (!service.user) {

                    userNameView
                        .$el.on("ok", function (e, name, isObserver) {
                            
                            service.saveUser({
                                Name: name,
                                IsObserver: isObserver
                            });

                            service.connect(teamId);
                        });
                    
                    userNameView.render();

                    return;
                }

                if (teamId !== undefined) {
                    service.user.TeamId = teamId ? teamId.substr(1) : null;
                    logger.log("SessionService.connect teamId set " + service.user.TeamId);
                }

                hub.server.connect(service.user)
                    .done(function(session) {
                        service.saveUser(session.User);
                        service.teamUpdate(session.Team);
                    })
                    .fail(service.error);
            },
            teamUpdate: function(team) {
                logger.log("SessionService.handleResponse");

                if ($.grep(team.Users,
                    function(user) {
                        return user.Name == service.user.Name;
                }).length === 0) {
                    service.connect();
                } else {
                    
                    view.model = {
                        user: service.user,
                        team: team
                    };

                    $.each(team.Users, function() {
                        var user = this;

                        user.hasVoted = false;
                        if (team.CurrentStory != null) {
                            var userPoints =
                                $.grep(team.CurrentStory.Points,
                                    function(p) { return p.Name === user.Name; });
                            if (userPoints.length > 0) {
                                if (team.CurrentStoryResult) user.vote = userPoints[0].Value;
                                user.hasVoted = true;
                            }
                        }

                        if (user.Name == service.user.Name) {
                            service.user.IsObserver = user.IsObserver;
                            service.saveUser();
                        }
                    });

                    var prime = function (u) {
                        if (u.IsObserver) return 99;

                        if (team.CurrentStoryResult)
                            return -Math.abs(team.CurrentStoryResult - u.vote);

                        return u.hasVoted;
                    };
                    team.Users.sort(function (a, b) {
                        var pa = prime(a), pb = prime(b);
                        return pa < pb ? -1 : pa > pb ? 1 :
                            a.Name < b.Name ? -1 : a.Name > b.Name ? 1 : 0;
                    });
                    
                    view.render();
                    helpView.render();
                }
            },
            user: cookie(userCookieName),
            error: function(m) {
                logger.log("SessionService.error: " + m);
                service.connect();
            }
        };

        view.$el
            .on("user-change-name", function (e, name) {
                logger.log("user-change-name " + name);

                hub.server
                    .updateCurrentUserName(name)
                    .done(function (session) {
                        service.saveUser(session.User);
                        service.teamUpdate(session.Team);
                    })
                    .fail(function () {
                        view.render();
                    });
            })
            .on("user-change-observer", function (e, name, isObserver) {
                logger.log("user-change-observer " + name + "=" + isObserver);

                hub.server
                    .updateUserIsObserver(name, isObserver);
            })
            .on("story-lock", function (e, data) {
                logger.log("story-lock " + JSON.stringify(data));

                hub.server
                    .lockCurrentStory(data.Title)
                    .fail(service.error);
            })
            .on("story-release", function() {
                logger.log("story-release");

                hub.server
                    .releaseCurrentStory()
                    .fail(service.error);
            })
            .on("user-points", function(e, points) {
                logger.log("user-points " + points);

                hub.server
                    .updateCurrentUserVote(points)
                    .fail(service.error);
            })
            .on("open-voting", function (e, mins) {
                logger.log("open-voting");

                var schedule = mins
                    ? {
                        Percent: 100,
                        Seconds: mins * 60
                    }
                    : {};

                hub.server
                    .openVoting(schedule)
                    .fail(service.error);

                var clearSchedule = function() {
                    if (view.schedule) {
                        window.clearTimeout(view.schedule);
                        view.schedule = null;
                    }
                };

                clearSchedule();

                if (mins) {
                    var start = new Date().getTime(),
                        end = start + (schedule.Seconds * 1000);

                    var scheduleTick = function() {

                        var left = end - new Date().getTime();

                        schedule.Percent = 100 * left / (end - start);
                        schedule.Seconds = Math.floor(left / 1000);

                        hub.server
                            .openVoting(schedule)
                            .fail(service.error);

                        if (schedule.Seconds > 0) {
                            var interval = schedule.Seconds < 100
                                ? .99
                                : schedule.Seconds / 100;
                            view.schedule = window.setTimeout(scheduleTick, interval * 1000);
                        } else {
                            clearSchedule();
                        }
                    };

                    scheduleTick();
                }
            })
            .on("close-voting", function () {
                logger.log("close-voting");

                hub.server
                    .closeVoting()
                    .fail(service.error);
            })
            .on("clear-votes", function () {
                logger.log("clear-votes");

                hub.server
                    .clearVotes()
                    .fail(service.error);
            })
            .on("demo-toggle", function() {
                logger.log("demo-toggle");

                hub.server
                    .demoStart()
                    .fail(service.error);
            });

        return service;
    };
});