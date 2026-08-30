using FluentValidation;
using InteriorMarketplace.Modules.Projects.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
namespace InteriorMarketplace.Modules.Projects.Adapters.Inbound;
public sealed record ProjectRequest(string Title,string RoomImageUrl);
public sealed record ElementRequest(string Category,string Title,string? Description,string? Dimensions,string? Color,decimal? TargetBudget,decimal X,decimal Y,decimal Width,decimal Height)
{public ElementInput ToInput()=>new(Category,Title,Description,Dimensions,Color,TargetBudget,X,Y,Width,Height);}
public sealed class ProjectRequestValidator:AbstractValidator<ProjectRequest>{public ProjectRequestValidator(){RuleFor(x=>x.Title).NotEmpty().MaximumLength(200);RuleFor(x=>x.RoomImageUrl).NotEmpty().MaximumLength(1000);}}
public static class ProjectEndpoints
{
 public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app){var g=app.MapGroup("/api/projects").RequireAuthorization();
 g.MapPost("/",async(ProjectRequest r,IValidator<ProjectRequest> v,ProjectService s,CancellationToken ct)=>{await v.ValidateAndThrowAsync(r,ct);return Results.Created("",await s.CreateProject(r.Title,r.RoomImageUrl,ct));}).RequireAuthorization(p=>p.RequireRole("Homeowner","Admin"));
 g.MapPut("/{id:guid}",async(Guid id,ProjectRequest r,ProjectService s,CancellationToken ct)=>Results.Ok(await s.UpdateProject(id,r.Title,r.RoomImageUrl,ct)));
 g.MapPost("/{id:guid}/elements",async(Guid id,ElementRequest r,ProjectService s,CancellationToken ct)=>Results.Ok(await s.AddProjectElement(id,r.ToInput(),ct)));
 g.MapPut("/{id:guid}/elements/{elementId:guid}",async(Guid id,Guid elementId,ElementRequest r,ProjectService s,CancellationToken ct)=>Results.Ok(await s.UpdateProjectElement(id,elementId,r.ToInput(),ct)));
 g.MapDelete("/{id:guid}/elements/{elementId:guid}",async(Guid id,Guid elementId,ProjectService s,CancellationToken ct)=>{await s.RemoveProjectElement(id,elementId,ct);return Results.NoContent();});
 g.MapPost("/{id:guid}/publish",async(Guid id,ProjectService s,CancellationToken ct)=>Results.Ok(await s.PublishProject(id,ct)));
 g.MapGet("/{id:guid}/owner",async(Guid id,ProjectService s,CancellationToken ct)=>Results.Ok(await s.GetProjectForOwner(id,ct)));
 g.MapGet("/{id:guid}/public",async(Guid id,ProjectService s,CancellationToken ct)=>Results.Ok(await s.GetPublicProject(id,ct))).AllowAnonymous();
 g.MapGet("/published",async(ProjectService s,CancellationToken ct)=>Results.Ok(await s.ListPublishedProjects(ct))).AllowAnonymous();return app;}
}
