using InteriorMarketplace.Modules.VendorOffers.Application;
using Microsoft.Extensions.Logging;
namespace InteriorMarketplace.Modules.Notifications;

public sealed class LoggingNotificationAdapter(
    ILogger<LoggingNotificationAdapter> logger) : INotificationPort
{
    public Task OfferAcceptedAsync(
        Guid offerId,
        Guid vendorId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Offer {OfferId} accepted; vendor {VendorId} notified",
            offerId,
            vendorId);

        return Task.CompletedTask;
    }
}
