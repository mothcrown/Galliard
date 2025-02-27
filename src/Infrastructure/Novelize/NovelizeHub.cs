using Microsoft.AspNetCore.SignalR;

namespace Galliard.Infrastructure.Novelize;

public class NovelizeHub : Hub
{
    public async Task FileSaved(string message)
    {
        await Clients.All.SendAsync("FileSaved", message);
    }
}
