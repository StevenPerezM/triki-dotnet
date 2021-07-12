using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tricky.Players
{    
    public sealed class Startup
    {
        public Startup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();
        }
        public void Configure(IApplicationBuilder app)
        {
            try
            {
                 app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
