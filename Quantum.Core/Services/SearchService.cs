using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
    public class SearchService : ISearchService
    {
        private IMappingSearchService _mapSearchServ;
        private IUserManagerService _userMgrServ;
        private ISaveSearchResultsRepository _saveSearchResultsRepo;
        private ISaveSearchTagsRepository _saveSearchTagsRepo;
        private ITagRepository _tagRepo;
        public SearchService(
            IMappingSearchService mapSearchServ,
            IUserManagerService userMgrServ,
            ISaveSearchResultsRepository saveSearchResultsRepo,
            ISaveSearchTagsRepository saveSearchTagsRepo,
            ITagRepository tagRepo
        )
        {
            _mapSearchServ = mapSearchServ;
            _userMgrServ = userMgrServ;
            _saveSearchResultsRepo = saveSearchResultsRepo;
            _saveSearchTagsRepo = saveSearchTagsRepo;
            _tagRepo = tagRepo;
        }

        public SearchRequestModel ParseSearchRequest(string query)
        {
            var cleanedString = System.Text.RegularExpressions.Regex.Replace(query, @"\s+", " ");

            var queryComponents = cleanedString.Split(',').Select(qc => qc.Trim().ToLower()).Where(qc => qc != "");

            var dirtyTags = queryComponents.Where(qc => qc.Contains("#"));

            var queryTag = dirtyTags.Where(qc => !qc.StartsWith("#"));

            var extractedQueries = dirtyTags.Select(dt => dt.Split('#')[1].ToLower());

            var extractedTags = dirtyTags.SelectMany(dt => dt.Split('#').Skip(1).ToArray());

            var doubleTags = dirtyTags.Where(qc => qc.StartsWith("#") && qc.Count(c => c == '#') > 1)
                .SelectMany(dt => dt.Split('#'))
                .Union(extractedTags);

            var cleanTags = dirtyTags.Where(qc => qc.StartsWith("#") && qc.Count(c => c == '#') == 1)
                .Union(doubleTags)
                .Select(ct => ct.Trim('#').ToLower())
                .Distinct();

            var queryCollection = queryComponents.Where(qc => !qc.Contains("#"))
                .Union(extractedQueries)
                .Select(qc => qc.ToLower());

            if (queryCollection.Count() == 0)
            {
                queryCollection = Array.Empty<string>();
            }

            return new SearchRequestModel()
            {
                QueryCollection = queryCollection,
                Tags = cleanTags
            };
        }

        public async Task<SavedSearchResultModel> SaveSearch(SaveSearchModel model, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);

            await _saveSearchResultsRepo.CheckExistingTitle(model.Title.Trim(), user.Id);

            var searchRequestModel = ParseSaveSearchResultsRequest(model.SearchQuery);

            var existingTags = await _tagRepo.Query(t => !t.IsDeleted && searchRequestModel.Tags.Contains(t.Name)).ToListAsync();

            var existingTagsIds = existingTags.Select(et => et.ID);

            await _saveSearchResultsRepo.CheckExistingSaveSearchResults(searchRequestModel.QueryCollection, existingTagsIds, user.Id);

            var saveSearchResults = await _mapSearchServ.MapSaveSearchResultFromSaveSearchModel(model, searchRequestModel.QueryCollection);

            await _saveSearchResultsRepo.Insert(saveSearchResults, user);

            if(existingTags.Count() > 0)
            {
                await SaveSearchTags(saveSearchResults, existingTags, user);
            }

            var mapSaveSearchView = await _mapSearchServ.MapSaveSearchViewModelFromSaveSearchResults(saveSearchResults, existingTags);

            return mapSaveSearchView;
        }

        private async Task SaveSearchTags(SaveSearchResults saveSearchResults, IEnumerable<Tag> existingTags, IdentityUser user)
        {
            foreach(var saveSearchTag in existingTags)
            {
                var mapSaveSearchTag = await _mapSearchServ.MapSaveSearchTag(saveSearchResults, saveSearchTag);
                await _saveSearchTagsRepo.Insert(mapSaveSearchTag, user);
            }

            //await _saveSearchTagsRepo.Save();

        }

        private SearchRequestModel ParseSaveSearchResultsRequest(string query)
        {
            var cleanedString = System.Text.RegularExpressions.Regex.Replace(query, @"\s+", " ");

            var queryComponents = cleanedString.Split(',').Select(qc => qc.Trim().ToLower()).Where(qc => qc != "");

            var dirtyTags = queryComponents.Where(qc => qc.Contains("#"));

            var queryTag = dirtyTags.Where(qc => !qc.StartsWith("#"));

            //var extractedQueries = dirtyTags.Select(dt => dt.Split('#')[1].ToLower());

            var extractedTags = dirtyTags.SelectMany(dt => dt.Split('#').Skip(1).ToArray());

            var doubleTags = dirtyTags.Where(qc => qc.StartsWith("#") && qc.Count(c => c == '#') > 1)
                .SelectMany(dt => dt.Split('#'))
                .Union(extractedTags);

            var cleanTags = dirtyTags.Where(qc => qc.StartsWith("#") && qc.Count(c => c == '#') == 1)
                .Union(doubleTags)
                .Select(ct => ct.Trim('#').ToLower())
                .Distinct();

            var queryCollection = queryComponents.Where(qc => !qc.Contains("#"))
                //.Union(extractedQueries)
                .Select(qc => qc.ToLower());



            return new SearchRequestModel()
            {
                QueryCollection = queryCollection,
                Tags = cleanTags
            };
        }

        public async Task<SavedSearchResultModel[]> GetSavedSearchResults(IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);

            var savedSearchResults = await _saveSearchResultsRepo.GetResultsByUserId(user.Id);

            List<Tag> tags = await GetTagsBySavedSearchResults(savedSearchResults);

            var mappingViewSavedSearch = savedSearchResults.Select(ssr => _mapSearchServ.MapSaveSearchViewModelFromSaveSearchResults(ssr, tags).GetAwaiter().GetResult()).ToArray();

            var viewItems = await Task.FromResult(mappingViewSavedSearch);

            return viewItems;
        }

        private async Task<List<Tag>> GetTagsBySavedSearchResults(List<SaveSearchResults> savedSearchResults)
        {
            var saveSearchTagsIds = savedSearchResults.SelectMany(ssr => ssr.SaveSearchTags
                            .Select(sst => sst.ID))
                            .Distinct();

            var tags = await _saveSearchTagsRepo.GetTagsBySaveSearchTagsIds(saveSearchTagsIds);

            return tags;
        }

        public async Task<SavedSearchResultModel> UpdateSaveSearch(SavedSearchResultModel model, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);

            await _saveSearchResultsRepo.CheckExistingTitle(model.Title.Trim(), user.Id);

            var savedSearchResult = await _saveSearchResultsRepo.GetById(model.Id);

            if (savedSearchResult != null && savedSearchResult != default)
            {
                savedSearchResult.Title = model.Title;
                await _saveSearchResultsRepo.Update(savedSearchResult, user);

                return model;
            }

            throw new HttpStatusCodeException(
                    HttpStatusCode.BadRequest,
                    null,
                    Errors.GeneralError,
                    null
                  );
        }

        public async Task<string[]> DeleteSaveSearch(string[] model, IIdentity identity)
        {
            var user = await _userMgrServ.GetAuthUser(identity);

            //_saveSearchResultsRepo.DeletebyIds(model, user.Id);

            var savedSearchResults = await _saveSearchResultsRepo.GetByIdsAndUserId(model, user.Id);

            foreach (var savedSearchResult in savedSearchResults)
            {
                await _saveSearchResultsRepo.Delete(savedSearchResult.ID, user);
            }

            //await _saveSearchResultsRepo.Save();

            return model;

            //return savedSearchResults.Select(s => s.ID).ToArray();
        }
    }
}
