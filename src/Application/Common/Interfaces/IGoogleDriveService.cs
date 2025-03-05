namespace Galliard.Application.Common.Interfaces;

public interface IGoogleDriveService
{
    public Task<string> UploadNovelization(string filePath);
}
