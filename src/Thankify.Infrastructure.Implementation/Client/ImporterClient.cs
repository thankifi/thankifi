using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Thankify.Infrastructure.Contract.Client;
using Thankify.Infrastructure.Contract.Model;

namespace TaaS.Infrastructure.Implementation.Client
{
    public class ImporterClient : IImporterClient
    {
        protected readonly ILogger<ImporterClient> Logger;
        protected readonly HttpClient Client;
        protected readonly string? Version;

        public ImporterClient(ILogger<ImporterClient> logger, HttpClient client, IConfiguration configuration)
        {
            client.BaseAddress = new Uri(configuration["IMPORTER_SOURCE_URL"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            Version = configuration["IMPORTER_DB_VERSION"];
            Logger = logger;
            Client = client;
        }

        public async Task<ImportResponse> GetData(CancellationToken cancellationToken = default)
        {
            var requestUri = Version != null ? $"/data_{Version}.json" : "/data.json";
            
            var requestResponse = await Client.GetAsync($"{Client.BaseAddress}{requestUri}", cancellationToken);

            requestResponse.EnsureSuccessStatusCode();

            await using var importDataStream = await requestResponse.Content.ReadAsStreamAsync();
            
            var importData = await JsonSerializer.DeserializeAsync<ImportResponse>(importDataStream, cancellationToken: cancellationToken);
                
            return importData;
        }

        public async Task<VersionResponse> GetVersion(CancellationToken cancellationToken = default)
        {
            var requestResponse = await Client.GetAsync($"{Client.BaseAddress}/version.json", cancellationToken);

            requestResponse.EnsureSuccessStatusCode();

            await using var importDataStream = await requestResponse.Content.ReadAsStreamAsync();
            
            var importData = await JsonSerializer.DeserializeAsync<VersionResponse>(importDataStream, cancellationToken: cancellationToken);
                
            return importData;
        }
    }
}