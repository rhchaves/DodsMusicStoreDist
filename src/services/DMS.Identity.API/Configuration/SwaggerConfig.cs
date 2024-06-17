using Microsoft.OpenApi.Models;

namespace DMS.Identity.API.Configuration;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Dosds Muisic Store Identity API",
                Version = "v1",
                Description = "Esta API tem por finalizade administrar a autenticação e autorização dos usuários na plataforma DMStore.",
                Contact = new OpenApiContact() { Name = "Rodolfo Chaves", Email = "rodolfo.chaves@dmstore.com" },
                License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
