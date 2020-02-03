using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TaaS.Core.Entity;

namespace TaaS.Api.WebApi.Model.V1
{
    public class GratitudeViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Categories { get; set; }

        public static GratitudeViewModel Parse(Gratitude gratitude)
        {
            return new GratitudeViewModel
            {
                Id = gratitude.Id,
                Text = gratitude.Text,
                Categories = gratitude.Categories.Select(c => c.Category.Title)
            };
        }
    }
}