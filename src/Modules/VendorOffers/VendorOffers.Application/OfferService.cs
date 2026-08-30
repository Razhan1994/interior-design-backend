using InteriorMarketplace.BuildingBlocks.Application;
using InteriorMarketplace.Modules.Projects.Application;
using InteriorMarketplace.Modules.Projects.Domain;
using InteriorMarketplace.Modules.VendorOffers.Domain;

namespace InteriorMarketplace.Modules.VendorOffers.Application;

public sealed class OfferService(
    IOfferRepository offerRepository,
    IProjectRepository projectRepository,
    ICurrentUser currentUser,
    IClock clock,
    INotificationPort notificationPort)
{
    public async Task<Offer> CreateOffer(
        Guid projectId,
        Guid elementId,
        decimal price,
        int deliveryDays,
        string? note,
        string? productImageUrl,
        CancellationToken cancellationToken)
    {
        EnsureCurrentUserIsVendor();
        var project = await GetPublishedProject(projectId, cancellationToken);

        if (project.OwnerId == currentUser.UserId)
        {
            throw new InvalidOperationException(
                "A vendor cannot offer on their own project.");
        }

        _ = project.GetElement(new ProjectElementId(elementId));

        var hasPendingOffer = await offerRepository.HasPendingAsync(
            currentUser.UserId,
            elementId,
            cancellationToken);

        if (hasPendingOffer)
        {
            throw new InvalidOperationException(
                "Only one pending offer per element is allowed.");
        }

        var offer = new Offer(
            OfferId.New(),
            projectId,
            elementId,
            currentUser.UserId,
            price,
            deliveryDays,
            note,
            productImageUrl,
            clock.UtcNow);

        await offerRepository.AddAsync(offer, cancellationToken);
        await offerRepository.SaveChangesAsync(cancellationToken);
        return offer;
    }

    public async Task<Offer> UpdateOwnPendingOffer(
        Guid offerId,
        decimal price,
        int deliveryDays,
        string? note,
        string? productImageUrl,
        CancellationToken cancellationToken)
    {
        var offer = await GetOwnOffer(offerId, cancellationToken);
        offer.Update(price, deliveryDays, note, productImageUrl);
        await offerRepository.SaveChangesAsync(cancellationToken);
        return offer;
    }

    public async Task WithdrawOwnOffer(
        Guid offerId,
        CancellationToken cancellationToken)
    {
        var offer = await GetOwnOffer(offerId, cancellationToken);
        offer.Withdraw();
        await offerRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Offer>> ListOffersForProjectElement(
        Guid projectId,
        Guid elementId,
        CancellationToken cancellationToken)
    {
        var project = await GetProject(projectId, cancellationToken);
        project.EnsureOwner(currentUser.UserId);
        return await offerRepository.ListForElementAsync(elementId, cancellationToken);
    }

    public Task<IReadOnlyList<Offer>> ListMyVendorOffers(
        CancellationToken cancellationToken)
    {
        EnsureCurrentUserIsVendor();
        return offerRepository.ListForVendorAsync(currentUser.UserId, cancellationToken);
    }

    public async Task<Offer> AcceptOffer(
        Guid projectId,
        Guid elementId,
        Guid offerId,
        CancellationToken cancellationToken)
    {
        var project = await GetProject(projectId, cancellationToken);
        project.EnsureOwner(currentUser.UserId);

        var selectedOffer = await offerRepository.GetAsync(
                new OfferId(offerId),
                cancellationToken)
            ?? throw new KeyNotFoundException("Offer not found.");

        if (selectedOffer.ElementId != elementId)
        {
            throw new InvalidOperationException(
                "Offer does not belong to this element.");
        }

        selectedOffer.Accept();
        var elementOffers = await offerRepository.ListForElementAsync(
            elementId,
            cancellationToken);

        foreach (var otherOffer in elementOffers.Where(offer => offer.Id != selectedOffer.Id))
        {
            otherOffer.Reject();
        }

        await offerRepository.SaveChangesAsync(cancellationToken);
        await notificationPort.OfferAcceptedAsync(
            selectedOffer.Id.Value,
            selectedOffer.VendorId,
            cancellationToken);

        return selectedOffer;
    }

    private async Task<Project> GetPublishedProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var project = await GetProject(projectId, cancellationToken);

        if (project.Status != ProjectStatus.Published)
        {
            throw new InvalidOperationException(
                "Only published projects can receive offers.");
        }

        return project;
    }

    private async Task<Project> GetProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        return await projectRepository.GetAsync(
                new ProjectId(projectId),
                cancellationToken)
            ?? throw new KeyNotFoundException("Project not found.");
    }

    private async Task<Offer> GetOwnOffer(
        Guid offerId,
        CancellationToken cancellationToken)
    {
        EnsureCurrentUserIsVendor();

        var offer = await offerRepository.GetAsync(
                new OfferId(offerId),
                cancellationToken)
            ?? throw new KeyNotFoundException("Offer not found.");

        if (offer.VendorId != currentUser.UserId)
        {
            throw new UnauthorizedAccessException(
                "Only the vendor who created an offer may change it.");
        }

        return offer;
    }

    private void EnsureCurrentUserIsVendor()
    {
        if (currentUser.Role is not ("Vendor" or "Admin"))
        {
            throw new UnauthorizedAccessException("Vendor role is required.");
        }
    }
}
