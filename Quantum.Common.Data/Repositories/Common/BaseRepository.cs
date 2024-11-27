using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Quantum.Data.Entities.Common;

namespace Quantum.Data.Repositories.Common
{
	public class BaseRepository<TEntity> : IBaseRepository<TEntity, IdentityUser> 
        where TEntity : BaseEntity
    {
		private QDbContext _context;

		public BaseRepository(QDbContext context)
		{
			_context = context;
		}

		public virtual async Task<TEntity> GetById(string id, bool tracking = true)
		{
			var entity = await Query(e => e.ID == id, tracking)
				.OrderByDescending(e => e.CreatedDate)
				.FirstOrDefaultAsync();

            if (entity != null && !entity.IsDeleted)
			{
				return entity;
			}

			return default(TEntity);
		}

		public virtual async Task<IEnumerable<TEntity>> GetPaged(int skip, int take, bool tracking = true)
		{
            return await Query(i => !i.IsDeleted, tracking)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
		}

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter, bool tracking = true)
		{
			var result = _context.Set<TEntity>().Where(filter);

            if(tracking)
            {
                return result;
            }

            return result.AsNoTracking();
        }

        public virtual async Task<int> Delete(string id, IdentityUser user, bool save = true)
        {
            var entity = await GetById(id, true);

            ///Check if entity == default(TEntity) -> Exception

            if (entity != null && !entity.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.DeletedBy = user;
                entity.DeletedOn = DateTime.UtcNow;

                _context.Entry(entity).State = EntityState.Modified;
            }

            if (save)
            {
                return await this.Save();
            }

            return await Task.FromResult(0);
        }

        public virtual async Task<int> DeleteEntity(TEntity entity, IdentityUser user, bool save = true)
        {

            if (entity != null && !entity.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.DeletedBy = user;
                entity.DeletedOn = DateTime.UtcNow;

                _context.Entry(entity).State = EntityState.Modified;
            }

            if (save)
            {
                return await this.Save();
            }

            return await Task.FromResult(0);
        }

        public virtual async Task<int> Insert(TEntity item, IdentityUser user, bool save = true)
        {
            if (item.ID == default(string))
            {
                item.ID = Guid.NewGuid().ToString("N");
            }

            item.CreatedDate = DateTime.UtcNow;
            item.CreatedBy = user;
            item.LastModified = item.CreatedDate;
            item.LastModifiedBy = user;

            _context.Entry(item).State = EntityState.Added;

            if (save)
            {
                return await this.Save();
            }

            return await Task.FromResult(0);
        }

        public virtual async Task<int> Update(TEntity item, IdentityUser user, bool save = true)
        {
            item.LastModified = DateTime.UtcNow;
            item.LastModifiedBy = user;

            _context.Entry(item).State = EntityState.Modified;

            if (save)
            {
                return await this.Save();
            }

            return await Task.FromResult(0);
        }

        public virtual async Task<int> Remove(string id)
        {
            var entity = await GetById(id, true);

            _context.Entry(entity).State = EntityState.Deleted;

             return await this.Save();
        }

        public virtual async Task<int> Save()
		{
            return await _context.SaveChangesAsync();
        }

		#region IDisposable Support

		private bool disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					_context.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

        #endregion
    }
}
