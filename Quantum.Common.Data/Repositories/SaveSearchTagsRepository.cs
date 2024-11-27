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
    public class SaveSearchTagsRepository : BaseRepository<SaveSearchTags>, ISaveSearchTagsRepository
    {
        private QDbContext _context;
        public SaveSearchTagsRepository(
             QDbContext context
        ) : base (context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetTagsBySaveSearchTagsIds(IEnumerable<string> saveSearchTagsIds)
        {
            var tags = await base.Query(ti => !ti.IsDeleted && saveSearchTagsIds.Contains(ti.ID))
                        .Include(ti => ti.Tag)
                        .Select(ti => ti.Tag)
                        .DistinctBy(t => t.Name)
                        .ToListAsync();

            return tags;
        }
    }
}
