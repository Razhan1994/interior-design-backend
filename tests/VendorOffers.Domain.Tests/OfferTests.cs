using InteriorMarketplace.Modules.VendorOffers.Domain;
namespace VendorOffers.Domain.Tests;
public class OfferTests
{[Fact]public void Accepted_offer_cannot_be_updated(){var o=new Offer(OfferId.New(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),100,2,null,null,DateTime.UtcNow);o.Accept();Assert.Throws<InvalidOperationException>(()=>o.Update(200,3,null,null));}[Fact]public void Price_must_be_positive()=>Assert.Throws<ArgumentOutOfRangeException>(()=>new Offer(OfferId.New(),Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),0,1,null,null,DateTime.UtcNow));}
