namespace InteriorMarketplace.Modules.Projects.Domain;

public readonly record struct ProjectId(Guid Value) { public static ProjectId New() => new(Guid.NewGuid()); }
public readonly record struct ProjectElementId(Guid Value) { public static ProjectElementId New() => new(Guid.NewGuid()); }
public enum ProjectStatus { Draft, Published }

public sealed class Project
{
    private readonly List<ProjectElement> _elements = [];
    private Project() { }
    public Project(ProjectId id, Guid ownerId, string title, string roomImageUrl, DateTime createdAtUtc)
    { Id=id; OwnerId=ownerId; Update(title, roomImageUrl); CreatedAtUtc=createdAtUtc; }
    public ProjectId Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public string Title { get; private set; } = "";
    public string RoomImageUrl { get; private set; } = "";
    public ProjectStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? PublishedAtUtc { get; private set; }
    public IReadOnlyCollection<ProjectElement> Elements => _elements;
    public void EnsureOwner(Guid userId) { if (OwnerId != userId) throw new InvalidOperationException("Only the project owner may perform this operation."); }
    public void Update(string title, string roomImageUrl) { if (Status==ProjectStatus.Published) throw new InvalidOperationException("Published projects cannot be edited."); Title=Required(title,nameof(title)); RoomImageUrl=Required(roomImageUrl,nameof(roomImageUrl)); }
    public ProjectElement AddElement(string category,string title,string? description,string? dimensions,string? color,decimal? targetBudget,NormalizedRectangle rectangle)
    { EnsureDraft(); var element=new ProjectElement(ProjectElementId.New(),Id,category,title,description,dimensions,color,targetBudget,rectangle); _elements.Add(element); return element; }
    public ProjectElement GetElement(ProjectElementId id) => _elements.SingleOrDefault(x=>x.Id==id) ?? throw new KeyNotFoundException("Element not found.");
    public ProjectElement UpdateElement(ProjectElementId id,string category,string title,string? description,string? dimensions,string? color,decimal? targetBudget,NormalizedRectangle rectangle) { EnsureDraft(); var element=GetElement(id); element.Update(category,title,description,dimensions,color,targetBudget,rectangle); return element; }
    public void RemoveElement(ProjectElementId id) { EnsureDraft(); _elements.Remove(GetElement(id)); }
    public void Publish(DateTime utcNow) { EnsureDraft(); if (_elements.Count==0) throw new InvalidOperationException("A project needs at least one element before publishing."); Status=ProjectStatus.Published; PublishedAtUtc=utcNow; }
    private void EnsureDraft() { if(Status!=ProjectStatus.Draft) throw new InvalidOperationException("Published projects cannot be edited."); }
    internal static string Required(string value,string name) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"{name} is required.") : value.Trim();
}

public sealed class ProjectElement
{
    private ProjectElement() { }
    internal ProjectElement(ProjectElementId id,ProjectId projectId,string category,string title,string? description,string? dimensions,string? color,decimal? targetBudget,NormalizedRectangle rectangle)
    { Id=id; ProjectId=projectId; Update(category,title,description,dimensions,color,targetBudget,rectangle); }
    public ProjectElementId Id { get; private set; }
    public ProjectId ProjectId { get; private set; }
    public string Category { get; private set; }=""; public string Title { get; private set; }="";
    public string? Description { get; private set; } public string? Dimensions { get; private set; } public string? Color { get; private set; }
    public decimal? TargetBudget { get; private set; } public NormalizedRectangle Rectangle { get; private set; }
    public void Update(string category,string title,string? description,string? dimensions,string? color,decimal? targetBudget,NormalizedRectangle rectangle)
    { Category=Project.Required(category,nameof(category)); Title=Project.Required(title,nameof(title)); if(targetBudget<0) throw new ArgumentOutOfRangeException(nameof(targetBudget)); Description=description; Dimensions=dimensions; Color=color; TargetBudget=targetBudget; Rectangle=rectangle; }
}

public readonly record struct NormalizedRectangle
{
    public NormalizedRectangle(decimal x,decimal y,decimal width,decimal height)
    { if(x<0||y<0||width<=0||height<=0||x>1||y>1||x+width>1||y+height>1) throw new ArgumentOutOfRangeException(nameof(NormalizedRectangle),"Coordinates must be normalized and remain inside the image."); X=x;Y=y;Width=width;Height=height; }
    public decimal X { get; init; } public decimal Y { get; init; } public decimal Width { get; init; } public decimal Height { get; init; }
}
