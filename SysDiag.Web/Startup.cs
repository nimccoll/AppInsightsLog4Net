//===============================================================================
// Microsoft FastTrack for Azure
// Application Insights Tracing Examples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.TraceListener;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SysDiag.Web
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
            // Add the Application Insights Telemtry Client
            services.AddApplicationInsightsTelemetry();

            // Add a trace source and set its listener to the Application Insights Trace Listener
            TraceSource traceSource = new TraceSource("SysDiag.Web")
            {
                Switch = new SourceSwitch("SysDiag.Web", "Information")
            };
            traceSource.Listeners.Add(new ApplicationInsightsTraceListener(Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey")));
            services.AddSingleton<TraceSource>(traceSource);

            // Make sure to set the Application Insights InstrumentationKey on the 
            // active TelemetryConfiguration before adding the Log4Net logger. The log4net.config
            // file should be in the root directory of the web application and contain
            // the Application Insights Log4Net appender.
            TelemetryConfiguration.Active.InstrumentationKey = Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
            services.AddLogging(configure => configure.AddLog4Net("log4net.config"));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
