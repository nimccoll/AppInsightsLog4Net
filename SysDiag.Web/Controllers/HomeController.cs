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
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SysDiag.Web.Models;
using System.Diagnostics;

namespace SysDiag.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TraceSource _traceSource;
        private TelemetryClient _telemetryClient = null;

        public HomeController(ILogger<HomeController> logger, TraceSource traceSource, TelemetryClient telemetryClient)
        {
            // .Net Core Dependency Injection Framework will inject the 
            // implementations of ILogger, TraceSource, and TelemetryClient
            _logger = logger;
            _traceSource = traceSource;
            _telemetryClient = telemetryClient;
        }

        public IActionResult Index()
        {
            // When using the TraceListener use a unique id to identity different events
            // you are logging to the trace. Trace data stored in JSON can be serialized
            // to a string for retrieval from Application Insights from the traces table.
            _traceSource.TraceData(TraceEventType.Information, 1, "{\"data\": \"Index loaded\"}");

            // When using the TelemetryClient take advantage of custom events and use a unique name
            // to identify different events you are logging. The Properties dictionary can be used to
            // add trace data to event. Information can be retrieved from the customEvents table
            // in Application Insights.
            EventTelemetry eventTelemetry = new EventTelemetry() { Name = "Index loaded" };
            eventTelemetry.Properties.Add("FirstName", "Nick");
            eventTelemetry.Properties.Add("LastName", "McCollum");
            _telemetryClient.TrackEvent(eventTelemetry);

            // When using the logger implementation (in this case Log4Net) you will need to somehow
            // identify the event in the payload since it will just be placed in the default message
            // column in the Application Insights traces table. Log data stored in JSON can be serialized
            // to a string for retrieval from Application Insights.
            _logger.LogInformation("{\"source\":\"log4net\", \"event\":\"Index loaded\", \"FirstName\":\"Nick\", \"LastName\":\"McCollum\"}");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
