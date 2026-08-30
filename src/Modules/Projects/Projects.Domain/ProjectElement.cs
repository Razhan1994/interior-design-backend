namespace InteriorMarketplace.Modules.Projects.Domain;

public sealed class ProjectElement
{
    private ProjectElement()
    {
    }

    internal ProjectElement(
        ProjectElementId id,
        ProjectId projectId,
        string category,
        string title,
        string? description,
        string? dimensions,
        string? color,
        decimal? targetBudget,
        NormalizedRectangle rectangle)
    {
        Id = id;
        ProjectId = projectId;
        Update(category, title, description, dimensions, color, targetBudget, rectangle);
    }

    public ProjectElementId Id { get; private set; }

    public ProjectId ProjectId { get; private set; }

    public string Category { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? Dimensions { get; private set; }

    public string? Color { get; private set; }

    public decimal? TargetBudget { get; private set; }

    public NormalizedRectangle Rectangle { get; private set; }

    internal void Update(
        string category,
        string title,
        string? description,
        string? dimensions,
        string? color,
        decimal? targetBudget,
        NormalizedRectangle rectangle)
    {
        if (targetBudget < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetBudget));
        }

        Category = Project.GetRequiredText(category, nameof(category));
        Title = Project.GetRequiredText(title, nameof(title));
        Description = description;
        Dimensions = dimensions;
        Color = color;
        TargetBudget = targetBudget;
        Rectangle = rectangle;
    }
}
