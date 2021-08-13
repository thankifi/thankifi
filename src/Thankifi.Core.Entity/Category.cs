using System;
using System.Collections.Generic;

namespace Thankifi.Core.Entity
{
    public class Category
    {
        public Category()
        {
            Gratitudes = new List<Gratitude>();
        }
        
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public List<Gratitude> Gratitudes { get; set; }
    }
}