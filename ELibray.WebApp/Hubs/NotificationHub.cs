using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;

namespace ELibrary.WebApp.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            if (http == null)
            {
                await base.OnConnectedAsync();
                return;
            }

            var readerId = http.Session.GetInt32("readerId");
            var roleId = http.Session.GetInt32("roleId");

           
            if (roleId != null)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    $"role-{roleId}"
                );
            }

            
            if (readerId != null)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    $"reader-{readerId}"
                );
            }

            await base.OnConnectedAsync();
        }
    }
}
