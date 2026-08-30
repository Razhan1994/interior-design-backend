using InteriorMarketplace.Modules.Identity;
using InteriorMarketplace.Modules.Projects.Adapters.Outbound;
using InteriorMarketplace.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;

namespace InteriorMarketplace.WebApi;

internal static class DatabaseInitializer
{
    private static readonly ProjectId SampleProjectId = new(
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();

        await dbContext.Database.MigrateAsync();

        if (await dbContext.Projects.AnyAsync())
        {
            return;
        }

        dbContext.Projects.Add(CreateSampleProject());
        await dbContext.SaveChangesAsync();
    }

    private static Project CreateSampleProject()
    {
        var utcNow = DateTime.UtcNow;
        var project = new Project(
            SampleProjectId,
            SeedUsers.HomeownerId,
            "نشیمن مدرن نمونه",
            "https://images.example.local/sample-living-room.jpg",
            utcNow);

        project.AddElement(
            "Sofa", "مبل سه‌نفره", "پارچه‌ای", "220×90 cm", "کرم",
            45_000_000, new NormalizedRectangle(.10m, .45m, .42m, .35m));
        project.AddElement(
            "Rug", "فرش مدرن", null, "300×200 cm", "طوسی",
            18_000_000, new NormalizedRectangle(.20m, .72m, .55m, .20m));
        project.AddElement(
            "FloorLamp", "آباژور ایستاده", null, "160 cm", "مشکی",
            8_000_000, new NormalizedRectangle(.78m, .25m, .12m, .55m));
        project.Publish(utcNow);

        return project;
    }
}
