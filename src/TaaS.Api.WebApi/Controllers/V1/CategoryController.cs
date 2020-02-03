using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Core.Domain.Category.Query.GetAllCategories;
using TaaS.Core.Domain.Category.Query.GetCategoryById;

namespace TaaS.Api.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class CategoryController : ControllerBase
    {
        protected readonly ILogger<ThanksController> Logger;
        protected readonly IMediator Mediator;

        public CategoryController(ILogger<ThanksController> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }
        
        /// <summary>
        /// Get a list of all the categories available in TaaS.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Categories list.. Thanks!</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), 200)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetAllCategoriesQuery(), cancellationToken);

            return Ok(CategoryViewModel.Parse(result));
        }

        /// <summary>
        /// Get a list of all the categories available in TaaS.
        /// </summary>
        /// <param name="id">Id of the category.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Detailed view of the category. Thanks!</response>
        /// <response code="404">Category not found! Thanks! Thanks!</response>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(typeof(CategoryDetailViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetById([FromRoute, Required] int id,
            CancellationToken cancellationToken = default)
        {
            var (total, category) = await Mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);

            if (category != null)
            {
                return Ok(CategoryDetailViewModel.Parse(category, total));
            }

            return NotFound("Category Not Found.");
        }
    }
}