using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaaS.Core.Domain.Gratitude.Dto;
using TaaS.Core.Entity;

namespace TaaS.Api.WebApi.Model.V1
{
    public class GratitudeViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Categories { get; set; }
        
        public static GratitudeViewModel Parse(GratitudeDto gratitudeDto)
        {
            return new GratitudeViewModel
            {
                Id = gratitudeDto.Id,
                Text = gratitudeDto.Text,
                Categories = gratitudeDto.Categories.Select(c => c.ToLower())
            };
        }
    }
}