using System.Threading.Tasks;

namespace Antix.Work.Sizing.Services
{
    public interface IDemoService
    {
        Task Start(string memberId);
        Task Next(string memberId);
        Task Cancel(string memberId);
    }
}