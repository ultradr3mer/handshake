﻿using handshake.Contexts;
using handshake.Data;
using handshake.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace handshake.Services
{
  internal class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
  {
    private readonly IAuthService userService;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IAuthService userService)
        : base(options, logger, encoder, clock)
    {
      this.userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      // skip authentication if endpoint has [AllowAnonymous] attribute
      var endpoint = Context.GetEndpoint();
      if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        return AuthenticateResult.NoResult();

      if (!Request.Headers.ContainsKey("Authorization"))
        return AuthenticateResult.Fail("Missing Authorization Header");

      string username = null;
      try
      {
        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
        username = credentials[0];
        var password = credentials[1];
        await userService.Authenticate(username, password);
      }
      catch
      {
        return AuthenticateResult.Fail("Invalid Authorization Header");
      }

      var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
            };
      var identity = new ClaimsIdentity(claims, Scheme.Name);
      var principal = new ClaimsPrincipal(identity);
      var ticket = new AuthenticationTicket(principal, Scheme.Name);

      return AuthenticateResult.Success(ticket);
    }
  }
}
