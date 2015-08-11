using Vendjuuren.SQL;
using System;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class Brand : Record
  {
    private const string TableName = "Brand";
    private const string TypeColumnName = "Type";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    
    /// <summary>
    /// Serializable
    /// </summary>
    public Brand()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuw Brand
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public Brand(BrandType type, string name, string description)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      Type = type;
      Name = name;
      Description = description;
    }

    /// <summary>
    /// Bestaand Brand
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public Brand(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public void GetByName(string name)
    {
      SELECT select = new SELECT(Table);
      select.AddWhere(new Column(NameColumnName), Operator.Equals, name, Vendjuuren.SQL.Boolean.AND);
      base.GetByStatement(select);
    }

    /// <summary>
    /// Name
    /// </summary>
    public BrandType Type
    {
      get { return (Row[TypeColumnName].GetBrandType()); }
      set { Row[TypeColumnName] = value.ToString(); }
    }

    /// <summary>
    /// Name
    /// </summary>
    public string Name
    {
      get { return (Row[NameColumnName].GetString()); }
      set { Row[NameColumnName] = value; }
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description
    {
      get { return (Row[DescriptionColumnName].GetString()); }
      set { Row[DescriptionColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Name);
    }
  }
}
