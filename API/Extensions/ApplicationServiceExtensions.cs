using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, 
        IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"))
                );
            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            // used to scan all assemblies in the current application domain for types that implement Profile (AutoMapper profiles). This allows AutoMapper to discover and register the profiles automatically.
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}