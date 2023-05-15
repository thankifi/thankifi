using System;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Thankifi.Core.Domain.Contract.Gratitude.Dto;

namespace Thankifi.Core.Domain.Contract.Gratitude.Queries;

public record RetrieveByIdFlavourful : Query<GratitudeFlavourfulDto?>
{
    public Guid Id { get; init; }
    public string? Subject { get; init; }
    public string? Signature { get; init; }
}