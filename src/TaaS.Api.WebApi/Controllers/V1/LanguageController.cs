using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TaaS.Api.WebApi.Model.V1;
using TaaS.Common;
using TaaS.Core.Domain.Language.Query.GetAllLanguages;

namespace TaaS.Api.WebApi.Controllers.V1
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
    }
}