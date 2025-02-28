using Galliard.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Galliard.Application.Novelize.Commands.Novelize;

public record NovelizeCommand : IRequest<string>
{
    public string? FileName { get; set; }
    public string? Contents { get; set; }
}

public class NovelizeCommandHandler : IRequestHandler<NovelizeCommand, string>
{
    private readonly ILogger<NovelizeCommandHandler> _logger;
    private readonly IFileService _fileService;
    private readonly ITranscriptionService _transcriptionService;
    private readonly INovelizeService _novelizeService;


    public NovelizeCommandHandler(ILogger<NovelizeCommandHandler> logger, IFileService fileService,
        ITranscriptionService transcriptionService, INovelizeService novelizeService)
    {
        _logger = logger;
        _fileService = fileService;
        _transcriptionService = transcriptionService;
        _novelizeService = novelizeService;
    }
    
    public Task<string> Handle(NovelizeCommand request, CancellationToken cancellationToken)
    {
        FireAndForget(request);

        // Smiley face
        return Task.FromResult(":D");

    }

    private async void FireAndForget(NovelizeCommand request)
    {
        var filePath = await _fileService.SaveAudio(request.FileName!, Convert.FromBase64String(request.Contents!.Split(',')[1]));
        _logger.LogInformation($"Audio saved: {filePath}");
        
        // User is safe to disconnect
        var transcriptedFile = await _transcriptionService.Transcribe(filePath!);
        _logger.LogInformation($"Transcription saved: {transcriptedFile}");
        
        // var novelization = await _novelizeService.Novelize(transcriptedFile);
        // _logger.LogInformation($"Novelized text saved: {novelization}");
    }
}
