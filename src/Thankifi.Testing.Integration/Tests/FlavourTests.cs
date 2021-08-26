using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Thankifi.Api;
using Thankifi.Api.Model.V1.Responses;

namespace Thankifi.Testing.Integration.Tests
{
    [TestFixture]
    public class FlavourTests
    {
        private readonly CustomWebApplicationFactory<Startup> _clientFactory = new();

        /// <summary>
        /// Assert that a client can retrieve all available flavours.
        /// </summary>
        [Test]
        public async Task RetrieveAllFlavours()
        {
            using var client = _clientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<List<FlavourViewModel>>("api/v1/flavour");

            response.Should().NotBeEmpty();
            response.Should().HaveCount(5);
        }
    }
}