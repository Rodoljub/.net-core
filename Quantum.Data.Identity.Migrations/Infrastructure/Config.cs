// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace Quantum.Data.Identity.Migrations.Infrastructure
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource(IdentityServerConstants.LocalApi.ScopeName){
                        Scopes = { IdentityServerConstants.LocalApi.ScopeName }
                    }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "angular-client",
                    ClientName = "angular-client",
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    // environment Config
                    RedirectUris = new List<string> {
                        "http://creativecrafts-001-site1.ftempurl.com/signin-callback",
                        "http://creativecrafts-001-site1.ftempurl.com/iframe-signin-callback",
                        "http://creativecrafts-001-site1.ftempurl.com/assets/silent-callback.html"
                    },

                    PostLogoutRedirectUris = { "http://creativecrafts-001-site1.ftempurl.com/signout-callback" },
                    AllowedCorsOrigins =     { "http://creativecrafts-001-site1.ftempurl.com" },
                    // environment Config
                    
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    ////This feature refresh token
                    //AllowOfflineAccess = true,
                    ////Access token life time is 7200 seconds (2 hour)
                    AccessTokenLifetime = 7200,
                    ////Identity token life time is 7200 seconds (2 hour)
                    IdentityTokenLifetime = 7200
                }
            };

        public static IEnumerable<ApiScope> Scopes =>
            new List<ApiScope>
            {
                new ApiScope
                { 
                    Name = IdentityServerConstants.LocalApi.ScopeName
                }
            };
    }
}