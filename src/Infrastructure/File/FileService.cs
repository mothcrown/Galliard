using Galliard.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Galliard.Infrastructure.File;

public class FileService(ILogger<FileService> logger, IConfiguration configuration, IWebHostEnvironment environment) : IFileService
{
    public async Task<string?> SaveAudio(string fileName, byte[] contents)
    {
        string filePath;
        try
        {
            var uploadPath = CreateDirectory(configuration.GetValue<string>("File:AudiosDir")!);
            string uniqueFileName = $"{Guid.NewGuid()}_{DateTime.Now:yyyyMMddHHmmss}_{fileName}";
            filePath = Path.Combine(uploadPath, uniqueFileName);

            await using FileStream fs = new(filePath, FileMode.Create, FileAccess.Write);
            await fs.WriteAsync(contents, 0, contents.Length);
            
            logger.LogInformation($"Audio {uniqueFileName} has been saved to {uploadPath}");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }

        return filePath;
    }

    public async Task<string?> SaveNovelization(string fileName, string novelization)
    {
        string filePath;
        try
        {
            var uploadPath = CreateDirectory(configuration.GetValue<string>("File:NovelizationsDir")!);
            filePath = Path.Combine(uploadPath, fileName);
            await System.IO.File.WriteAllTextAsync(filePath, novelization);
            logger.LogInformation($"Novelization saved to {filePath}");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }

        return filePath;
    }

    public string CreateDirectory(string dir)
    {
        string rootPath = environment.ContentRootPath;
        var uploadPath = Path.Combine(rootPath, dir);

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        
        return uploadPath;
    }

    public async Task<string> ReadTranscription(string transcriptionFilePath)
    {
        return await System.IO.File.ReadAllTextAsync(transcriptionFilePath);
    }
}
