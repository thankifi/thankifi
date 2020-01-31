using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OperationResult;
using TaaS.Infrastructure.Contract.Client;
using TaaS.Infrastructure.Contract.Model;
using TaaS.Infrastructure.Contract.Service;
using static OperationResult.Helpers;

namespace TaaS.Infrastructure.Implementation.Service
{
    public class ImporterService : IImporterService
    {
        protected readonly ILogger<ImporterService> Logger;
        protected readonly IImporterClient Client;

        public ImporterService(ILogger<ImporterService> logger, IImporterClient client)
        {
            Logger = logger;
            Client = client;
        }

        public async Task<Result<(List<Gratitude>, List<Category>), string>> Fetch()
        {
            try
            {
                var rawData = await Client.GetData();

                return Ok((rawData.Gratitudes, rawData.Categories));
            }
            catch (Exception e)
            {
                Logger.LogError(e,"Error while fetching data from source.");
                
                return Error(e.Message);
            }
        }
    }
}