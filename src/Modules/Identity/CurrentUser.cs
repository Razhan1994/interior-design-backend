using System.Security.Claims;
using InteriorMarketplace.BuildingBlocks.Application;
using Microsoft.AspNetCore.Http;
namespace InteriorMarketplace.Modules.Identity;
public static class SeedUsers { public static readonly Guid HomeownerId=Guid.Parse("11111111-1111-1111-1111-111111111111"); public static readonly Guid VendorId=Guid.Parse("22222222-2222-2222-2222-222222222222"); }
public sealed class HttpCurrentUser(IHttpContextAccessor accessor):ICurrentUser
{private ClaimsPrincipal Principal=>accessor.HttpContext?.User??throw new UnauthorizedAccessException();public Guid UserId=>Guid.Parse(Principal.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());public string Role=>Principal.FindFirstValue(ClaimTypes.Role)??throw new UnauthorizedAccessException();}
