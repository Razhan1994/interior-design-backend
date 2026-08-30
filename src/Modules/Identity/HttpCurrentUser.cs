using System.Security.Claims;
using InteriorMarketplace.BuildingBlocks.Application;
using Microsoft.AspNetCore.Http;

namespace InteriorMarketplace.Modules.Identity;

public sealed class HttpCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var userId = Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User identifier claim is missing.");

            return Guid.Parse(userId);
        }
    }

    public string Role => Principal.FindFirstValue(ClaimTypes.Role)
        ?? throw new UnauthorizedAccessException("User role claim is missing.");

    private ClaimsPrincipal Principal => httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("The current HTTP context is unavailable.");
}
