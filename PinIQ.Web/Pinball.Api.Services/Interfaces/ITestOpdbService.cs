using Pinball.OpdbClient.Entities;
using System.Threading.Tasks;

namespace Pinball.Api.Services.Interfaces
{
    public interface ITestOpdbService
    {
        Task<string> GetAsync(OpdbId id);
    }
}
