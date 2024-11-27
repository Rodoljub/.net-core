//using HotChocolate.Types;
using Microsoft.Extensions.Configuration;
using Quantum.Data.Entities;
using Quantum.Utility.Extensions;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Core.Services.Graphql
{
    public class GraphFileType //: ObjectType<File>
    {
        private IConfiguration _config;
        private IUtilityService _utilServ;
        public GraphFileType(
            IConfiguration config,
            IUtilityService utilServ
)
        {
            _config = config;
            _utilServ = utilServ;
        }
        //protected override void Configure(IObjectTypeDescriptor<File> descriptor)
        //{
        //    descriptor.Field("userImagePath")
        //        .Type<StringType>()
        //        .Resolver(ctx =>
        //        {
        //        var file = ctx.Parent<File>();
        //        var userProfileImagePath = string.Empty;
        //            if (file != null)
        //            {
        //                userProfileImagePath = file.ID;
        //            }
        //            return userProfileImagePath;
        //        });
        //}
    }
}
