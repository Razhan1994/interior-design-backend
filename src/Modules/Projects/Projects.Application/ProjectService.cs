using InteriorMarketplace.BuildingBlocks.Application;
using InteriorMarketplace.Modules.Projects.Domain;

namespace InteriorMarketplace.Modules.Projects.Application;

public sealed class ProjectService(
    IProjectRepository projectRepository,
    ICurrentUser currentUser,
    IClock clock)
{
    public async Task<Project> CreateProject(
        string title,
        string roomImageUrl,
        CancellationToken cancellationToken)
    {
        EnsureCurrentUserIsHomeowner();

        var project = new Project(
            ProjectId.New(),
            currentUser.UserId,
            title,
            roomImageUrl,
            clock.UtcNow);

        await projectRepository.AddAsync(project, cancellationToken);
        await projectRepository.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<Project> UpdateProject(
        Guid projectId,
        string title,
        string roomImageUrl,
        CancellationToken cancellationToken)
    {
        var project = await GetOwnedProject(projectId, cancellationToken);
        project.Update(title, roomImageUrl);
        await projectRepository.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task<ProjectElement> AddProjectElement(
        Guid projectId,
        ProjectElementInput input,
        CancellationToken cancellationToken)
    {
        var project = await GetOwnedProject(projectId, cancellationToken);
        var element = project.AddElement(
            input.Category,
            input.Title,
            input.Description,
            input.Dimensions,
            input.Color,
            input.TargetBudget,
            input.ToRectangle());

        await projectRepository.SaveChangesAsync(cancellationToken);
        return element;
    }

    public async Task<ProjectElement> UpdateProjectElement(
        Guid projectId,
        Guid elementId,
        ProjectElementInput input,
        CancellationToken cancellationToken)
    {
        var project = await GetOwnedProject(projectId, cancellationToken);
        var element = project.UpdateElement(
            new ProjectElementId(elementId),
            input.Category,
            input.Title,
            input.Description,
            input.Dimensions,
            input.Color,
            input.TargetBudget,
            input.ToRectangle());

        await projectRepository.SaveChangesAsync(cancellationToken);
        return element;
    }

    public async Task RemoveProjectElement(
        Guid projectId,
        Guid elementId,
        CancellationToken cancellationToken)
    {
        var project = await GetOwnedProject(projectId, cancellationToken);
        project.RemoveElement(new ProjectElementId(elementId));
        await projectRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<Project> PublishProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var project = await GetOwnedProject(projectId, cancellationToken);
        project.Publish(clock.UtcNow);
        await projectRepository.SaveChangesAsync(cancellationToken);
        return project;
    }

    public Task<Project> GetProjectForOwner(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        return GetOwnedProject(projectId, cancellationToken);
    }

    public async Task<Project> GetPublicProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var project = await GetProject(projectId, cancellationToken);

        if (project.Status != ProjectStatus.Published)
        {
            throw new KeyNotFoundException("Published project not found.");
        }

        return project;
    }

    public Task<IReadOnlyList<Project>> ListPublishedProjects(
        CancellationToken cancellationToken)
    {
        return projectRepository.ListPublishedAsync(cancellationToken);
    }

    private async Task<Project> GetOwnedProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        EnsureCurrentUserIsHomeowner();
        var project = await GetProject(projectId, cancellationToken);
        project.EnsureOwner(currentUser.UserId);
        return project;
    }

    private async Task<Project> GetProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        return await projectRepository.GetAsync(
                new ProjectId(projectId),
                cancellationToken)
            ?? throw new KeyNotFoundException("Project not found.");
    }

    private void EnsureCurrentUserIsHomeowner()
    {
        if (currentUser.Role is not ("Homeowner" or "Admin"))
        {
            throw new UnauthorizedAccessException("Homeowner role is required.");
        }
    }
}
