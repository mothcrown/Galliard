using Galliard.Application.Common.Interfaces;
using Galliard.Infrastructure.Novelize;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection.Hub;

public class NovelizeHubService(ILogger<NovelizeHubService> logger, IHubContext<NovelizeHub> hubContext) : INovelizeHubService
{
    public async void UpdateProcessStage(string method, string message)
    {
        try
        {
            logger.LogInformation($"Sending {method}: {message}");
            await hubContext.Clients.All.SendAsync(method, message);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
