using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quantum.Core.Models;
using Quantum.Data.Entities;

namespace Quantum.Core.Mapping.Services.Contracts
{
    public interface IMappingSearchService
    {
        Task<SaveSearchResults> MapSaveSearchResultFromSaveSearchModel(SaveSearchModel model, IEnumerable<string> queryCollection);
        Task<SaveSearchTags> MapSaveSearchTag(SaveSearchResults saveSearchResults, Tag saveSearchTag);
        Task<SavedSearchResultModel> MapSaveSearchViewModelFromSaveSearchResults(SaveSearchResults saveSearchResults, IEnumerable<Tag> tags);
    }
}
