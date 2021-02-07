using Pinball.OpdbClient.Entities;
using Pinball.OpdbClient.Interfaces;
using System.Threading.Tasks;

namespace Pinball.Api.Services.Interfaces.Impl
{
    public class TestOpdbService : ITestOpdbService
    {
        private IOpdbClient _opdbClient;

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
}