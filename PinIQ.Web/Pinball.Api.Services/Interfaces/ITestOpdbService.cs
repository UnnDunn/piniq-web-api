using System.Threading.Tasks;
using Pinball.Entities.Opdb;

namespace Pinball.Api.Services.Interfaces;

public interface ITestOpdbService
{
    Task<string> GetAsync(OpdbId id);
}