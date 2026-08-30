using InteriorMarketplace.Modules.Projects.Adapters.Inbound;
using InteriorMarketplace.Modules.VendorOffers.Adapters.Inbound;

namespace InteriorMarketplace.WebApi;

internal static class ApplicationPipeline
{
    public static WebApplication UseMarketplace(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }

    public static WebApplication MapMarketplaceEndpoints(this WebApplication app)
    {
        app.MapDevelopmentAuthenticationEndpoints();
        app.MapProjectEndpoints();
        app.MapOfferEndpoints();
        return app;
    }
}
