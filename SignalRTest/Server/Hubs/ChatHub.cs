using System.Security.Claims;
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

            //string name = Context.User.Identity.Name;
            //Groups.Add(Context.ConnectionId, name);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var name = Context.User.Identity.Name;
            Clients.All.SendAsync("ReceiveMessage", "system", $"{Context.ConnectionId} left the conversation");
            await base.OnDisconnectedAsync(exception);
        }

        //https://docs.microsoft.com/en-us/aspnet/core/signalr/groups?view=aspnetcore-5.0
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }


    //https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-5.0#use-claims-to-customize-identity-handling
    public class EmailBasedUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}