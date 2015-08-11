using Vendjuuren.SQL;

namespace Vendjuuren.Domotica.Library
{
  public class ProgramCollection : RecordCollection
  {
    private const string TableName = "Program";

    public ProgramCollection()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// 
    /// </summary>
    public override void BuildCollection()
    {
      foreach (Row row in Rows)
        Add(new Program(row));
    }

    /// <summary>
    /// Delete
    /// </summary>
    public override void DeleteAll()
    {
      foreach (Program program in this)
        program.Delete();
    }
  }
}