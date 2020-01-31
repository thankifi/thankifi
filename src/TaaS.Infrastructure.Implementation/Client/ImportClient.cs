using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaaS.Infrastructure.Contract.Client;
using TaaS.Infrastructure.Contract.Model;

namespace TaaS.Infrastructure.Implementation.Client
{
    public class ImportClient : IImporterClient
    {
        protected readonly ILogger<ImportClient> Logger;
        protected readonly HttpClient Client;
        protected readonly string? Version;

        public ImportClient(ILogger<ImportClient> logger, HttpClient client, IConfiguration configuration)
        {
            client.BaseAddress = new Uri(configuration["IMPORTER_SOURCE_URL"]);
            Version = configuration["IMPORTER_DB_VERSION"];
            Logger = logger;
            Client = client;
        }

        public async Task<ImportResponse> GetData()
        {
            var requestResponse = await Client.GetAsync(Version != null ? $"/data_{Version}.json" : "/data.json");

            requestResponse.EnsureSuccessStatusCode();
            
            var importData = await requestResponse.Content.ReadAsAsync<ImportResponse>();

            return importData;
        }
    }
}