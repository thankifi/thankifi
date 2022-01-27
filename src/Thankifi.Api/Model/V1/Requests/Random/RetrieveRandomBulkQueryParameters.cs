namespace Thankifi.Api.Model.V1.Requests.Random;

public record RetrieveRandomBulkQueryParameters
{
    private const int MaxQuantity = 50;
    private int _quantity = 10;

    /// <summary>
    /// Number of gratitudes to retrieve. Retrieves ten by default.
    /// </summary>
    public int Quantity { get => _quantity; init => _quantity = value is > MaxQuantity or <= 0 ? MaxQuantity : value; }
        
    /// <summary>
    /// Subject receiving the gratitude.
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    /// Signature to attach at the end of the gratitude.
    /// </summary>
    public string? Signature { get; init; }

    /// <summary>
    /// List of flavours to apply to the gratitude.
    /// </summary>
    public string[]? Flavours { get; init; }

    /// <summary>
    /// Filter gratitudes by categories.
    /// </summary>
    public string[]? Categories { get; init; }
        
    /// <summary>
    /// Filter gratitudes by languages.
    /// </summary>
    public string[]? Languages { get; set; }
}