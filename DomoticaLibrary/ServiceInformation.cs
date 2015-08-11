using System;
using Newtonsoft.Json;

namespace Vendjuuren.Domotica.Library
{
  public class ServiceInformation
  {
    /// <summary>
    /// Wordt gebruikt voor algemene informatie over de service.
    /// </summary>
    public ServiceInformation()
    { }

    public ServiceInformation(Sun sun, Weather weather)
    {
      sun.Resolve();
      Sunrise = sun.Sunrise;
      Sunset = sun.Sunset;
      weather.Resolve();
      Temperature = weather.Temperature;
    }

    /// <summary>
    /// Sunrise
    /// </summary>
    [JsonProperty("sunrise")]
    public DateTime Sunrise { get; set; }

    /// <summary>
    /// Sunset
    /// </summary>
    [JsonProperty("sunset")]
    public DateTime Sunset { get; set; }

    /// <summary>
    /// Temperature
    /// </summary>
    [JsonProperty("temp")]
    public int Temperature { get; set; }
  }
}
