namespace InteriorMarketplace.Modules.VendorOffers.Domain;

public readonly record struct OfferId(Guid Value)
{
    public static OfferId New() => new(Guid.NewGuid());
}
