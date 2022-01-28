using System;

namespace Thankifi.Api.Model.V1.Responses;

public record CategoryViewModel
{
    public Guid Id { get; init; }
    public string Slug { get; init; }
}