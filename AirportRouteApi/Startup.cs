using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using AirportRouteApi.BL;
using AirportRouteApi.BL.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.SecurePolicy = new Microsoft.AspNetCore.Http.CookieSecurePolicy();
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<RouteParams>(x => new RouteParams()
            {
                RouteUri = Configuration.GetSection("Data").GetValue(typeof(string), "RouteUri").ToString(),
                AirportUri = Configuration.GetSection("Data").GetValue(typeof(string), "AirportUri").ToString(),
                AirlineUri = Configuration.GetSection("Data").GetValue(typeof(string), "AirlineUri").ToString(),
                MaxRequestCount = Convert.ToInt32(Configuration.GetSection("Data").GetValue(typeof(int), "MaxRequestCount"))
            });
            services.AddSingleton<RequestParams>(x => new RequestParams()
            {
                MaxConcurrentRequests = Convert.ToInt32(Configuration.GetSection("Data").GetValue(typeof(int), "MaxConcurrentRequests")),
                MaxTransferCount = Convert.ToInt32(Configuration.GetSection("Data").GetValue(typeof(int), "MaxTransferCount"))
            });
            services.AddTransient<IRequestsManager, RequestsManager>();
            services.AddTransient<IHttpSender, HttpSender>();
            services.AddTransient<IApiClient, ApiClient>();
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
                options.AutomaticAuthentication = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IConfiguration configuration, IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            loggerFactory.AddFile(configuration.GetSection("Data").GetValue(typeof(string), "LogsFile").ToString());
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var logger = (ILogger<Startup>)app.ApplicationServices.GetService(typeof(ILogger<Startup>));

                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var ex = feature?.Error;
                    logger.LogError(ex, ex.StackTrace);

                    var isDev = env.IsDevelopment();
                    var problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Instance = feature?.Path,
                        Title = isDev ? $"{ex.GetType().Name}: {ex.Message}" : ErrorMessages.ExceptionError,
                        Detail = isDev ? ex.StackTrace : null,
                    };

                    byte[] bytes;
                    BinaryFormatter bf = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, problemDetails);
                        bytes = ms.ToArray();
                    }

                    context.Response.StatusCode = problemDetails.Status.Value;
                    await context.Response.Body.WriteAsync(bytes);
                });
            });
            app.UseSession();
            app.UseMvc();
        }
    }
}
