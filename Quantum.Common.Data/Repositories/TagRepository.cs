using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
	public class TagRepository : BaseRepository<Tag>, ITagRepository
	{
		private QDbContext _context;
		private IConfiguration _config;
		public TagRepository(
			QDbContext context,
			IConfiguration config
			) : base(context)
		{
			_context = context;
			_config = config;
		}

		public async Task<List<ItemTagsCountModel>> GetMathedItemTagsByParsedQuery(SearchRequestModel parsedQuery)
		{
			return await base.Query(t => !t.IsDeleted && parsedQuery.Tags.Contains(t.Name.ToLower()))
									.Include(t => t.ItemTags)
									.SelectMany(t => t.ItemTags)
                                    .DistinctBy(t => t.Tag.Name)
                                    .GroupBy(it => it.ItemId)
									.Select(group => new ItemTagsCountModel
									{
										ItemId = group.FirstOrDefault().ItemId,
										MatchingTagsCount = group.Count()
									})
									.OrderByDescending(itc => itc.MatchingTagsCount)
									.ToListAsync();

		}

		public async Task<List<TagModel>> SearchTags(string searchQuery, IEnumerable<string> selectedTags)
		{
			var take = _config.GetAsInteger($"Application:MaxTags", 7);
			return await 
				base.Query(
						t => !t.IsDeleted 
						&& t.Name.ToLower().StartsWith(searchQuery)
						&& !selectedTags.Contains(t.Name.ToLower())
					)
					//.Where(t => !selectedTags.Contains(t.Name.ToLower()))
					.Take(take)
                    .DistinctBy(tag => tag.Name)
                    .Select(tag => new TagModel
						{
							ID = tag.ID,
							Name = tag.Name.ToLower()
						})
					.ToListAsync();
		}
	}
}
