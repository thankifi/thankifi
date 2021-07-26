using System.Collections.Generic;

namespace Thankify.Core.Entity
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<GratitudeCategory> Gratitudes { get; set; }
    }
}