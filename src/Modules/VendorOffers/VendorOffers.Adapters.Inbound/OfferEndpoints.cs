using FluentValidation;
using InteriorMarketplace.Modules.VendorOffers.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InteriorMarketplace.Modules.VendorOffers.Adapters.Inbound;

public static class OfferEndpoints
{
    public static IEndpointRouteBuilder MapOfferEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api").RequireAuthorization();

        api.MapPost(
                "/projects/{projectId:guid}/elements/{elementId:guid}/offers",
                CreateOffer)
            .RequireAuthorization(policy => policy.RequireRole("Vendor", "Admin"));
        api.MapPut("/offers/{offerId:guid}", UpdateOffer);
        api.MapPost("/offers/{offerId:guid}/withdraw", WithdrawOffer);
        api.MapGet(
            "/projects/{projectId:guid}/elements/{elementId:guid}/offers",
            ListElementOffers);
        api.MapGet("/offers/mine", ListMyOffers);
        api.MapPost(
            "/projects/{projectId:guid}/elements/{elementId:guid}/offers/{offerId:guid}/accept",
            AcceptOffer);

        return endpoints;
    }

    private static async Task<IResult> CreateOffer(
        Guid projectId,
        Guid elementId,
        OfferRequest request,
        IValidator<OfferRequest> validator,
        OfferService offerService,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var offer = await offerService.CreateOffer(
            projectId,
            elementId,
            request.Price,
            request.DeliveryDays,
            request.Note,
            request.ProductImageUrl,
            cancellationToken);
        return Results.Ok(offer);
    }

    private static async Task<IResult> UpdateOffer(
        Guid offerId,
        OfferRequest request,
        OfferService offerService,
        CancellationToken cancellationToken)
    {
        var offer = await offerService.UpdateOwnPendingOffer(
            offerId,
            request.Price,
            request.DeliveryDays,
            request.Note,
            request.ProductImageUrl,
            cancellationToken);
        return Results.Ok(offer);
    }

    private static async Task<IResult> WithdrawOffer(
        Guid offerId,
        OfferService offerService,
        CancellationToken cancellationToken)
    {
        await offerService.WithdrawOwnOffer(offerId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ListElementOffers(
        Guid projectId,
        Guid elementId,
        OfferService offerService,
        CancellationToken cancellationToken)
    {
        var offers = await offerService.ListOffersForProjectElement(
            projectId,
            elementId,
            cancellationToken);
        return Results.Ok(offers);
    }

    private static async Task<IResult> ListMyOffers(
        OfferService offerService,
        CancellationToken cancellationToken)
    {
        var offers = await offerService.ListMyVendorOffers(cancellationToken);
        return Results.Ok(offers);
    }

    private static async Task<IResult> AcceptOffer(
        Guid projectId,
        Guid elementId,
        Guid offerId,
        OfferService offerService,
        CancellationToken cancellationToken)
    {
        var offer = await offerService.AcceptOffer(
            projectId,
            elementId,
            offerId,
            cancellationToken);
        return Results.Ok(offer);
    }
}
