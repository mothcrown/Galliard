using System.Diagnostics;
using Galliard.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Galliard.Infrastructure.Novelize;

public class TranscriptionService(ILogger<TranscriptionService> logger, IConfiguration configuration,
    IHubContext<NovelizeHub> hubContext, IFileService fileService) : ITranscriptionService
{
    public async Task<string> Transcribe(string audioFilePath)
    {
        string filePath;

        try
        {
            var uploadPath = fileService.CreateDirectory(configuration.GetValue<string>("File:TranscriptionsDir")!);
            
            var transcriptionProcess = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = configuration.GetValue<string>("Whisper");
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = $"--language es --output_dir {uploadPath} --output_format srt {audioFilePath}";
            Console.WriteLine($"Starting transcription process: {startInfo.Arguments}");
            transcriptionProcess.StartInfo = startInfo;
            transcriptionProcess.Start();
            await transcriptionProcess.WaitForExitAsync();

            var fileName = audioFilePath.Split('\\').Last().Split('.').First() + ".srt";
            filePath = Path.Combine(uploadPath, fileName);
            await hubContext.Clients.All.SendAsync("AudioTranscribed", "OK");
            logger.LogInformation($"Audio has been transcribed to {filePath}");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }

        return filePath;
    }
}
