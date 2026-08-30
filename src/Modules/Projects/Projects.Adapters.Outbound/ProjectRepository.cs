using InteriorMarketplace.Modules.Projects.Application;
using InteriorMarketplace.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;

public sealed class ProjectRepository(MarketplaceDbContext dbContext) : IProjectRepository
{
    public Task<Project?> GetAsync(
        ProjectId projectId,
        CancellationToken cancellationToken)
    {
        return dbContext.Projects
            .Include(project => project.Elements)
            .SingleOrDefaultAsync(project => project.Id == projectId, cancellationToken);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken)
    {
        await dbContext.Projects.AddAsync(project, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> ListPublishedAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Elements)
            .Where(project => project.Status == ProjectStatus.Published)
            .ToListAsync(cancellationToken);
    }
}
