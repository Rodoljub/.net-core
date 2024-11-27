//using HotChocolate.Types;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Core.Services.Graphql
{
    public class ItemType //: ObjectType<Item>
    {
        
        //protected override void Configure(IObjectTypeDescriptor<Item> descriptor)
        //{
        //    descriptor.Field("userProfile")
        //                .Type<UserProfileType>()
        //              //.Name("userProfile")
        //              .Resolver(ctx => ctx.Service<IUserProfileRepository>()
        //              .GetByUserId(ctx.Parent<Item>().CreatedById));
        //    //descriptor.Field(b => b.Id).Type<IdType>();
        //    //descriptor.Field(b => b.Title).Type<StringType>();
        //    //descriptor.Field(b => b.Price).Type<DecimalType>();
        //    //descriptor.Field<AuthorResolver>(t => t.GetAuthor(default, default));
        //}
    }
}
