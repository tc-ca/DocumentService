using DocumentService.Azure;
using DocumentService.Contexts;
using DocumentService.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DocumentService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<DocumentContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DocumentContext")));
            services.AddDbContext<DocumentContext>();

            services.AddSingleton<IKeyVaultService, AzureKeyVaultService>();
            services.AddScoped<IAzureBlobService, AzureBlobService>();
            services.AddScoped<IAzureBlobConnectionFactory, AzureBlobConnectionFactory>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DocumentService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next => context =>
            {
                // Set the environment on start up
                context.Response.Cookies.Append("Version", Configuration.GetValue<string>("Version"), new CookieOptions()
                {
                    HttpOnly = true,
                });

                return next(context);
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DocumentService v1 | Environment = " + Configuration.GetValue<string>("Env")));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
