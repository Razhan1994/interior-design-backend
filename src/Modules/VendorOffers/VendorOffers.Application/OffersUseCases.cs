using InteriorMarketplace.BuildingBlocks.Application;
using InteriorMarketplace.Modules.Projects.Application;
using InteriorMarketplace.Modules.Projects.Domain;
using InteriorMarketplace.Modules.VendorOffers.Domain;
namespace InteriorMarketplace.Modules.VendorOffers.Application;
public interface IOfferRepository
{
 Task<Offer?> GetAsync(OfferId id,CancellationToken ct); Task AddAsync(Offer offer,CancellationToken ct); Task SaveChangesAsync(CancellationToken ct);
 Task<bool> HasPendingAsync(Guid vendorId,Guid elementId,CancellationToken ct); Task<IReadOnlyList<Offer>> ListForElementAsync(Guid elementId,CancellationToken ct); Task<IReadOnlyList<Offer>> ListForVendorAsync(Guid vendorId,CancellationToken ct);
}
public interface INotificationPort { Task OfferAcceptedAsync(Guid offerId,Guid vendorId,CancellationToken ct); }
public sealed class OfferService(IOfferRepository offers,IProjectRepository projects,ICurrentUser user,IClock clock,INotificationPort notifications)
{
 public async Task<Offer> CreateOffer(Guid projectId,Guid elementId,decimal price,int days,string? note,string? image,CancellationToken ct){RequireVendor();var p=await Published(projectId,ct);if(p.OwnerId==user.UserId)throw new InvalidOperationException("A vendor cannot offer on their own project.");_ = p.GetElement(new(elementId));if(await offers.HasPendingAsync(user.UserId,elementId,ct))throw new InvalidOperationException("Only one pending offer per element is allowed.");var o=new Offer(OfferId.New(),projectId,elementId,user.UserId,price,days,note,image,clock.UtcNow);await offers.AddAsync(o,ct);await offers.SaveChangesAsync(ct);return o;}
 public async Task<Offer> UpdateOwnPendingOffer(Guid id,decimal price,int days,string? note,string? image,CancellationToken ct){var o=await Own(id,ct);o.Update(price,days,note,image);await offers.SaveChangesAsync(ct);return o;}
 public async Task WithdrawOwnOffer(Guid id,CancellationToken ct){var o=await Own(id,ct);o.Withdraw();await offers.SaveChangesAsync(ct);}
 public async Task<IReadOnlyList<Offer>> ListOffersForProjectElement(Guid projectId,Guid elementId,CancellationToken ct){var p=await projects.GetAsync(new(projectId),ct)??throw new KeyNotFoundException();p.EnsureOwner(user.UserId);return await offers.ListForElementAsync(elementId,ct);}
 public Task<IReadOnlyList<Offer>> ListMyVendorOffers(CancellationToken ct){RequireVendor();return offers.ListForVendorAsync(user.UserId,ct);}
 public async Task<Offer> AcceptOffer(Guid projectId,Guid elementId,Guid offerId,CancellationToken ct){var p=await projects.GetAsync(new(projectId),ct)??throw new KeyNotFoundException();p.EnsureOwner(user.UserId);var selected=await offers.GetAsync(new(offerId),ct)??throw new KeyNotFoundException();if(selected.ElementId!=elementId)throw new InvalidOperationException("Offer does not belong to this element.");selected.Accept();foreach(var other in await offers.ListForElementAsync(elementId,ct))if(other.Id!=selected.Id)other.Reject();await offers.SaveChangesAsync(ct);await notifications.OfferAcceptedAsync(selected.Id.Value,selected.VendorId,ct);return selected;}
 private async Task<Project> Published(Guid id,CancellationToken ct){var p=await projects.GetAsync(new(id),ct)??throw new KeyNotFoundException();return p.Status==ProjectStatus.Published?p:throw new InvalidOperationException("Only published projects can receive offers.");}
 private async Task<Offer> Own(Guid id,CancellationToken ct){RequireVendor();var o=await offers.GetAsync(new(id),ct)??throw new KeyNotFoundException();if(o.VendorId!=user.UserId)throw new UnauthorizedAccessException();return o;}
 private void RequireVendor(){if(user.Role is not ("Vendor" or "Admin"))throw new UnauthorizedAccessException("Vendor role is required.");}
}
