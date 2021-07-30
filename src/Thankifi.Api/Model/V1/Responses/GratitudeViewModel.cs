using System.Collections.Generic;

namespace Thankifi.Api.Model.V1.Responses
{
    public class GratitudeViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public IEnumerable<CategoryViewModel> Categories { get; set; } = null!;
    }
}