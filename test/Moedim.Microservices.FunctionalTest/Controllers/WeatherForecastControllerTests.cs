using System.Net;

using Xunit.Abstractions;

namespace Moedim.Microservices.FunctionalTest.Controllers;

public class WeatherForecastControllerTests : IClassFixture<DemoWebApplicationFactory>
{
    private readonly ITestOutputHelper _output;
    private readonly DemoWebApplicationFactory _factory;

    public WeatherForecastControllerTests(
        DemoWebApplicationFactory factory,
        ITestOutputHelper output)
    {
        _output = output;
        _factory = factory;
        _factory.SetOutput(output);
    }

    [Fact]
    public async Task GetWeatherAsync()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("apiKey", Constants.ApiKey);

        var response = await client.GetAsync("v1/WeatherForecast/GetSecureWeather");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
