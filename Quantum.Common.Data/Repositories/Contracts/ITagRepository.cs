using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Repositories.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
	public interface ITagRepository : IBaseRepository<Tag, IdentityUser>
	{
		Task<List<ItemTagsCountModel>> GetMathedItemTagsByParsedQuery(SearchRequestModel parsedQuery);

		Task<List<TagModel>> SearchTags(string searchQuery, IEnumerable<string> selectedTags);
	}
}
