namespace Galliard.Application.Common.Interfaces;

public interface INovelizeService
{
    public Task<string> Novelize(string transcriptionFilePath);
}
