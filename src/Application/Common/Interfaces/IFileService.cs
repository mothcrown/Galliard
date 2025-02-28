namespace Galliard.Application.Common.Interfaces;

public interface IFileService
{
    public Task<string?> SaveAudio(string fileName, byte[] contents);
    public string CreateDirectory(string dir);
}
