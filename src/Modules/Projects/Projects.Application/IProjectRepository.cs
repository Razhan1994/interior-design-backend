using InteriorMarketplace.Modules.Projects.Domain;

namespace InteriorMarketplace.Modules.Projects.Application;

public interface IProjectRepository
{
    Task<Project?> GetAsync(ProjectId projectId, CancellationToken cancellationToken);

    Task AddAsync(Project project, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Project>> ListPublishedAsync(CancellationToken cancellationToken);
}
