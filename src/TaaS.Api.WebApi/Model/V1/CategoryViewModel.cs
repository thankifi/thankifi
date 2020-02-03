using System.Collections.Generic;
using System.Linq;
using TaaS.Core.Entity;

namespace TaaS.Api.WebApi.Model.V1
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public static CategoryViewModel Parse(Category category)
        {
            return new CategoryViewModel
            {
                Id = category.Id,
                Title = category.Title
            };
        }

        public static IEnumerable<CategoryViewModel> Parse(IEnumerable<Category> categories)
        {
            return categories.Select(Parse);
        }
    }
}