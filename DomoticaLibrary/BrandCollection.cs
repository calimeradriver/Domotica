using Vendjuuren.SQL;

namespace Vendjuuren.Domotica.Library
{
  public class BrandCollection : RecordCollection
  {
    private const string TableName = "Brand";

    public BrandCollection()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// 
    /// </summary>
    public override void BuildCollection()
    {
      foreach (Row row in Rows)
        Add(new Brand(row));
    }
  }
}