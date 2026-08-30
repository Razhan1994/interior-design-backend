using InteriorMarketplace.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarketplace(builder.Configuration, builder.Environment);

var app = builder.Build();
app.UseMarketplace();
app.MapMarketplaceEndpoints();

await DatabaseInitializer.InitializeAsync(app.Services);
await app.RunAsync();

public partial class Program;
