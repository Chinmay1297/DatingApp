using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions   //declaring static here means, we can use methods of this class without instantiating the class
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)         //'this IServiceCollection' means we are extending it
        {
            
            services.AddCors();

            services.AddScoped<ITokenService, TokenService>();   //AddScope ensures disposal of service instance after the controller that uses this service is disposed

            services.AddDbContext<DataContext>(opt => 
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            
            return services;
        }
    }
}