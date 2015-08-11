using System;
using System.Text;

namespace Vendjuuren.Domotica.Radiographically
{
  public class SendCode
  {
    /// <summary>
    /// Serializable
    /// </summary>
    public SendCode()
    { }

    public SendCode(string groupLetter, int number)
    {
      _groupLetter = groupLetter;
      _number = number;
      _transmitCode = getSendCode();
    }

    /// <summary>
    /// Build transmit code
    /// </summary>
    /// <returns></returns>
    private int getSendCode()
    {
      //Scode = (Cijfercode-1) x 16 + ascii(uppercase(Lettercode)-65 (I5)
      byte[] asciiCharacters = Encoding.ASCII.GetBytes(_groupLetter.ToString());
      return ((_number - 1) * 16 + asciiCharacters[0] - 65);
    }

    public override string ToString()
    {
      return _groupLetter + _number;
    }

    /// <summary>
    /// Device group (A - J)
    /// </summary>
    private string _groupLetter;
    public string GroupLetter
    {
      get { return _groupLetter; }
      set { _groupLetter = value; }
    }

    /// <summary>
    /// Device number
    /// </summary>
    private int _number;
    public int Number
    {
      get { return _number; }
      set { _number = value; }
    }

    /// <summary>
    /// Code to send to devices
    /// </summary>
    private int _transmitCode;
    public int TransmitCode
    {
      get { return _transmitCode; }
      set { _transmitCode = value; }
    }
   
  }
}
