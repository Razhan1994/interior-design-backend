using InteriorMarketplace.BuildingBlocks.Application;
using InteriorMarketplace.Modules.Projects.Domain;

namespace InteriorMarketplace.Modules.Projects.Application;
public interface IProjectRepository
{
 Task<Project?> GetAsync(ProjectId id,CancellationToken ct); Task AddAsync(Project project,CancellationToken ct); Task SaveChangesAsync(CancellationToken ct);
 Task<IReadOnlyList<Project>> ListPublishedAsync(CancellationToken ct);
}
public sealed record ElementInput(string Category,string Title,string? Description,string? Dimensions,string? Color,decimal? TargetBudget,decimal X,decimal Y,decimal Width,decimal Height);
public sealed class ProjectService(IProjectRepository repository,ICurrentUser currentUser,IClock clock)
{
 public async Task<Project> CreateProject(string title,string imageUrl,CancellationToken ct){RequireHomeowner();var p=new Project(ProjectId.New(),currentUser.UserId,title,imageUrl,clock.UtcNow);await repository.AddAsync(p,ct);await repository.SaveChangesAsync(ct);return p;}
 public async Task<Project> UpdateProject(Guid id,string title,string imageUrl,CancellationToken ct){var p=await Owned(id,ct);p.Update(title,imageUrl);await repository.SaveChangesAsync(ct);return p;}
 public async Task<ProjectElement> AddProjectElement(Guid id,ElementInput i,CancellationToken ct){var p=await Owned(id,ct);var e=p.AddElement(i.Category,i.Title,i.Description,i.Dimensions,i.Color,i.TargetBudget,Rect(i));await repository.SaveChangesAsync(ct);return e;}
 public async Task<ProjectElement> UpdateProjectElement(Guid id,Guid elementId,ElementInput i,CancellationToken ct){var p=await Owned(id,ct);var e=p.UpdateElement(new(elementId),i.Category,i.Title,i.Description,i.Dimensions,i.Color,i.TargetBudget,Rect(i));await repository.SaveChangesAsync(ct);return e;}
 public async Task RemoveProjectElement(Guid id,Guid elementId,CancellationToken ct){var p=await Owned(id,ct);p.RemoveElement(new(elementId));await repository.SaveChangesAsync(ct);}
 public async Task<Project> PublishProject(Guid id,CancellationToken ct){var p=await Owned(id,ct);p.Publish(clock.UtcNow);await repository.SaveChangesAsync(ct);return p;}
 public Task<Project> GetProjectForOwner(Guid id,CancellationToken ct)=>Owned(id,ct);
 public async Task<Project> GetPublicProject(Guid id,CancellationToken ct){var p=await Find(id,ct);return p.Status==ProjectStatus.Published?p:throw new KeyNotFoundException("Published project not found.");}
 public Task<IReadOnlyList<Project>> ListPublishedProjects(CancellationToken ct)=>repository.ListPublishedAsync(ct);
 private async Task<Project> Owned(Guid id,CancellationToken ct){RequireHomeowner();var p=await Find(id,ct);p.EnsureOwner(currentUser.UserId);return p;}
 private async Task<Project> Find(Guid id,CancellationToken ct)=>await repository.GetAsync(new(id),ct)??throw new KeyNotFoundException("Project not found.");
 private void RequireHomeowner(){if(currentUser.Role is not ("Homeowner" or "Admin"))throw new UnauthorizedAccessException("Homeowner role is required.");}
 private static NormalizedRectangle Rect(ElementInput i)=>new(i.X,i.Y,i.Width,i.Height);
}
