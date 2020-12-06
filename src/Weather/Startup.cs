using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weather.Services.Interfaces;
using Weather.Services;
using Microsoft.Extensions.Logging;
using System.IO;
using Weather.Logging;
using Weather.Middlewares;

namespace Weather
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            ConfigureWeatherServices(services);

            services.AddSingleton<FileLogger>((_) => new FileLogger(Directory.GetCurrentDirectory() + "/log.txt"));

            services.AddSwaggerGen();

        }
        ///I decided to move user-defined services that are connected with weather into separate method
        //this way it's easier to read and update these services separately
        //Also, I could use a reflection to get rid of similar code, but I didn't do it
        //because each service might have different parameters or steps of initialization
        //even in our services AccuWeather requires more data to work
        public void ConfigureWeatherServices(IServiceCollection services)
        {
            var weatherBitOptions = Configuration.GetSection("WeatherApis:Weatherbit");
            services.Configure<WeatherbitApiOptions>(wao =>
            {
                wao.Router = weatherBitOptions["WeatherRouter"];
                wao.API_KEY = weatherBitOptions["API_KEY"];
                wao.URL = weatherBitOptions["URL"];
            });
            services.AddScoped<WeatherbitService>();

            var openWeatherMapOptions = Configuration.GetSection("WeatherApis:OpenWeatherMap");
            services.Configure<OpenWeatherMapOptions>(owmo =>
            {
                owmo.Router = openWeatherMapOptions["WeatherRouter"];
                owmo.API_KEY = openWeatherMapOptions["API_KEY"];
                owmo.URL = openWeatherMapOptions["URL"];
            });
            services.AddScoped<OpenWeatherMapService>();

            var accuWeatherOptions = Configuration.GetSection("WeatherApis:AccuWeather");
            services.Configure<AccuWeatherOptions>(awo =>
            {
                awo.API_KEY = accuWeatherOptions["API_KEY"];
                awo.URL = accuWeatherOptions["URL"];
                awo.LocationsRouter = accuWeatherOptions["LocationsRouter"];
                awo.WeatherRouter = accuWeatherOptions["WeatherRouter"];
            });
            services.AddScoped<AccuWeatherService>();

            services.AddScoped<WeatherAggregator>();

            services.AddScoped<Solution>();

        }
        public interface IService { }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), Configuration["LoggingPath"]));

            // var logger = loggerFactory.CreateLogger<FileLogger>();

            app.UseMiddleware<RequestLoggingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather");
            });

        }
    }
}
