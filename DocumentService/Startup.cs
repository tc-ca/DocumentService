using DocumentService.Authorization;
using DocumentService.Azure;
using DocumentService.Contexts;
using DocumentService.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace DocumentService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);

            services.AddAuthorization(options =>
            {
                var readerPolicyConfig = Configuration.GetSection(RolePolicy.PolicyConfigKeyReaders).Get<RolePolicy>();
                var writerPolicyConfig = Configuration.GetSection(RolePolicy.PolicyConfigKeyWriters).Get<RolePolicy>();

                options.AddPolicy(RolePolicy.RoleAssignmentRequiredReaders, policy => policy.RequireRole(readerPolicyConfig.Role));
                options.AddPolicy(RolePolicy.RoleAssignmentRequiredWriters, policy => policy.RequireRole(writerPolicyConfig.Role));
            });

            services.AddDbContext<DocumentContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DocumentContext")));

            services.AddTransient<IKeyVaultService, AzureKeyVaultService>();
            services.AddScoped<IAzureBlobService, AzureBlobService>();
            services.AddScoped<IAzureBlobConnectionFactory, AzureBlobConnectionFactory>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DocumentService", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"{Configuration.GetSection("AzureAd:Instance").Value}{Configuration.GetSection("AzureAd:TenantId").Value}/oauth2/v2.0/authorize"),
                            TokenUrl = new Uri($"{Configuration.GetSection("AzureAd:Instance").Value}{Configuration.GetSection("AzureAd:TenantId").Value}/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string> {
                                {$"{Configuration.GetSection("AzureAd:Audience").Value}/DocumentService.Read.All", "Allow the application to have create/update access to all work item data."},
                                {$"{Configuration.GetSection("AzureAd:Audience").Value}/DocumentService.CreateUpdate.All", "Allow the application to have read-only access to all work item data."}
                            }
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                        Id = "oauth2"
                                },
                                Scheme = "oauth2",
                                Name = "oauth2",
                                In = ParameterLocation.Header
                        },
                        new List <string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IKeyVaultService kvService, IConfiguration appConfiguration)
        {
            this.SetApplicationSecretsFromKeyVault(kvService, appConfiguration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DocumentService v1");
                c.OAuthClientId(Configuration.GetSection("AzureAd:ClientId").Value);
                c.OAuthClientSecret(Configuration.GetSection("AzureAd:ClientSecret").Value);
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void SetApplicationSecretsFromKeyVault(IKeyVaultService kvService, IConfiguration appConfiguration)
        {
            //client Id
            var secretName = appConfiguration.GetSection("AzureAd")["ClientId"];
            var token = kvService.GetSecretByName(secretName);
            appConfiguration.GetSection("AzureAd")["ClientId"] = token;

            //"TenantId"
            secretName = appConfiguration.GetSection("AzureAd")["TenantId"];
            token = kvService.GetSecretByName(secretName);
            appConfiguration.GetSection("AzureAd")["TenantId"] = token;

            // "ClientSecret"
            secretName = appConfiguration.GetSection("AzureAd")["ClientSecret"];
            token = kvService.GetSecretByName(secretName);
            appConfiguration.GetSection("AzureAd")["ClientSecret"] = token;
        }
    }
}
