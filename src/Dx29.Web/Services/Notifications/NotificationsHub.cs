using System;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Dx29.Web
{
    [Authorize]
    public class NotificationsHub : Hub
    {
    }
}
