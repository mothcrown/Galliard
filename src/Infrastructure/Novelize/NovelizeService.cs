using Galliard.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OllamaSharp;

namespace Galliard.Infrastructure.Novelize;

public class NovelizeService(ILogger<TranscriptionService> logger, IConfiguration configuration,
    IFileService fileService) : INovelizeService
{
    public async Task<string> Novelize(string transcriptionFilePath)
    {
        var ollama = new OllamaApiClient(new Uri(configuration.GetValue<string>("Ollama:Url")!));
        ollama.SelectedModel = configuration.GetValue<string>("Ollama:Model")!;

        var prompt =
            "Eres un escritor amigo de un grupo de jugadores de rol que están jugando a una partida ambientada en la antigua Roma, ellos te han dado una transcripción de su última partida y te han pedido por favor que la resumas evitando escenas de violencia gratuitas. Es muy importante ignorar discusiones sobre reglas del juego o comentarios fuera de personaje, no te inventes nada que no aparezca en el texto! Aquí te pasamos la transcripción, responde directamente con su resumen: ";
        
        prompt += await fileService.ReadTranscription(transcriptionFilePath);
        logger.LogInformation("Starting novelization...");
        string novelization = "";
        await foreach (var stream in ollama.GenerateAsync(prompt))
        {
            novelization += stream!.Response;
        }
        
        var fileName = transcriptionFilePath.Split('\\').Last().Split('.').First() + ".txt";
        var filePath = await fileService.SaveNovelization(fileName, novelization);
        logger.LogInformation($"Novelization done: {filePath}");

        return filePath!;
    }
}
