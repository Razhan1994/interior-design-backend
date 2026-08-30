namespace InteriorMarketplace.BuildingBlocks.Application;

public interface ICurrentUser
{
    Guid UserId { get; }

    string Role { get; }
}
