using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Quantum.Data.Repositories.Common
{
	public abstract class UserVersionedRepository<TEntity, TEntityVersion, TUser>
		: BaseVersionedRepository<TEntity, TEntityVersion>, IUserVersionedRepository<TEntity, TEntityVersion, TUser>
		where TEntity : BaseUserEntity, IBaseVersionedEntity<TEntityVersion>, new()
		where TEntityVersion : BaseEntity, IUserEntityVersion<TEntity>
		where TUser : IdentityUser

	{
		private QuantumContext _context;
		private ILogger<UserVersionedRepository<TEntity, TEntityVersion, TUser>> _logger;

		public UserVersionedRepository(QuantumContext context,
			ILogger<UserVersionedRepository<TEntity, TEntityVersion, TUser>> logger)
			: base(context, logger)
		{
			_context = context;
			_logger = logger;
		}

		public void Delete(string id, TUser user)
		{
			var entity = _context.Set<TEntity>()
				.FirstOrDefault(e => e.ReferenceID == id);

			if (entity != null && !entity.IsDeleted && entity.CreatedById == user.Id)
			{
				entity.IsDeleted = true;
				entity.DeletedOn = DateTime.Now;
				entity.DeletedBy = user;
				_context.Entry(entity).State = EntityState.Modified;
			}
		}

		public async Task<bool> DeleteAsync(string id, TUser user)
		{
			var entity = await _context.Set<TEntity>()
				.FirstOrDefaultAsync(e => e.ReferenceID == id);

			if (entity != null && !entity.IsDeleted && entity.CreatedById == user.Id)
			{
				entity.IsDeleted = true;
				entity.DeletedOn = DateTime.Now;
				entity.DeletedBy = user;
				_context.Entry(entity).State = EntityState.Modified;

				return true;
			}

			return false;
		}

		public string Insert(TEntityVersion item, TUser user)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{

					var baseItemId = Helper.Instance.GetID();
					var baseItem = new TEntity();

					baseItem.ReferenceID = baseItemId;
					baseItem.CreatedBy = user;
					baseItem.CreatedDate = DateTime.Now;
					baseItem.LastModifiedBy = user;
					baseItem.LastModified = item.CreatedDate;
					//baseItem.User = user;

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
				}

				return null;
			}
		}

		public async Task<string> InsertAsync(TEntityVersion item, TUser user)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var baseItemId = Helper.Instance.GetID();
					var baseItem = new TEntity();

					baseItem.ReferenceID = baseItemId;
					baseItem.CreatedBy = user;
					baseItem.CreatedDate = DateTime.Now;
					baseItem.LastModifiedBy = user;
					baseItem.LastModified = item.CreatedDate;
					//baseItem.User = user;

					_context.Entry(baseItem).State = EntityState.Added;


					var itemId = Helper.Instance.GetID();
					item.ReferenceID = itemId;
                    item.ID = itemId;
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
				}

				return null;
			}
		}

		public void Update(TEntityVersion item, TUser user)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var entity = _context.Set<TEntity>()
						//.Include(e => e.User)
						.FirstOrDefault(e => e.ReferenceID == item.BaseId);

					if (entity != null && entity.CreatedById == user.Id)
					{
						var entities = _context.Set<TEntityVersion>()
							.Where(v => v.Base.ReferenceID == entity.ReferenceID && !v.IsDeleted)
							.ToList();

						entity.LastModified = DateTime.Now;
						entity.LastModifiedBy = user;
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
				}
			}
		}

		public async Task UpdateAsync(TEntityVersion item, TUser user)
		{
			using (var transaction = _context.Database.BeginTransaction())
			{
				try
				{
					var entity = await _context.Set<TEntity>()
						//.Include(e => e.User)
						.FirstOrDefaultAsync(e => e.ReferenceID == item.BaseId);

					if (entity != null && entity.CreatedById == user.Id)
					{
						var entities = await _context.Set<TEntityVersion>()
						.Where(v => v.Base.ReferenceID == entity.ReferenceID && !v.IsDeleted)
						.ToListAsync();

						entity.LastModified = DateTime.Now;
						entity.LastModifiedBy = user;
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
					throw;
				}
			}
		}
	}
}
