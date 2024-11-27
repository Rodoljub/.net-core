//using HotChocolate;
//using HotChocolate.Types;
using Quantum.Data;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum.Core.Services.Graphql
{
    public class ItemsQuery
    {
        private readonly IItemRepository _itemRepo;
        public ItemsQuery(
            IItemRepository itemRepo
        )
        {
            _itemRepo = itemRepo;
        }

        //[UsePaging]
        //public IQueryable<Item> GetItems([Service] QDbContext context) =>
        //       context.Items;
        public IQueryable<Item> Items => _itemRepo.Query(i => !i.IsDeleted);
    }
}
