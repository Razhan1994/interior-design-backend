namespace InteriorMarketplace.BuildingBlocks.Application;

public interface IImageStorage
{
    Task<string> SaveAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken);
}
