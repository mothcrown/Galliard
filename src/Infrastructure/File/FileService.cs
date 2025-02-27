using Galliard.Application.Common.Interfaces;
using Galliard.Infrastructure.Novelize;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Galliard.Infrastructure.File;

public class FileService(ILogger<FileService> logger, IConfiguration configuration, IWebHostEnvironment environment,
    IHubContext<NovelizeHub> hubContext) : IFileService
{
    public async Task<string?> SaveAudio(string fileName, byte[] contents)
    {
        string filePath;
        try
        {
            var audioDir = configuration.GetValue<string>("File:AudiosDir");
            string rootPath = environment.ContentRootPath;
            var uploadPath = Path.Combine(rootPath, audioDir!);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
        
            string uniqueFileName = $"{Guid.NewGuid()}_{DateTime.Now:yyyyMMddHHmmss}_{fileName}";
            filePath = Path.Combine(uploadPath, uniqueFileName);

            await using FileStream fs = new(filePath, FileMode.Create, FileAccess.Write);
            await fs.WriteAsync(contents, 0, contents.Length);
            
            await hubContext.Clients.All.SendAsync("FileSaved", "OK");
            logger.LogInformation($"Audio {uniqueFileName} has been saved to {uploadPath}");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }

        return filePath;
    }
}
