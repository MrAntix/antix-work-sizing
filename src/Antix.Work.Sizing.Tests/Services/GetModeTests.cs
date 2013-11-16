using Antix.Work.Sizing.Services.Models;

using Xunit;

namespace Antix.Work.Sizing.Tests.Services
{
    public class GetModeTests
    {
        [Fact]
        public void get_mode()
        {
            Assert.Equal(3, new[] { 1, 3, 3 }.GetMode());
            Assert.Equal(3, new[] { 1, 3, 3, 5 }.GetMode());
            Assert.Equal(3, new[] { 3, 3, 5 }.GetMode());
            Assert.Equal(3, new[] { 3, 3 }.GetMode());
        }
    }
}