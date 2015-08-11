using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vendjuuren.Domotica.Library
{
  public class Weather : Service
  {
    /// <summary>
    /// Bron: http://www.myweather2.com/
    /// Location: Jan Sluijtersstraat 28, Woerden
    /// Position: lat=52.091226 & lng=4.866822
    /// </summary>
    public Weather()
    {
      GetByID(new Guid("7B7C915B-B134-4083-895D-84E950FCE066")); // Weather
    }

    /// <summary>
    /// Temperature
    /// </summary>
    [JsonProperty("temp")]
    public int Temperature { get; set; }

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
          // Iedere 5 minuten bijwerken.
          resolveNeeded = timeSpan.Minutes >= 5;
        }

        return (resolveNeeded);
      }
    }

    public override void Resolve()
    {
      base.Resolve();

      Weather weather = new Weather();
      JToken root = JObject.Parse(LastResponse);
      JToken currentWeather = root["weather"]["curren_weather"].First;
      Weather deserializedWeather = JsonConvert.DeserializeObject<Weather>(currentWeather.ToString());
      Temperature = deserializedWeather.Temperature;
    }
  }
}