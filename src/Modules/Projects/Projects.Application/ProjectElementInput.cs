using InteriorMarketplace.Modules.Projects.Domain;

namespace InteriorMarketplace.Modules.Projects.Application;

public sealed record ProjectElementInput(
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
    public NormalizedRectangle ToRectangle()
    {
        return new NormalizedRectangle(X, Y, Width, Height);
    }
}
