using FluentValidation;

namespace InteriorMarketplace.Modules.Projects.Adapters.Inbound;

public sealed class ProjectRequestValidator : AbstractValidator<ProjectRequest>
{
    public ProjectRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().MaximumLength(200);
        RuleFor(request => request.RoomImageUrl).NotEmpty().MaximumLength(1000);
    }
}
