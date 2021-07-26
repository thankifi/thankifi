using System.Collections.Generic;
using System.Linq;
using Thankify.Core.Domain.Gratitude.Dto;

namespace Thankify.Api.Model.V1
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

        public static IEnumerable<GratitudeViewModel> Parse(IEnumerable<GratitudeDto> gratitudeDtos)
        {
            return gratitudeDtos.Select(Parse);
        }
    }
}