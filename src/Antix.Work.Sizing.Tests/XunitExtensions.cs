using System;
using System.Threading.Tasks;
using Xunit;

namespace Antix.Work.Sizing.Tests
{
    public static class AssertEx
    {
        public static async Task Throws<TException>(Func<Task> func)
        {
            var expected = typeof (TException);
            Type actual = null;
            try
            {
                await func();
            }
            catch (Exception e)
            {
                actual = e.GetType();
            }
            Assert.Equal(expected, actual);
        }

        public static async Task DoesNotThrow(Func<Task> func)
        {
            await func();
        }
    }
}