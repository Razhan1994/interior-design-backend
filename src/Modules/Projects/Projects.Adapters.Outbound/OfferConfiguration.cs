using InteriorMarketplace.Modules.VendorOffers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;

internal sealed class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("offers");
        builder.HasKey(offer => offer.Id);

        builder.Property(offer => offer.Id)
            .HasConversion(id => id.Value, value => new OfferId(value));
        builder.Property(offer => offer.Price).HasPrecision(18, 2);

        builder.HasIndex(offer => new { offer.VendorId, offer.ElementId })
            .HasFilter("\"Status\" = 0")
            .IsUnique();
    }
}
