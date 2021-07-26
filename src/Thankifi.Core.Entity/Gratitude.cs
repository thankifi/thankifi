using System;
using System.Collections.Generic;

namespace Thankifi.Core.Entity
{
    public class Gratitude
    {
        public Guid Id { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }
        public List<Category> Categories { get; set; }
    }
}