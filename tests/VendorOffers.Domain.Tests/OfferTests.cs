using InteriorMarketplace.Modules.VendorOffers.Domain;

namespace VendorOffers.Domain.Tests;

public class OfferTests
{
    [Fact]
    public void Accepted_offer_cannot_be_updated()
    {
        var offer = CreateOffer(price: 100);
        offer.Accept();

        Assert.Throws<InvalidOperationException>(
            () => offer.Update(200, 3, null, null));
    }

    [Fact]
    public void Price_must_be_positive()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOffer(price: 0));
    }

    private static Offer CreateOffer(decimal price)
    {
        return new Offer(
            OfferId.New(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            price, 2, null, null, DateTime.UtcNow);
    }
}
