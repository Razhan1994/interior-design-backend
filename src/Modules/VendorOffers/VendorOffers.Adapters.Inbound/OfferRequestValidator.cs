using FluentValidation;

namespace InteriorMarketplace.Modules.VendorOffers.Adapters.Inbound;

public sealed class OfferRequestValidator : AbstractValidator<OfferRequest>
{
    public OfferRequestValidator()
    {
        RuleFor(request => request.Price).GreaterThan(0);
        RuleFor(request => request.DeliveryDays).GreaterThan(0);
        RuleFor(request => request.Note).MaximumLength(2000);
    }
}
