using System;
using Vendjuuren.SQL;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class Group : Record
  {
    private const string TableName = "Group";
    private const string GroupIDColumnName = "Group_ID";
    private const string NumberColumnName = "Number";
    private const string LetterColumnName = "Letter";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    private const string DefaultPowerColumnName = "DefaultPower";
    private const string PowerColumnName = "Power";

    /// <summary>
    /// Serializable
    /// </summary>
    public Group()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuw Group
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="group"></param>
    /// <param name="number"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public Group(Letter letter, string description)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      Letter = letter;
      Description = description;
    }

    /// <summary>
    /// Bestaand Group
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public Group(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="letter"></param>
    public void GetByLetter(Letter letter)
    {
      SELECT select = new SELECT(Table);
      select.AddWhere(new Column(LetterColumnName), Operator.Equals, letter, Vendjuuren.SQL.Boolean.AND);
      base.GetByStatement(select);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Letter.ToString());
    }
    
    /// <summary>
    /// Letter
    /// </summary>
    public Letter Letter
    {
      get { return (Row[LetterColumnName].GetLetter()); }
      set { Row[LetterColumnName] = value.ToString(); }
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description
    {
      get { return (Row[DescriptionColumnName].GetString()); }
      set { Row[DescriptionColumnName] = value; }
    }
  }
}
