using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Quantum.Data.Repositories.Common
{
	public abstract class BaseVersionedRepository<TEntity, TEntityVersion> 
		: IVersionedRepository<TEntity, TEntityVersion>
		where TEntity : BaseEntity, IBaseVersionedEntity<TEntityVersion>, new()
		where TEntityVersion : BaseEntity, IEntityVersion<TEntity>
	{
		private QuantumContext _context;
		private ILogger<BaseVersionedRepository<TEntity, TEntityVersion>> _logger;

		public BaseVersionedRepository(
			QuantumContext context,
			ILogger<BaseVersionedRepository<TEntity, TEntityVersion>> logger
		)
		{
			_context = context;
			_logger = logger;
		}

		public virtual TEntityVersion GetById(string id)
		{
			var entity = _context.Set<TEntityVersion>()
				.Where(e => e.Base.ReferenceID == id && !e.Base.IsDeleted && !e.IsDeleted)
				.OrderByDescending(e => e.CreatedDate)
				.FirstOrDefault();

			return entity;
		}

		public virtual async Task<TEntityVersion> GetByIDAsync(string id)
		{
			var entity = await _context.Set<TEntityVersion>()
				.Where(e => e.Base.ReferenceID == id && !e.Base.IsDeleted && !e.IsDeleted)
				.OrderByDescending(e => e.CreatedDate)
				.FirstOrDefaultAsync();

			return entity;
		}

		public virtual IEnumerable<TEntityVersion> GetPaged(int skip, int take)
		{
			var result = _context.Set<TEntityVersion>()
				.Where(e => !e.Base.IsDeleted && !e.IsDeleted)
				.OrderByDescending(e => e.Base.CreatedDate)
				.ThenByDescending(e => e.CreatedDate)
				.GroupBy(keySelector: e => e.Base.ReferenceID, elementSelector: e => e)
				.Select(g => g.OrderByDescending(e => e.CreatedDate).FirstOrDefault())
				.Skip(skip).Take(take);

			return result;
		}

		public virtual async Task<IEnumerable<TEntityVersion>> GetPagedAsync(int skip, int take)
		{
			var result = await _context.Set<TEntityVersion>()
			.Where(e => !e.Base.IsDeleted && !e.IsDeleted)
			.OrderByDescending(e => e.Base.CreatedDate)
			.ThenByDescending(e => e.CreatedDate)
			.GroupBy(keySelector: e => e.Base.ReferenceID, elementSelector: e => e)
			.Select(g => g.OrderByDescending(e => e.CreatedDate).FirstOrDefault())
			.Skip(skip).Take(take)
			.ToListAsync();

			return result;
		}

		public virtual string Insert(TEntityVersion item)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var baseItemId = Helper.Instance.GetID();
					var baseItem = new TEntity();

					baseItem.ReferenceID = baseItemId;
					baseItem.CreatedDate = DateTime.Now;
					baseItem.LastModified = item.CreatedDate;

					_context.Entry(baseItem).State = EntityState.Added;


					var itemId = Helper.Instance.GetID();
					item.ReferenceID = itemId;
                    item.ID = itemId;
                    item.CreatedDate = DateTime.Now;
					item.LastModified = item.CreatedDate;
					item.Base = baseItem;

					_context.Entry(item).State = EntityState.Added;

					this.Save();

					transaction.Commit();

					return baseItemId;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					_logger.LogError($"Insert failed due to following error: {ex.Message}");
					throw ex;
				}
			}
		}

		public async Task<string> InsertAsync(TEntityVersion item)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var baseItemId = Helper.Instance.GetID();
					var baseItem = new TEntity();

					baseItem.ReferenceID = baseItemId;
					baseItem.CreatedDate = DateTime.Now;
					baseItem.LastModified = item.CreatedDate;

					_context.Entry(baseItem).State = EntityState.Added;


					var itemId = Helper.Instance.GetID();
					item.ReferenceID = itemId;
					item.CreatedDate = DateTime.Now;
					item.LastModified = item.CreatedDate;
					item.Base = baseItem;

					_context.Entry(item).State = EntityState.Added;

					await this.SaveAsync();

					transaction.Commit();

					return baseItemId;
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					_logger.LogError($"Insert failed due to following error: {ex.Message}");
					throw ex;
				}

			}
		}

		public async Task UpdateAsync(TEntityVersion item)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var entity = await _context.Set<TEntity>()
				.FirstOrDefaultAsync(e => e.ReferenceID == item.BaseId);

					if (entity != null)
					{
						var entities = await _context.Set<TEntityVersion>()
						.Where(v => v.Base.ReferenceID == entity.ReferenceID && !v.IsDeleted)
						.ToListAsync();

						entity.LastModified = DateTime.Now;
						_context.Entry(entity).State = EntityState.Modified;

						entities.ForEach(e =>
						{
							e.IsDeleted = true;
							e.DeletedOn = DateTime.Now;
							_context.Entry(e).State = EntityState.Modified;
						});

						var itemId = Helper.Instance.GetID();
						item.ReferenceID = itemId;
                        item.ID = itemId;
                        item.CreatedDate = DateTime.Now;
						item.LastModified = item.CreatedDate;
						item.Base = entity;

						_context.Entry(item).State = EntityState.Added;

						await this.SaveAsync();

						transaction.Commit();
					}
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					_logger.LogError($"Update failed due to following error: {ex.Message}");
					throw ex;
				}
			}
		}

		public void Update(TEntityVersion item)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{

					var entity = _context.Set<TEntity>()
				.FirstOrDefault(e => e.ReferenceID == item.BaseId);

					if (entity != null)
					{
						var entities = _context.Set<TEntityVersion>()
							.Where(v => v.Base.ReferenceID == entity.ReferenceID && !v.IsDeleted)
							.ToList();

						entity.LastModified = DateTime.Now;
						_context.Entry(entity).State = EntityState.Modified;

						entities.ForEach(e =>
						{
							e.IsDeleted = true;
							e.DeletedOn = DateTime.Now;
							_context.Entry(e).State = EntityState.Modified;
						});


						var itemId = Helper.Instance.GetID();
						item.ReferenceID = itemId;
                        item.ID = itemId;
                        item.CreatedDate = DateTime.Now;
						item.LastModified = item.CreatedDate;
						item.Base = entity;

						_context.Entry(item).State = EntityState.Added;

						this.Save();

						transaction.Commit();
					}
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					_logger.LogError($"Update failed due to following error: {ex.Message}");
					throw ex;
				}
			}
		}

		public async Task DeleteAsync(string id)
		{
			var entity = await _context.Set<TEntity>().FindAsync(id);

			if (entity != null && !entity.IsDeleted)
			{
				entity.IsDeleted = true;
				entity.DeletedOn = DateTime.Now;
				_context.Entry(entity).State = EntityState.Modified;
			}
		}

		public virtual IQueryable<TEntityVersion> Query(Expression<Func<TEntityVersion, bool>> filter)
		{
			var query = _context.Set<TEntityVersion>()
				.Include(up => up.Base)
				.Where(e => e != null && e.Base != null)
				.GroupBy(keySelector: e => e.Base.ReferenceID, elementSelector: e => e)
				.Select(g => g.OrderByDescending(e => e.CreatedDate).FirstOrDefault())
				.Where(filter);

			return query;
		}

		//private TEntityVersion GetMaxVersion(TEntity entity)
		//{
		//	if (entity.Versions != null)
		//	{
		//		return entity.Versions
		//		.Where(item => !item.IsDeleted)
		//		.OrderByDescending(item => item.ID)
		//		.FirstOrDefault();
		//	}

		//	return null;
		//}

		public virtual int Save()
		{
			return _context.SaveChanges();
		}

		public virtual async Task<int> SaveAsync()
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
