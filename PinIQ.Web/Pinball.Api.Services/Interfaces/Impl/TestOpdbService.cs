using System.Threading.Tasks;
using Pinball.Entities.Opdb;
using Pinball.OpdbClient.Interfaces;

namespace Pinball.Api.Services.Interfaces.Impl;

public class TestOpdbService : ITestOpdbService
{
    private readonly IOpdbClient _opdbClient;

    public TestOpdbService(IOpdbClient opdbClient)
    {
        _opdbClient = opdbClient;
    }

    public async Task<string> GetAsync(OpdbId id)
    {
        var response = await _opdbClient.GetAsync(id);
        return response.JsonResponse;
    }
}