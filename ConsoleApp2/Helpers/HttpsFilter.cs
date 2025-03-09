using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConsoleApp2.Helpers
{
    public class HttpsFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpsFilter(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var host = this._httpContextAccessor.HttpContext?.Request.Host.Value;
            
            swaggerDoc.Servers.Add(new OpenApiServer
            {
                Url = $"https://{host}"
            });
        }
    }
}