define("SessionService", function() {
    var userCookieName = "User";

    return function(hub, view, userNameView, helpView) {
        var logger = require("logger"),
            cookie = require("cookie");

        var service = {
            saveUser:function(user) {
                service.user = user;
                cookie(userCookieName, service.user, { expires: 7 });
            },
            connect: function (teamId) {
                logger.log("SessionService.connect");

                if (!service.user) {

                    userNameView
                        .$el.on("ok", function (e, name) {

                            service.saveUser({
                                Name: name
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
                    .done(function(user) {
                        service.saveUser(user);
                        hub.server.getCurrentTeam()
                            .done(service.handleResponse)
                            .fail(service.error);
                    })
                    .fail(service.error);
            },
            handleResponse: function(team) {
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

                        if (user.Name == service.user.Name)
                            service.user.IsActive = user.IsActive;
                    });

                    var prime = function (u) {
                        if (!u.IsActive) return 99;

                        if (team.CurrentStoryResult)
                            return -Math.abs(team.CurrentStoryResult - u.vote);

                        return u.hasVoted;
                    };
                    team.Users.sort(function (a, b) {
                        a = prime(a), b = prime(b);
                        return a < b ? -1 : a > b ? 1 : 0;
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
            .on("user-change", function (e, name) {
                logger.log("user-change " + name);

                hub.server
                    .updateCurrentUser(name)
                    .done(function () {
                        service.user.Name = name;
                        cookie(userCookieName, service.user);
                    })
                    .fail(function () {
                        view.render();
                    });
            })
            .on("user-active", function (e, name, isActive) {
                logger.log("user-active " + name + "=" + isActive);

                hub.server
                    .updateUserActive(name, isActive);
            })
            .on("story-lock", function (e, data) {
                logger.log("story-lock " + JSON.stringify(data));

                hub.server
                    .lockCurrentStory(data.Title, service.user)
                    .fail(service.error);
            })
            .on("story-release", function() {
                logger.log("story-release");

                hub.server
                    .releaseCurrentStory(service.user)
                    .fail(service.error);
            })
            .on("user-points", function(e, points) {
                logger.log("user-points " + points);

                hub.server
                    .updateCurrentStoryPoints(points, service.user)
                    .fail(service.error);
            })
            .on("open-voting", function () {
                logger.log("open-voting");

                hub.server
                    .openVoting()
                    .fail(service.error);
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
                    .demoToggle()
                    .fail(service.error);
            });

        return service;
    };
});