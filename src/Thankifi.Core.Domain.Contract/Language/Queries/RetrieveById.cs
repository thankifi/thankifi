using System;
using Incremental.Common.Sourcing.Abstractions.Queries;
using Thankifi.Core.Domain.Contract.Category.Dto;
using Thankifi.Core.Domain.Contract.Language.Dto;

namespace Thankifi.Core.Domain.Contract.Language.Queries
{
    public record RetrieveById : IQuery<LanguageDetailDto?>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public Guid Id { get; init; }
        public string? Subject { get; init; }
        public string? Signature { get; init; }
        public string[]? Flavours { get; init; }
        public string[]? Categories { get; init; }
    }
}