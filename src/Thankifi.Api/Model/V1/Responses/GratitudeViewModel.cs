using System;
using System.Collections.Generic;

namespace Thankifi.Api.Model.V1.Responses
{
    public record GratitudeViewModel
    {
        public Guid Id { get; init; }
        public LanguageViewModel Language { get; init; }
        public string Text { get; init; } 
        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}