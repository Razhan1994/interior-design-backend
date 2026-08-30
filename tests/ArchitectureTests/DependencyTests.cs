using InteriorMarketplace.Modules.Projects.Domain;
using InteriorMarketplace.Modules.VendorOffers.Domain;

namespace ArchitectureTests;

public class DependencyTests
{
    private static readonly System.Reflection.Assembly[] DomainAssemblies =
    [
        typeof(Project).Assembly,
        typeof(Offer).Assembly
    ];

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Microsoft.AspNetCore")]
    [InlineData("Npgsql")]
    [InlineData("System.IdentityModel.Tokens.Jwt")]
    public void Domain_has_no_forbidden_dependency(string forbiddenDependency)
    {
        Assert.All(
            DomainAssemblies,
            assembly => Assert.DoesNotContain(
                assembly.GetReferencedAssemblies(),
                reference => reference.Name!.StartsWith(
                    forbiddenDependency,
                    StringComparison.Ordinal)));
    }

    [Fact]
    public void Projects_domain_references_only_framework_assemblies()
    {
        string[] allowedAssemblies =
        [
            "System.Runtime",
            "System.Collections",
            "System.Linq",
            "System.Private.CoreLib"
        ];

        Assert.All(
            typeof(Project).Assembly.GetReferencedAssemblies(),
            reference => Assert.Contains(reference.Name, allowedAssemblies));
    }
}
