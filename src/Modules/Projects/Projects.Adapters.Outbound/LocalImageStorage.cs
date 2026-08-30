using InteriorMarketplace.BuildingBlocks.Application;

namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;

public sealed class LocalImageStorage(string storageRoot) : IImageStorage
{
    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(storageRoot);

        var storedFileName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        var storedFilePath = Path.Combine(storageRoot, storedFileName);

        await using var targetStream = File.Create(storedFilePath);
        await content.CopyToAsync(targetStream, cancellationToken);

        return $"/images/{storedFileName}";
    }
}
