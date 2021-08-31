using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRTest.Server.Hubs
{
    //[Authorize]
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            Clients.All.SendAsync("ReceiveMessage", "system", $"{Context.ConnectionId} joined the conversation");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var name = Context.User.Identity.Name;
            Clients.All.SendAsync("ReceiveMessage", "system", $"{Context.ConnectionId} left the conversation");
            await base.OnDisconnectedAsync(exception);
        }
    }
}