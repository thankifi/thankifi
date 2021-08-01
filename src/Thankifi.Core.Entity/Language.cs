using System;
using System.Collections.Generic;

namespace Thankifi.Core.Entity
{
    public class Language
    {
        public Language()
        {
            Gratitudes = new List<Gratitude>();
        }
        
        public Guid Id { get; set; }
        public string Code { get; set; }
        public List<Gratitude> Gratitudes { get; set; }
    }
}