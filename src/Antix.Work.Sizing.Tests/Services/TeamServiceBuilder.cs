using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Antix.Logging;
using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.Models;

using Moq;

namespace Antix.Work.Sizing.Tests.Services
{
    public class TeamServiceBuilder
    {
        IList<TeamModel> _teams = new List<TeamModel>();

        public TeamServiceBuilder With(IList<TeamModel> teams)
        {
            _teams = teams;
            return this;
        }
        
        public TeamServiceBuilder With(TeamModel team)
        {
            _teams.Add(team);
            return this;
        }

        public ITeamService Build()
        {
            var dataServiceMock = new Mock<ITeamDataService>();
            dataServiceMock
                .Setup(o => o.TryGetById(It.IsAny<string>()))
                .Returns((string id) =>
                         Task.FromResult(_teams.TryGetById(id)));
            dataServiceMock
                .Setup(o => o.Update(It.IsAny<TeamModel>()))
                .Returns((TeamModel team) =>
                    {
                        if (team.Id == null)
                            team.Id = Guid.NewGuid().ToString();

                        return Task.FromResult(team);
                    });
            dataServiceMock
                .Setup(o => o.Remove(It.IsAny<string>()))
                .Returns((string id) =>
                    {
                        var team = _teams.TryGetById(id);
                        _teams.Remove(team);

                        return Task.FromResult(team);
                    });

            dataServiceMock
                .Setup(o => o.TryAddIndex(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(0));
            dataServiceMock
                .Setup(o => o.TryRemoveIndex(It.IsAny<string>()))
                .Returns(Task.FromResult(_teams.Select(t => t.Id).FirstOrDefault()));

            return new TeamService(
                dataServiceMock.Object,
                Log.ToConsole);
        }
    }
}