namespace InteriorMarketplace.WebApi;

internal static class DevelopmentAuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapDevelopmentAuthenticationEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                "/api/auth/dev-token",
                (DevelopmentTokenRequest request, JwtDevelopmentTokenService tokenService) =>
                    Results.Ok(tokenService.Create(request.Role)))
            .AllowAnonymous();

        return endpoints;
    }
}

internal sealed record DevelopmentTokenRequest(string Role);
