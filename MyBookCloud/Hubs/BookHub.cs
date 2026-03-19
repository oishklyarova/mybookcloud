using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MyBookCloud.Hubs
{
    [Authorize]
    public class BookHub : Hub
    {
        private readonly ILogger<BookHub> _logger;

        public BookHub(ILogger<BookHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("SignalR connected. UserIdentifier: {userIdentifier}", Context.UserIdentifier);
            return base.OnConnectedAsync();
        }
    }
}

