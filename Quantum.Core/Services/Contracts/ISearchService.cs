using Quantum.Core.Models;
using Quantum.Data.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
     public interface ISearchService
    {
        SearchRequestModel ParseSearchRequest(string query);

        Task<SavedSearchResultModel> SaveSearch(SaveSearchModel model, IIdentity identity);

        Task<SavedSearchResultModel> UpdateSaveSearch(SavedSearchResultModel model, IIdentity identity);

        Task<string[]> DeleteSaveSearch(string[] model, IIdentity identity);

        Task<SavedSearchResultModel[]> GetSavedSearchResults(IIdentity identity);
    }
}
