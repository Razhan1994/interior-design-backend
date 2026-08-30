using InteriorMarketplace.Modules.VendorOffers.Domain;

namespace InteriorMarketplace.Modules.VendorOffers.Application;

public interface IOfferRepository
{
    Task<Offer?> GetAsync(OfferId offerId, CancellationToken cancellationToken);

    Task AddAsync(Offer offer, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> HasPendingAsync(
        Guid vendorId,
        Guid elementId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Offer>> ListForElementAsync(
        Guid elementId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Offer>> ListForVendorAsync(
        Guid vendorId,
        CancellationToken cancellationToken);
}
