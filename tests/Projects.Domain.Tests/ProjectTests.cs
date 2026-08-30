using InteriorMarketplace.Modules.Projects.Domain;

namespace Projects.Domain.Tests;

public class ProjectTests
{
    [Fact]
    public void Rectangle_must_stay_inside_image()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new NormalizedRectangle(.8m, .2m, .3m, .2m));
    }

    [Fact]
    public void Empty_project_cannot_be_published()
    {
        var project = CreateProject();
        Assert.Throws<InvalidOperationException>(() => project.Publish(DateTime.UtcNow));
    }

    [Fact]
    public void Project_with_element_can_be_published()
    {
        var project = CreateProject();
        project.AddElement(
            "Sofa", "Sofa", null, null, null, null,
            new NormalizedRectangle(0, 0, 1, 1));

        project.Publish(DateTime.UtcNow);

        Assert.Equal(ProjectStatus.Published, project.Status);
    }

    private static Project CreateProject()
    {
        return new Project(
            ProjectId.New(), Guid.NewGuid(), "Room", "room.jpg", DateTime.UtcNow);
    }
}
