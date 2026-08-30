using InteriorMarketplace.BuildingBlocks.Application;
namespace InteriorMarketplace.Modules.Projects.Adapters.Outbound;
public sealed class LocalImageStorage(string root):IImageStorage
{public async Task<string> SaveAsync(Stream content,string fileName,CancellationToken ct){Directory.CreateDirectory(root);var safe=$"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";await using var target=File.Create(Path.Combine(root,safe));await content.CopyToAsync(target,ct);return $"/images/{safe}";}}
