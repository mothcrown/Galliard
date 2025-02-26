using Galliard.Infrastructure.File;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Galliard.Infrastructure.Novelize;

public class NovelizeHub : Hub
{
    public async Task SaveAudio(string fileName, byte[] contents)
    {
        var fileService = Context.GetHttpContext()?.RequestServices.GetService<FileService>();
        await fileService!.SaveAudio(fileName, contents);
    }
}
