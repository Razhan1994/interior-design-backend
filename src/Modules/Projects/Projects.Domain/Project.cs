namespace InteriorMarketplace.Modules.Projects.Domain;

public sealed class Project
{
    private readonly List<ProjectElement> _elements = [];

    private Project()
    {
    }

    public Project(
        ProjectId id,
        Guid ownerId,
        string title,
        string roomImageUrl,
        DateTime createdAtUtc)
    {
        Id = id;
        OwnerId = ownerId;
        CreatedAtUtc = createdAtUtc;
        Update(title, roomImageUrl);
    }

    public ProjectId Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string RoomImageUrl { get; private set; } = string.Empty;
    public ProjectStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? PublishedAtUtc { get; private set; }
    public IReadOnlyCollection<ProjectElement> Elements => _elements;

    public void EnsureOwner(Guid userId)
    {
        if (OwnerId != userId)
        {
            throw new InvalidOperationException(
                "Only the project owner may perform this operation.");
        }
    }

    public void Update(string title, string roomImageUrl)
    {
        EnsureDraft();
        Title = GetRequiredText(title, nameof(title));
        RoomImageUrl = GetRequiredText(roomImageUrl, nameof(roomImageUrl));
    }

    public ProjectElement AddElement(
        string category,
        string title,
        string? description,
        string? dimensions,
        string? color,
        decimal? targetBudget,
        NormalizedRectangle rectangle)
    {
        EnsureDraft();

        var element = new ProjectElement(
            ProjectElementId.New(), Id, category, title, description,
            dimensions, color, targetBudget, rectangle);

        _elements.Add(element);
        return element;
    }

    public ProjectElement GetElement(ProjectElementId elementId)
    {
        return _elements.SingleOrDefault(element => element.Id == elementId)
            ?? throw new KeyNotFoundException("Element not found.");
    }

    public ProjectElement UpdateElement(
        ProjectElementId elementId,
        string category,
        string title,
        string? description,
        string? dimensions,
        string? color,
        decimal? targetBudget,
        NormalizedRectangle rectangle)
    {
        EnsureDraft();
        var element = GetElement(elementId);
        element.Update(category, title, description, dimensions, color, targetBudget, rectangle);
        return element;
    }

    public void RemoveElement(ProjectElementId elementId)
    {
        EnsureDraft();
        _elements.Remove(GetElement(elementId));
    }

    public void Publish(DateTime publishedAtUtc)
    {
        EnsureDraft();

        if (_elements.Count == 0)
        {
            throw new InvalidOperationException(
                "A project needs at least one element before publishing.");
        }

        Status = ProjectStatus.Published;
        PublishedAtUtc = publishedAtUtc;
    }

    internal static string GetRequiredText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} is required.", parameterName);
        }

        return value.Trim();
    }

    private void EnsureDraft()
    {
        if (Status != ProjectStatus.Draft)
        {
            throw new InvalidOperationException("Published projects cannot be edited.");
        }
    }
}
