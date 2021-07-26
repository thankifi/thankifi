using System;
using System.Collections.Generic;

namespace Thankifi.Core.Entity
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<Gratitude> Gratitudes { get; set; }
    }
}