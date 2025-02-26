namespace Galliard.Application.Common.Interfaces;

public interface ITranscriptionService
{
    public Task<string> Transcribe(string audioFilePath);
}
