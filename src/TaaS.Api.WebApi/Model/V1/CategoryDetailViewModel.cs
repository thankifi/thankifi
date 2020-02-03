using TaaS.Core.Entity;

namespace TaaS.Api.WebApi.Model.V1
{
    public class CategoryDetailViewModel : CategoryViewModel
    {
        public int Total { get; set; }

        public static CategoryDetailViewModel Parse(Category category, int total)
        {
            return new CategoryDetailViewModel
            {
                Id = category.Id,
                Title = category.Title,
                Total = total
            };;
        }
    }
}