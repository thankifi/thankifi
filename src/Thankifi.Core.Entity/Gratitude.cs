using System;
using System.Collections.Generic;

namespace Thankifi.Core.Entity
{
    public class Gratitude
    {
        public Gratitude()
        {
            Categories = new List<Category>();
        }
        
        public Guid Id { get; set; }
        public Language Language { get; set; }
        public string Text { get; set; }
        public List<Category> Categories { get; set; }
    }
}