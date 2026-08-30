using FluentValidation;
using InteriorMarketplace.Modules.Projects.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace InteriorMarketplace.Modules.Projects.Adapters.Inbound;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var projects = endpoints.MapGroup("/api/projects").RequireAuthorization();

        projects.MapPost("/", CreateProject)
            .RequireAuthorization(policy => policy.RequireRole("Homeowner", "Admin"));
        projects.MapPut("/{projectId:guid}", UpdateProject);
        projects.MapPost("/{projectId:guid}/elements", AddProjectElement);
        projects.MapPut("/{projectId:guid}/elements/{elementId:guid}", UpdateProjectElement);
        projects.MapDelete("/{projectId:guid}/elements/{elementId:guid}", RemoveProjectElement);
        projects.MapPost("/{projectId:guid}/publish", PublishProject);
        projects.MapGet("/{projectId:guid}/owner", GetProjectForOwner);
        projects.MapGet("/{projectId:guid}/public", GetPublicProject).AllowAnonymous();
        projects.MapGet("/published", ListPublishedProjects).AllowAnonymous();

        return endpoints;
    }

    private static async Task<IResult> CreateProject(
        ProjectRequest request,
        IValidator<ProjectRequest> validator,
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var project = await projectService.CreateProject(
            request.Title, request.RoomImageUrl, cancellationToken);
        return Results.Created($"/api/projects/{project.Id.Value}/owner", project);
    }

    private static async Task<IResult> UpdateProject(
        Guid projectId,
        ProjectRequest request,
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var project = await projectService.UpdateProject(
            projectId, request.Title, request.RoomImageUrl, cancellationToken);
        return Results.Ok(project);
    }

    private static async Task<IResult> AddProjectElement(
        Guid projectId,
        ProjectElementRequest request,
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var element = await projectService.AddProjectElement(
            projectId, request.ToApplicationInput(), cancellationToken);
        return Results.Ok(element);
    }

    private static async Task<IResult> UpdateProjectElement(
        Guid projectId,
        Guid elementId,
        ProjectElementRequest request,
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var element = await projectService.UpdateProjectElement(
            projectId, elementId, request.ToApplicationInput(), cancellationToken);
        return Results.Ok(element);
    }

    private static async Task<IResult> RemoveProjectElement(
        Guid projectId,
        Guid elementId,
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        await projectService.RemoveProjectElement(projectId, elementId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> PublishProject(
        Guid projectId,
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var project = await projectService.PublishProject(projectId, cancellationToken);
        return Results.Ok(project);
    }

    private static async Task<IResult> GetProjectForOwner(
        Guid projectId,
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var project = await projectService.GetProjectForOwner(projectId, cancellationToken);
        return Results.Ok(project);
    }

    private static async Task<IResult> GetPublicProject(
        Guid projectId,
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var project = await projectService.GetPublicProject(projectId, cancellationToken);
        return Results.Ok(project);
    }

    private static async Task<IResult> ListPublishedProjects(
        ProjectService projectService,
        CancellationToken cancellationToken)
    {
        var projects = await projectService.ListPublishedProjects(cancellationToken);
        return Results.Ok(projects);
    }
}
