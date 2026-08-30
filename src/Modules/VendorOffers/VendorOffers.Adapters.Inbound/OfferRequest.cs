namespace InteriorMarketplace.Modules.VendorOffers.Adapters.Inbound;

public sealed record OfferRequest(
    decimal Price,
    int DeliveryDays,
    string? Note,
    string? ProductImageUrl);
