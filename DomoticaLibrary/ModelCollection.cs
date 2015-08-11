using Vendjuuren.SQL;

namespace Vendjuuren.Domotica.Library
{
  public class ModelCollection : RecordCollection
  {
    private const string TableName = "Model";

    public ModelCollection()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// 
    /// </summary>
    public override void BuildCollection()
    {
      foreach (Row row in Rows)
        Add(new Model(row));
    }
  }
}