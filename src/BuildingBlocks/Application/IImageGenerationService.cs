namespace InteriorMarketplace.BuildingBlocks.Application;

public interface IImageGenerationService
{
    Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken);
}
