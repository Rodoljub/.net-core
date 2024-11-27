using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Quantum.Data.Repositories.Common.Contracts
{
    public interface IBaseRepository<TEntity, TUser> : IDisposable 
        where TEntity : BaseEntity
        where TUser : IdentityUser
    {
	    Task<TEntity> GetById(string id, bool tracking = false);

		Task<IEnumerable<TEntity>> GetPaged(int skip, int take, bool tracking = false);

		Task<int> Save();

		IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter, bool tracking = false);

        Task<int> Insert(TEntity item, TUser user, bool save = true);

        Task<int> Update(TEntity item, TUser user, bool save = true);

        Task<int> Delete(string id, TUser user, bool save = true);

        Task<int> DeleteEntity(TEntity entity, TUser user, bool save = true);

        Task<int> Remove(string id);
    }
}
