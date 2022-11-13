using System.Diagnostics;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;

using Moedim.AspNetCore.Demo.Models;

namespace Moedim.AspNetCore.Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly TelemetryConfiguration _telemetryConfiguration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            TelemetryClient telemetryClient,
            TelemetryConfiguration telemetryConfiguration,
            ILogger<HomeController> logger)
        {
            _telemetryClient = telemetryClient;

            // In a real app, you wouldn't need the TelemetryConfiguration here.
            // This is included in this sample because it allows you to debug
            // and verify that the configuration at runtime matches the expected configuration.
            _telemetryConfiguration = telemetryConfiguration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _telemetryClient.TrackEvent(eventName: "Hello World!");

            // by default Application Insight Capture logs from Warning level
            _logger.LogWarning("An example of a Warning trace..");
            _logger.LogError("An example of an Error level message");

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
