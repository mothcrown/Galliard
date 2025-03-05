using Galliard.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OllamaSharp;

namespace Galliard.Infrastructure.Novelize;

public class NovelizeService(ILogger<TranscriptionService> logger, IConfiguration configuration,
    IFileService fileService) : INovelizeService
{
    private const int MAX_BLOCK_SIZE = 3000;
    
    public async Task<string> Novelize(string transcriptionFilePath)
    {
        var ollama = new OllamaApiClient(new Uri(configuration.GetValue<string>("Ollama:Url")!));
        ollama.SelectedModel = configuration.GetValue<string>("Ollama:Model")!;
        
        var fullTranscription = await fileService.ReadTextFile(transcriptionFilePath);
        List<string> storyBlocks = SplitTranscription(fullTranscription);
        var blockLength = storyBlocks.Count;
        
        logger.LogInformation("Starting novelization...");
        string novelization = "";
        for (int i = 0; i < blockLength; i++)
        {
            await foreach (var stream in ollama.GenerateAsync(storyBlocks[i]))
            {
                novelization += stream!.Response;
            }

            novelization += Environment.NewLine;
            logger.LogInformation($"Story block: {i + 1}/{blockLength}");
        }
        
        var fileName = transcriptionFilePath.Split('\\').Last().Split('.').First() + ".txt";
        var filePath = await fileService.SaveNovelization(fileName, novelization);
        logger.LogInformation($"Novelization done: {filePath}");

        return filePath!;
    }

    private List<string> SplitTranscription(string fullTranscription)
    {
        List<string> result = [];
        int start = 0;

        while (start < fullTranscription.Length)
        {
            int end = Math.Min(start + MAX_BLOCK_SIZE, fullTranscription.Length);
            
            int lastNewline = fullTranscription.LastIndexOf('\n', end - 1, end - start);
            if (lastNewline == -1 || lastNewline <= start)
            {
                lastNewline = end;
            }
            
            result.Add(fullTranscription.Substring(start, lastNewline - start));
            start = lastNewline;
        }
        
        return result;
    }
}
