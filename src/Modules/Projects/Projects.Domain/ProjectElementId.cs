namespace InteriorMarketplace.Modules.Projects.Domain;

public readonly record struct ProjectElementId(Guid Value)
{
    public static ProjectElementId New() => new(Guid.NewGuid());
}
