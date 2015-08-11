using Vendjuuren.SQL;
using System;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class Model : Record
  {
    private const string TableName = "Model";
    private const string BrandIDColumnName = "Brand_ID";
    private const string TypeColumnName = "Type";
    private const string DescriptionColumnName = "Description";
    private const string DimmableColumnName = "Dimmable";
    private const string TwoWayColumnName = "TwoWay";

    /// <summary>
    /// Serializable
    /// </summary>
    public Model()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuw Model
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="brand"></param>
    /// <param name="type"></param>
    /// <param name="dimmable"></param>
    /// <param name="twoWay"></param>
    public Model(Brand brand, string type, bool dimmable, bool twoWay)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      Brand = brand;
      Type = type;
      Dimmable = dimmable;
      TwoWay = twoWay;
    }

    /// <summary>
    /// Bestaand Model
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public Model(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public void GetByBrandAndType(Brand brand, string type)
    {
      SELECT select = new SELECT(Table);
      select.AddWhere(new Column(BrandIDColumnName), Operator.Equals, brand.ID, Vendjuuren.SQL.Boolean.AND);
      select.AddWhere(new Column(TypeColumnName), Operator.Equals, type, Vendjuuren.SQL.Boolean.AND);
      base.GetByStatement(select);
    }

    /// <summary>
    /// GroupID
    /// </summary>
    public Guid BrandID
    {
      get { return (Row[BrandIDColumnName].GetGuid()); }
      set { Row[BrandIDColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private Brand _brand;
    public Brand Brand
    {
      get
      {
        if (_brand == null)
        {
          _brand = new Brand();
          _brand.GetByID(BrandID);
        }

        return (_brand);
      }
      set
      {
        _brand = value;
        BrandID = _brand.ID;
      }
    }

    /// <summary>
    /// Type
    /// </summary>
    public string Type
    {
      get { return (Row[TypeColumnName].GetString()); }
      set { Row[TypeColumnName] = value; }
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
    /// Dimmable
    /// </summary>
    public bool Dimmable
    {
      get { return (Row[DimmableColumnName].GetBool()); }
      set { Row[DimmableColumnName] = value; }
    }

    /// <summary>
    /// Two-way
    /// </summary>
    public bool TwoWay
    {
      get { return (Row[TwoWayColumnName].GetBool()); }
      set { Row[TwoWayColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Brand.Name + " " + Type);
    }
  }
}
