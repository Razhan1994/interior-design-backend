using InteriorMarketplace.BuildingBlocks.Application;
using InteriorMarketplace.Modules.Projects.Application;
using InteriorMarketplace.Modules.Projects.Domain;

namespace Projects.Application.Tests;

public class ProjectServiceTests
{
    [Fact]
    public async Task Owner_can_create_project()
    {
        var repository = new InMemoryProjectRepository();
        var currentUser = new HomeownerCurrentUser(Guid.NewGuid());
        var service = new ProjectService(repository, currentUser, new FixedClock());

        var project = await service.CreateProject("Room", "room.jpg", default);

        Assert.Equal(currentUser.UserId, project.OwnerId);
    }

    private sealed class HomeownerCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId => userId;
        public string Role => "Homeowner";
    }

    private sealed class FixedClock : IClock
    {
        public DateTime UtcNow => DateTime.UnixEpoch;
    }

    private sealed class InMemoryProjectRepository : IProjectRepository
    {
        public List<Project> Projects { get; } = [];

        public Task AddAsync(Project project, CancellationToken cancellationToken)
        {
            Projects.Add(project);
            return Task.CompletedTask;
        }

        public Task<Project?> GetAsync(
            ProjectId projectId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                Projects.SingleOrDefault(project => project.Id == projectId));
        }

        public Task<IReadOnlyList<Project>> ListPublishedAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Project>>([]);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
