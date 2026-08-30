using System.Reflection;using InteriorMarketplace.Modules.Projects.Domain;using InteriorMarketplace.Modules.VendorOffers.Domain;
namespace ArchitectureTests;
public class DependencyTests
{[Theory][InlineData("Microsoft.EntityFrameworkCore")][InlineData("Microsoft.AspNetCore")][InlineData("Npgsql")][InlineData("System.IdentityModel.Tokens.Jwt")]
 public void Domain_has_no_forbidden_dependency(string forbidden){var assemblies=new[]{typeof(Project).Assembly,typeof(Offer).Assembly};Assert.All(assemblies,a=>Assert.DoesNotContain(a.GetReferencedAssemblies(),r=>r.Name!.StartsWith(forbidden,StringComparison.Ordinal)));}
 [Fact]public void Domain_references_only_framework(){Assert.All(typeof(Project).Assembly.GetReferencedAssemblies(),r=>Assert.True(r.Name is "System.Runtime" or "System.Collections" or "System.Linq" or "System.Private.CoreLib",$"Unexpected dependency: {r.Name}"));}}
