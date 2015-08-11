/*       Copyright (c) Eric Ledoux.  All rights reserved.       */
/* See http://www.dwell.net/terms for code sharing information. */

// DnSerialPort.cs
//
// Implements the DnSerialPort class.
//

using System;
using System.IO;
using System.IO.Ports;

namespace DwellNet
{

/// <summary>
/// Represents a serial port resource.  Similar to
/// <n>System.IO.Ports.SerialPort</n>, but with functionality such as
/// the ability to "peek" at the next byte received on the serial port without
/// removing it from the input buffer.
/// </summary>
///
/// <remarks>
/// <para>
/// Unlike <r>SerialPort</r>, <r>DnSerialPort</r> is intended purely for
/// asynchronous access.  Reading from the serial port will never block, and
/// writing will only block for at most a short time.
/// </para>
/// <para>
/// This class converts any I/O-related exception (such as <r>IOException</r>
/// or <r>InvalidOperationException</r>) that occurs during serial port
/// communication to a <r>DnSerialPortException</r>, so the application can
/// handle all such exceptions uniformly.  The original exception is retained
/// as the inner exception of the <r>DnSerialPortException</r>.
/// </para>
/// </remarks>
///
internal class DnSerialPort
{
	// Implementation note:  Although DnSerialPort wraps a SerialPort object,
	// DnSerialPort doesn't derive from SerialPort because some properties and
	// methods of SerialPort won't work correctly if a byte has been stored in
	// <m_peekedInputByte>.  For example, SerialPort.ByteCount would not
	// account for the byte in <m_peekedInputByte>.  Since most properties and
	// methods of SerialPort aren't virtual, we need to wrap SerialPort instead
	// of derive from it.

	//////////////////////////////////////////////////////////////////////////
	// Private Fields
	//

    /// <summary>
    /// The name of the serial port; for example, "COM1".
    /// </summary>
    string m_serialPortName;

	/// <summary>
	/// The wrapped <r>SerialPort</r> object.
	/// </summary>
    SerialPort m_serialPort;

	/// <summary>
	/// A byte that has been read from the serial port using
	/// <n>PeekSerialPortByte</n>.  A subsequent call to
	/// <n>PeekSerialPortByte</n> or <n>ReadSerialPortByte</n> will return this
	/// byte instead of reading directly from the serial port.
	/// </summary>
	byte? m_peekedInputByte;

	/// <summary>
	/// Localized string resources used by this class.
	/// </summary>
	DnSerialPortStrings m_strings;

	//////////////////////////////////////////////////////////////////////////
	// Public Events
	//

	/// <summary>
	/// Fired when data is received on the serial port.
	/// </summary>
	public event SerialDataReceivedEventHandler DataReceived;

	/// <summary>
	/// Fired when a serial port I/O error occurs.
	/// </summary>
	public event SerialErrorReceivedEventHandler ErrorReceived;

	//////////////////////////////////////////////////////////////////////////
	// Public Methods
	//

    /// <summary>
	/// Initializes a new instance of the <r>DnSerialPort</r> class.
    /// </summary>
	///
    /// <param name="strings">String resources for use by this class.</param>
	///
    /// <param name="portName">The name of the serial port to use (for example,
	/// 	"COM1").</param>
	///
    /// <param name="baudRate">The baud rate (for example, 9600).</param>
	///
    /// <param name="parity">One of the <r>Parity</r> values.</param>
	///
    /// <param name="dataBits">The data bits value to use (for example, 8).
	/// 	</param>
	///
    /// <param name="stopBits">One of the <r>StopBits</r> values.</param>
	///
	/// <remarks>
	/// See <r>SerialPort</r> for more information.
	/// </remarks>
	///
	/// <exception cref="DnSerialPortException">
	/// Thrown when an error occurs while accessing the serial port.
	/// </exception>
	///
	public DnSerialPort(DnSerialPortStrings strings, string portName,
		int baudRate, Parity parity, int dataBits, StopBits stopBits)
	{
		try
		{
			m_strings = strings;
            m_serialPortName = portName;
            m_serialPort = new SerialPort(portName, baudRate, parity, dataBits,
				stopBits);
			m_serialPort.DataReceived += new SerialDataReceivedEventHandler(
				m_serialPort_DataReceived);
			m_serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(
				m_serialPort_ErrorReceived);
		}
		catch (InvalidOperationException ex)
		{
			throw NewDnSerialPortException(ex);
		}
		catch (IOException ex)
		{
			throw NewDnSerialPortException(ex);
		}
		catch (UnauthorizedAccessException ex)
		{
			// this exception may occur if a USB serial port adapter is
			// disconnected after the serial port was opened
			throw NewDnSerialPortException(ex);
		}
	}

	/// <summary>
	/// Opens the serial port connection.
	/// </summary>
	///
	/// <remarks>
	/// See <n>SerialPort.Open</n> for more information.
	/// </remarks>
	///
	/// <exception cref="DnSerialPortException">
	/// Thrown when an error occurs while accessing the serial port.
	/// </exception>
	///
	public void Open()
	{
		try
		{
			m_serialPort.Open();
		}
		catch (InvalidOperationException ex)
		{
			throw NewDnSerialPortException(ex);
		}
		catch (IOException ex)
		{
			throw NewDnSerialPortException(ex);
		}
		catch (UnauthorizedAccessException ex)
		{
			// this exception may occur if a USB serial port adapter is
			// disconnected after the serial port was opened
			throw NewDnSerialPortException(ex);
		}
	}

	/// <summary>
	/// Closes the serial port connection.
	/// </summary>
	///
	/// <remarks>
	/// See <n>SerialPort.Close</n> for more information.
	/// </remarks>
	///
	/// <exception cref="DnSerialPortException">
	/// Thrown when an error occurs while accessing the serial port.
	/// </exception>
	///
	public void Close()
	{
		try
		{
			m_serialPort.Close();
		}
		catch (InvalidOperationException ex)
		{
			throw NewDnSerialPortException(ex);
		}
		catch (IOException ex)
		{
			throw NewDnSerialPortException(ex);
		}
		catch (UnauthorizedAccessException ex)
		{
			// this exception may occur if a USB serial port adapter is
			// disconnected after the serial port was opened
			throw NewDnSerialPortException(ex);
		}
	}

	/// <summary>
	/// Returns the next input byte from the serial port without removing it
	/// from the input queue.  Returns null if there is no byte waiting to be
	/// read.
	/// </summary>
	///
	/// <exception cref="DnSerialPortException">
	/// Thrown when an error occurs while accessing the serial port.
	/// </exception>
	///
	public byte? PeekByte()
	{
        if (m_peekedInputByte.HasValue)
            return m_peekedInputByte;
        else
            return (m_peekedInputByte = ReadByte());
	}

	/// <summary>
	/// Returns the next input byte from the serial port.  Returns null if
	/// there is no byte waiting to be read.
	/// </summary>
	///
	/// <exception cref="DnSerialPortException">
	/// Thrown when an error occurs while accessing the serial port.
	/// </exception>
	///
	public byte? ReadByte()
	{
        if (m_peekedInputByte.HasValue)
        {
            byte b = m_peekedInputByte.Value;
            m_peekedInputByte = null;
            return b;
        }
        else
        {
			try
			{
				if (m_serialPort.BytesToRead > 0)
				{
					return (byte) m_serialPort.ReadByte();
				}
				else
					return null;
			}
			catch (TimeoutException) // in case bytes vanished somehow
			{
				return null;
			}
			catch (InvalidOperationException ex)
			{
				throw NewDnSerialPortException(ex);
			}
			catch (IOException ex)
			{
				throw NewDnSerialPortException(ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				// this exception may occur if a USB serial port adapter is
				// disconnected after the serial port was opened
				throw NewDnSerialPortException(ex);
			}
        }
	}

	/// <summary>
	/// Reads a number of bytes from the serial port input buffer and writes
	/// those bytes into a byte array.  If fewer than the requested number of
	/// bytes are available, this method retrieves the available bytes.
	/// </summary>
	///
	/// <param name="buffer">The byte array to write the input to.</param>
	///
	/// <param name="offset">The offset in the buffer array to begin writing.
	/// 	</param>
	///
	/// <param name="count">The number of bytes to read.</param>
	///
	/// <exception cref="DnSerialPortException">
	/// Thrown when an error occurs while accessing the serial port.
	/// </exception>
	///
	public int Read(byte[] buffer, int offset, int count)
	{
		// do nothing if no bytes are requested
		if (count <= 0)
			return 0;

		// keep track of the number of bytes read
		int result = 0;

		// check <m_peekedInputByte> first
        if (m_peekedInputByte.HasValue)
        {
            byte b = m_peekedInputByte.Value;
            m_peekedInputByte = null;
			buffer[offset++] = b;
			result++;
			count--;
        }

		// read from <m_serialPort>
		while (count > 0)
		{
			// set <bytesToReadNow> to the number smaller of (a) the number of
			// bytes available to read from the serial port, and (b) the number
			// of bytes we still need to read; break of no bytes are
			// available
			int bytesToReadNow;
			try
			{
				bytesToReadNow = Math.Min(count, m_serialPort.BytesToRead);
			}
			catch (InvalidOperationException ex)
			{
				throw NewDnSerialPortException(ex);
			}
			catch (IOException ex)
			{
				throw NewDnSerialPortException(ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				// this exception may occur if a USB serial port adapter is
				// disconnected after the serial port was opened
				throw NewDnSerialPortException(ex);
			}
			if (bytesToReadNow == 0)
				break;

			// read <bytesToReadNow> bytes
			try
			{
				int bytesReadNow = m_serialPort.Read(buffer, offset,
					bytesToReadNow);
				offset += bytesReadNow;
				result += bytesReadNow;
				count -= bytesReadNow;
			}
			catch (TimeoutException) // in case bytes vanished somehow
			{
				break;
			}
			catch (InvalidOperationException ex)
			{
				throw NewDnSerialPortException(ex);
			}
			catch (IOException ex)
			{
				throw NewDnSerialPortException(ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				// this exception may occur if a USB serial port adapter is
				// disconnected after the serial port was opened
				throw NewDnSerialPortException(ex);
			}
        }

		return result;
	}

	/// <summary>
	/// Writes a specified number of bytes to an output buffer at the specified
	/// offset. 
	/// </summary>
	///
	/// <param name="buffer">The byte array to write the output to.</param>
	///
	/// <param name="offset">The offset in the buffer array to begin writing.
	/// 	</param>
	///
	/// <param name="count">The number of bytes to write.</param>
	///
	/// <exception cref="DnSerialPortException">
	/// Thrown when an error occurs while accessing the serial port.
	/// </exception>
	///
    public void Write(byte[] buffer, int offset, int count)
	{
		try
		{
			m_serialPort.Write(buffer, offset, count);
		}
		catch (InvalidOperationException ex)
		{
			throw NewDnSerialPortException(ex);
		}
		catch (IOException ex)
		{
			throw NewDnSerialPortException(ex);
		}
		catch (UnauthorizedAccessException ex)
		{
			// this exception may occur if a USB serial port adapter is
			// disconnected after the serial port was opened
			throw NewDnSerialPortException(ex);
		}
	}

	/// <summary>
	/// Discards data in the input and output buffers of the serial port.
	/// </summary>
	///
    public void Purge()
    {
        try
        {
            m_peekedInputByte = null;
            if (m_serialPort != null)
            {
                m_serialPort.DiscardInBuffer();
                m_serialPort.DiscardOutBuffer();
            }
        }
        catch (InvalidOperationException)
        {
            // ignore this exception
        }
        catch (IOException)
        {
            // ignore this exception
        }
        catch (UnauthorizedAccessException)
        {
            // ignore this exception -- it happens, for example, when a USB
            // serial port is unplugged before calling this method
        }
    }

	//////////////////////////////////////////////////////////////////////////
	// Private Methods
	//

	/// <summary>
	/// Wraps an exception related to serial port access (e.g.
	/// <r>IOException</r> or <r>InvalidOperationException</r>) in a
	/// <r>DnSerialPortException</r>.
	/// </summary>
	///
	DnSerialPortException NewDnSerialPortException(Exception innerException)
	{
        return new DnSerialPortException(
            String.Format(m_strings.SerialPortError, m_serialPortName),
            innerException);
	}

	//////////////////////////////////////////////////////////////////////////
	// Event Handlers for <m_serialPort>
	//

    void m_serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
		if (DataReceived != null)
			DataReceived(sender, e);
    }

    void m_serialPort_ErrorReceived(object sender,
		SerialErrorReceivedEventArgs e)
    {
		if (ErrorReceived != null)
			ErrorReceived(sender, e);
    }
}

/// <summary>
/// Indicates an error that occurred while attempting to communicate using the
/// serial port.
/// </summary>
///
internal class DnSerialPortException : Exception
{
    /// <summary>
    /// Initializes an instance of this class.
    /// </summary>
    ///
    /// <param name="message">The exception message.</param>
    /// 
    /// <param name="innerException">The inner exception, or <n>null</n> if
    ///     none.</param>
    ///
	public DnSerialPortException(string message, Exception innerException) :
		base(message, innerException)
	{
	}
}

/// <summary>
/// Provides <r>DnSerialPort</r> with a way to get one of its localized
/// strings from the application on demand.
/// </summary>
///
internal abstract class DnSerialPortStrings
{
	/// <summary>
	/// Gets a string like: Error accessing serial port "{0}"
	/// </summary>
	///
	/// <remarks>
	/// {0} is the serial port name, e.g. "COM1"
	/// </remarks>
	///
	public abstract string SerialPortError { get; }
}

}

