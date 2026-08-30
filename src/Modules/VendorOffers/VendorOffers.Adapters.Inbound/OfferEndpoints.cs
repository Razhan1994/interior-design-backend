using FluentValidation;
using InteriorMarketplace.Modules.VendorOffers.Application;
using Microsoft.AspNetCore.Builder;using Microsoft.AspNetCore.Http;using Microsoft.AspNetCore.Routing;
namespace InteriorMarketplace.Modules.VendorOffers.Adapters.Inbound;
public sealed record OfferRequest(decimal Price,int DeliveryDays,string? Note,string? ProductImageUrl);
public sealed class OfferRequestValidator:AbstractValidator<OfferRequest>{public OfferRequestValidator(){RuleFor(x=>x.Price).GreaterThan(0);RuleFor(x=>x.DeliveryDays).GreaterThan(0);RuleFor(x=>x.Note).MaximumLength(2000);}}
public static class OfferEndpoints
{public static IEndpointRouteBuilder MapOfferEndpoints(this IEndpointRouteBuilder app){var g=app.MapGroup("/api").RequireAuthorization();
g.MapPost("/projects/{projectId:guid}/elements/{elementId:guid}/offers",async(Guid projectId,Guid elementId,OfferRequest r,IValidator<OfferRequest> v,OfferService s,CancellationToken ct)=>{await v.ValidateAndThrowAsync(r,ct);return Results.Ok(await s.CreateOffer(projectId,elementId,r.Price,r.DeliveryDays,r.Note,r.ProductImageUrl,ct));}).RequireAuthorization(p=>p.RequireRole("Vendor","Admin"));
g.MapPut("/offers/{id:guid}",async(Guid id,OfferRequest r,OfferService s,CancellationToken ct)=>Results.Ok(await s.UpdateOwnPendingOffer(id,r.Price,r.DeliveryDays,r.Note,r.ProductImageUrl,ct)));
g.MapPost("/offers/{id:guid}/withdraw",async(Guid id,OfferService s,CancellationToken ct)=>{await s.WithdrawOwnOffer(id,ct);return Results.NoContent();});
g.MapGet("/projects/{projectId:guid}/elements/{elementId:guid}/offers",async(Guid projectId,Guid elementId,OfferService s,CancellationToken ct)=>Results.Ok(await s.ListOffersForProjectElement(projectId,elementId,ct)));
g.MapGet("/offers/mine",async(OfferService s,CancellationToken ct)=>Results.Ok(await s.ListMyVendorOffers(ct)));
g.MapPost("/projects/{projectId:guid}/elements/{elementId:guid}/offers/{offerId:guid}/accept",async(Guid projectId,Guid elementId,Guid offerId,OfferService s,CancellationToken ct)=>Results.Ok(await s.AcceptOffer(projectId,elementId,offerId,ct)));return app;}}
