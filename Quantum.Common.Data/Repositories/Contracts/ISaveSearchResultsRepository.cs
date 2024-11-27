using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface ISaveSearchResultsRepository : IBaseRepository<SaveSearchResults, IdentityUser>
    {
        Task<List<SaveSearchResults>> GetResultsByUserId(string userId);

        Task<List<SaveSearchResults>> GetByIdsAndUserId(string[] ids, string userId);

        Task CheckExistingTitle(string title, string userId);

        Task CheckExistingSaveSearchResults(IEnumerable<string> queryCollection, IEnumerable<string> existingTagsIds, string userId);
    }
}
