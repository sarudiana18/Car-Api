using ConsoleApp2.Helpers;
using ConsoleApp2.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ConsoleApp2
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options => { options.SuppressMapClientErrors = true; });
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("1.0.0", new OpenApiInfo 
                {
                    Title = "Car Listings API",
                    Version = "v1",
                    Description = "A REST API for filtering car listings with price analysis",
                });
                
                //options.DocumentFilter<HttpsFilter>();
            });

            services.AddHealthChecks();
            
            
            //Required for the above HttpsFilter
            //services.AddHttpContextAccessor();
            
            /*services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });*/
            
            services.AddSingleton<CarService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //Send stack traces in exceptions
                app.UseDeveloperExceptionPage();
                //app.UseCors("AllowAll");
            }
            
            //Forces clients onto https:// if they're using http://
            app.UseHttpsRedirection();

            app.UseSwagger(options => { options.RouteTemplate = $"swagger/{{documentName}}/swagger.json"; });

            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = $"swagger";

                options.SwaggerEndpoint("1.0.0/swagger.json", "1.0.0");
            });

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSwagger("swagger/{*documentName}");
                
                endpoints.MapControllers();
            });
        }
    }
}