using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gimli.Host;

public sealed class Startup(
    IConfiguration configuration,
    IWebHostEnvironment environment)
{
    public IConfiguration Configuration { get; } = configuration;

    public IWebHostEnvironment Environment { get; } = environment;

    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(WebApplication application)
    {
    }
}
