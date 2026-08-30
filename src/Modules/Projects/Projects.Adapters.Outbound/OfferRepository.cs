using InteriorMarketplace.Modules.VendorOffers.Application;
using InteriorMarketplace.Modules.VendorOffers.Domain;
using Microsoft.EntityFrameworkCore;

namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;

public sealed class OfferRepository(MarketplaceDbContext dbContext) : IOfferRepository
{
    public Task<Offer?> GetAsync(OfferId offerId, CancellationToken cancellationToken)
    {
        return dbContext.Offers.SingleOrDefaultAsync(
            offer => offer.Id == offerId,
            cancellationToken);
    }

    public async Task AddAsync(Offer offer, CancellationToken cancellationToken)
    {
        await dbContext.Offers.AddAsync(offer, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> HasPendingAsync(
        Guid vendorId,
        Guid elementId,
        CancellationToken cancellationToken)
    {
        return dbContext.Offers.AnyAsync(
            offer => offer.VendorId == vendorId
                && offer.ElementId == elementId
                && offer.Status == OfferStatus.Pending,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Offer>> ListForElementAsync(
        Guid elementId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Offers
            .Where(offer => offer.ElementId == elementId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Offer>> ListForVendorAsync(
        Guid vendorId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Offers
            .Where(offer => offer.VendorId == vendorId)
            .ToListAsync(cancellationToken);
    }
}
