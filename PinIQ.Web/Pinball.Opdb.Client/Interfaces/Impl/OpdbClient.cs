using Pinball.OpdbClient.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Pinball.OpdbClient.Helpers;
using System.Text.Json;

namespace Pinball.OpdbClient.Interfaces.Impl
{
    public class OpdbClient : IOpdbClient
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly OpdbClientOptions _options;

        public OpdbClient(IHttpClientFactory httpClient, IOptionsMonitor<OpdbClientOptions> optionsAccessor)
        {
            _httpClientFactory = httpClient;
            _options = optionsAccessor.CurrentValue;
        }

        public async Task<IOpdbResponse> ExportAsync()
        {
            var result = await ExecuteRequest<IEnumerable<Machine>>("/export");
            return result;
        }

        public async Task<IOpdbResponse> ExportGroupsAsync()
        {
            var result = await ExecuteRequest<IEnumerable<MachineGroup>>("/export/groups");
            return result;
        }

        public async Task<IOpdbResponse> GetAsync(OpdbId id)
        {
            var result = await ExecuteRequest<Machine>($"/machines/{id}");
            return result;
        }

        public async Task<IOpdbResponse> GetAsync(int id)
        {
            var result = await ExecuteRequest<Machine>($"/machines/ipdb/{id}");
            return result;
        }

        public async Task<IOpdbResponse> GetChangelogAsync(DateTimeOffset? from = null)
        {
            if (from.HasValue)
            {
                var queryOptions = new Dictionary<string, string>
                {
                    { "from", from.Value.ToString("yyyy-MM-dd hh:mm:ss") }
                };
                var result = await ExecuteRequest<Changelog>("/changelog", queryOptions);
                return result;
            } 
            else
            {
                var result = await ExecuteRequest<Changelog>("/changelog");
                return result;
            }
        }

        public Task<IEnumerable<Machine>> SearchAsync(string query, bool requireOpdb = true, bool includeGroups = false, bool includeAliases = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Machine>> TypeaheadSearchAsync(string query, bool includeGroups = false, bool includeAliases = true)
        {
            throw new NotImplementedException();
        }

        private async Task<OpdbResponse> ExecuteRequest<T>(string uriString, Dictionary<string, string>? queryOptions = null)
        {
            var uri = GetUri(uriString, queryOptions);
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var resultObject = JsonSerializer.Deserialize<T>(responseJson);
            if (resultObject is null)
                return new OpdbResponse
                {
                    Status = OpdbResponseStatus.Ok,
                    JsonResponse = responseJson
                };
            
            return new OpdbResponse<T>
            {
                Status = OpdbResponseStatus.Ok,
                JsonResponse = responseJson,
                Result = resultObject
            };
        }
        

        private Uri GetUri(string endpoint, Dictionary<string, string>? queryOptions = null)
        {
            var ub = new UriBuilder(_options.BaseUrl);

            var qsb = new QueryStringBuilder();
            if(queryOptions != null)
            {
                foreach(var kvp in queryOptions)
                {
                    qsb.Add(kvp.Key, kvp.Value);
                }
            }

            if (string.IsNullOrEmpty(_options.ApiKey)) throw new Exception("Api Key not set");
            qsb.Add("api_token", _options.ApiKey);
            ub.Query = qsb.ToString();

            ub.Path = string.Concat(ub.Path, endpoint.TrimStart('/'));

            return ub.Uri;
        }
    }
}
