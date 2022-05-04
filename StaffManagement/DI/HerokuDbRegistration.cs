using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StaffManagement.DataAccess;
using StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.DI
{
    public static class HerokuDbRegistration
    { 
        public static void AddHerokuDb(this IServiceCollection services, IConfiguration config)
        {
            HerokuDb herokuDb = new ();
            config.GetSection("HerokuDb").Bind(herokuDb);
            services.AddDbContext<AppDbContext>(
                options => options.UseNpgsql(config.GetConnectionString("PostGreSql")
                        .Replace("#Host", herokuDb.Host)
                        .Replace("#User", herokuDb.User)
                        .Replace("#Password", herokuDb.Password)
                        .Replace("#Database", herokuDb.Database)
                        .Replace("#Port", herokuDb.Port.ToString())
                        ));
        }
    }
}
