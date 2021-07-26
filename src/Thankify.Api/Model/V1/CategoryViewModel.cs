using System.Collections.Generic;
using System.Linq;
using Thankify.Core.Domain.Category.Dto;

namespace Thankify.Api.Model.V1
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public static CategoryViewModel Parse(CategoryDto category)
        {
            return new CategoryViewModel
            {
                Id = category.Id,
                Title = category.Title
            };
        }

        public static IEnumerable<CategoryViewModel> Parse(IEnumerable<CategoryDto> categories)
        {
            return categories.Select(Parse);
        }
    }
}