using handshake.Interfaces;
using handshake.Repositories;
using handshake.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace handshake
{
  internal class Startup
  {
    #region Fields

    private IServiceCollection services;

    #endregion Fields

    #region Constructors

    public Startup(IConfiguration configuration)
    {
    }

    #endregion Constructors

    #region Methods

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseDeveloperExceptionPage();

      app.UseHttpsRedirection();

      if (env.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI(setupAction =>
        {
          setupAction.SwaggerEndpoint("/swagger/shake/swagger.json", "Handshake");
        });
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      this.services = services;

      this.services.AddControllers();

      this.services.AddAuthentication("BasicAuthentication")
       .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

      services.AddScoped<IAuthService, AuthService>();

      services.AddScoped<UserDatabaseAccess>();

      services.AddSwaggerGen(setupAction =>
      {
        setupAction.SwaggerDoc("shake", new OpenApiInfo()
        {
          Title = "Handshake",
          Version = "0.0.1"
        });

        string xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

        setupAction.IncludeXmlComments(xmlCommentsFullPath);

        setupAction.AddSecurityDefinition("http", new OpenApiSecurityScheme
        {
          Description = "Basic",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.Http,
          Scheme = "basic"
        });

        setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "http"
              },
              Scheme = "basic",
              Name = "basic",
              In = ParameterLocation.Header
            },
            new List<string>()
          }
        }); ;

        setupAction.AddServer(new OpenApiServer()
        {
          Url = "https://handshake.azurewebsites.net",
          Description = "Azure host"
        });

        setupAction.AddServer(new OpenApiServer()
        {
          Url = "https://localhost:44370",
          Description = "Local host"
        });
      });
    }

    #endregion Methods
  }
}