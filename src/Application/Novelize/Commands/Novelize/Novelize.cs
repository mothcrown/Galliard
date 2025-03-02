using Galliard.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Galliard.Application.Novelize.Commands.Novelize;

public record NovelizeCommand : IRequest<string>
{
    public string? FileName { get; set; }
    public string? Contents { get; set; }
}

public class NovelizeCommandHandler(
    ILogger<NovelizeCommandHandler> logger,
    IFileService fileService,
    ITranscriptionService transcriptionService,
    INovelizeService novelizeService,
    INovelizeHubService novelizeHubService)
    : IRequestHandler<NovelizeCommand, string>
{
    public Task<string> Handle(NovelizeCommand request, CancellationToken cancellationToken)
    {
        FireAndForget(request);

        // Smiley face
        return Task.FromResult(":D");

    }

    private async void FireAndForget(NovelizeCommand request)
    {
        var filePath = await SaveAudio(request);
        var transcriptedFilePath = await TranscribeAudio(filePath);
        var novelization = await Novelize(transcriptedFilePath);
    }

    private async Task<string?> SaveAudio(NovelizeCommand request)
    {
        string? filePath;
        string hubMessage = "ERROR";
        try
        {
            filePath = await fileService.SaveAudio(request.FileName!,
                Convert.FromBase64String(request.Contents!.Split(',')[1]));
            hubMessage = "OK";
        }
        catch (Exception ex)
        {
            logger.LogError($"Error: {ex.Message}");
            throw;
        }
        finally
        {
            novelizeHubService.UpdateProcessStage("FileSaved", hubMessage);
        }

        return filePath;
    }
    
    private async Task<string?> TranscribeAudio(string? audioFilePath)
    {
        string? transcriptedFilePath;
        string hubMessage = "ERROR";
        try
        {
            transcriptedFilePath = await transcriptionService.Transcribe(audioFilePath!);
            hubMessage = "OK";
        }
        catch (Exception ex)
        {
            logger.LogError($"Error: {ex.Message}");
            throw;
        }
        finally
        {
            novelizeHubService.UpdateProcessStage("AudioTranscribed", hubMessage);
        }

        return transcriptedFilePath;
    }
    
    private async Task<string?> Novelize(string? transcriptedFilePath)
    {
        string? novelizationFilePath;
        string hubMessage = "ERROR";
        try
        {
            novelizationFilePath = await novelizeService.Novelize(transcriptedFilePath!);
            hubMessage = "OK";
        }
        catch (Exception ex)
        {
            logger.LogError($"Error: {ex.Message}");
            throw;
        }
        finally
        {
            novelizeHubService.UpdateProcessStage("TranscriptionNovelized", hubMessage);
        }

        return novelizationFilePath;
    }
}
