using Pinball.OpdbClient.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pinball.OpdbClient.Interfaces
{
    public interface IOpdbClient
    {
        Task<IEnumerable<Machine>> TypeaheadSearchAsync(string query, bool includeGroups = false, bool includeAliases = true);
        Task<IEnumerable<Machine>> SearchAsync(string query, bool requireOpdb = true, bool includeGroups = false, bool includeAliases = true);
        Task<IOpdbResponse> GetAsync(OpdbId id);
        Task<IOpdbResponse> GetAsync(int id);
        Task<IOpdbResponse> ExportAsync();
        Task<IOpdbResponse> ExportGroupsAsync();
        Task<IOpdbResponse> GetChangelogAsync(DateTimeOffset? from = null);
    }
}
