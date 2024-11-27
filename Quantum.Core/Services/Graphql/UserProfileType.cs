//using HotChocolate.Resolvers;
//using HotChocolate.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Extensions;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Graphql
{
    public class UserProfileType //: ObjectType<UserProfile>
    {
        private IConfiguration _config;
        private IUtilityService _utilServ;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserProfileType(
            IConfiguration config,
            IUtilityService utilServ,
            IServiceScopeFactory serviceScopeFactory
        ) 
        {
            _config = config;
            _utilServ = utilServ;
            _serviceScopeFactory = serviceScopeFactory;
        }
        //protected override void Configure(IObjectTypeDescriptor<UserProfile> descriptor)
        //{
        //    //descriptor.Field("file")
        //    //            .Type<GraphFileType>()
        //    //          //.Name("userProfile")
        //    //          .Resolver(ctx => ctx.Service<IFileRepository>()
        //    //            .GetFileById(ctx.Parent<UserProfile>().ImageFileId));

        //    //descriptor.Field(up => up.)
        //}
    }
}
