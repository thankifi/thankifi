using System;

namespace Thankifi.Core.Domain.Contract.Language.Dto;

public record LanguageDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
}