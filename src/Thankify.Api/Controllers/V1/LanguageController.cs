using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Thankify.Core.Domain.Language.Query.GetAllLanguages;
using Thankify.Core.Domain.Language.Query.GetLanguageByCode;
using Thankify.Api.Model.V1;
using Thankify.Common;

namespace Thankify.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class LanguageController : ControllerBase
    {
        protected readonly ILogger<ThanksController> Logger;
        protected readonly IMemoryCache Cache;
        protected readonly IMediator Mediator;

        public LanguageController(ILogger<ThanksController> logger, IMemoryCache cache, IMediator mediator)
        {
            Logger = logger;
            Cache = cache;
            Mediator = mediator;
        }
        
        /// <summary>
        /// Get a list of all the languages available. Thanks!
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Languages list. Thanks!</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LanguageViewModel>), 200)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            if (!Cache.TryGetValue(CacheKeys.LanguageViewModelList, out IEnumerable<LanguageViewModel> cacheEntry))
            {
                var result = await Mediator.Send(new GetAllLanguagesQuery(), cancellationToken);
                
                cacheEntry = LanguageViewModel.Parse(result);

                Cache.Set(CacheKeys.LanguageViewModelList, cacheEntry, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            
            return Ok(cacheEntry);
        }

        /// <summary>
        /// Get detailed language. Includes number of items. Thanks!
        /// </summary>
        /// <param name="language">Language code</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Detailed view of the language. Thanks!</response>
        /// <response code="404">Language not found! Thanks!</response>
        [HttpGet("{language}")]
        [ProducesResponseType(typeof(LanguageDetailViewModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetById(
            [FromRoute, Required] string language,
            CancellationToken cancellationToken)
        {
            if (!Cache.TryGetValue(CacheKeys.LanguageDetailViewModel(language), out LanguageDetailViewModel cacheEntry))
            {
                var result = await Mediator.Send(new GetLanguageByCodeQuery
                {
                    Code = language
                }, cancellationToken);
                
                cacheEntry = LanguageDetailViewModel.Parse(result);

                Cache.Set(CacheKeys.LanguageDetailViewModel(language), cacheEntry, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12)));
            }
            
            return Ok(cacheEntry);
        }
    }
}