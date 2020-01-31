using System.Collections.Generic;

namespace TaaS.Infrastructure.Contract.Model
{
    public class ImportResponse
    {
        public List<Category> Categories { get; set; }
        public List<Gratitude> Gratitudes { get; set; }
    }

    public class Gratitude
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public List<string> Categories { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}