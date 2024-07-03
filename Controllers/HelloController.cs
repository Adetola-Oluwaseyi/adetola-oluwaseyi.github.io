using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string visitor_name)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        //if (clientIp == "::1" || clientIp == "127.0.0.1")
        //{
        //    clientIp = "102.89.47.175"; // Example IP for testing purposes
        //}

        var location = await GetLocation(clientIp);
        var temperature = await GetTemperature(location);

        var response = new
        {
            client_ip = clientIp,
            location = location,
            greeting = $"Hello, {visitor_name}!, the temperature is {temperature} degrees Celsius in {location}"
        };

        return Ok(response);
    }

    private async Task<string> GetLocation(string ip)
    {
        var apiKey = "bf438c5381a34e6ab3abdc70f8a7cd0b"; // Replace with your ipgeolocation.io API key
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync($"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}&ip={ip}");
            var json = JObject.Parse(response);
            var city = json["city"]?.ToString();

            if (string.IsNullOrEmpty(city))
            {
                city = "Unknown"; // Default value if city is not present
            }

            return city;
        }
    }

    private async Task<double> GetTemperature(string city)
    {
        // Use a third-party service to get temperature
        var apiKey = "e5c21600a80b3f07a2b68a0333492c18"; // Replace with your OpenWeather API key
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={apiKey}");
            var json = JObject.Parse(response);
            return double.Parse(json["main"]["temp"].ToString());
        }
    }
}
