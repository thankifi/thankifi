using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OperationResult;
using Thankify.Infrastructure.Contract.Client;
using Thankify.Infrastructure.Contract.Model;
using Thankify.Infrastructure.Contract.Service;
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

        public async Task<Result<(List<Gratitude>, List<Category>), string>> Fetch(CancellationToken cancellationToken = default)
        {
            try
            {
                var rawData = await Client.GetData(cancellationToken);

                return Ok((rawData.Gratitudes, rawData.Categories));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while fetching data from source.");

                return Error(e.Message);
            }
        }

        public async Task<Result<string, string>> FindCurrentVersion(CancellationToken cancellationToken = default)
        {
            try
            {
                var rawData = await Client.GetVersion(cancellationToken);

                return Ok(rawData.Version);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error while fetching version from source.");

                return Error(e.Message);
            }
        }
    }
}