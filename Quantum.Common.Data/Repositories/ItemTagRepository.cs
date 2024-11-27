using Microsoft.EntityFrameworkCore;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories
{
	public class ItemTagRepository : BaseRepository<ItemTag>, IItemTagRepository
	{
		private QDbContext _context;

		public ItemTagRepository(
			QDbContext context
			) : base(context)
		{
			_context = context;
		}

		public async Task<List<Tag>> GetTagsByItemTagsIds(IEnumerable<string> itemTagsIds)
		{
			var tags = await base.Query(ti => !ti.IsDeleted && itemTagsIds.Contains(ti.ID) && ti.Display)
						.Include(ti => ti.Tag)
						.Select(ti => ti.Tag)
						.DistinctBy(t => t.Name)
						.ToListAsync();

			return tags;
		}
	}
}
