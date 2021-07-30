using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Thankifi.Api.Model.V1;
using Thankifi.Api.Model.V1.Responses;
using Thankifi.Core.Domain.Category.Query.GetAllCategories;
using Thankifi.Core.Domain.Category.Query.GetCategoryById;
using Thankifi.Common;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class CategoryController : ControllerBase
    {
        /// <summary>
        /// Retrieve a paginated list of all the available categories. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllCategories))]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RetrieveAllCategories(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve a detail view of a category and a paginated list of gratitudes for the specified category.
        /// Optionally specify a subject, a signature, flavours and languages. Thanks!
        /// </summary>
        [HttpGet("{slug:required}", Name = nameof(RetrieveByCategorySlug))]
        [ProducesResponseType(typeof(CategoryDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetrieveByCategorySlug([FromRoute, Required] string slug, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}