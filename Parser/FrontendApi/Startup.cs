using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataAccess;
using DataAccess.Repositories;
using FrontendApi.Helpers;
using FrontendApi.Service;
using FrontendApi.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FrontendApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            // Create the container builder.
            var builder = new ContainerBuilder();
            var appSettings = Configuration.Get<AppSettings>("AppSettings");

            // Register dependencies, populate the services from
            // the collection, and build the container. If you want
            // to dispose of the container at the end of the app,
            // be sure to keep a reference to it as a property or field.
            var tenantId = appSettings.PowerBiSettings.TenantId;
            var userName = appSettings.PowerBiSettings.UserName;
            var password = appSettings.PowerBiSettings.Password;
            var clientId = appSettings.PowerBiSettings.ClientId;
            var clientSecret = appSettings.PowerBiSettings.ClientSecret;
            builder.RegisterType<AnalyseRepository>().As<IAnalyseRepository>();
            builder.RegisterType<PowerBiService>().As<IPowerBiService>();
            builder.RegisterType<DealerService>().As<IDealerService>();
            builder.RegisterType<PowerBiService>().As<IPowerBiService>()
                .WithParameter(nameof(tenantId), tenantId)
                .WithParameter(nameof(userName), userName)
                .WithParameter(nameof(password), password)
                .WithParameter(nameof(clientId), clientId)
                .WithParameter(nameof(clientSecret), clientSecret);
            builder
                .RegisterType<CarnagyContext>()
                .WithParameter("connnectionString", Configuration.GetConnectionString("DefaultConnection"))
                .AsSelf();
            builder.Populate(services);
            ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
