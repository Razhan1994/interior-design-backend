using InteriorMarketplace.Modules.Projects.Domain;
using InteriorMarketplace.Modules.VendorOffers.Domain;
using Microsoft.EntityFrameworkCore;

namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;

public sealed class MarketplaceDbContext(
    DbContextOptions<MarketplaceDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectElement> ProjectElements => Set<ProjectElement>();

    public DbSet<Offer> Offers => Set<Offer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("marketplace");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketplaceDbContext).Assembly);
    }
}
