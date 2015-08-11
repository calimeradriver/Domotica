using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vendjuuren.Domotica.Library
{
  public class Sun : Service
  {
    /// <summary>
    /// Bron: http://sunrise-sunset.org/api
    /// Location: Jan Sluijtersstraat 28, Woerden
    /// Position: lat=52.091226 & lng=4.866822
    /// </summary>
    public Sun()
    {
      GetByID(new Guid("a97dda12-90ea-4076-b03d-18627e956949")); // SunriseSunset
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
    /// 
    /// </summary>
    /// <returns></returns>
    public override bool ResolveNeeded
    {
      get
      {
        bool resolveNeeded = !LastUpdatedSuccessful.HasValue;
        if (!resolveNeeded)
        {
          TimeSpan timeSpan = DateTime.Now.Subtract(LastUpdatedSuccessful.Value);
          // Elke dag bijwerken.
          resolveNeeded = timeSpan.Days >= 1;
        }

        return (resolveNeeded);
      }
    }

    public override void Resolve()
    {
      base.Resolve();

      Sun sun = new Sun();
      JToken root = JObject.Parse(LastResponse);
      JToken results = root["results"];
      Sun deserializedSun = JsonConvert.DeserializeObject<Sun>(results.ToString());
      Sunrise = deserializedSun.Sunrise;
      Sunset = deserializedSun.Sunset;
    }
  }
}
