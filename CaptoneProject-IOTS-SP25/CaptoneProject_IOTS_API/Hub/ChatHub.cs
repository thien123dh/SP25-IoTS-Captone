using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = Context.User?.Claims?.FirstOrDefault(c => c.Type == "userId")?.Value;
        Console.WriteLine($"User connected with ID: {userId}");
        return base.OnConnectedAsync();
    }

    public async Task SendMessage(int senderId, int receiverId, string message)
    {
        Console.WriteLine($"Send to {senderId} from {receiverId}: {message}");

        await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, message);
    }


}