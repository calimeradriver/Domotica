/*       Copyright (c) Eric Ledoux.  All rights reserved.       */
/* See http://www.dwell.net/terms for code sharing information. */

// Cm11.cs
//
// Implements the DwellNet.Cm11 class.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Vendjuuren.Domotica.X10
{

  /// <summary>
  /// Controls a CM11 device (or an equivalent, such as CM11A) connected
  /// through a serial port.
  /// </summary>
  ///
  /// <remarks>
  /// See <a href="Default.htm">Dwell.Net CM11 Introduction</a> for information
  /// about how to use the <n>Cm11</n> class.
  /// </remarks>
  ///
  /// <example>
  /// The following example turns on the X10 device with address "A1" (i.e.
  /// house code "A", device code "1"), and then waits until the command
  /// completes (successfully or not).
  /// <code>
  /// using (Cm11 cm11 = new Cm11())
  /// {
  ///     cm11.Open("COM1");
  ///     cm11.Execute("A1 On");
  /// 	cm11.WaitUntilIdle();
  /// }
  /// </code>
  /// The following code is equivalent -- it uses the higher-level
  /// <r>TurnOnDevice</r> method.
  /// <code>
  /// using (Cm11 cm11 = new Cm11())
  /// {
  ///     cm11.Open("COM1");
  ///     cm11.TurnOnDevice("A1");
  /// 	cm11.WaitUntilIdle();
  /// }
  /// </code>
  /// </example>
  ///
  /// <seealso href="Default.htm">Dwell.Net CM11 Introduction</seealso>
  ///
  public class Cm11 : IDisposable
  {
    public Cm11()
    { }
    //////////////////////////////////////////////////////////////////////////
    // Private Constants and Statics
    //

    #region constants
    /// <summary>
    ///   Looks up a localized string similar to acknowledging checksum.
    /// </summary>
    const string AckChecksum = "acknowledging checksum";

    /// <summary>
    ///   Looks up a localized string similar to acknowledging notification.
    /// </summary>
    const string AckNotification = "acknowledging notification";

    /// <summary>
    ///   Looks up a localized string similar to Cm11.Open was already called.
    /// </summary>
    const string AlreadyOpen = "Cm11.Open was already called";

    /// <summary>
    ///   Looks up a localized string similar to Invalid brighten or dim percentage {0}; must be between 0 and 100, inclusive..
    /// </summary>
    const string BrightenOrDimAmountOutOfRange = "Invalid brighten or dim percentage {0}; must be between 0 and 100, inclusive.";

    /// <summary>
    ///   Looks up a localized string similar to CM11 --&gt; {0:X2} (acknowledgement reply).
    /// </summary>
    const string Cm11AckReply = "CM11 --&gt; {0:X2} (acknowledgement reply)";

    /// <summary>
    ///   Looks up a localized string similar to CM11 --&gt; {0:X2} (checksum).
    /// </summary>
    const string Cm11Checksum = "CM11 --&gt; {0:X2} (checksum)";

    /// <summary>
    ///   Looks up a localized string similar to CM11 --&gt; {0:X2} ({1}).
    /// </summary>
    const string Cm11Notification = "CM11 --&gt; {0:X2} (data length = {0} bytes)";

    /// <summary>
    ///   Looks up a localized string similar to CM11 --&gt; {0:X2} (data length = {0} bytes).
    /// </summary>
    const string Cm11NotificationDataLength = "CM11 --&gt; {0:X2} (data length = {0} bytes)";

    /// <summary>
    ///   Looks up a localized string similar to Timeout waiting for CM11 reply.
    /// </summary>
    const string Cm11ReplyTimeout = "Timeout waiting for CM11 reply";

    /// <summary>
    ///   Looks up a localized string similar to request to set clock.
    /// </summary>
    const string Cm11TimeRequestNotification = "request to set clock";

    /// <summary>
    ///   Looks up a localized string similar to CM11 --&gt; {0} (unexpected bytes).
    /// </summary>
    const string Cm11UnexpectedBytes = "CM11 --&gt; {0} (unexpected bytes)";

    /// <summary>
    ///   Looks up a localized string similar to unknown notification: ignored.
    /// </summary>
    const string Cm11UnknownNotification = "unknown notification: ignored";

    /// <summary>
    ///   Looks up a localized string similar to device notification.
    /// </summary>
    const string Cm11X10DeviceNotification = "device notification";

    /// <summary>
    ///   Looks up a localized string similar to Command failed despite {0} retries.
    /// </summary>
    const string CommandRetryCountExceeded = "Command failed despite {0} retries";

    /// <summary>
    ///   Looks up a localized string similar to &quot;{0}&quot; command.
    /// </summary>
    const string CommandType = "\"{0}\" command";

    /// <summary>
    ///   Looks up a localized string similar to Error communicating with the CM11 device: {0}.
    /// </summary>
    const string CommunicationError = "Error communicating with the CM11 device: {0}";

    /// <summary>
    ///   Looks up a localized string similar to device notification data.
    /// </summary>
    const string DeviceNotificationData = "device notification data";

    /// <summary>
    ///   Looks up a localized string similar to Error accessing serial port &quot;{0}&quot;.
    /// </summary>
    const string DnSerialPort_SerialPortError = "Error accessing serial port \"{0}\"";

    /// <summary>
    ///   Looks up a localized string similar to Serial port reset complete.
    /// </summary>
    const string HardResyncComplete = "Serial port reset complete";

    /// <summary>
    ///   Looks up a localized string similar to A serious CM11 communication error required the serial port to be reset.  {0} command(s) were discarded..
    /// </summary>
    const string HardResyncOccurred = "A serious CM11 communication error required the serial port to be reset.  {0} command(s) were discarded.";

    /// <summary>
    ///   Looks up a localized string similar to The house codes of all consecutive address commands must be the same.  For example, &quot;A1 A2 Dim70&quot; is valid, but &quot;A1 B2 Dim70&quot; is not..
    /// </summary>
    const string InconsistentHouseCodes = "The house codes of all consecutive address commands must be the same.  For example, \"A1 A2 Dim70\" is valid, but \"A1 B2 Dim70\" is not.";

    /// <summary>
    ///   Looks up a localized string similar to Incorrect checksum (expecting 0x{0:X2}).
    /// </summary>
    const string IncorrectChecksum = "Incorrect checksum (expecting 0x{0:X2})";

    /// <summary>
    ///   Looks up a localized string similar to Incorrect reply to checksum acknowledgement (expecting 0x55).
    /// </summary>
    const string IncorrectChecksumAckReply = "Incorrect reply to checksum acknowledgement (expecting 0x55)";

    /// <summary>
    ///   Looks up a localized string similar to Invalid device address &quot;{0}&quot;.  A valid address is something like &quot;A1&quot; or &quot;P16&quot;..
    /// </summary>
    const string InvalidAddress = "Invalid device address &quot;{0}&quot;.  A valid address is something like &quot;A1&quot; or &quot;P16&quot;.";

    /// <summary>
    ///   Looks up a localized string similar to Invalid command &quot;{0}&quot;.  A valid command is something like &quot;A1&quot; or &quot;On&quot; or &quot;Dim70&quot;.  Commands are case-sensitive..
    /// </summary>
    const string InvalidCommand = "Invalid command &quot;{0}&quot;.  A valid command is something like &quot;A1&quot; or &quot;On&quot; or &quot;Dim70&quot;.  Commands are case-sensitive.";

    /// <summary>
    ///   Looks up a localized string similar to Invalid device code &quot;{0}&quot;.  Valid device codes are &quot;1&quot; through &quot;16&quot;..
    /// </summary>
    const string InvalidDeviceCode = "Invalid device code &quot;{0}&quot;.  Valid device codes are &quot;1&quot; through &quot;16&quot;.";

    /// <summary>
    ///   Looks up a localized string similar to Invalid hexadecimal digits in &quot;{0}&quot; command.  A nonzero even number of hexadecimal digits is required..
    /// </summary>
    const string InvalidHexDigits = "Invalid hexadecimal digits in &quot;{0}&quot; command.  A nonzero even number of hexadecimal digits is required.";

    /// <summary>
    ///   Looks up a localized string similar to Invalid house code &quot;{0}&quot;.  Valid house codes are &quot;A&quot; through &quot;P&quot;..
    /// </summary>
    const string InvalidHouseCode = "Invalid house code &quot;{0}&quot;.  Valid house codes are &quot;A&quot; through &quot;P&quot;.";

    /// <summary>
    ///   Looks up a localized string similar to Invalid reply from CM11.
    /// </summary>
    const string InvalidReply = "Invalid reply from CM11";

    /// <summary>
    ///   Looks up a localized string similar to Function command &quot;{0}&quot; cannot execute because there is no current house code.  Precede this function command with a house code prefix (e.g. &quot;A.&quot;)..
    /// </summary>
    const string NoHouseCode = "Function command &quot;{0}&quot; cannot execute because there is no current house code.  Precede this function command with a house code prefix (e.g. &quot;A.&quot;).";

    /// <summary>
    ///   Looks up a localized string similar to Cm11.Open was not called.
    /// </summary>
    const string NotOpen = "Cm11.Open was not called";

    /// <summary>
    ///   Looks up a localized string similar to Unexpected CM11 behavior -- pausing communications for {0} milliseconds to clear CM11 state.
    /// </summary>
    const string Pausing = "Unexpected CM11 behavior -- pausing communications for {0} milliseconds to clear CM11 state";

    /// <summary>
    ///   Looks up a localized string similar to Returned 0x{0:X2} to the serial input buffer for reprocessing.
    /// </summary>
    const string PushedByteBack = "Returned 0x{0:X2} to the serial input buffer for reprocessing";

    /// <summary>
    ///   Looks up a localized string similar to Retrying command.
    /// </summary>
    const string RetryingCommand = "Retrying command";

    /// <summary>
    ///   Looks up a localized string similar to Serial port hardware error: {0}.
    /// </summary>
    const string SerialPortError = "Serial port hardware error: {0}";

    /// <summary>
    ///   Looks up a localized string similar to setting clock.
    /// </summary>
    const string SettingCm11Clock = "setting clock";
    #endregion

    /// <summary>
    /// Used to convert a house code ('A' through 'P') or a device code
    /// (1 through 15) to a nibble value used in the X10 protocol.
    /// </summary>
    static byte[] s_codesToNibbles = new byte[]
	{
		0x6,	// binary 0110: house code "A" or device code "1"
		0xE,	// binary 1110: house code "B" or device code "2"
		0x2,	// binary 0010: house code "C" or device code "3"
		0xA,	// binary 1010: house code "D" or device code "4"
		0x1,	// binary 0001: house code "E" or device code "5"
		0x9,	// binary 1001: house code "F" or device code "6"
		0x5,	// binary 0101: house code "G" or device code "7"
		0xD,	// binary 1101: house code "H" or device code "8"
		0x7,	// binary 0111: house code "I" or device code "9"
		0xF,	// binary 1111: house code "J" or device code "10"
		0x3,	// binary 0011: house code "K" or device code "11"
		0xB,	// binary 1011: house code "L" or device code "12"
		0x0,	// binary 0000: house code "M" or device code "13"
		0x8,	// binary 1000: house code "N" or device code "14"
		0x4,	// binary 0100: house code "O" or device code "15"
		0xC,	// binary 1100: house code "P" or device code "16"
	};

    /// <summary>
    /// Used to convert a nibble value used in the X10 protocol to a house
    /// code ('A' through 'P') or a device code (1 through 15).  Maps a nibble
    /// value to the a zero-based index
    /// </summary>
    ///
    static byte[] s_nibblesToCodes = new byte[]
	{
		12,	// 0x0 binary 0000: house code "M" or device code "13"
		4,	// 0x1 binary 0001: house code "E" or device code "5"
		2,	// 0x2 binary 0010: house code "C" or device code "3"
		10,	// 0x3 binary 0011: house code "K" or device code "11"
		14,	// 0x4 binary 0100: house code "O" or device code "15"
		6,	// 0x5 binary 0101: house code "G" or device code "7"
		0,	// 0x6 binary 0110: house code "A" or device code "1"
		8,	// 0x7 binary 0111: house code "I" or device code "9"
		13,	// 0x8 binary 1000: house code "N" or device code "14"
		5,	// 0x9 binary 1001: house code "F" or device code "6"
		3,	// 0xA binary 1010: house code "D" or device code "4"
		11,	// 0xB binary 1011: house code "L" or device code "12"
		15,	// 0xC binary 1100: house code "P" or device code "16"
		7,	// 0xD binary 1101: house code "H" or device code "8"
		1,	// 0xE binary 1110: house code "B" or device code "2"
		9,	// 0xF binary 1111: house code "J" or device code "10"
	};

    /// <summary>
    /// X10 <i>function code nibble</i> values.
    /// </summary>
    ///
    enum X10Function
    {
      /// <summary>
      /// 0000 = All Units Off
      /// </summary>
      AllOff = 0,

      /// <summary>
      /// 0001 = All Lights On
      /// </summary>
      AllLightsOn = 1,

      /// <summary>
      /// 0010 = On
      /// </summary>
      On = 2,

      /// <summary>
      /// 0011 = Off
      /// </summary>
      Off = 3,

      /// <summary>
      /// 0100 = Dim
      /// </summary>
      Dim = 4,

      /// <summary>
      /// 0101 = Brighten
      /// </summary>
      Brighten = 5,

      /// <summary>
      /// 0110 = All Lights Off
      /// </summary>
      AllLightsOff = 6,

      /// <summary>
      /// 0111 = Extended Code
      /// </summary>
      ExtCode = 7,

      /// <summary>
      /// 1000 = Hail Request
      /// </summary>
      HailReq = 8,

      /// <summary>
      /// 1001 = Hail Acknowledge
      /// </summary>
      HailAck = 9,

      /// <summary>
      /// 1010 = Preset Dim 1
      /// </summary>
      PresetDim1 = 10,

      /// <summary>
      /// 1011 = Preset Dim 2
      /// </summary>
      PresetDim2 = 11,

      /// <summary>
      /// 1100 = Extended Data Transfer
      /// </summary>
      ExtDataXfer = 12,

      /// <summary>
      /// 1101 = Status On
      /// </summary>
      StatusOn = 13,

      /// <summary>
      /// 1110 = Status Off
      /// </summary>
      StatusOff = 14,

      /// <summary>
      /// 1111 = Status Request
      /// </summary>
      StatusReq = 15
    };


    /// <summary>
    /// A regular expression that matches an X10 address or *address command*,
    /// e.g. "A1" or "P16".  Note that some invalid address, such as "A99",
    /// are also matched.
    /// </summary>
    Regex s_addressRegex = new Regex(@"^([A-P])(\d{1,2})$");

    /// <summary>
    /// A regular expression that matches a command of the form
    /// "Dim&lt;percent&gt;", with an optional house code prefix; for example,
    /// "A.Dim0", "Dim50", and "Dim100".  Note that some invalid commands,
    /// such as "Dim999", are also matched.
    /// </summary>
    Regex s_dimRegex = new Regex(@"^(?:([A-P])\.)?Dim(\d{0,4})$");

    /// <summary>
    /// A regular expression that matches a command of the form
    /// "Brighten&lt;percent&gt;", with an optional house code prefix; for
    /// example, "A.Brighten0", "Brighten50", and "Brighten100".  Note that
    /// some invalid commands, such as "Brighten999", are also matched.
    /// </summary>
    Regex s_brightenRegex = new Regex(@"^(?:([A-P])\.)?Brighten(\d{0,4})$");

    /// <summary>
    /// A regular expression that matches a *function command* with an optional
    /// house code prefi; for example, "A.On" or "On".  However, this is very
    /// loose match; "Abcde" is also matched, for example.
    /// </summary>
    Regex s_functionRegex = new Regex(@"^(?:([A-P])\.)?([A-Za-z0-9]+)$");

    /// <summary>
    /// A regular expression that matches a command of the form
    /// "Hex&lt;hex-digits&gt;"; for example, "Hex046E".  Note that some
    /// invalid commands, such as "Hex1A2" (odd number of hexadecimal digits),
    /// are also matched.
    /// </summary>
    Regex s_hexRegex = new Regex(@"^Hex([0-9a-fA-F]*)$");

    /// <summary>
    /// The maximum number of milliseconds to wait for a response from the
    /// CM11 device.
    /// </summary>
    const int SERIAL_TIMEOUT = 5000;

    //////////////////////////////////////////////////////////////////////////
    // Private Fields
    //

    /// <summary>
    /// <r>m_lock</r> is locked while a thread accesses any state of this
    /// object, to serialize access to the object.  Exception: Access to
    /// the command queue (<r>m_commandQueue</r>) is serialized using
    /// <c>lock (m_commandQueue)</c> so that the application isn't blocked from
    /// queuing new commands while an existing message is being transmitted.
    /// </summary>
    object m_lock = new object();

    /// <summary>
    /// Holds the value of the <r>IsOpen</r> property.
    /// </summary>
    bool m_isOpen;

    /// <summary>
    /// The managed thread ID of the worker thread.
    /// </summary>
    int m_workerThreadId;

    /// <summary>
    /// The <n>WaitHandle</n> of the worker thread.  This <n>WaitHandle</n> is
    /// set when the <r>WorkerThread</r> method exits.
    /// </summary>
    WaitHandle m_workerThreadWaitHandle;

    /// <summary>
    /// Signaled when it's time to "wake up" the worker thread (if it's
    /// sleeping).
    /// </summary>
    ///
    /// <remarks>
    /// For example, <c>m_wakeWorkerThread.Set</c> is called when
    /// serial data is received from the CM11, or a new command is added to
    /// <r>m_commandQueue</r>, or <r>Close</r> is called.  Also, when the
    /// worker thread is initially created, <r>m_wakeWorkerThread</r> is used
    /// in the opposite way: the application temporarily blocks on
    /// <r>m_wakeWorkerThread</r> until the worker thread begins running,
    /// so that we can be sure that <r>m_workerThreadId</r> is set before
    /// <r>Open</r> returns.
    /// </remarks>
    ///
    AutoResetEvent m_wakeWorkerThread = new AutoResetEvent(false);

    /// <summary>
    /// Signaled when <r>m_commandQueue</r> becomes empty.
    /// </summary>
    ///
    ManualResetEvent m_idleEvent = new ManualResetEvent(true);

    /// <summary>
    /// True while no commands are being processed.
    /// </summary>
    ///
    bool m_idle = true;

    /// <summary>
    /// Set to true when it's time to dispose of this <r>Cm11</r> object.  Once
    /// set to true, further calls to <r>Execute</r> will silently fail, and
    /// messages already in the message queue won't be sent.
    /// </summary>
    bool m_quitting;

    /// <summary>
    /// Holds the value of the <r>SerialPortName</r> property.
    /// </summary>
    string m_serialPortName;

    /// <summary>
    /// The serial port used to communicate with the CM11 device.
    /// </summary>
    DnSerialPort m_serialPort;

    /// <summary>
    /// The queue of <r>DwellNet.Cm11</r> commands to execute.
    /// </summary>
    ///
    Queue<string> m_commandQueue = new Queue<string>(20);

    /// <summary>
    /// Incremented each time the command queue is modified.  Used to track
    /// whether the command queue may have changed between two points in time.
    /// </summary>
    long m_commandQueueChangeCount = 0;

    /// <summary>
    /// Tracks which X10 devices are <i>currently addressed</i>.  (See
    /// Cm11.doc and <r>AddressTracker</r> for more information.)
    /// </summary>
    AddressTracker m_addressTracker = new AddressTracker(false);

#if DEBUG
    /// <summary>
    /// Tracks time from when this object was created, for debugging purposes.
    /// </summary>
    Stopwatch m_traceStopwatch = Stopwatch.StartNew();
#endif

    /// <summary>
    /// Holds the value of the <r>InvokeEventsUsing</r> property.
    /// </summary>
    ISynchronizeInvoke m_invokeEventsUsing;

    //////////////////////////////////////////////////////////////////////////
    // Public Properties
    //

    /// <summary>
    /// Gets the serial port that the CM11 is connected to; for example,
    /// "COM1".
    /// </summary>
    ///
    [Browsable(false)]
    public string SerialPortName
    {
      get
      {
        return m_serialPortName;
      }
    }

    /// <summary>
    /// Gets a value indicating the open or closed status of the
    /// <r>Cm11</r> object. 
    /// </summary>
    ///
    [Browsable(false)]
    public bool IsOpen
    {
      get
      {
        return m_isOpen;
      }
    }

    /// <summary>
    /// Gets or sets an application-provided <r>ISynchronizeInvoke</r>
    /// instance to use to invoke methods.  Provides a way to invoke events
    /// on an application-provided thread, for situations in which firing an
    /// event on the internal worker thread would cause cross-thread errors.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// If <r>InvokeEventsUsing</r> is set to an application-provided
    /// <n>ISynchronizeInvoke</n> instance, then <r>Cm11</r> will call
    /// <n>ISynchronizeInvoke.Invoke</n> to fire events, provided that
    /// <n>ISynchronizeInvoke.InvokeRequired</n> is true.  Otherwise,
    /// <r>Cm11</r> will invoke event delegates directly.
    /// </para>
    /// <para>
    /// If you're writing a Windows Forms application that uses <r>Cm11</r> and
    /// you get a "Cross-thread operation not valid" error when <r>Cm11</r>
    /// fires an event, you can often solve this problem by setting the
    /// <r>InvokeEventsUsing</r> to the <n>Form</n> instance; for example:
    /// <code>
    /// cm11.InvokeEventsUsing = Form1;
    /// </code>
    /// </para>
    /// </remarks>
    ///
    [Description("Runs events on the thread provided by this object.  For Windows Forms applications, set this to the form.")]
    public ISynchronizeInvoke InvokeEventsUsing
    {
      get
      {
        return m_invokeEventsUsing;
      }
      set
      {
        m_invokeEventsUsing = value;
      }
    }

    //////////////////////////////////////////////////////////////////////////
    // Public Events
    //

    /// <summary>
    /// Fired when an "On" command is transmitted by a controller or device
    /// on the X10 network.
    /// </summary>
    ///
    /// <remarks>
    /// This event is fired once for each device that the "On" command applies
    /// to.
    /// </remarks>
    ///
    [Description("Occurs when an \"On\" command is transmitted by a controller or device on the X10 network.")]
    public event Cm11DeviceNotificationEventDelegate OnReceived;

    /// <summary>
    /// Fired when an "Off" command is transmitted by a controller or device
    /// on the X10 network.
    /// </summary>
    ///
    /// <remarks>
    /// This event is fired once for each device that the "Off" command applies
    /// to.
    /// </remarks>
    ///
    [Description("Occurs when an \"Off\" command is transmitted by a controller or device on the X10 network.")]
    public event Cm11DeviceNotificationEventDelegate OffReceived;

    /// <summary>
    /// Fired when a "Brighten" command is transmitted by a controller or
    /// device on the X10 network.
    /// </summary>
    ///
    /// <remarks>
    /// This event is fired once for each device that the "Brighten" command
    /// applies to.
    /// </remarks>
    ///
    [Description("Occurs when a \"Brighten\" command is transmitted by a controller or device on the X10 network.")]
    public event Cm11BrightenOrDimNotificationEventDelegate BrightenReceived;

    /// <summary>
    /// Fired when a "Dim" command is transmitted by a controller or device
    /// on the X10 network.
    /// </summary>
    ///
    /// <remarks>
    /// This event is fired once for each device that the "Dim" command
    /// applies to.
    /// </remarks>
    ///
    [Description("Occurs when a \"Dim\" command is transmitted by a controller or device on the X10 network.")]
    public event Cm11BrightenOrDimNotificationEventDelegate DimReceived;

    /// <summary>
    /// Fired when an "AllLightsOn" command is transmitted by a controller or
    /// device on the X10 network.
    /// </summary>
    ///
    [Description("Occurs when an \"AllLightsOn\" command is transmitted by a controller or device on the X10 network.")]
    public event Cm11HouseNotificationEventDelegate AllLightsOnReceived;

    /// <summary>
    /// Fired when an "AllLightsOff" command is transmitted by a controller
    /// or device on the X10 network.
    /// </summary>
    ///
    [Description("Occurs when an \"AllLightsOff\" command is transmitted by a controller or device on the X10 network.")]
    public event Cm11HouseNotificationEventDelegate AllLightsOffReceived;

    /// <summary>
    /// Fired when an "AllOff" command is transmitted by a controller or
    /// device on the X10 network.
    /// </summary>
    ///
    [Description("Occurs when an \"AllOff\" command is transmitted by a controller or device on the X10 network.")]
    public event Cm11HouseNotificationEventDelegate AllOffReceived;

    /// <summary>
    /// Fired when a notification of an event on the X10 network is received
    /// from the CM11 hardware.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// This is a lower-level event than events such as <r>OnReceived</r> and
    /// <r>OffReceived</r>.  For example, if the "Unit 1 On" button is pressed
    /// on an X10 controller set to house code "A", three events are fired:
    /// (1) a "Notification" event with command name "A1"; a "Notification"
    /// event with a command name "On"; (3) a "OnReceived" event with address
    /// "A1".  Applications can choose to handle the low-level events, the
    /// high-level events, both, or neither; handling low-level events requires
    /// that the application keep track of which X10 devices are <i>currently
    /// addressed</i>.
    /// </para>
    /// <para>
    /// This event is fired on a thread other than the thread which created
    /// the <r>Cm11</r> object, unless <r>InvokeEventsUsing</r> is used.
    /// </para>
    /// </remarks>
    ///
    [Description("Occurs when a notification of an event on the X10 network is received from the CM11 hardware.")]
    public event Cm11LowLevelNotificationEventDelegate Notification;

    /// <summary>
    /// Fired when the <r>Cm11</r> object changes from processing commands to
    /// being idle, or vice versa.
    /// </summary>
    ///
    /// <remarks>
    /// This event is fired on a thread other than the thread which created
    /// the <r>Cm11</r> object, unless <r>InvokeEventsUsing</r> is used.
    /// </remarks>
    ///
    [Description("Occurs when the Cm11 object changes from processing commands to being idle, or vice versa.")]
    public event Cm11IdleStateChangeEventDelegate IdleStateChange;

    /// <summary>
    /// Fired when communication with the CM11 hardware fails, or the hardware
    /// itself fails.
    /// </summary>
    ///
    /// <remarks>
    /// This event is fired on a thread other than the thread which created
    /// the <r>Cm11</r> object, unless <r>InvokeEventsUsing</r> is used.
    /// </remarks>
    ///
    [Description("Occurs when communication with the CM11 hardware fails, or the hardware itself fails.")]
    public event Cm11ErrorEventDelegate Error;

    /// <summary>
    /// Fired when the <r>Cm11</r> object has information to provide to the
    /// application that may be useful for later review by the user.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// Typically the application handles this event in order to log the
    /// messages provided by this event into a file or event log that can be
    /// reviewed later by a user.  The messages sent by this event include:
    /// <list type="bullet">
    /// <item>
    /// 	<description>Messages prefixed by "&lt;--" that indicate bytes sent
    /// 	to the CM11 hardware, including a decoding of those bytes.
    /// 	</description>
    /// </item>
    /// <item>
    /// 	<description>Messages prefixed by "--&gt;" that indicate bytes
    /// 	received from the CM11 hardware, including a decoding of those
    /// 	bytes.</description>
    /// </item>
    /// <item>
    /// 	<description>Other messages indication error conditions.
    /// 	</description>
    /// </item>
    /// </list>
    /// This information may help the user debug problems with the CM11
    /// hardware or the X10 network.
    /// </para>
    /// <para>
    /// This event is fired on a thread other than the thread which created
    /// the <r>Cm11</r> object, unless <r>InvokeEventsUsing</r> is used.
    /// </para>
    /// </remarks>
    ///
    [Description("Occurs when the Cm11 object has information to provide to the application that may be useful for later review by the user.")]
    public event Cm11LogMessageEventDelegate LogMessage;

    //////////////////////////////////////////////////////////////////////////
    // Public Methods
    //

    /// <summary>
    /// Begins communication with the CM11 device.
    /// </summary>
    ///
    /// <param name="serialPortName">The name of the serial port that the CM11
    ///     device is connected to; for example, "COM1".</param>
    ///
    /// <remarks>
    /// <para>
    /// This method throws an <r>InvalidOperationException</r> if this
    /// <r>Cm11</r> instance is already open.
    /// closed.
    /// </para>
    /// <para>
    /// This method cannot be called while a <r>Cm11</r> event handler is
    /// executing.
    /// </para>
    /// </remarks>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown if <r>Open</r> was already called.
    /// </exception>
    ///
    public void Open(string serialPortName)
    {
      TraceInfo("Open");

      lock (m_lock)
      {
        // make sure we're not already open
        if (m_isOpen)
          throw new InvalidOperationException(AlreadyOpen);

        // update state
        m_serialPortName = serialPortName;

        // open the serial port, if it wasn't opened yet
        OpenSerialPort();

        // start the worker thread, if it wasn't started yet
        if (m_workerThreadWaitHandle == null)
        {
          // start the thread
          VoidDelegate workerThreadDelegate = WorkerThread;
          IAsyncResult ar = workerThreadDelegate.BeginInvoke(null, null);

          // initialize <m_workerThreadHandle>
          m_workerThreadWaitHandle = ar.AsyncWaitHandle;

          // wait until the worker thread has started to ensure that
          // <m_workerThreadId> is initialized before Open() returns;
          // note that this is the opposite of the normal usage of
          // <m_wakeWorkerThread>, since in this case the application
          // thread is using it to wait on the worker thread rather than
          // vice versa
          m_wakeWorkerThread.WaitOne();
        }

        // update state
        m_isOpen = true;
      }
    }

    /// <summary>
    /// Closes the serial port and frees resources used by this object.  No
    /// further access to the CM11 device by this object is possible until
    /// <r>Open</r> is called again.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// This method does nothing if this <r>Cm11</r> instance is already
    /// closed.
    /// </para>
    /// </remarks>
    ///
    public void Close()
    {
      TraceInfo("Close");

      // update state; don't rely on <isOpen> to tell us if the object is
      // open or closed, since if an exception occurred inside Open() then
      // <isOpen> will be false but some resources may still need to be
      // cleaned up
      m_isOpen = false;
      m_serialPortName = null;

      // do nothing further if we're already closed
      if (m_workerThreadWaitHandle == null)
        return;

      // tell the worker thread that it's quitting time
      m_quitting = true;
      m_wakeWorkerThread.Set();

      // Wait for the worker thread to quit -- unless we're currently
      // running in the worker thread.
      //
      // Consider the following two scenarios:
      //
      // Scenario 1:  The worker thread fires an event.  While the event is
      // executing on the worker thread, the user closes the application,
      // which causes Close() to be called on the application's UI thread.
      // No problem: Close() sets <m_quitting> to true and waits on
      // <m_workerThreadWaitHandle> and eventually the event handler returns
      // and the worker thread notices that <m_quitting> is true and quits.
      //
      // Scenario 2:  The worker thread fires event; in application's event
      // handler the application calls Close().  (Note that this time we're
      // executing Close() in the worker thread.)  Close() sets <m_quitting>
      // to true and waits on <m_workerThreadWaitHandle>.  The result is a
      // deadlock: the current thread will never exit because it's waiting
      // for itself to exit.
      //
      // Moral of the story: don't wait on <m_workerThreadWaitHandle> if the
      // current thread is the worker thread.
      //
      if (m_workerThreadId != Thread.CurrentThread.ManagedThreadId)
        m_workerThreadWaitHandle.WaitOne();
    }

    /// <summary>
    /// Sends one or more <r>DwellNet.Cm11</r> commands to the CM11 device.
    /// Commands are translated to the CM11 binary protocol.
    /// </summary>
    ///
    /// <param name="commands">The series of <r>DwellNet.Cm11</r> commands to
    /// 	execute, separated by spaces.  See Cm11Help.htm for a list of valid
    /// 	<r>DwellNet.Cm11</r> commands.</param>
    ///
    /// <remarks>
    /// <para>
    /// This method starts sending commands to the CM11 device.  This method
    /// returns immediately -- it doesn't wait for the commands to complete.
    /// You can call <r>WaitUntilIdle</r> after calling <r>Execute</r> if you'd
    /// like to wait until all queued commands have been executed.
    /// </para>
    /// <para>
    /// Commands are separated by spaces; each command must not contain
    /// spaces within it.  For example, if <paramref name="commands"/> equals
    /// "A1 A2 Dim70", that specifies three commands, "A1", "A2", and "Dim70".
    /// Commands are case-sensitive; you can't specify "a1" or "dim70".
    /// </para>
    /// <para>
    /// <r>Open</r> must be called before calling this method.
    /// </para>
    /// </remarks>
    ///
    /// <example>
    /// The following code turns on lamps A1, A2, and A3, then dims them by
    /// 25%, then turns off lamp B1.
    /// <code>
    /// cm11.Execute("A1 A2 A3 On Dim25 B1 Off");
    /// </code>
    /// </example>
    ///
    public void Execute(string commands)
    {
      TraceInfo("Execute: queue: {0}", commands);

      // "lock (m_lock)" is not called here because we don't want to block
      // unnecessarily -- we're just adding commands to the queue, and
      // <m_commandQueue> has its own lock

      // make sure Open() was called
      if (!m_isOpen)
        throw new InvalidOperationException(NotOpen);

      // split <commands> into an array of individual commands, one per
      // command per array element
      string[] commandArray = commands.Split(
          new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      // perform two passes: in the first pass, check the syntax of each
      // command; in the second, add the command to the command queue;
      // the purpose of this two-pass approach is to reduce the chance of
      // having invalid commands in the queue...

      // pass one:
      AddressTracker addressTracker = new AddressTracker(true);
      foreach (string command in commandArray)
        ProcessCommand(command, addressTracker, false);

      // pass two:
      foreach (string command in commandArray)
      {
        lock (m_commandQueue)
        {
          m_commandQueue.Enqueue(command);
          m_commandQueueChangeCount++;
          SetIdleState(false);
        }
      }

      // wake up the worker thread so it can process the commands
      m_wakeWorkerThread.Set();
    }

    /// <summary>
    /// Removes all <r>DwellNet.Cm11</r> commands from the <r>Cm11</r>
    /// command queue.
    /// </summary>
    ///
    /// <remarks>
    /// <r>Open</r> must be called before calling this method.
    /// </remarks>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown if <r>Open</r> was not called before this method was called.
    /// </exception>
    ///
    public void Clear()
    {
      // make sure Open() was called
      if (!m_isOpen)
        throw new InvalidOperationException(NotOpen);

      // clear the command queue
      lock (m_commandQueue)
      {
        m_commandQueue.Clear();
        m_commandQueueChangeCount++;
        SetIdleState(true);
      }
    }

    /// <summary>
    /// Turns on an X10 device.
    /// </summary>
    ///
    /// <param name="address">The X10 address of the device to control;
    /// 	for example, "A1".  An address consists of a house code, "A"
    /// 	through "P" inclusive, followed by a device code, "1" through "16"
    /// 	inclusive.</param>
    ///
    /// <remarks>
    /// <r>Open</r> must be called before calling this method.
    /// </remarks>
    ///
    /// <example>
    /// The following code turns on the X10 device at address "A1", assuming
    /// a CM11 (or compatible) device is plugged into serial port "COM1".
    /// <c>cm11</c> is a variable of type <r>Cm11</r> or, in the case of a
    /// Windows Forms application, a <r>Cm11</r> component that was dragged
    /// onto the form.
    /// <code>
    /// cm11.Open("COM1");
    /// cm11.TurnOnDevice("A1");
    /// </code>
    /// </example>
    ///
    public void TurnOnDevice(string address)
    {
      // prevalidate <address>: make sure the string contains only a single
      // command; beyond that, Execute() will complete validaton of <address>
      if (address.IndexOf(' ') >= 0)
      {
        throw new Cm11InvalidCommandException(InvalidAddress,
            address);
      }

      // execute the necessary commands
      Execute(String.Format("{0} On", address));
    }

    /// <summary>
    /// Turns off an X10 device.
    /// </summary>
    ///
    /// <param name="address">The X10 address of the device to control;
    /// 	for example, "A1".  An address consists of a house code, "A"
    /// 	through "P" inclusive, followed by a device code, "1" through "16"
    /// 	inclusive.</param>
    ///
    /// <remarks>
    /// <r>Open</r> must be called before calling this method.
    /// </remarks>
    ///
    /// <example>
    /// The following code turns off the X10 device at address "A1", assuming
    /// a CM11 (or compatible) device is plugged into serial port "COM1".
    /// <c>cm11</c> is a variable of type <r>Cm11</r> or, in the case of a
    /// Windows Forms application, a <r>Cm11</r> component that was dragged
    /// onto the form.
    /// <code>
    /// cm11.Open("COM1");
    /// cm11.TurnOffDevice("A1");
    /// </code>
    /// </example>
    ///
    public void TurnOffDevice(string address)
    {
      // prevalidate <address>: make sure the string contains only a single
      // command; beyond that, Execute() will complete validaton of <address>
      if (address.IndexOf(' ') >= 0)
      {
        throw new Cm11InvalidCommandException(InvalidAddress,
            address);
      }

      // execute the necessary commands
      Execute(String.Format("{0} Off", address));
    }

    /// <summary>
    /// Brightens an X10 lamp device.
    /// </summary>
    ///
    /// <param name="address">The X10 address of the device to control;
    /// 	for example, "A1".  An address consists of a house code, "A"
    /// 	through "P" inclusive, followed by a device code, "1" through "16"
    /// 	inclusive.</param>
    ///
    /// <param name="percent">The amount to brighten the lamp by, measured
    /// 	as a percentage value from 0 to 100.</param>
    ///
    /// <remarks>
    /// <r>Open</r> must be called before calling this method.
    /// </remarks>
    ///
    /// <example>
    /// Assuming the X10 device at address "A1" is a lamp module, and assuming
    /// a CM11 (or compatible) device is plugged into serial port "COM1", the
    /// following code brightens the lamp by 25%.  <c>cm11</c> is a variable
    /// of type <r>Cm11</r> or, in the case of a Windows Forms application, a
    /// <r>Cm11</r> component that was dragged onto the form.
    /// <code>
    /// cm11.Open("COM1");
    /// cm11.BrightenLamp("A1", 33);
    /// </code>
    /// </example>
    ///
    public void BrightenLamp(string address, int percent)
    {
      // prevalidate <address>: make sure the string contains only a single
      // command; beyond that, Execute() will complete validaton of <address>
      if (address.IndexOf(' ') >= 0)
      {
        throw new Cm11InvalidCommandException(InvalidAddress,
            address);
      }

      // execute the necessary commands
      Execute(String.Format("{0} Brighten{1}", address, percent));
    }

    /// <summary>
    /// Dims an X10 lamp device.
    /// </summary>
    ///
    /// <param name="address">The X10 address of the device to control;
    /// 	for example, "A1".  An address consists of a house code, "A"
    /// 	through "P" inclusive, followed by a device code, "1" through "16"
    /// 	inclusive.</param>
    ///
    /// <param name="percent">The amount to dim the lamp by, measured as a
    /// 	percentage value from 0 to 100.</param>
    ///
    /// <remarks>
    /// <r>Open</r> must be called before calling this method.
    /// </remarks>
    ///
    /// <example>
    /// Assuming the X10 device at address "A1" is a lamp module, and assuming
    /// a CM11 (or compatible) device is plugged into serial port "COM1", the
    /// following code dims the lamp by 25%.  <c>cm11</c> is a variable of type
    /// <r>Cm11</r> or, in the case of a Windows Forms application, a
    /// <r>Cm11</r> component that was dragged onto the form.
    /// <code>
    /// cm11.Open("COM1");
    /// cm11.DimLamp("A1", 33);
    /// </code>
    /// </example>
    ///
    public void DimLamp(string address, int percent)
    {
      // prevalidate <address>: make sure the string contains only a single
      // command; beyond that, Execute() will complete validaton of <address>
      if (address.IndexOf(' ') >= 0)
      {
        throw new Cm11InvalidCommandException(InvalidAddress,
            address);
      }

      // execute the necessary commands
      Execute(String.Format("{0} Dim{1}", address, percent));
    }

    /// <summary>
    /// Returns after all <r>Cm11</r> commands queued by <r>Execute</r>
    /// have completed.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// This method blocks the caller until the internal command queue is
    /// empty, either because the commands were successfully executed or
    /// because there was an error that caused the commands to be discarded.
    /// </para>
    /// <para>
    /// This method cannot be called while a <r>Cm11</r> event handler is
    /// executing.
    /// </para>
    /// <para>
    /// <r>Open</r> must be called before calling this method.
    /// </para>
    /// </remarks>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown if <r>Open</r> was not called before this method was called.
    /// </exception>
    ///
    public void WaitUntilIdle()
    {
      // make sure Open() was called
      if (!m_isOpen)
        throw new InvalidOperationException(NotOpen);

      // "lock (m_lock)" is not needed here
      m_idleEvent.WaitOne();
    }

    //////////////////////////////////////////////////////////////////////////
    // Event Handlers for <m_serialPort>
    //

    void m_serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      // if we received bytes, notify the worker thread
      TraceInfo("Serial port: DataReceived ({0})", e.EventType);
      if (e.EventType == SerialData.Chars)
        m_wakeWorkerThread.Set();
    }

    void m_serialPort_ErrorReceived(object sender,
    SerialErrorReceivedEventArgs e)
    {
      // a serial port communication error occurred -- log informatin to the
      // application and resync
      FireLogMessage(SerialPortError, e.EventType);
      HardResync();
    }

    //////////////////////////////////////////////////////////////////////////
    // IDisposable Implemention
    //

    /// <summary>
    /// Disposes of resources used by this object.
    /// </summary>
    ///
    void IDisposable.Dispose()
    {
      Close();
    }

    //////////////////////////////////////////////////////////////////////////
    // Private Methods
    //
    // NOTE: These private properties and methods are generally NOT
    // thread-safe -- call these methods within "lock (m_lock)" or an
    // equivalent.
    //

    /// <summary>
    /// Opens the serial port, if it's not open yet.
    /// </summary>
    ///
    void OpenSerialPort()
    {
      // do nothing if OpenSerialPort() was already called
      if (m_serialPort != null)
        return;

      // create and initialize <m_serialPort>
      m_serialPort = new DnSerialPort(new DnSerialPortStringResources(),
          m_serialPortName, 4800, Parity.None, 8, StopBits.One);
      m_serialPort.DataReceived += new SerialDataReceivedEventHandler(
    m_serialPort_DataReceived);
      m_serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(
    m_serialPort_ErrorReceived);
      m_serialPort.Open();
    }

    /// <summary>
    /// Closes the serial port, if it's open.
    /// </summary>
    ///
    void CloseSerialPort()
    {
      if (m_serialPort != null)
      {
        m_serialPort.Purge();
        m_serialPort.Close();
        m_serialPort = null;
      }
    }

    /// <summary>
    /// The code of the worker thread which services the message queue.
    /// </summary>
    ///
    void WorkerThread()
    {
      TraceInfo("worker thread started");
      // initialize <m_workerThreadId>
      m_workerThreadId = Thread.CurrentThread.ManagedThreadId;

      // allow Open() to complete, now that <m_workerThreadId> has been
      // initialized; note that this is the opposite of the normal usage of
      // <m_wakeWorkerThread>, since in this case the application thread is
      // using it to wait on the worker thread rather than vice versa
      m_wakeWorkerThread.Set();

      TraceInfo("worker thread initialized");

      // process commands in the command queue until we're told to quit;
      // NOTE: we don't "lock (m_lock)" until absolutely necessary, since
      // doing so restricts concurrent access
      try
      {
        while (true)
        {
          try
          {
            // wait for one of the following events:
            //   (a) serial port data the CM11;
            //   (b) a command from the application (from Execute());
            //   (c) Close() called -- this causes a QuittingException
            //       to be thrown
            WaitResult wr =
              WaitForSerialInputOrCommand(DateTime.MaxValue, true);
            if (wr == WaitResult.SerialInput)
              ProcessNotification();
            else
              if (wr == WaitResult.CommandQueued)
                ProcessQueuedCommands();
          }
          catch (TimeoutException ex)
          {
            // the CM11 took too long to reply during an exchange with
            // the PC -- pause to allow the CM11 to return to a
            // "normal" state (waiting for a command or sending a
            // notification)
            TraceException(ex);
            FireLogMessage(Cm11ReplyTimeout);
            SoftResync();
          }
          catch (HardResyncException ex)
          {
            // a serious serial port error occurred -- close and reopen
            // the serial port, and discard any queued commands
            TraceException(ex, "HardResyncException");
            HardResync();
          }
          catch (DnSerialPortException ex)
          {
            // a serious serial port error occurred -- close and reopen
            // the serial port, and discard any queued commands
            TraceException(ex);
            FireLogMessage(CommunicationError,
              GetExceptionFullMessage(null, ex));
            HardResync();
          }
        }
      }
      catch (QuittingException ex)
      {
        // stop worker thread by exiting this method
        TraceException(ex);
      }

      TraceInfo("worker thread exiting");

      // clear state
      m_workerThreadId = 0;
      m_workerThreadWaitHandle = null;

      // close the serial port, if it's open
      lock (m_lock)
        CloseSerialPort();

      TraceInfo("worker thread exited");
    }

    /// <summary>
    /// Called when we receive one or more bytes from the CM11.  This method
    /// reads the bytes and processes them as a notification.
    /// </summary>
    ///
    void ProcessNotification()
    {
      // set <b1> to the first byte of the notification; return if none
      byte? b1 = m_serialPort.ReadByte();
      if (!b1.HasValue)
        return;

      // process the notification
      if (b1 == 0xA5)
      {
        // this is a *clock set request*, i.e. the CM11 is requesting that
        // we set its internal clock...

        // feedback to application
        FireLogMessage(Cm11Notification, b1,
          Cm11TimeRequestNotification);

        // set <reply> to a *clock set reply* to send to the CM11...
        DateTime now = DateTime.Now;
        int minutes = now.Hour * 60 + now.Minute;
        DateTime startOfYear = new DateTime(now.Year, 1, 1);
        int dayOfYear = (int)(now - startOfYear).TotalDays;
        byte[] reply = new byte[]
			{
				// set-time header:
				0x9B,
				// seconds component of current time
				(byte) now.Second,
				// (minute-of-day component of current time) % 120
				(byte) (minutes % 120), 
				// (minute-of-day component of current time) / 120
				(byte) (minutes / 120),
				// low order 8 bits of day of the year
				(byte) (dayOfYear & 0xFF),
				// high order bit of day of the year in MSB,
				// day of week specified by setting bit 0 (Sunday)
				// through 7 (Saturday)
				(byte) (((dayOfYear & 0x100) >> 1) |
						(1 << (int) now.DayOfWeek)),
				// monitored house code in upper 4 bits; bit 3 is
				// reserved; bit 2 is battery timer clear flag;
				// bit 1 is monitored status clear flag; bit 0 is
				// timer purge flag
				(byte) (s_codesToNibbles[0] << 4)
			};

        // write <reply> to the CM11
        WriteToSerialPort(reply, SettingCm11Clock);

        // the CM11 will send one 0xA5 request every second after power-up,
        // so it's not unusual for a series of these to be present in the
        // serial input buffer; for efficiency, delete all remaining 0xA5
        // bytes in the input buffer
        while (m_serialPort.PeekByte() == 0xA5)
          m_serialPort.ReadByte();
      }
      else
        if (b1 == 0x5A)
        {
          // this is a *device notification*...

          // feedback to application
          FireLogMessage(Cm11Notification, b1,
            Cm11X10DeviceNotification);

          // tell the CM11 we received the first byte of the notification
          WriteToSerialPort(new byte[] { 0xC3 }, AckNotification);

          // wait for the CM11 to send a count of data bytes; ignore all
          // 0x5A bytes we receive, because those are likely just extra
          // 0x5A's that got queued before we got around to responding
          // (since the CM11 sends one 0x5A per second until we respond)
          int dataByteCount;
          while (true)
          {
            if ((dataByteCount = ReadSerialPortByte(SERIAL_TIMEOUT))
                != 0x5A)
              break;
          }
          FireLogMessage(Cm11NotificationDataLength, dataByteCount);
          if ((dataByteCount < 2/*note below*/) || (dataByteCount > 9))
          {
            // invalid data byte count -- ignore this notification;
            // note that there must be at least 2 data bytes: one for the
            // data byte specifier and at least one following data byte
            FireLogMessage(InvalidReply);
            SoftResync();
            return;
          }

          // read the specified number of bytes from the CM11
          byte[] dataBytes = ReadSerialPortBytes(dataByteCount,
      SERIAL_TIMEOUT);
          FireLogMessage("CM11 --> {0} ({1})", FormatBytes(dataBytes),
            DeviceNotificationData);

          // parse and process the notification bytes
          NotificationDataParser parser =
              new NotificationDataParser(dataBytes);
          while (!parser.AtEndOfDataBytes)
          {
            bool isFunctionCode;
            byte dataByte = parser.GetNextDataByte(out isFunctionCode);
            if (!isFunctionCode)
            {
              // this is an *address code*
              ProcessDeviceNotification(false, dataByte, 0);
            }
            else
            {
              // this is a *function code* -- check to see if there is
              // a following *function parameter* byte and set
              // <parameterByte> to it if so, otherwise set
              // <parameterByte> to zero
              byte parameterByte = 0;
              if (((dataByte & 0xF) == (int)X10Function.Dim) ||
                  ((dataByte & 0xF) == (int)X10Function.Brighten))
              {
                // this is a *function code* that requires a parameter
                if (!parser.AtEndOfDataBytes)
                {
                  bool isFunctionCode2;
                  byte b = parser.GetNextDataByte(
                    out isFunctionCode2);
                  if (!isFunctionCode2)
                    parameterByte = b;
                  else
                    parser.UngetDataByte();
                }
              }
              ProcessDeviceNotification(true, dataByte, parameterByte);
            }
          }
        }
        else
        {
          // unknown notification (ignored)
          FireLogMessage(Cm11Notification, b1,
            Cm11UnknownNotification);
        }
    }

    /// <summary>
    /// Called when we receive a <i>device notification</i> from the CM11.
    /// This method is called once for each <i>address code</i> or <i>function
    /// code</i> contained within each <i>device notification</i>.
    /// </summary>
    ///
    /// <param name="isFunctionCode"><n>true</n> if this is a <i>function
    /// 	code</i>, false if it's an <i>address code</i>.</param>
    ///
    /// <param name="dataByte">The <i>address code</i> or <i>function code</i>
    /// 	byte.</param>
    ///
    /// <param name="parameterByte">If this is a <i>function code</i>, and the
    /// 	<i>function code nibble</i> is is <r>X10Function.Dim</r> or
    /// 	<r>X10Function.Brighten</r>, <paramref name="parameterByte"/>
    /// 	contains the additional parameter byte specifying the relative
    ///		brighten-by or dim-by amount, from 0 (meaning 0%) to 210 (meaning
    /// 	100%).</param>
    ///
    void ProcessDeviceNotification(bool isFunctionCode, byte dataByte,
        byte parameterByte)
    {
      // set <commandName> to the string version of the notification,
      // e.g. "On" or "Dim"; set <commandParameter> to the parameter value,
      // i.e. a brighten-by or dim-by value in the range 0 to 100 inclusive
      // in the case of "Dim" and "Brighten" commands, -1 otherwise
      string commandName;
      int commandParameter;
      if (isFunctionCode)
      {
        // this is a *function code*
        X10Function x10Function = (X10Function)(dataByte & 0xF);
        byte highNibble = (byte)((dataByte >> 4) & 0xF);
        if ((x10Function == X10Function.Dim) ||
          (x10Function == X10Function.Brighten))
        {
          // this is a "Dim" or "Brighten" notification
          commandName = String.Format("{0}.{1}",
            HouseCodeNibbleToChar(highNibble), x10Function);
          commandParameter = (parameterByte * 100 + 105) / 210;

          // keep track of which devices are *currently addressed*
          m_addressTracker.RegisterFunctionCommand(highNibble);
        }
        else
          if ((x10Function == X10Function.PresetDim1) ||
              (x10Function == X10Function.PresetDim2))
          {
            // this is a *device level notification* -- see Cm11.doc
            byte appended5Bits = (byte)((highNibble << 1) |
              ((x10Function == X10Function.PresetDim2) ? 1 : 0));
            byte reversed5Bits = (byte)
              (((appended5Bits & 0x10) >> 4) |
               ((appended5Bits & 0x08) >> 2) |
               (appended5Bits & 0x04) |
               ((appended5Bits & 0x02) << 2) |
               ((appended5Bits & 0x01) << 4));
            commandName = "Level";
            commandParameter = ((reversed5Bits + 1) * 100 + 16) / 32;
          }
          else
          {
            // this is some other *function code* notification
            commandName = String.Format("{0}.{1}",
              HouseCodeNibbleToChar(highNibble), x10Function);
            commandParameter = -1;

            // keep track of which devices are *currently addressed*
            m_addressTracker.RegisterFunctionCommand(highNibble);
          }
      }
      else
      {
        // this is an *address code* notification
        commandName = AddressByteToString(dataByte);
        commandParameter = -1;

        // keep track of which devices are *currently addressed*
        m_addressTracker.RegisterAddressCommand(dataByte);
      }

      // fire a low-level notification event
      FireNotification(commandName, commandParameter);

      // fire a high-level notification event, if appropriate
      int percent;
      if (isFunctionCode)
      {
        X10Function x10Function = (X10Function)(dataByte & 0xF);
        byte houseCodeNibble = (byte)((dataByte >> 4) & 0xF);
        switch (x10Function)
        {

          case X10Function.On:

            foreach (string address in
        m_addressTracker.GetAddressedDevices(houseCodeNibble))
              FireOnReceived(address);
            break;

          case X10Function.Off:

            foreach (string address in
        m_addressTracker.GetAddressedDevices(houseCodeNibble))
              FireOffReceived(address);
            break;

          case X10Function.Brighten:

            percent = (parameterByte * 100 + 105) / 210;
            foreach (string address in
        m_addressTracker.GetAddressedDevices(houseCodeNibble))
              FireBrightenReceived(address, percent);
            break;

          case X10Function.Dim:

            percent = (parameterByte * 100 + 105) / 210;
            foreach (string address in
        m_addressTracker.GetAddressedDevices(houseCodeNibble))
              FireDimReceived(address, percent);
            break;

          case X10Function.AllLightsOn:

            FireAllLightsOnReceived(
              HouseCodeNibbleToChar(houseCodeNibble));
            break;

          case X10Function.AllLightsOff:

            FireAllLightsOffReceived(
              HouseCodeNibbleToChar(houseCodeNibble));
            break;

          case X10Function.AllOff:

            FireAllOffReceived(HouseCodeNibbleToChar(houseCodeNibble));
            break;
        }
      }
    }

    /// <summary>
    /// Called when we one or more commands were queued in
    /// <r>m_commandQueue</r>.  This method reads and executes queued commands
    /// until <r>Close</r> is called, serial input is received from the CM11,
    /// or there are no more queued commands to execute, whichever happens
    /// first.
    /// </summary>
    ///
    void ProcessQueuedCommands()
    {
      // process
      while (true)
      {
        // return if Close() was called or serial input is received from
        // the CM11; in the latter case is due to the fact that we need to
        // process C11 notifications before executing commands, because the
        // CM11 ignores commands while it's in notification mode
        WaitResult wr = WaitForSerialInputOrCommand(null, true);
        if (wr != WaitResult.CommandQueued)
          return;

        // set <command> to the next command in the queue, but don't remove
        // it from the queue yet
        string command;
        lock (m_commandQueue)
        {
          // check the command queue again in case it was emptied by
          // another thread
          if (m_commandQueue.Count == 0)
            return;
          command = m_commandQueue.Peek();
        }

        // execute <command> -- don't "lock <m_commandQueue>" here because
        // ProcessCommand() could take quite a while and we don't want to
        // lock the command queue for the entire time
        if (ProcessCommand(command, m_addressTracker, true))
        {
          // the command was successfully executed -- remove it from the
          // queue
          lock (m_commandQueue)
          {
            // check the command queue again in case it was emptied by
            // another thread (with possibly another command queued --
            // we don't want to delete the wrong command)
            if ((m_commandQueue.Count > 0) &&
              (command == m_commandQueue.Peek()))
            {
              m_commandQueue.Dequeue();
              m_commandQueueChangeCount++;
            }
            if (m_commandQueue.Count == 0)
              SetIdleState(true);
          }
        }
        else
        {
          // the command could not be executed, presumably because a
          // notification arrived from the CM11 -- leave the command in
          // the queue for now, and return so the notification can be
          // processed
          return;
        }
      }
    }

    /// <summary>
    /// Performs a "soft resynchronization" of serial port communications.
    /// This process attempts to correct a serial port communication problem
    /// without losing queued
    /// commands.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    /// This method informs the application that the CM11 is behaving
    /// unexpectedly (for example, an invalid reply), then pauses for a short
    /// time to allow the CM11 to reset its internal state and thereby
    /// (hopefully) resync communications with the PC.
    /// </para>
    /// <para>
    /// During most back-and-forth communication exchanges between the CM11 and
    /// the PC, if the CM11 doesn't hear back from the PC within a second or so
    /// it appears to cancel the exchange.  This method exploits that
    /// characteristic as a way to get the CM11 back to a normal state
    /// (waiting for a command for the PC or sending a new notification to the
    /// PC).
    /// </para>
    /// </remarks>
    ///
    void SoftResync()
    {
      try
      {
        FireLogMessage(Pausing, SERIAL_TIMEOUT);
        Wait(SERIAL_TIMEOUT);
      }
      catch (HardResyncException ex)
      {
        // a serious serial port error occurred -- close and reopen
        // the serial port, and discard any queued commands
        TraceException(ex, "HardResyncException during SoftResync");
        HardResync();
      }
      catch (DnSerialPortException ex)
      {
        // a serious serial port error occurred -- close and reopen
        // the serial port, and discard any queued commands
        TraceException(ex, "DnSerialPortException during SoftResync");
        HardResync();
      }
    }

    /// <summary>
    /// Performs a "hard resynchronization" of serial port communications.
    /// This process attempts to correct a serious serial port communication
    /// problem by disconnecting and reconnecting the serial port.  Any queued
    /// commands are discarded, to prevent commands from building up
    /// indefinitely during a period of poor communication with the CM11.
    /// </summary>
    ///
    void HardResync()
    {
      // loop until the serial port is successfully reopened
      for (int attemptNumber = 1; ; attemptNumber++)
      {
        TraceInfo("HardResync: attempt #{0}", attemptNumber);
        try
        {
          // purge the serial port input and output buffers, then close
          // the serial port
          CloseSerialPort();

          // discard any queued commands; set <discardedMessageCount> to
          // the number of discarded messages
          int discardedMessagesCount;
          lock (m_commandQueue)
          {
            discardedMessagesCount = m_commandQueue.Count;
            Clear();
          }

          // notify the application of the situation
          FireError(HardResyncOccurred,
              discardedMessagesCount);

          // give the CM11 time to reset its state; this also prevents
          // rapid close/open/close/open/etc. of the serial port if
          // reopening the serial port (below) fails continually
          Wait(SERIAL_TIMEOUT);

          // reopen the serial port and purge any collected serial port
          // input
          OpenSerialPort();
          m_serialPort.Purge();

          // done with no exceptions
          FireLogMessage(HardResyncComplete);
          break;
        }
        catch (HardResyncException ex)
        {
          TraceException(ex,
            "HardResyncException during HardResync");
        }
        catch (DnSerialPortException ex)
        {
          TraceException(ex, "DnSerialPortException during HardResync");
        }
      }
    }

    /// <summary>
    /// Waits for a specified number of milliseconds.
    /// </summary>
    ///
    /// <param name="timeoutMsec">The number of milliseconds to wait.</param>
    ///
    /// <exception cref="QuittingException">
    /// Thrown if <r>Close</r> was called before or during this method call.
    /// </exception>
    ///
    ///
    void Wait(int timeoutMsec)
    {
      DateTime timeoutTime = DateTime.Now.AddMilliseconds(timeoutMsec);
      bool timeout = false;
      while (true)
      {
        // if Close() was called, it's time to quit
        if (m_quitting)
          throw new QuittingException();

        // see if it's time to return
        if (timeout)
          return;

        // sleep until m_wakeWorkerThread.Set() is called or we reach
        // <timeoutTime> (if not null), whichever happens first; set
        // <timeout> to true if the latter happens first, false if the
        // former happens first
        TimeSpan sleepTime = timeoutTime - DateTime.Now;
        if (sleepTime <= TimeSpan.Zero)
          timeout = true;
        else
          timeout = !m_wakeWorkerThread.WaitOne(sleepTime, false);
      }
    }

    /// <summary>
    /// Waits until one or more bytes are received from the CM11, or a command
    /// is queued in <r>m_commandQueue</r>, or <r>Close</r> is called.
    /// </summary>
    ///
    /// <param name="timeoutTime">The time at which to give up waiting and
    /// 	throw a <r>TimeoutException</r>.  If <paramref name="timeoutTime"/>
    /// 	is null, then this method performs a poll returns immediately;
    /// 	in this case, <r>WaitResult.Idle</r> is returned if no bytes are
    /// 	in the serial input buffer and no commands are queued in
    /// 	<r>m_commandQueue</r> and <r>Close</r> has not been called.  Use
    /// 	<n>DateTime.MaxValue</n> to wait indefinitely, i.e. never time out.
    /// 	</param>
    ///
    /// <param name="waitForCommand">If <n>true</n>, wait for either serial
    /// 	input or a queued command.  If <n>false</n>, wait only for serial
    ///		input.</param>
    ///
    /// <returns>
    /// One of the following values:
    /// <list type="table">
    /// <listheader>
    ///     <term>Return Value</term>
    ///     <description>Description</description>
    /// </listheader>
    /// <item>
    ///     <term><r>WaitResult.Idle</r></term>
    ///     <term>No bytes are in the serial input buffer and no commands are
    /// 		queued in <r>m_commandQueue</r>.  In this situation,
    /// 		<r>WaitResult.Idle</r> is only returned if
    /// 		<paramref name="timeoutTime"/>; otherwise, this method waits
    /// 		until the specified timeout time and then, if no serial
    ///			input or queued commands are available yet and <r>Close</r>
    ///			has not been called, <r>TimeoutException</r> is thrown.</term>
    /// </item>
    /// <item>
    /// 	<term><r>WaitResult.SerialInput</r></term>
    /// 	<term>One or more bytes were received from the CM11 device and are
    /// 		waiting to be read in <r>m_serialPort</r>.</term>
    /// </item>
    /// <item>
    /// 	<term><r>WaitResult.CommandQueued</r></term>
    /// 	<term>One or more commands were queued by the application into
    /// 		<r>m_commandQueue</r> and are waiting to be executed.</term>
    /// </item>
    /// </list>
    /// </returns>
    ///
    /// <exception cref="TimeoutException">
    /// Thrown if <paramref name="timeoutTime"/> was reached before serial
    /// input was received or a command was queued.
    /// </exception>
    ///
    /// <exception cref="QuittingException">
    /// Thrown if <r>Close</r> was called before or during this method call.
    /// </exception>
    ///
    WaitResult WaitForSerialInputOrCommand(DateTime? timeoutTime,
      bool waitForCommand)
    {
      bool timeout = false; // true once a timeout occurs
      while (true)
      {
        // if Close() was called, it's time to quit
        if (m_quitting)
          throw new QuittingException();

        // check to see if one or more bytes are now present at the serial
        // port
        if (m_serialPort.PeekByte() != null)
          return WaitResult.SerialInput;

        // check to see if a command is already available in the queue,
        // unless <waitForCommand> is false
        if (waitForCommand)
        {
          lock (m_commandQueue)
          {
            if (m_commandQueue.Count > 0)
              return WaitResult.CommandQueued;
          }
        }

        // if the caller is polling, return immediately
        if (timeoutTime == null)
          return WaitResult.Idle;

        // if a timeout occurred in a previous iteration of this loop,
        // throw TimeoutException
        if (timeout)
        {
          TraceInfo("WaitForSerialInputOrCommand: Timeout: {0} msec late",
            (DateTime.Now - timeoutTime.Value).TotalMilliseconds);
          throw new TimeoutException();
        }

        // sleep until m_wakeWorkerThread.Set() is called or we reach
        // <timeoutTime>, whichever happens first; set <timeout> to true
        // if the latter happens first, false if the former happens first
        if (timeoutTime.Value != DateTime.MaxValue)
        {
          // wait for the specified time
          TimeSpan sleepTime = timeoutTime.Value - DateTime.Now;
          if (sleepTime <= TimeSpan.Zero)
            timeout = true;
          else
            timeout = !m_wakeWorkerThread.WaitOne(sleepTime, false);
        }
        else
        {
          // wait indefinitely
          m_wakeWorkerThread.WaitOne();
        }
      }
    }

    /// <summary>
    /// Waits until one or more bytes are received from the CM11, or
    /// <r>Close</r> is called.
    /// </summary>
    ///
    /// <param name="timeoutTime">The time at which to give up waiting and
    /// 	throw a <r>TimeoutException</r>.  Use <n>DateTime.MaxValue</n> to
    /// 	wait indefinitely, i.e. never time out.</param>
    ///
    /// <exception cref="TimeoutException">
    /// Thrown if <paramref name="timeoutTime"/> was reached before serial
    /// input was received or a command was queued.
    /// </exception>
    ///
    /// <exception cref="QuittingException">
    /// Thrown if <r>Close</r> was called before or during this method call.
    /// </exception>
    ///
    void WaitForSerialInput(DateTime timeoutTime)
    {
      WaitResult wr = WaitForSerialInputOrCommand(timeoutTime, false);
      Debug.Assert(wr == WaitResult.SerialInput);
    }

    /// <summary>
    /// Updates the idle state, i.e. specifies if no commands are currently
    /// being processed by the <r>Cm11</r> class.
    /// </summary>
    ///
    void SetIdleState(bool idle)
    {
      // do nothing if the idle state hasn't changed
      if (m_idle == idle)
        return;
      m_idle = idle;

      // update <m_idleEvent> (used by WaitUntilIdle)
      if (m_idle)
        m_idleEvent.Set();
      else
        m_idleEvent.Reset();

      // notify the application of the idle state change
      FireIdleStateChange(m_idle);
    }

    /// <summary>
    /// Reads a byte from the serial port.
    /// </summary>
    ///
    /// <param name="timeoutMsec">The maximum number of milliseconds to wait
    /// 	for the requested data before throwing a <r>TimeoutException</r>
    /// 	</param>
    ///
    /// <exception cref="TimeoutException">
    /// Thrown if <paramref name="timeoutMsec"/> milliseconds passed before the
    /// requested data was read from the serial port.
    /// </exception>
    ///
    /// <exception cref="QuittingException">
    /// Thrown if <r>Close</r> was called before or during this method call.
    /// </exception>
    ///
    byte ReadSerialPortByte(int timeoutMsec)
    {
      DateTime timeoutTime = DateTime.Now.AddMilliseconds(timeoutMsec);
      while (true)
      {
        WaitForSerialInput(timeoutTime);
        byte? b = m_serialPort.ReadByte();
        if (b.HasValue)
          return b.Value;
      }
    }

    /// <summary>
    /// Reads a byte from the serial port and returns it, but "pushes" the
    /// byte back to the serial port so it will be read again on the next
    /// call to <r>ReadSerialPortByte</r> or <r>PeekSerialPortByte</r>.
    /// </summary>
    ///
    /// <param name="timeoutMsec">The maximum number of milliseconds to wait
    /// 	for the requested data before throwing a <r>TimeoutException</r>
    /// 	</param>
    ///
    /// <exception cref="TimeoutException">
    /// Thrown if <paramref name="timeoutMsec"/> milliseconds passed before the
    /// requested data was read from the serial port.
    /// </exception>
    ///
    /// <exception cref="QuittingException">
    /// Thrown if <r>Close</r> was called before or during this method call.
    /// </exception>
    ///
    byte PeekSerialPortByte(int timeoutMsec)
    {
      DateTime timeoutTime = DateTime.Now.AddMilliseconds(timeoutMsec);
      while (true)
      {
        WaitForSerialInput(timeoutTime);
        byte? b = m_serialPort.PeekByte();
        if (b.HasValue)
          return b.Value;
      }
    }

    /// <summary>
    /// Reads a series of bytes from the serial port.
    /// </summary>
    ///
    /// <param name="byteCount">The number of bytes to read.</param>
    ///
    /// <param name="timeoutMsec">The maximum number of milliseconds to wait
    /// 	for the requested data before throwing a <r>TimeoutException</r>
    /// 	</param>
    ///
    /// <returns>
    /// The <paramref name="byteCount"/> bytes read from the serial port.
    /// </returns>
    ///
    /// <exception cref="TimeoutException">
    /// Thrown if <paramref name="timeoutMsec"/> milliseconds passed before the
    /// requested data was read from the serial port.
    /// </exception>
    ///
    /// <exception cref="QuittingException">
    /// Thrown if <r>Close</r> was called before or during this method call.
    /// </exception>
    ///
    byte[] ReadSerialPortBytes(int byteCount, int timeoutMsec)
    {
      DateTime timeoutTime = DateTime.Now.AddMilliseconds(timeoutMsec);
      byte[] result = new byte[byteCount];
      int offset = 0; // where in <result> to read next
      while (true)
      {
        int bytesDesired = byteCount - offset;
        if (bytesDesired <= 0)
          return result;
        WaitForSerialInput(timeoutTime);
        int bytesRead = m_serialPort.Read(result, offset, bytesDesired);
        offset += bytesRead;
      }
    }

    /// <summary>
    /// Throws an exception if the serial input buffer is not empty.
    /// </summary>
    ///
    void ExpectingEmptySerialInputBuffer()
    {
      // read all bytes in the serial input buffer; set <bytesList> to
      // a string containing the bytes in hex format (e.g. "8A B2 1C")
      StringBuilder bytesList = null;
      while (m_serialPort.PeekByte() != null)
      {
        if (bytesList == null)
          bytesList = new StringBuilder(100);
        else
          bytesList.Append(' ');
        bytesList.AppendFormat("{0:X2}", m_serialPort.ReadByte());
      }

      // if any bytes were read, throw an exception
      if (bytesList != null)
      {
        FireLogMessage(Cm11UnexpectedBytes, bytesList);
      }
    }

    /// <summary>
    /// Executes a command, or simply checks its syntax and usage.
    /// </summary>
    ///
    /// <param name="command">The <r>DwellNet.Cm11</r> command to process.
    /// 	Commands are case-sensitive; for example, "Dim70" is a valid
    /// 	command, but "dim70" is not.  See Cm11Help.htm for a list of valid
    /// 	<r>DwellNet.Cm11</r> commands.</param>
    ///
    /// <param name="addressTracker">Tracks which X10 devices are <i>currently
    /// 	addressed</i>.</param>
    ///
    /// <param name="execute"><n>true</n> to execute the command, <n>false</n>
    /// 	to check its syntax and usage.  If <paramref name="execute"/> is
    /// 	<n>true</n>, this method should only be called from the worker
    /// 	thread.</param>
    ///
    /// <returns>
    /// <n>true</n> if the command was executed, <n>false</n> if not.
    /// <n>false</n> is returned if <paramref name="execute"/> is <n>false</n>.
    /// </returns>
    ///
    /// <remarks>
    /// The CM11 doesn't allow device address for the same function (e.g. Dim)
    /// to have different house codes.  For example, the command sequence
    /// "A1 A2 Dim70" is valid, but "A1 B2 Dim70" is not.
    /// <r>ProcessCommand</r> checks for this error condition by using
    /// <paramef name="addressTracker"/>, which the caller should ensure is
    /// created in single-house mode.  If a house code mismatch occurs, a
    /// <r>Cm11InvalidCommandException</r> is thrown.
    /// </remarks>
    ///
    /// <exception cref="Cm11InvalidCommandException">
    /// Thrown when the command is incorrect (e.g. incorrect syntax,
    /// inconsistent house codes, etc.) See Remarks for more information.
    /// </exception>
    ///
    bool ProcessCommand(string command, AddressTracker addressTracker,
      bool execute)
    {
      // parse <command>; set <commandBytes> to the byte sequence for the
      // X10 address or function corresponding to <command>; set
      // <commandLabel> to a label for the command to use in a LogMessage
      // event; exception: if <command> is just a house code ("A" through
      // "P", inclusive), just set <m_currentHouseCodeNibble> and return,
      // since this command simply sets internal state in this class -- no
      // communication with the CM11 hardware is required
      byte[] commandBytes;
      string commandLabel;
      byte? houseCodeNibble;
      byte? addressByte;
      bool hexCommand;
      if ((addressByte = ParseAddress(command)) != null)
      {
        // <command> is an X10 address, e.g. "A1"...

        // this isn't a "Hex<hex-digits>" command
        hexCommand = false;

        // set <commandLabel> to a string to use in the LogMessage event
        commandLabel = String.Format(CommandType, command);

        // set <houseCodeNibble> to the nibble value of the house
        // code of this address
        houseCodeNibble = (byte)(addressByte.Value >> 4);

        // set <commandBytes> to the byte sequence for an X10 address
        commandBytes = new byte[] { (byte)0x04, addressByte.Value };

        // update the state of <addressTracker>
        addressTracker.RegisterAddressCommand(addressByte.Value);
      }
      else
        if ((commandBytes = ParseHexCommand(command)) != null)
        {
          // <command> is a "Hex<hex-digits>" command
          hexCommand = true;

          // set <commandLabel> to a string to use in the LogMessage event
          commandLabel = String.Format(CommandType, command);
        }
        else
          if ((commandBytes = ParseFunction(command,
              addressTracker.LastCommandHouseCodeNibble)) != null)
          {
            // <command> is an X10 function, e.g. "Dim70"...

            // this isn't a "Hex<hex-digits>" command
            hexCommand = false;

            // set <commandLabel> to a string to use in the LogMessage event
            houseCodeNibble = (byte)((commandBytes[1] >> 4) & 0xF);
            X10Function x10Function = (X10Function)(commandBytes[1] & 0xF);
            int amount22 = (commandBytes[0] >> 3) & 0x1F;
            commandLabel = String.Format(CommandType,
                FormatFunctionCommand(houseCodeNibble.Value, x10Function,
          amount22));

            // update the state of <addressTracker>
            addressTracker.RegisterFunctionCommand(houseCodeNibble.Value);
          }
          else
          {
            // <command> is invalid
            throw new Cm11InvalidCommandException(InvalidCommand,
              command);
          }

      // if we're only performing a syntax & usage check, we're done
      if (!execute)
        return false; // command not executed

      // send the command to the CM11, and wait for a correct checksum reply;
      // retry sending the command a few times if necessary
      for (int retry = 0; ; retry++)
      {
        // if we ran out of retries, reset communications
        int maxRetries = 5;
        if (retry > maxRetries)
        {
          FireLogMessage(CommandRetryCountExceeded, maxRetries);
          throw new HardResyncException();
        }

        // if this is a retry, tell the application
        if (retry > 0)
          FireLogMessage(RetryingCommand);

        // just before writing to the serial port we need to check to see
        // if the serial input buffer is empty; if it's not, we should
        // abort sending this command to the CM11 because the CM11 will
        // ignore any command sent while it is in *device notification
        // mode*
        if (m_serialPort.PeekByte() != null)
          return false; // command not executed

        // send a *normal command* or a raw hex command to the CM11 device
        WriteToSerialPort(commandBytes, "{0}", commandLabel);

        // if we sent a raw hex command, we're done -- for example we can't
        // assume that a checksum is to be expected
        if (hexCommand)
          return true;

        // wait for a checksum from the CM11
        byte receivedChecksum;
        try
        {
          receivedChecksum = PeekSerialPortByte(SERIAL_TIMEOUT);
        }
        catch (TimeoutException ex)
        {
          // timeout -- try again
          TraceException(ex);
          continue;
        }
        FireLogMessage(Cm11Checksum, receivedChecksum);

        // if the checksum is correct move on to the next step of the
        // protocol
        byte correctChecksum = CalculateChecksum(commandBytes);
        if (receivedChecksum == correctChecksum)
        {
          // we "peeked" the checksum above, so read it now
          ReadSerialPortByte(SERIAL_TIMEOUT);
          break;
        }

        // incorrect checksum
        FireLogMessage(IncorrectChecksum, correctChecksum);
        if (receivedChecksum == 0x5A)
        {
          // Our command probably** got interrupted by a notification;
          // leave the 0x5A in the serial input buffer and return so
          // the notification can be processed before the command is
          // retried (since the CM11 ignores commands while it's in
          // *device notification mode*.
          //
          // ** Why "probably"? because it's possible that 0x5A is simply
          // the wrong checksum.  (It's also possible that the correct
          // checksum is 0x5A and we incorrectly assumed success above.)
          // If that happens, we'll let the normal resync logic of Cm11
          // take care of the (hopefully rare) situation.
          //
          FireLogMessage(PushedByteBack, receivedChecksum);
          return false;
        }
        else
        {
          // plain old wrong checksum -- read it (we "peeked" it above),
          // wait for a short period (in case the CM11 is in the middle
          // of sending a bunch of other bytes), then purge the remaining
          // serial port input and output buffer contents and try again
          ReadSerialPortByte(SERIAL_TIMEOUT);
          Wait(250);
          m_serialPort.Purge();
        }
      }

      // the CM11 sent a correct checksum; acknowledge it with a 0x00
      ExpectingEmptySerialInputBuffer();
      WriteToSerialPort(new byte[] { 0x00 }, AckChecksum);

      // wait for a 0x55 or 0x5A reply from the CM11
      byte ackReply = PeekSerialPortByte(SERIAL_TIMEOUT);
      FireLogMessage(Cm11AckReply, ackReply);
      if (ackReply == 0x55)
      {
        // the command was successfully executed -- we "peeked" <ackReply>
        // above above, so read it now
        ReadSerialPortByte(SERIAL_TIMEOUT);
        return true;
      }
      else
        if (ackReply == 0x5A)
        {
          // the command was successfully executed; during the command,
          // the CM11 received a powerline notification which it is now
          // sending to the PC -- leave the 0x5A in the serial port "peek"
          // buffer so the notification will be processed next
          FireLogMessage(PushedByteBack, ackReply);
          return true;
        }
        else
        {
          // reply incorrect -- reset communications; we "peeked" <ackReply>
          // above above, so read it now
          ReadSerialPortByte(SERIAL_TIMEOUT);
          FireLogMessage(IncorrectChecksumAckReply);
          throw new HardResyncException();
        }
    }

    /// <summary>
    /// Parses a <r>DwellNet.Cm11</r> <i>house code</i>; for example, "A".
    /// </summary>
    ///
    /// <param name="houseCode">The house code to parse.  House codes are
    /// 	case-sensitive; "A" is a house code, but "a" is not.</param>
    ///
    /// <returns>
    /// A CM11 <i>house code</i> nibble value, between 0 and 15 inclusive,
    /// corresponding to <paramref name="houseCode"/>.  Returns <n>null</n> if
    /// <paramref name="houseCode"/> isn't a house code.
    /// </returns>
    ///
    byte? ParseHouseCode(string houseCode)
    {
      if (houseCode.Length != 1)
        return null;
      return HouseCodeToNibble(houseCode[0], false);
    }

    /// <summary>
    /// Parses a <r>DwellNet.Cm11</r> <i>device address</i>; for example, "A1".
    /// </summary>
    ///
    /// <param name="address">The address to parse.  Addresses are
    /// 	case-sensitive; "A10" is a valid address, but "a10" is not.</param>
    ///
    /// <returns>
    /// A CM11 <i>address code</i> byte value corresponding to the address.
    /// The house code nibble value (from <r>s_codesToNibbles</r>) is in the
    /// high-order 4 bits, and the device code nibble value (also from
    /// <r>s_codesToNibbles</r>) is in the low-order 4 bits.  Returns
    /// <n>null</n> if <paramref name="address"/> doesn't appear to be an
    /// address.
    /// </returns>
    ///
    /// <remarks>
    /// <para>
    /// Valid addreses include a house code between "A" and "P" inclusive and
    /// a device code between "1" and "16" inclusive; examples: "A1", "C12",
    /// "P16".
    /// </para>
    /// <para>
    /// If <param name="address"/> isn't an address (for example, if
    /// <param name="address"/> is "Abc"), <n>null</n> is returned.  However,
    /// if <param name="address"/> looks like an address address but the device
    /// code is out of range (for example, "A99"), a
    /// <r>Cm11InvalidCommandException</r> is thrown.
    /// </para>
    /// </remarks>
    ///
    /// <exception cref="Cm11InvalidCommandException">
    /// Thrown when the syntax of <paramref name="address"/> is incorrect.
    /// See Remarks for more information.
    /// </exception>
    ///
    byte? ParseAddress(string address)
    {
      // parse <address>; set <houseCodeNibble> and <deviceCodeNibble> to
      // the corresponding house code and device code nibbles; throw a
      // Cm11InvalidCommandException if the device code is out of range
      Match match = s_addressRegex.Match(address);
      if (!match.Success)
        return null;
      byte houseCodeNibble = HouseCodeToNibble(
        match.Groups[1].Value[0], true).Value;
      byte deviceCodeNibble = DeviceCodeToNibble(
        int.Parse(match.Groups[2].Value));

      // return the byte code corresponding to this address
      return (byte)((houseCodeNibble << 4) | deviceCodeNibble);
    }

    /// <summary>
    /// Converts a CM11 address code byte (e.g. 0x6E) to its string
    /// representation (e.g. "A2").
    /// </summary>
    ///
    static string AddressByteToString(byte addressCode)
    {
      return HouseCodeNibbleToChar((byte)((addressCode >> 4) & 0xF)) +
        DeviceCodeNibbleToString((byte)(addressCode & 0xF));
    }

    /// <summary>
    /// Parses a <r>DwellNet.Cm11</r> <i>function command</i>; for example,
    /// "A.On", "On", "B.Dim70", etc.
    /// </summary>
    ///
    /// <param name="command">The <r>DwellNet.Cm11</r> command to parse.
    /// 	Commands are case-sensitive; for example, "Dim70" is a valid
    /// 	command, but "dim70" is not.  See Cm11Help.htm for a list of valid
    /// 	<r>DwellNet.Cm11</r> commands.</param>
    ///
    /// <param name="defaultHouseCodeNibble">The X10 house code nibble value
    /// 	to assume for the command (i.e. that the function will apply to)
    /// 	if <paramref name="command"/> doesn't contain a house code prefix
    /// 	(e.g. "A.").  -1 indicates that there is no default to use.</param>
    ///
    /// <returns>
    /// The CM11 2-byte sequence corresponding to <paramref name="command"/>,
    /// or <n>null</n> if <paramref name="command"/> doesn't appear to be a
    /// function command.
    /// </returns>
    ///
    /// <remarks>
    /// <para>
    /// If <param name="command"/> isn't a <r>DwellNet.Cm11</r> command,
    /// <n>null</n> is returned.  However, if <param name="command"/> looks
    /// like a valid <r>Execute</r> command but a parameter is out of range
    /// (for example, "Dim999"), a <r>Cm11InvalidCommandException</r> is
    /// thrown.
    /// </para>
    /// </remarks>
    ///
    /// <exception cref="Cm11InvalidCommandException">
    /// Thrown when the syntax or usage of <paramref name="command"/> is
    /// incorrect.  See Remarks for more information.
    /// </exception>
    ///
    byte[] ParseFunction(string command, int defaultHouseCodeNibble)
    {
      // see if <command> is a "Dim<percent>" command; if so, return its
      // 2-byte sequence
      byte[] bytes = ParseBrightenOrDimCommand(command,
        defaultHouseCodeNibble);
      if (bytes != null)
        return bytes;

      // see if <command> is follows the general syntax of any other
      // *function command* (than the ones checked for above); if so, set
      // <houseCode> and <commandName> to its house code prefix (e.g. "A", or
      // "" if none) and command name (e.g. "AllLightsOn")
      Match match = s_functionRegex.Match(command);
      if (!match.Success)
        return null;
      string houseCode = match.Groups[1].Value;
      string commandName = match.Groups[2].Value;

      // see if <commandName> is an X10Function member; if so, set
      // <x10Function> to it; if not, return null since at that point we know
      // <command> is not a valid DwellNet.Cm11 command
      X10Function x10Function;
      try
      {
        x10Function = (X10Function)Enum.Parse(typeof(X10Function),
                  commandName, false);
      }
      catch (ArgumentException)
      {
        return null;
      }

      // set <houseCodeNibble> to the *house code nibble* of the command,
      // using <defaultHouseCodeNibble> as a default (if provided)
      byte? houseCodeNibble = ParseHouseCode(houseCode);
      if (houseCodeNibble == null)
      {
        if (defaultHouseCodeNibble < 0)
        {
          throw new Cm11InvalidCommandException(NoHouseCode,
            command);
        }
        else
          houseCodeNibble = (byte)defaultHouseCodeNibble;
      }

      // return the 2-byte sequence for this command
      return new byte[]
		{
			(byte) 0x06,
			(byte) ((byte) x10Function | (houseCodeNibble << 4))
		};
    }

    /// <summary>
    /// Parses a "Dim&lt;percent&gt;" or "Brighten&lt;percent&gt;" command;
    /// for example "Dim70".
    /// </summary>
    ///
    /// <param name="command">The command to parse.  Commands are
    /// 	case-sensitive; "Dim70" is a valid command, but "dim70" is not.
    /// 	</param>
    ///
    /// <param name="defaultHouseCodeNibble">The X10 house code nibble value
    /// 	to assume for the command (i.e. that the function will apply to)
    /// 	if <paramref name="command"/> doesn't contain a house code prefix
    /// 	(e.g. "A.").  -1 indicates that there is no default to use.</param>
    ///
    /// <returns>
    /// The CM11 2-byte sequence corresponding to <paramref name="command"/>,
    /// or <n>null</n> if <paramref name="command"/> doesn't appear to be a
    /// "Dim" or "Brighten" command.
    /// </returns>
    ///
    /// <remarks>
    /// <para>
    /// Valid "Dim" commands are "Dim0", "Dim1", etc. up to and including
    /// "Dim100".  Similarly, valid "Brighten" commands are "Brighten0",
    /// "Brighten1", etc. up to and including "Brighten100".
    /// </para>
    /// <para>
    /// If <param name="command"/> isn't a "Dim&lt;percent&gt;" command
    /// (for example, if <param name="command"/> is "Abc"), <n>null</n> is
    /// returned.  However, if <param name="command"/> looks like a "Dim"
    /// command but the dim amount is out of range (for example, "Dim999"),
    /// a <r>Cm11InvalidCommandException</r> is thrown.
    /// </para>
    /// </remarks>
    ///
    /// <exception cref="Cm11InvalidCommandException">
    /// Thrown when the syntax or usage of <paramref name="command"/> is
    /// incorrect.  See Remarks for more information.
    /// </exception>
    ///
    byte[] ParseBrightenOrDimCommand(string command,
      int defaultHouseCodeNibble)
    {
      // parse <command>; set <houseCodeNibble> to the *house code nibble*
      // (using <defaultHouseCodeNibble> as the default, if provided); set
      // <x10Function> to the X10Function value (X10Function.Dim or
      // X10Function.Brighten); set <amount> to the brighten-by or dim-by
      // percentage amount (0 to 100)
      X10Function x10Function;
      Match match = s_dimRegex.Match(command);
      if (match.Success)
        x10Function = X10Function.Dim;
      else
      {
        match = s_brightenRegex.Match(command);
        if (match.Success)
          x10Function = X10Function.Brighten;
        else
          return null; // not a "Dim" or "Brighten" command
      }
      byte? houseCodeNibble = ParseHouseCode(match.Groups[1].Value);
      if (houseCodeNibble == null)
      {
        if (defaultHouseCodeNibble < 0)
        {
          throw new Cm11InvalidCommandException(NoHouseCode,
            command);
        }
        else
          houseCodeNibble = (byte)defaultHouseCodeNibble;
      }
      string amountString = match.Groups[2].Value;
      if (amountString.Length == 0)
      {
        // missing dim amount percent
        throw new Cm11InvalidCommandException(InvalidCommand,
          command);
      }
      int amount = int.Parse(match.Groups[2].Value);
      if (amount > 100)
      {
        throw new Cm11InvalidCommandException(BrightenOrDimAmountOutOfRange,
          amount);
      }

      // return the 2-byte sequence for this command
      return new byte[]
		{
			(byte) ((((amount * 22 + 50) / 100) << 3) | 0x06),
			(byte) ((byte) x10Function | (houseCodeNibble << 4))
		};
    }

    /// <summary>
    /// Parses a "Hex&lt;hex-digits&gt;" command; for example "Hex046E".
    /// This command results in arbitrary bytes being sent to the CM11.
    /// This command is primarily for debugging purposes.
    /// </summary>
    ///
    /// <param name="command">The command to parse.  Commands are
    /// 	case-sensitive; "Hex1A" is a valid command, but "hex1A" is not.
    /// 	However, the hexadecimal digits are not case-sensitive, so
    /// 	"Hex1a" is valid.
    /// 	</param>
    ///
    /// <returns>
    /// The CM11 byte sequence corresponding to <paramref name="command"/>,
    /// or <n>null</n> if <paramref name="command"/> doesn't appear to be a
    /// "Hex" command.
    /// </returns>
    ///
    /// <remarks>
    /// <para>
    /// Valid "Hex" commands are "Hex&lt;hex-digits&gt;", where
    /// &lt;hex-digits&gt; consist of an even number of hexadecimal digits
    /// two or more.
    /// "Hex100".
    /// </para>
    /// <para>
    /// If <param name="command"/> isn't a "Hex&lt;hex-digits&gt;" command
    /// (for example, if <param name="command"/> is "Abc"), <n>null</n> is
    /// returned.  However, if <param name="command"/> looks like a "Hex"
    /// command but &lt;hex-digits&gt; doesn't contain an even number of valid
    /// hexadecimal digits, a <r>Cm11InvalidCommandException</r> is thrown.
    /// </para>
    /// </remarks>
    ///
    /// <exception cref="Cm11InvalidCommandException">
    /// Thrown when the syntax of <paramref name="command"/> is incorrect.
    /// See Remarks for more information.
    /// </exception>
    ///
    byte[] ParseHexCommand(string command)
    {
      // parse <command>; set <hexDigits> to the string of hexadecimal digits
      Match match = s_hexRegex.Match(command);
      if (!match.Success)
        return null; // not a "Hex<hex-digits>" command
      string hexDigits = match.Groups[1].Value;
      if ((hexDigits.Length == 0) || ((hexDigits.Length & 1) == 1))
      {
        // missing or odd number of hex-digits
        throw new Cm11InvalidCommandException(InvalidHexDigits,
          command);
      }

      // parse <hexDigits> into <bytes>
      byte[] bytes = new byte[hexDigits.Length / 2];
      for (int iByte = 0; iByte < bytes.Length; iByte++)
      {
        bytes[iByte] = (byte)int.Parse(hexDigits.Substring(iByte * 2, 2),
          System.Globalization.NumberStyles.AllowHexSpecifier);
      }

      return bytes;
    }

    /// <summary>
    /// Formats a <i>function command</i> as a string, given the binary
    /// components of the <i>function command</i>.
    /// </summary>
    ///
    /// <param name="houseCodeNibble">The <i>house code nibble</i> of the house
    /// 	code of the <i>function command</i>.
    /// 	</param>
    ///
    /// <param name="x10Function">The <i>function code nibble</i> of the
    /// 	<i>function code</i> of the <i>function command</i>.
    /// 	</param>
    ///
    /// <param name="amount22">The brighten-by or dim-by amount of the
    /// 	<i>function command</i> -- a value between 0 and 22 inclusive.
    /// 	This parameter is ignored if the <i>function command</i> isn't
    ///		a "Dim" or "Brighten" command.</param>
    ///
    string FormatFunctionCommand(byte houseCodeNibble, X10Function x10Function,
      int amount22)
    {
      if ((x10Function == X10Function.Dim) ||
          (x10Function == X10Function.Brighten))
      {
        return String.Format("{0}.{1}{2}",
          HouseCodeNibbleToChar(houseCodeNibble), x10Function,
          (amount22 * 100 + 11) / 22);
      }
      else
      {
        return String.Format("{0}.{1}",
          HouseCodeNibbleToChar(houseCodeNibble), x10Function);
      }
    }

    /// <summary>
    /// Writes bytes to the serial port.
    /// </summary>
    ///
    /// <param name="bytes">The bytes to write to the serial port.</param>
    ///
    /// <param name="logMessageFormat">The format string of a log message to
    /// 	send to the application.  The formatted string is provided to the
    /// 	application within a <r>LogMessage</r> event.</param>
    ///
    /// <param name="logMessageArgs">Formatting arguments for
    /// 	<paramref name="logMessageFormat"/>.</param>
    ///
    /// <remarks>
    /// If a serial communication occurs, <r>Close</r> is called automatically
    /// and a <r>Cm11CommunicationException</r> is thrown.
    /// </remarks>
    ///
    /// <exception cref="Cm11CommunicationException">
    /// Thrown when an error occurs while attempting to communicate with the
    /// CM11 device.
    /// </exception>
    ///
    void WriteToSerialPort(byte[] bytes, string logMessageFormat,
      params object[] logMessageArgs)
    {
      FireLogMessage("CM11 <-- {0} ({1})", FormatBytes(bytes),
        String.Format(logMessageFormat, logMessageArgs));
      m_serialPort.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Fires an <r>OnReceived</r> event.
    /// </summary>
    ///
    void FireOnReceived(string address)
    {
      if (OnReceived != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          OnReceived(address);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(OnReceived,
            new object[] { address });
        }
      }
    }

    /// <summary>
    /// Fires an <r>OffReceived</r> event.
    /// </summary>
    ///
    void FireOffReceived(string address)
    {
      if (OffReceived != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          OffReceived(address);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(OffReceived,
            new object[] { address });
        }
      }
    }

    /// <summary>
    /// Fires a <r>BrightenReceived</r> event.
    /// </summary>
    ///
    void FireBrightenReceived(string address, int percent)
    {
      if (BrightenReceived != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          BrightenReceived(address, percent);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(BrightenReceived,
            new object[] { address, percent });
        }
      }
    }

    /// <summary>
    /// Fires a <r>DimReceived</r> event.
    /// </summary>
    ///
    void FireDimReceived(string address, int percent)
    {
      if (DimReceived != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          DimReceived(address, percent);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(DimReceived,
            new object[] { address, percent });
        }
      }
    }

    /// <summary>
    /// Fires an <r>AllLightsOnReceived</r> event.
    /// </summary>
    ///
    void FireAllLightsOnReceived(char houseCode)
    {
      if (AllLightsOnReceived != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          AllLightsOnReceived(houseCode);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(AllLightsOnReceived,
            new object[] { houseCode });
        }
      }
    }

    /// <summary>
    /// Fires an <r>AllLightsOffReceived</r> event.
    /// </summary>
    ///
    void FireAllLightsOffReceived(char houseCode)
    {
      if (AllLightsOffReceived != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          AllLightsOffReceived(houseCode);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(AllLightsOffReceived,
            new object[] { houseCode });
        }
      }
    }

    /// <summary>
    /// Fires an <r>AllOffReceived</r> event.
    /// </summary>
    ///
    void FireAllOffReceived(char houseCode)
    {
      if (AllOffReceived != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          AllOffReceived(houseCode);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(AllOffReceived,
            new object[] { houseCode });
        }
      }
    }

    /// <summary>
    /// Fires a <r>Notification</r> event.
    /// </summary>
    ///
    void FireNotification(string commandName, int commandParameter)
    {
      if (Notification != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          Notification(commandName, commandParameter);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(Notification,
            new object[] { commandName, commandParameter });
        }
      }
    }

    /// <summary>
    /// Fires a <r>IdleStateChange</r> event.
    /// </summary>
    ///
    void FireIdleStateChange(bool idle)
    {
      if (IdleStateChange != null)
      {
        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          IdleStateChange(idle);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(IdleStateChange,
            new object[] { idle });
        }
      }
    }

    /// <summary>
    /// Formats an error message and fires an <r>Error</r> event containing
    /// that message.
    /// </summary>
    ///
    void FireError(string format, params object[] args)
    {
      string message = String.Format(format, args);
      TraceInfo("ERROR: " + message);
      if (Error != null)
      {

        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          Error(message);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(Error, new object[] { message });
        }
      }
    }

    /// <summary>
    /// Formats a log message and fires a <r>LogMessage</r> event containing
    /// that message.
    /// </summary>
    ///
    void FireLogMessage(string format, params object[] args)
    {
      string message = String.Format(format, args);
      TraceInfo(message);
      if (LogMessage != null)
      {

        if ((m_invokeEventsUsing == null) ||
            !m_invokeEventsUsing.InvokeRequired)
        {
          // fire the event on the current thread
          LogMessage(message);
        }
        else
        {
          // fire the event on an application thread
          m_invokeEventsUsing.Invoke(LogMessage,
            new object[] { message });
        }
      }
    }

    /// <summary>
    /// Converts a house code ('A' to 'P') to a <i>house code nibble</i> value.
    /// </summary>
    ///
    /// <param name="houseCode">The house code, 'A' through 'P' inclusive.
    /// 	This corresponds to the house code value set on the X10 device.
    /// 	This value is case-sensitive.</param>
    ///
    /// <param name="exceptionOnError">If <n>true</n>, then an exception is
    /// 	thrown if <paramref name="houseCode"/> isn't a valid house code.
    /// 	If <n>false</n>, then <n>null</n> is returned if 
    /// 	<paramref name="houseCode"/> isn't a valid house code.</param>
    ///
    /// <returns>
    /// The <i>house code nibble</i> value corresponding to
    /// <paramref name="houseCode"/>.  For example, house code "A" corresponds
    /// to <i>house code nibble</i> value 0x6.  Returns <n>null</n> if
    /// <paramref name="houseCode"/> isn't a valid house code and
    /// <paramref name="exceptionOnError"/> is <n>false</n>.
    /// </returns>
    ///
    /// <exception cref="Cm11InvalidCommandException">
    /// Thrown if <paramref name="houseCode"/> isn't a valid house code
    /// and <paramref name="exceptionOnError"/> is <n>true</n>.
    /// </exception>
    ///
    byte? HouseCodeToNibble(char houseCode, bool exceptionOnError)
    {
      if ((houseCode >= 'A') && (houseCode <= 'P'))
        return s_codesToNibbles[houseCode - 'A'];
      else
        if ((houseCode > ' ') && (houseCode < 0x7F))
        {
          if (exceptionOnError)
          {
            throw new Cm11InvalidCommandException(InvalidHouseCode,
              houseCode);
          }
          else
            return null;
        }
        else
        {
          if (exceptionOnError)
          {
            throw new Cm11InvalidCommandException(InvalidHouseCode,
              String.Format("0x{0:X2}", houseCode));
          }
          else
            return null;
        }
    }

    /// <summary>
    /// Converts a CM11 house code nibble value (e.g. 0x6) to a character
    /// representation (e.g. "A").
    /// </summary>
    ///
    static char HouseCodeNibbleToChar(byte houseCodeNibble)
    {
      return (char)('A' + s_nibblesToCodes[houseCodeNibble]);
    }

    /// <summary>
    /// Converts a device code (1 to 16) to a nibble value used in the X10
    /// protocol.
    /// </summary>
    ///
    /// <param name="deviceCode">The device code, 1 through 16 inclusive.
    /// 	This corresponds to the device code value set on the X10 device.
    /// 	</param>
    ///
    byte DeviceCodeToNibble(int deviceCode)
    {
      if ((deviceCode >= 1) && (deviceCode <= 16))
        return s_codesToNibbles[deviceCode - 1];
      else
      {
        throw new Cm11InvalidCommandException(InvalidDeviceCode,
          deviceCode);
      }
    }

    /// <summary>
    /// Converts a CM11 device code nibble value (e.g. 0xE) to the integer form
    /// of its string representation (e.g. 2).
    /// </summary>
    ///
    static int DeviceCodeNibbleToInt(byte deviceCodeNibble)
    {
      return s_nibblesToCodes[deviceCodeNibble] + 1;
    }

    /// <summary>
    /// Converts a CM11 device code nibble value (e.g. 0xE) to a string
    /// representation (e.g. "2").
    /// </summary>
    ///
    static string DeviceCodeNibbleToString(byte deviceCodeNibble)
    {
      return String.Format("{0}", DeviceCodeNibbleToInt(deviceCodeNibble));
    }

    /// <summary>
    /// Formats and writes a line of informational message text to trace
    /// listeners, in debug builds only.  The line is prefixed with a string
    /// that identifies the message as coming from this class.
    /// </summary>
    /// 
    /// <param name="format">The format string.</param>
    ///
    /// <param name="args">Formatting arguments</param>
    ///
    [Conditional("DEBUG")]
    protected void TraceInfo(string format, params object[] args)
    {
#if DEBUG
      // no "lock (m_lock)" -- this operation is thread-safe
      Trace.WriteLine(String.Format("Cm11 {0:n3}: {1}",
          ((double)m_traceStopwatch.ElapsedMilliseconds) / 1000,
          String.Format(format, args)));
#endif
    }

    /// <summary>
    /// Writes a line of exception message text to trace listeners.  The line
    /// is prefixed with a string that identifies the message as coming from
    /// this class.
    /// </summary>
    /// 
    /// <param name="ex">An exception to display the message text of.</param>
    ///
    protected void TraceException(Exception ex)
    {
      TraceException(ex, ex.GetType().Name);
    }

    /// <summary>
    /// Formats and writes a line of error message text to trace listeners,
    /// followed by message text from a provided exception.  The line is
    /// prefixed with a string that identifies the message as coming from this
    /// class.
    /// </summary>
    /// 
    /// <param name="ex">An exception to display the message text of.</param>
    ///
    /// <param name="format">The format string.</param>
    ///
    /// <param name="args">Formatting arguments</param>
    ///
    protected void TraceException(Exception ex, string format,
    params object[] args)
    {
      // no "lock (m_lock)" -- this operation is thread-safe...
      Trace.WriteLine(String.Format("Cm11: {0}{1}",
    String.Format(format, args), GetExceptionFullMessage(": ", ex)));
    }

    /// <summary>
    /// Returns the concatenation of the exception message of a given exception
    /// and its inner exceptions.
    /// </summary>
    ///
    /// <param name="prefix">A string to prefix the returned exception message
    /// 	text with; for example, ": ".  This prefix is not used if the
    /// 	exception message text is an empty string.</param>
    ///
    /// <param name="ex">The exception to retrieve the message of.</param>
    ///
    static string GetExceptionFullMessage(string prefix, Exception ex)
    {
      StringBuilder exceptionMessage = new StringBuilder(1000);
      while (ex != null)
      {
        if (exceptionMessage.Length == 0)
        {
          if (prefix != null)
            exceptionMessage.Append(prefix);
        }
        else
          exceptionMessage.Append(": ");
        exceptionMessage.Append(ex.Message);
        ex = ex.InnerException;
      }
      return exceptionMessage.ToString();
    }

    /// <summary>
    /// Formats a byte array as a string in hexadecimal form.
    /// </summary>
    ///
    static string FormatBytes(byte[] bytes)
    {
      StringBuilder result = new StringBuilder(3 * bytes.Length);
      foreach (byte b in bytes)
      {
        if (result.Length > 0)
          result.Append(' ');
        result.AppendFormat("{0:X2}", b);
      }
      return result.ToString();
    }

    /// <summary>
    /// Calculates a byte checksum of a byte array.
    /// </summary>
    ///
    static byte CalculateChecksum(byte[] bytes)
    {
      int checksum = 0;
      foreach (byte b in bytes)
        checksum += b;
      return (byte)(checksum & 0xFF);
    }

    //////////////////////////////////////////////////////////////////////////
    // Private Helper Classes and Enumerations
    //

    /// <summary>
    /// A method with no parameters and no return value.
    /// </summary>
    ///
    delegate void VoidDelegate();

    /// <summary>
    /// Indicates that a serious problem occurred with the serial port (or
    /// the device connected to it) requiring the serial port to be reset
    /// and <r>m_commandQueue</r> to be cleared (to prevent commands from
    /// queuing up indefinitely with nowhere to go).
    /// </summary>
    ///
    class HardResyncException : Exception
    {
    }

    /// <summary>
    /// Indicates that <r>m_quitting</r> was set to true and so the
    /// executing code decided it was time to stop executing.
    /// </summary>
    ///
    class QuittingException : Exception
    {
    }

    /// <summary>
    /// Specifies what situation caused a <c>Wait*</c> method to return.
    /// </summary>
    ///
    enum WaitResult
    {
      /// <summary>
      /// No bytes are in the serial input buffer, and
      /// <r>m_commandQueue</r> is empty.
      /// </summary>
      Idle,

      /// <summary>
      /// One or more bytes were received from the CM11 device and are
      /// waiting to be read in <r>m_serialPort</r>.
      /// </summary>
      SerialInput,

      /// <summary>
      /// One or more commands were queued by the application into
      /// <r>m_commandQueue</r> and are waiting to be executed.
      /// </summary>
      CommandQueued,
    }

    /// <summary>
    /// Parses the data bytes sent within a <i>device notification</i>.
    /// </summary>
    ///
    class NotificationDataParser
    {
      // private fields
      byte[] m_dataBytes;
      int m_iDataByte; // index within <m_dataBytes>
      int m_dataByteSpecifier; // see Cm11.doc

      /// <summary>
      /// Returns true if we've reached the end of the data bytes, in which
      /// case calling <r>GetNextDataByte</r> will throw an
      /// <r>InvalidOperationException</r>.
      /// </summary>
      ///
      public bool AtEndOfDataBytes
      {
        get
        {
          return m_iDataByte >= m_dataBytes.Length - 1;
        }
      }

      /// <summary>
      /// Initializes an instance of the <r>NotificationDataParser</r> class.
      /// </summary>
      ///
      /// <param name="dataBytes">Between 2 and 9 (inclusive) data bytes
      /// 	sent from the CM11.</param>
      ///
      public NotificationDataParser(byte[] dataBytes)
      {
        // initialize state 
        m_dataBytes = dataBytes;
        m_dataByteSpecifier = dataBytes[0];

        // there must be at least 2 data bytes: one for the data byte
        // specifier and at least one following data byte
        if ((dataBytes.Length < 2) || (dataBytes.Length > 9))
          throw new ArgumentException("dataBytes");

        // set <m_dataByteSpecifier> to the *data byte specifier* which
        // contains one bit for each following data byte indicating if
        // that data byte is a function code (bit = 1) or an address or
        // parameter (bit = 0) -- see Cm11.doc for more
        // information
        m_dataByteSpecifier = dataBytes[0];
      }

      /// <summary>
      /// Returns the next data byte.
      /// </summary>
      ///
      /// <param name="isFunctionCode">Set to <n>true</n> if the returned
      /// 	data byte is a <i>function code</i>.  Set to <n>false</n> if
      /// 	the returned data byte is either an <i>address code</i> or a
      /// 	<i>function parameter</i>.</param>
      ///
      /// <exception cref="InvalidOperationException">
      /// Thrown if <r>AtEndOfDataBytes</r> is <n>true</n>
      /// </exception>
      ///
      public byte GetNextDataByte(out bool isFunctionCode)
      {
        // advance <m_iDataByte>, unless we're at the end of the data bytes
        if (m_iDataByte >= m_dataBytes.Length - 1)
          throw new InvalidOperationException();
        else
          m_iDataByte++;

        // set <isFunctionCode> based on the bit in <m_dataByteSpecifier>
        // corrsponding to the current index within <m_dataBytes>
        isFunctionCode =
          ((m_dataByteSpecifier >> (m_iDataByte - 1) & 1) == 1);

        // return the data byte
        return m_dataBytes[m_iDataByte];
      }

      /// <summary>
      /// Pushes the last byte returned by <r>GetNextDataByte</r> back into
      /// the parser, so it will be returned on the next call to
      /// <r>GetNextDataByte</r>.
      /// </summary>
      ///
      /// <exception cref="InvalidOperationException">
      /// Thrown if <r>GetNextDataByte</r> wasn't previously called.
      /// </exception>
      ///
      public void UngetDataByte()
      {
        m_iDataByte--;
      }
    }

    /// <summary>
    /// Tracks which X10 devices are <i>currently addressed</i>.
    /// </summary>
    ///
    /// <remarks>
    /// An instance of this class can be used to keep track of which X10
    /// devices are <i>currently addressed</i> -- see Cm11.doc for a
    /// definition of that term.  This class doesn't interact with the CM11
    /// hardware at all -- it's simply a record-keeper used internally by the
    /// <r>Cm11</r> class.
    /// </remarks>
    ///
    class AddressTracker
    {
      /// <summary>
      /// Tracks which X10 devices are <i>currently addressed</i>.
      /// </summary>
      ///
      /// <remarks>
      /// Each element of <r>m_addressedDevices</r> contains information
      /// about devices with a single house code; the index of
      /// <r>m_addressedDevices</r> is a <i>house code nibble</i>.
      /// Within each element, bit number <c>i</c> (0x0 to 0xF) is set to
      /// 1 if the device with <i>device code nibble</i> <c>i</c> is
      /// <i>currently addressed</i>, 0 if the device is <i>currently not
      /// addressed</i>.
      /// </remarks>
      ///
      UInt16[] m_addressedDevices = new UInt16[16];

      /// <summary>
      /// If <n>true</n>, an exception is thrown if the caller makes two
      /// consecutive calls to <r>RegisterAddressCommand</r> that specify
      /// different house codes.
      /// </summary>
      ///
      /// <remarks>
      /// For example, in single-house mode, "A1 A2 B1" is invalid, but
      /// "A1 A2 A.On B1 B.On" is valid.  Also, "A.On" alone is valid
      /// (it applies to whatever the <i>currently addressed</i> devices
      /// are on the physical X10 network with house code "A"), and "A1 A2
      /// A.On B.On" is also valid (the last "B.On" similarly applies to the
      /// <i>currently addressed</i> devices on the physical X10 network
      /// with house code "B".)</remarks>
      ///
      bool m_singleHouseMode;

      /// <summary>
      /// <n>true</n> if <r>RegisterFunctionCommand</r> was called more
      /// recently than <r>RegisterAddressCommand</r> for a given house code.
      /// </summary>
      ///
      /// <remarks>
      /// The index of <r>m_lastCommandWasFunction</r> is a <i>house code
      /// nibble</i>.
      /// </remarks>
      ///
      bool[] m_lastCommandWasFunction = new bool[16];

      /// <summary>
      /// Holds the value of the <r>LastCommandHouseCodeNibble</r> property.
      /// </summary>
      int m_lastCommandHouseCodeNibble = -1;

      /// <summary>
      /// The <i>house code nibble</i> specified in the most recent call to
      /// <r>RegisterAddressCommand</r> or <r>RegisterFunctionCommand</r>.
      /// -1 means that neither of these methods was called yet.
      /// </summary>
      public int LastCommandHouseCodeNibble
      {
        get
        {
          return m_lastCommandHouseCodeNibble;
        }
      }

      /// <summary>
      /// Returns an enumeration of the addresses (e.g. "A1") of all
      /// <i>currently addressed devices</i> with a given house code.
      /// </summary>
      ///
      /// <param name="houseCodeNibble">The <i>house code nibble</i> value
      /// 	of the house code for which to enumerate <i>currently
      /// 	addressed devices</i> of.</param>
      ///
      public IEnumerable<string> GetAddressedDevices(byte houseCodeNibble)
      {
        if (houseCodeNibble >= 16)
          throw new ArgumentException("houseCodeNibble");
        UInt16 devices = m_addressedDevices[houseCodeNibble];
        char houseCode = HouseCodeNibbleToChar(houseCodeNibble);
        UInt16 mask = 1;
        byte deviceCodeNibble = 0;
        while (devices != 0)
        {
          if ((devices & mask) != 0)
          {
            yield return String.Format("{0}{1}", houseCode,
              DeviceCodeNibbleToInt(deviceCodeNibble));
          }
          deviceCodeNibble++;
          devices = (UInt16)(devices & ~mask);
          mask = (UInt16)(mask << 1);
        }
      }

      /// <summary>
      /// Initializes a new instance of the <r>AddressTracker</r> class.
      /// </summary>
      ///
      /// <param name="singleHouseMode">If <n>true</n>, an exception is
      /// 	thrown if two consecutive <i>address commands</i> have
      /// 	different house codes.</param>
      ///
      public AddressTracker(bool singleHouseMode)
      {
        m_singleHouseMode = singleHouseMode;
      }

      /// <summary>
      /// Updates the internal state of this object to reflect the fact that
      /// an <i>address command</i> has executed and, as a result, a given
      /// X10 device is now <i>currently addressed</i>.
      /// </summary>
      ///
      /// <param name="addressCode">The <i>address code</i> of the device
      /// 	to add to the set of <i>currently addressed</i> devices.
      /// 	</param>
      ///
      /// <exception cref="Cm11InvalidCommandException">
      /// Thrown in single-house mode (see the constructor) when two
      /// consecutive <i>address commands</i> have different house codes.
      /// </exception>
      ///
      public void RegisterAddressCommand(byte addressCode)
      {
        // divide <addressCode> into nibbles
        byte houseCodeNibble = (byte)((addressCode >> 4) & 0xF);
        byte deviceCodeNibble = (byte)(addressCode & 0xF);

        // if we're in single-house mode, it's an error for two consecutive
        // *address commands* to have different house codes
        if (m_singleHouseMode)
        {
          if ((m_lastCommandHouseCodeNibble != -1) &&
              !m_lastCommandWasFunction[m_lastCommandHouseCodeNibble] &&
            (m_lastCommandHouseCodeNibble != houseCodeNibble))
          {
            throw new Cm11InvalidCommandException(
              InconsistentHouseCodes);
          }
        }

        // update <m_addressedDevices> based on the nibbles
        if (m_lastCommandWasFunction[houseCodeNibble])
        {
          // this is the first *address command* in a sequence of one or
          // more *address commands* for this house code -- so reset all
          // device codes for this house code, except this device code,
          // to *currently not addressed* state
          m_addressedDevices[houseCodeNibble] =
            (ushort)(1 << deviceCodeNibble);
        }
        else
        {
          // this is the second or later *address command* for this house
          // code -- add it to the set of *currently addressed* state
          m_addressedDevices[houseCodeNibble] |=
            (ushort)(1 << deviceCodeNibble);
        }

        // update other state
        m_lastCommandWasFunction[houseCodeNibble] = false;
        m_lastCommandHouseCodeNibble = houseCodeNibble;
      }

      /// <summary>
      /// Updates the internal state of this object to reflect the fact that
      /// a <i>function command</i> has excuted and, as a result, all X10
      /// devices with the same house code as the <i>function command</i>
      /// will become <i>currently not addressed</i> when the next
      /// <i>address command</i> for that house code starts to execute.
      /// </summary>
      ///
      /// <param name="houseCodeNibble">The <i>house code nibble</i> of the
      /// 	house code of the <i>function command</i> that executed.
      /// 	</param>
      ///
      public void RegisterFunctionCommand(byte houseCodeNibble)
      {
        // update state
        Debug.Assert(houseCodeNibble <= 0xF);
        m_lastCommandWasFunction[houseCodeNibble] = true;
        m_lastCommandHouseCodeNibble = houseCodeNibble;
      }
    }

    /// <summary>
    /// Provides <r>DnSerialPort</r> with its localized strings.
    /// </summary>
    ///
    internal class DnSerialPortStringResources : DnSerialPortStrings
    {
      public override string SerialPortError
      {
        get
        {
          return DnSerialPort_SerialPortError;
        }
      }
    }
  }

  /// <summary>
  /// Indicates an error in a command provided to the <r>Cm11</r> class.
  /// </summary>
  ///
  public class Cm11InvalidCommandException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <r>Cm11InvalidCommandException</r>
    /// class.  Formats a given error string.
    /// </summary>
    ///
    internal Cm11InvalidCommandException(string format, params object[] args) :
      base(String.Format(format, args))
    {
    }
  }

  /// <summary>
  /// Indicates an error in communicating with the CM11.  <r>Cm11.Close</r> is
  /// called automatically when this exception occurs.
  /// </summary>
  ///
  public class Cm11CommunicationException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the Cm11InvalidException class.
    /// Formats a given error string.
    /// </summary>
    ///
    internal Cm11CommunicationException(string format, params object[] args) :
      base(String.Format(format, args))
    {
    }
  }

  /// <summary>
  /// The type of an event handler for an event which is fired to indicate that
  /// a <i>funtion command</i> that applies to an X10 device was transmitted on
  /// the X10 network.
  /// </summary>
  ///
  /// <param name="address">The address of the device that the notification
  /// 	applies to; for example, "A1" or "P16".</param>
  ///
  /// <remarks>
  /// See the events such as <r>Cm11.OnReceived</r> and <r>Cm11.OffReceived</r>.
  /// </remarks>
  ///
  public delegate void Cm11DeviceNotificationEventDelegate(string address);

  /// <summary>
  /// The type of an event handler for an event which is fired to indicate that
  /// a "Brighten" or "Dim" command was transmitted on the X10 network.
  /// </summary>
  ///
  /// <param name="address">The address of the device that the notification
  /// 	applies to; for example, "A1" or "P16".</param>
  ///
  /// <param name="percent">The amount that the device was brighted or dimmed by,
  ///		as a percentage between 0 and 100 inclusive.</param>
  ///
  /// <remarks>
  /// See the events such as <r>Cm11.BrightenReceived</r> and
  /// <r>Cm11.DimReceived</r>.
  /// </remarks>
  ///
  public delegate void Cm11BrightenOrDimNotificationEventDelegate(string address,
    int percent);

  /// <summary>
  /// The type of an event handler for an event which is fired to indicate that
  /// a <i>funtion command</i> that applies to all devices of a given house code
  /// was transmitted on the X10 network.
  /// </summary>
  ///
  /// <param name="houseCode">The house code of the device that the notification
  /// 	applies to; for example, "A" or "P".</param>
  ///
  /// <remarks>
  /// See the events such as <r>Cm11.AllLightsOnReceived</r> and
  /// <r>Cm11.AllOffReceived</r>.
  /// </remarks>
  ///
  public delegate void Cm11HouseNotificationEventDelegate(char houseCode);

  /// <summary>
  /// The type of an event handler for an event which is fired when the
  /// <r>Cm11</r> class receives a notification of an event on the X10 network
  /// from the CM11 hardware.
  /// </summary>
  ///
  /// <param name="commandName">The command notification sent from the CM11
  /// 	hardware; for example, "A1" or "Dim" or "AllLightsOn".  In the case of
  /// 	"Dim" or "Brighten" commands, <paramref name="commandName"/> does not
  /// 	include a brighten-by or dim-by amount -- that value is in
  /// 	<paramref name="commandParameter"/>.</param>
  ///
  /// <param name="commandParameter">The brighten-by or dim-by amount (0 to 100)
  /// 	in the case of "Dim" and "Brighten" commands; -1 otherwise.</param>
  ///
  /// <remarks>
  /// See the <r>Cm11.Notification</r> event.
  /// </remarks>
  ///
  public delegate void Cm11LowLevelNotificationEventDelegate(string commandName,
    int commandParameter);

  /// <summary>
  /// The type of an event handler for an event which is fired when the
  /// idle state of the <r>Cm11</r> object changes.
  /// </summary>
  ///
  /// <param name="idle"><n>false</n> if the <r>Cm11</r> object just started
  /// 	processing commands, <n>true</n> if the <r>Cm11</r> object just
  /// 	completed processing all commands and is now idle.</param>
  ///
  /// <remarks>
  /// See the <r>Cm11.IdleStateChange</r> event.
  /// </remarks>
  ///
  public delegate void Cm11IdleStateChangeEventDelegate(bool idle);

  /// <summary>
  /// The type of an event handler for an event which is fired when an error
  /// occurs related to communication with, or operation of, the CM11 hardware.
  /// </summary>
  ///
  /// <param name="message">Information about the error.</param>
  ///
  /// <remarks>
  /// See the <r>Cm11.Error</r> event.
  /// </remarks>
  ///
  public delegate void Cm11ErrorEventDelegate(string message);

  /// <summary>
  /// The type of an event handler for an event which is fired to provide the
  /// application with information it may want to log for future review by the
  /// user.
  /// </summary>
  ///
  /// <param name="message">A text message to log.</param>
  ///
  /// <remarks>
  /// See the <r>Cm11.LogMessage</r> event.
  /// </remarks>
  ///
  public delegate void Cm11LogMessageEventDelegate(string message);

}

/// <summary>
/// Placeholder classe used to aid in locating certain resources embedded in
/// this DLL.
/// </summary>
///
/// <remarks>
/// <para>
/// In order for [ToolboxBitmap] attributes to work correctly, a class must be
/// created that has no namespace, since the default namespace of this project
/// (see project properties in Visual Studio) was changed and no longer
/// matches the name of the assembly.
/// </para>
/// <para>
/// Thanks to http://www.bobpowell.net/toolboxbitmap.htm for pointing out this
/// handy technique.
/// </para>
/// </remarks>
///
abstract internal class ResourceFinder // must not be in any namespace
{
}