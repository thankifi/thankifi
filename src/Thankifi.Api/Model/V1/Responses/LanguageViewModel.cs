﻿using System;

namespace Thankifi.Api.Model.V1.Responses;

public record LanguageViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Reference { get; set; }
}