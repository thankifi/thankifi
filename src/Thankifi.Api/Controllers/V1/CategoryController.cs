using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Thankifi.Api.Model.V1;
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
        private readonly IMemoryCache _cache;
        private readonly IMediator _mediator;

        public CategoryController(IMemoryCache memoryCache, IMediator mediator)
        {
            _cache = memoryCache;
            _mediator = mediator;
        }

        /// <summary>
        /// Get a list of all the categories available. Thanks!
        /// </summary>
        /// <param name="language">Filter by language.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Categories list. Thanks!</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), 200)]
        public async Task<IActionResult> Get(
            [FromQuery] string? language,
            CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKeys.CategoryViewModelList(language), out IEnumerable<CategoryViewModel> cacheEntry))
            {
                var result = await _mediator.Send(new GetAllCategoriesQuery
                {
                    Language = language
                }, cancellationToken);
                
                cacheEntry = CategoryViewModel.Parse(result);

                _cache.Set(CacheKeys.CategoryViewModelList(language), cacheEntry, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            
            return Ok(cacheEntry);
        }

        /// <summary>
        /// Get detailed category. Includes number of items. Thanks!
        /// </summary>
        /// <param name="categoryId">Id of the category.</param>
        /// <param name="language">Filter by language.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Detailed view of the category. Thanks!</response>
        /// <response code="404">Category not found! Thanks!</response>
        [HttpGet, Route("{categoryId}")]
        [ProducesResponseType(typeof(CategoryDetailViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetById(
            [FromRoute, Required] int categoryId,
            [FromQuery] string? language,
            CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKeys.CategoryDetailViewModel(categoryId, language), out CategoryDetailViewModel cacheEntry))
            {
                var result = await _mediator.Send(new GetCategoryByIdQuery
                {
                    Id = categoryId,
                    Language = language
                }, cancellationToken);

                if (result == null)
                {
                    return NotFound("Category Not Found.");
                }
                
                cacheEntry = CategoryDetailViewModel.Parse(result);

                _cache.Set(CacheKeys.CategoryDetailViewModel(categoryId, language), cacheEntry, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            
            return Ok(cacheEntry);
        }
    }
}