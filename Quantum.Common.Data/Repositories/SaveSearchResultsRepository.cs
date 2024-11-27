using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Infrastructure.Exceptions;

namespace Quantum.Data.Repositories
{

    public class SaveSearchResultsRepository : BaseRepository<SaveSearchResults>, ISaveSearchResultsRepository
    {
        private QDbContext _context;
        public SaveSearchResultsRepository(QDbContext context)
            : base (context)
        {
            _context = context;
        }

        public async Task CheckExistingSaveSearchResults(IEnumerable<string> queryCollection, IEnumerable<string> existingTagsIds, string userId)
        {
            var searchText = String.Join(" ", queryCollection);

            if (String.IsNullOrWhiteSpace(searchText))
            {
                searchText = null;
            }

            if (await _context.SaveSearchResults.AnyAsync())
            {
                var existingSearch = await _context.SaveSearchResults.DefaultIfEmpty().Where(r => !r.IsDeleted && r.CreatedById == userId && r.SearchText == searchText && r.SaveSearchTags != null)
                                    .Include(r => r.SaveSearchTags)
                                    .Where(r => r.SaveSearchTags.Any() && r.SaveSearchTags.Select(ss => ss.TagId).Count() == existingTagsIds.Count())       
                                    .Where(r => r.SaveSearchTags.Select(sst => sst.TagId).Any(tid => existingTagsIds.Contains(tid)))
                                    
                                    //test2.Where(t2 => !test1.Any(t1 => t2.Contains(t1)));
                                    //.Where(r => r.SaveSearchTags.Any() && r.SaveSearchTags.Select(ss => ss.TagId).Intersect(existingTagsIds).AsQueryable().Count() == existingTagsIds.AsQueryable().Count())
                                    .FirstOrDefaultAsync();

                if (existingSearch != null)
                {
                    throw new ExistingSaveSearchException(HttpStatusCode.BadRequest,
                        Errors.ErrorExistingSaveSearch);
                }
            }
            
        }

        public async Task CheckExistingTitle(string title, string userId)
        {
            var savesearchResults = await base.Query(r => !r.IsDeleted && r.CreatedById == userId && r.Title == title)
                    .FirstOrDefaultAsync();

            if (savesearchResults != null)
            {
                throw new SaveSearchTitleExistException(
                    HttpStatusCode.BadRequest, Errors.ErrorSaveSearchTitleExist);
            }
        }

        public async Task<List<SaveSearchResults>> GetByIdsAndUserId(string[] ids, string userId)
        {
            return await base.Query(r => !r.IsDeleted && r.CreatedById == userId && ids.Contains(r.ID), false)
                .Include(i => i.SaveSearchTags)
                .OrderByDescending(i => i.LastModified)
                .ToListAsync();
        }

        public async Task<List<SaveSearchResults>> GetResultsByUserId(string userId)
        {
            return await base.Query(r => !r.IsDeleted && r.CreatedById == userId)
                    .Include(i => i.SaveSearchTags)
                    .OrderByDescending(i => i.LastModified)
                    .ToListAsync();
        }
    }
}
