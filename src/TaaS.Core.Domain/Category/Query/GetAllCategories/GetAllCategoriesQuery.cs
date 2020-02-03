using System.Collections.Generic;
using MediatR;

namespace TaaS.Core.Domain.Category.Query.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<IEnumerable<Entity.Category>>
    {
        
    }
}