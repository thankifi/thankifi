using System;
using NUnit.Framework;

namespace Thankifi.Testing.Integration
{
    [SetUpFixture]
    public class IntegrationSetupFixture
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        }
    }
}