using System;
using AirportRouteApi.BL;
using AirportRouteApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AirportRouteApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            maxConcurrentRequests = Convert.ToInt32(Configuration.GetSection("Data").GetValue(typeof(int), "MaxConcurrentRequests"));
            routeUri = Configuration.GetSection("Data").GetValue(typeof(string), "RouteUri").ToString();
            airportUri = Configuration.GetSection("Data").GetValue(typeof(string), "AirportUri").ToString();
            airlineUri = Configuration.GetSection("Data").GetValue(typeof(string), "AirlineUri").ToString();
            logsFile = Configuration.GetSection("Data").GetValue(typeof(string), "LogsFile").ToString();
            exceptionHandler = Configuration.GetSection("Data").GetValue(typeof(string), "ExceptionHandler").ToString();
        }

        public IConfiguration Configuration { get; }

        private string routeUri;
        private string airportUri;
        private string airlineUri;
        private int maxConcurrentRequests;
        private string logsFile;
        private string exceptionHandler;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.SecurePolicy = new Microsoft.AspNetCore.Http.CookieSecurePolicy();
            });
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddTransient<IRequestsManager, RequestsManager>();
            services.AddTransient<IApiClient>(x => new ApiClient(routeUri, airportUri, airlineUri));
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
                options.AutomaticAuthentication = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IConfiguration configuration, IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(logsFile);
            app.UseExceptionHandler(exceptionHandler);
            app.UseMiddleware<MaxConcurrentRequestsMiddleware>(maxConcurrentRequests);
            app.UseSession();
            app.UseMvc();
        }
    }
}
