namespace InteriorMarketplace.BuildingBlocks.Application;

public readonly record struct Result<T>(T? Value, Error? Error)
{
    public bool IsSuccess => Error is null;

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, null);
    }

    public static Result<T> Failure(string code, string message)
    {
        return new Result<T>(default, new Error(code, message));
    }
}
