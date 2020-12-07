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
using System.Collections.Generic;
using System;

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

            services.AddSingleton<FileLogger>((_) => new FileLogger(Directory.GetCurrentDirectory() + Configuration["LoggingPath"]));

            services.AddSwaggerGen();

        }
        ///I decided to move user-defined services that are connected with weather into separate method
        //this way it's easier to read and update these services separately.
        //Also, I could use a reflection to get rid of similar code, but I didn't do it
        //because each service might have different parameters or steps of initialization
        //even among our services AccuWeather requires more data to work
        public void ConfigureWeatherServices(IServiceCollection services)
        {
            var weatherBitOptions = Configuration.GetSection("WeatherApis:Weatherbit");
            services.Configure<WeatherbitApiOptions>(weatherBitOptions);
            services.AddHttpClient("Weatherbit", c =>
                c.BaseAddress = new Uri(weatherBitOptions["URL"]));
            services.AddScoped<WeatherbitService>();

            var openWeatherMapOptions = Configuration.GetSection("WeatherApis:OpenWeatherMap");
            services.Configure<OpenWeatherMapOptions>(openWeatherMapOptions);
            services.AddHttpClient("OpenWeatherMap", c =>
                c.BaseAddress = new Uri(openWeatherMapOptions["URL"]));
            services.AddScoped<OpenWeatherMapService>();

            var accuWeatherOptions = Configuration.GetSection("WeatherApis:AccuWeather");
            services.Configure<AccuWeatherOptions>(accuWeatherOptions);
            services.AddHttpClient("AccuWeather", c =>
                c.BaseAddress = new Uri(accuWeatherOptions["URL"]));
            services.AddScoped<AccuWeatherService>();

            services.AddTransient<List<IWeatherService>>(serviceProvider =>
               new List<IWeatherService>
               {
                    serviceProvider.GetRequiredService<AccuWeatherService>(),
                    serviceProvider.GetRequiredService<WeatherbitService>(),
                    serviceProvider.GetRequiredService<OpenWeatherMapService>(),
                });

            services.AddScoped<IWeatherAggregator, WeatherAggregator>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

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
