namespace InteriorMarketplace.BuildingBlocks.Application;

public interface IClock
{
    DateTime UtcNow { get; }
}
