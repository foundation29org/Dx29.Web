using System;
using System.Collections.Generic;

using IdentityServer4.Models;

namespace Dx29.Web
{
    static class AuthConfiguration
    {
        public static IEnumerable<IdentityResource> IdentityResources
        {
            get
            {
                yield return new IdentityResources.OpenId();
                yield return new IdentityResources.Email();
                yield return new IdentityResource("roles", "User roles", new List<string> { "role" });
            }
        }

        public static IEnumerable<ApiScope> ApiScopes
        {
            get
            {
                yield return new ApiScope("Dx29.Web", "Dx29 Web API");
            }
        }

        public static IEnumerable<ApiResource> ApiResources
        {
            get
            {
                yield return new ApiResource("Dx29.WebAPI", "Dx29 Web API")
                {
                    Scopes = { "Dx29.Web" },
                    UserClaims = { "role", "email", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" }
                };
            }
        }

        public static IEnumerable<Client> Clients
        {
            get
            {
                yield return new Client
                {
                    ClientId = "Dx29",
                    ClientName = "Dx29 Client",
                    ClientSecrets = new[] { new Secret("THESECRET".Sha512()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = { "openid", "roles", "Dx29.Web" }
                };
            }
        }
    }
}
