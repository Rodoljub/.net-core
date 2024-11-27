using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common;
using Quantum.Data.Repositories.Contracts;

namespace Quantum.Data.Repositories
{
    public class ViewRepository : BaseRepository<View>, IViewRepository
	{
		private QDbContext _context;

		public ViewRepository(QDbContext context) 
			: base(context)
		{
			_context = context;
		}

		//public async Task AddItemView(string itemId, IdentityUser user, string ipAddress = null)
		//{
		//	var viewItem = await Query(vi => !vi.IsDeleted && vi.ID == itemId && vi.UserId == user.Id)
  //              .FirstOrDefaultAsync();

		//	if (viewItem == null)
		//	{
		//		var view = new View()
		//		{
		//			ID = itemId,
		//			UserId = user.Id,
		//			IPAddress = ipAddress
		//		};

		//		await base.Insert(view, user);
		//	}
		//}


	}
}
