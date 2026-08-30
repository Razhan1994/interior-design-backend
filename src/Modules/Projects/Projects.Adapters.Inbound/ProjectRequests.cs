using InteriorMarketplace.Modules.Projects.Application;

namespace InteriorMarketplace.Modules.Projects.Adapters.Inbound;

public sealed record ProjectRequest(string Title, string RoomImageUrl);

public sealed record ProjectElementRequest(
    string Category,
    string Title,
    string? Description,
    string? Dimensions,
    string? Color,
    decimal? TargetBudget,
    decimal X,
    decimal Y,
    decimal Width,
    decimal Height)
{
    public ProjectElementInput ToApplicationInput()
    {
        return new ProjectElementInput(
            Category, Title, Description, Dimensions, Color,
            TargetBudget, X, Y, Width, Height);
    }
}
