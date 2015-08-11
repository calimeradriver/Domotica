using System;
using System.Net;
using Newtonsoft.Json;
using Vendjuuren.SQL;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public abstract class Service : Record
  {
    private const string TableName = "Service";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    private const string UrlColumnName = "Url";
    private const string EnabledColumnName = "Enabled";
    private const string LastResponseColumnName = "LastResponse";
    private const string ErrorDetectedColumnName = "ErrorDetected";
    private const string LastUpdatedSuccessfulColumnName = "LastUpdatedSuccessful";

    /// <summary>
    /// Serializable
    /// </summary>
    public Service()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuwe Service
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public Service(string name, string description)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      Name = name;
      Description = description;
    }

    /// <summary>
    /// Bestaande Service
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public Service(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// Doe een request naar de service en download het response.
    /// </summary>
    /// <returns>Succesvol</returns>
    public virtual void Resolve()
    {
      if (Enabled && ResolveNeeded)
      {
        using (WebClient webclient = new WebClient())
        {
          webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
          try
          {
            LastResponse = webclient.DownloadString(Url);
            ErrorDetected = false;
            LastUpdatedSuccessful = DateTime.Now;

            new Log(LogType.Information,
          LogAction.SchedulerResolvedData, string.Format("Service: {0}, Url: {1}", Name, Url.OriginalString));
          }
          catch (WebException wex)
          {
            ErrorDetected = true;
            new Log(LogType.Error, LogAction.ServiceException, wex.Message);
          }
          finally
          {
            Enabled = !ErrorDetected;
            Save();
          }
        }
      }
    }

    public abstract bool ResolveNeeded {get;}

    ///// <summary>
    ///// Delete Service
    ///// </summary>
    //public override void Delete()
    //{
    //  base.Delete();

    //  // Verwijder; direct gekoppelde records
    //  if (Floor != null)
    //    Floor.TryDelete();
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Name + ", " + Description);
    }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty()]
    public string Name
    {
      get { return (Row[NameColumnName].GetString()); }
      set { Row[NameColumnName] = value; }
    }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty()]
    public string Description
    {
      get { return (Row[DescriptionColumnName].GetString()); }
      set { Row[DescriptionColumnName] = value; }
    }

    /// <summary>
    /// Url
    /// </summary>
    [JsonProperty()]
    public Uri Url
    {
      get { return (Row[UrlColumnName].GetUri()); }
      set { Row[UrlColumnName] = value; }
    }

    /// <summary>
    /// Enabled
    /// </summary>
    [JsonProperty()]
    public bool Enabled
    {
      get { return (Row[EnabledColumnName].GetBool()); }
      set { Row[EnabledColumnName] = value; }
    }

    /// <summary>
    /// Enabled
    /// </summary>
    [JsonProperty()]
    public string LastResponse
    {
      get { return (Row[LastResponseColumnName].GetString()); }
      set { Row[LastResponseColumnName] = value; }
    }

    /// <summary>
    /// Enabled
    /// </summary>
    [JsonProperty()]
    public bool ErrorDetected
    {
      get { return (Row[ErrorDetectedColumnName].GetBool()); }
      set { Row[ErrorDetectedColumnName] = value; }
    }

    /// <summary>
    /// Enabled
    /// </summary>
    [JsonProperty()]
    public DateTime? LastUpdatedSuccessful
    {
      get { return (Row[LastUpdatedSuccessfulColumnName].GetDateTime()); }
      set { Row[LastUpdatedSuccessfulColumnName] = value; }
    }
  }
}