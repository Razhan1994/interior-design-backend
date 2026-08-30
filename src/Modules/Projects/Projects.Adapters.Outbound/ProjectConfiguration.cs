using InteriorMarketplace.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(project => project.Id);

        builder.Property(project => project.Id)
            .HasConversion(id => id.Value, value => new ProjectId(value));
        builder.Property(project => project.Title).HasMaxLength(200);
        builder.Property(project => project.RoomImageUrl).HasMaxLength(1000);

        builder.HasMany(project => project.Elements)
            .WithOne()
            .HasForeignKey(element => element.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
