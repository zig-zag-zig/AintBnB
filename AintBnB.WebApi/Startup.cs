using AintBnB.BusinessLogic.Imp;
using AintBnB.BusinessLogic.Interfaces;
using AintBnB.Database.DbCtx;
using AintBnB.Repository.Imp;
using AintBnB.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AintBnB.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            string conString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AintBnB.Database;Integrated Security=True";

            services.AddDbContext<DatabaseContext>(
                options => options.UseSqlServer(conString));


            //Adds all the countries and cities of the world to a sql database table if the table isn't already filled
            BusinessLogic.Helpers.AllCountiresAndCities.con = new SqlConnection(conString);
            BusinessLogic.Helpers.AllCountiresAndCities.AllCitiesAndCountries();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IAccommodationService, AccommodationService>();
            services.AddTransient<IBookingService, BookingService>();
            services.AddTransient<IDeletionService, DeletionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
