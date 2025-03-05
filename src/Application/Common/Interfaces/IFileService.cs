namespace Galliard.Application.Common.Interfaces;

public interface IFileService
{
    public Task<string?> SaveAudio(string fileName, byte[] contents);
    public Task<string?> SaveNovelization(string fileName, string novelization);
    public string CreateDirectory(string dir);
    public Task<string> ReadTextFile(string transcriptionFilePath);
}
