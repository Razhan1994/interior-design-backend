namespace InteriorMarketplace.Modules.VendorOffers.Application;

public interface INotificationPort
{
    Task OfferAcceptedAsync(
        Guid offerId,
        Guid vendorId,
        CancellationToken cancellationToken);
}
