using System;
using System.Security.Claims;

using Microsoft.AspNetCore.SignalR;

using Dx29.Services;

namespace Dx29.Web.Services
{
    public class NotificationsIdProvider : IUserIdProvider
    {
        public NotificationsIdProvider(AccountHashService accountHashService)
        {
            AccountHashService = accountHashService;
        }

        public AccountHashService AccountHashService { get; }

        public string GetUserId(HubConnectionContext connection)
        {
            return GetUserId(connection.User);
        }

        private string GetUserId(ClaimsPrincipal principal)
        {
            if (principal != null)
            {
                if (principal.Identity.IsAuthenticated)
                {
                    string identifier = principal.FindFirst(ClaimTypes.Email)?.Value;
                    if (identifier != null)
                    {
                        return GetUserIdentifier(identifier);
                    }
                }
            }
            throw new InvalidOperationException("Authentication Error");
        }

        private string GetUserIdentifier(string identifier)
        {
            return AccountHashService.GetHash(identifier);
        }
    }
}
