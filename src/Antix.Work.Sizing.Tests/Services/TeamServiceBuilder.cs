using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Antix.Work.Sizing.Services;
using Antix.Work.Sizing.Services.Models;
using Moq;

namespace Antix.Work.Sizing.Tests.Services
{
    public class TeamServiceBuilder
    {
        readonly IList<TeamModel> _teams = new List<TeamModel>();

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
                         Task.FromResult(_teams.ByIdOrDefault(id)));
            dataServiceMock
                .Setup(o => o.Update(It.IsAny<TeamModel>()))
                .Returns((TeamModel team) =>
                    {
                        if (team.Id == null)
                            team.Id = Guid.NewGuid().ToString();

                        return Task.FromResult(team);
                    });

            return new TeamService(
                dataServiceMock.Object,
                new DebugLogger());
        }
    }
}