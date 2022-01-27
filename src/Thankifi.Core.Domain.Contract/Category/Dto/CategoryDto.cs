using System;

namespace Thankifi.Core.Domain.Contract.Category.Dto;

public record CategoryDto
{
    public Guid Id { get; init; }
    public string Slug { get; init; }
}