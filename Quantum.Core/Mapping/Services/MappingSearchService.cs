using AutoMapper;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services
{
    public class MappingSearchService : IMappingSearchService
    {
        private IMapper _mapper;
        public MappingSearchService(
            IMapper mapper
        )
        {
            _mapper = mapper;
        }

        public async Task<SaveSearchResults> MapSaveSearchResultFromSaveSearchModel(SaveSearchModel model, IEnumerable<string> queryCollection)
        {
            var mappedSaveSearchResults = _mapper.Map<SaveSearchModel, SaveSearchResults>(model);
            var searchText = String.Join(" ", queryCollection);
            if (!String.IsNullOrWhiteSpace(searchText))
            {
                mappedSaveSearchResults.SearchText = searchText;
            }
            else
            {
                mappedSaveSearchResults.SearchText = null;
            }
            return await Task.FromResult(mappedSaveSearchResults);
        }


        public async Task<SaveSearchTags> MapSaveSearchTag(SaveSearchResults saveSearchResults, Tag saveSearchTag)
        {
            var newSaveSearchTag = new SaveSearchTags();
            var mappedSaveSearchTags = _mapper.Map<SaveSearchTags, SaveSearchTags>(newSaveSearchTag);
            mappedSaveSearchTags.SaveSearchResults = saveSearchResults;
            mappedSaveSearchTags.Tag = saveSearchTag;
            return await Task.FromResult(mappedSaveSearchTags);
        }


        public async Task<SavedSearchResultModel> MapSaveSearchViewModelFromSaveSearchResults(SaveSearchResults saveSearchResults, IEnumerable<Tag> tags)
        {
            tags = tags.Where(t => saveSearchResults.SaveSearchTags.Select(it => it.TagId).Contains(t.ID));

            var tagNames = tags.Select(t => t.Name.ToLower())
                .Distinct()
                .ToArray();

            var mapSaveSearchView = _mapper.Map<SaveSearchResults, SavedSearchResultModel>(saveSearchResults);

            mapSaveSearchView.SearchText = saveSearchResults.SearchText;
            mapSaveSearchView.SearchTags = tagNames;

            return await Task.FromResult(mapSaveSearchView);
        }
    }
}
