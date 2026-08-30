using InteriorMarketplace.Modules.Projects.Application;
using InteriorMarketplace.Modules.Projects.Domain;
using InteriorMarketplace.Modules.VendorOffers.Application;
using InteriorMarketplace.Modules.VendorOffers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;

public sealed class MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options):DbContext(options)
{
 public DbSet<Project> Projects=>Set<Project>(); public DbSet<ProjectElement> ProjectElements=>Set<ProjectElement>(); public DbSet<Offer> Offers=>Set<Offer>();
 protected override void OnModelCreating(ModelBuilder b){b.HasDefaultSchema("marketplace");b.ApplyConfigurationsFromAssembly(typeof(MarketplaceDbContext).Assembly);}
}
internal sealed class ProjectConfiguration:IEntityTypeConfiguration<Project>
{public void Configure(EntityTypeBuilder<Project>b){b.ToTable("projects");b.HasKey(x=>x.Id);b.Property(x=>x.Id).HasConversion(x=>x.Value,x=>new(x));b.Property(x=>x.Title).HasMaxLength(200);b.Property(x=>x.RoomImageUrl).HasMaxLength(1000);b.HasMany(x=>x.Elements).WithOne().HasForeignKey(x=>x.ProjectId).OnDelete(DeleteBehavior.Cascade);}}
internal sealed class ElementConfiguration:IEntityTypeConfiguration<ProjectElement>
{public void Configure(EntityTypeBuilder<ProjectElement>b){b.ToTable("project_elements");b.HasKey(x=>x.Id);b.Property(x=>x.Id).HasConversion(x=>x.Value,x=>new(x));b.Property(x=>x.ProjectId).HasConversion(x=>x.Value,x=>new(x));b.Property(x=>x.Category).HasMaxLength(100);b.Property(x=>x.Title).HasMaxLength(200);b.ComplexProperty(x=>x.Rectangle);}}
internal sealed class OfferConfiguration:IEntityTypeConfiguration<Offer>
{public void Configure(EntityTypeBuilder<Offer>b){b.ToTable("offers");b.HasKey(x=>x.Id);b.Property(x=>x.Id).HasConversion(x=>x.Value,x=>new(x));b.Property(x=>x.Price).HasPrecision(18,2);b.HasIndex(x=>new{x.VendorId,x.ElementId}).HasFilter("\"Status\" = 0").IsUnique();}}
public sealed class ProjectRepository(MarketplaceDbContext db):IProjectRepository
{public Task<Project?> GetAsync(ProjectId id,CancellationToken ct)=>db.Projects.Include(x=>x.Elements).SingleOrDefaultAsync(x=>x.Id==id,ct);public async Task AddAsync(Project p,CancellationToken ct)=>await db.Projects.AddAsync(p,ct);public async Task SaveChangesAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);public async Task<IReadOnlyList<Project>> ListPublishedAsync(CancellationToken ct)=>await db.Projects.AsNoTracking().Include(x=>x.Elements).Where(x=>x.Status==ProjectStatus.Published).ToListAsync(ct);}
public sealed class OfferRepository(MarketplaceDbContext db):IOfferRepository
{public Task<Offer?> GetAsync(OfferId id,CancellationToken ct)=>db.Offers.SingleOrDefaultAsync(x=>x.Id==id,ct);public async Task AddAsync(Offer o,CancellationToken ct)=>await db.Offers.AddAsync(o,ct);public async Task SaveChangesAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);public Task<bool> HasPendingAsync(Guid v,Guid e,CancellationToken ct)=>db.Offers.AnyAsync(x=>x.VendorId==v&&x.ElementId==e&&x.Status==OfferStatus.Pending,ct);public async Task<IReadOnlyList<Offer>> ListForElementAsync(Guid e,CancellationToken ct)=>await db.Offers.Where(x=>x.ElementId==e).ToListAsync(ct);public async Task<IReadOnlyList<Offer>> ListForVendorAsync(Guid v,CancellationToken ct)=>await db.Offers.Where(x=>x.VendorId==v).ToListAsync(ct);}
