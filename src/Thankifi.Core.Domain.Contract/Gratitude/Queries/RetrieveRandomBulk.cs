using System.Collections.Generic;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;

namespace Thankifi.Core.Domain.Contract.Gratitude.Queries;

public record RetrieveRandomBulk : Query<IEnumerable<GratitudeDto>>
{
    public int Quantity { get; init; }
    public string? Subject { get; init; }
    public string? Signature { get; init; }
    public string[]? Flavours { get; init; }
    public string[]? Categories { get; init; }
    public string[]? Languages { get; set; }
}