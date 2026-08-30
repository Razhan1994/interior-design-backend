using InteriorMarketplace.Modules.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;

internal sealed class ProjectElementConfiguration : IEntityTypeConfiguration<ProjectElement>
{
    public void Configure(EntityTypeBuilder<ProjectElement> builder)
    {
        builder.ToTable("project_elements");
        builder.HasKey(element => element.Id);

        builder.Property(element => element.Id)
            .HasConversion(id => id.Value, value => new ProjectElementId(value));
        builder.Property(element => element.ProjectId)
            .HasConversion(id => id.Value, value => new ProjectId(value));
        builder.Property(element => element.Category).HasMaxLength(100);
        builder.Property(element => element.Title).HasMaxLength(200);
        builder.ComplexProperty(element => element.Rectangle);
    }
}
