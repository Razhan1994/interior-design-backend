namespace InteriorMarketplace.BuildingBlocks.Application;

public readonly record struct Error(string Code, string Message);
public readonly record struct Result<T>(T? Value, Error? Error)
{
    public bool IsSuccess => Error is null;
    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(string code, string message) => new(default, new(code, message));
}
public interface ICurrentUser { Guid UserId { get; } string Role { get; } }
public interface IClock { DateTime UtcNow { get; } }
public sealed class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }
public interface IImageStorage { Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken); }
public interface IImageGenerationService { Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken); }
