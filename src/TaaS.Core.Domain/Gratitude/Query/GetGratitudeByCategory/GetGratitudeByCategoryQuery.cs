﻿using System.Collections.Generic;
using MediatR;
using TaaS.Core.Domain.Gratitude.Dto;

namespace TaaS.Core.Domain.Gratitude.Query.GetGratitudeByCategory
{
    public class GetGratitudeByCategoryQuery : IRequest<GratitudeDto?>
    {
        public GetGratitudeByCategoryQuery()
        {
            Language = "eng";
            Filters = new List<string>();
        }

        public string CategoryName { get; set; }
        public string? Name { get; set; }
        public string? Signature { get; set; }
        public string Language { get; set; }
        public List<string> Filters { get; set; }
    }
}