using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Common;
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
        protected readonly IMemoryCache Cache;
        protected readonly IMediator Mediator;

        public CategoryController(ILogger<ThanksController> logger, IMemoryCache memoryCache, IMediator mediator)
        {
            Logger = logger;
            Cache = memoryCache;
            Mediator = mediator;
        }

        /// <summary>
        /// Get a list of all the categories available.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Categories list. Thanks!</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), 200)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            if (!Cache.TryGetValue(CacheKeys.CategoryViewModelList, out IEnumerable<CategoryViewModel> cacheEntry))
            {
                var result = await Mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
                
                cacheEntry = CategoryViewModel.Parse(result);

                Cache.Set(CacheKeys.CategoryViewModelList, cacheEntry, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            
            return Ok(cacheEntry);
        }

        /// <summary>
        /// Get detailed category. Includes number of items.
        /// </summary>
        /// <param name="id">Id of the category.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Detailed view of the category. Thanks!</response>
        /// <response code="404">Category not found! Thanks!</response>
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