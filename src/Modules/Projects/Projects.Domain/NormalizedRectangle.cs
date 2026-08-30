namespace InteriorMarketplace.Modules.Projects.Domain;

public readonly record struct NormalizedRectangle
{
    public NormalizedRectangle(decimal x, decimal y, decimal width, decimal height)
    {
        var hasInvalidPosition = x is < 0 or > 1 || y is < 0 or > 1;
        var hasInvalidSize = width <= 0 || height <= 0;
        var extendsOutsideImage = x + width > 1 || y + height > 1;

        if (hasInvalidPosition || hasInvalidSize || extendsOutsideImage)
        {
            throw new ArgumentOutOfRangeException(
                nameof(NormalizedRectangle),
                "Coordinates must be normalized and remain inside the image.");
        }

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public decimal X { get; init; }

    public decimal Y { get; init; }

    public decimal Width { get; init; }

    public decimal Height { get; init; }
}
