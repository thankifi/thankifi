using System;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Thankifi.Api.Configuration.Authorization;

public class ManagementAuthenticationHandler : AuthenticationHandler<ManagementAuthenticationScheme>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public ManagementAuthenticationHandler(IOptionsMonitor<ManagementAuthenticationScheme> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(options,
        logger, encoder, clock)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var header = httpContext?.Request.Headers["Management"];

        if (_configuration["MANAGEMENT_API_KEY"].Equals(header))
        {
            return Task.FromResult(
                AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new GenericPrincipal(new GenericIdentity("Manager"), Array.Empty<string>()),
                        nameof(ManagementAuthenticationScheme)
                    )
                )
            );
        }

        return Task.FromResult(AuthenticateResult.Fail("Management key is incorrect"));
    }
}

public class ManagementAuthenticationScheme : AuthenticationSchemeOptions
{
}