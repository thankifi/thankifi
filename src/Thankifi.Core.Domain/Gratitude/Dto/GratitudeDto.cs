using System.Collections.Generic;

namespace Thankifi.Core.Domain.Gratitude.Dto
{
    public class GratitudeDto
    {
        public GratitudeDto()
        {
            Categories = new List<string>();
        }

        public int Id { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Categories { get; set; }
    }
}