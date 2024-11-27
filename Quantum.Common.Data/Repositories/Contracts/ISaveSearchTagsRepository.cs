using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Contracts
{
    public interface ISaveSearchTagsRepository : IBaseRepository<SaveSearchTags, IdentityUser>
    {
        Task<List<Tag>> GetTagsBySaveSearchTagsIds(IEnumerable<string> saveSearchTagsIds);
   }
}
