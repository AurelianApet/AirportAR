using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO.Ports;

// System.IO.Ports requires a working Serial Port. On Mac, you will need to purcase the Uniduino plug-in on the Unity Store
// This adds a folder + a file into your local folder at ~/lib/libMonoPosixHelper.dylib
// This file will activate your serial port for C# / .NET
// The functions are the same as the standard C# SerialPort library
// cf. http://msdn.microsoft.com/en-us/library/system.io.ports.serialport(v=vs.110).aspx


public class Serial : MonoBehaviour
{
	float delta_angle;
	public GameObject MapHeadingObj;
	public Transform AirStickers;
	private float temp_angle = 0.0f;
	/// <summary>
	/// Enable notification of data as it arrives
	/// Sends OnSerialData(string data) message
	/// </summary>
	public bool NotifyData = false;

	/// <summary>
	/// Discard all received data until first line.
	/// Do not enable if you do not expect a \n as 
	/// this would prevent the notification of any line or value.
	/// Data notification is not impacted by this parameter.
	/// </summary>
	public bool SkipFirstLine = false;

	/// <summary>
	/// Enable line detection and notification on received data.
	/// Message OnSerialLine(string line) is sent for every received line
	/// </summary>
	public bool NotifyLines = false;

	/// <summary>
	/// Maximum number of lines to remember. Get them with GetLines() or GetLastLine()
	/// </summary>
	public int RememberLines = 0;

	/// <summary>
	/// Enable lines detection, values separation and notification.
	/// Each line is split with the value separator (TAB by default)
	/// Sends Message OnSerialValues(string [] values)
	/// </summary>
	public bool NotifyValues = false;

	/// <summary>
	/// The values separator.
	/// </summary>
	public char ValuesSeparator = '\t';

	/// <summary>
	/// The enable debug infos.
	/// The first script with debug infos enabled will enable them until the program stop. 
	/// Therefore, only one script need to enable the debug info to have them in all app, even with multiple scene
	/// and multiple instances, until the program stops.
	/// </summary>
	public bool EnableDebugInfos = false;

	/// <summary>
	/// The first line has been received.
	/// </summary>
	bool FirstLineReceived = false;

	//string serialOut = "";
	private List<string> linesIn = new List<string> ();

	/// <summary>
	/// Gets the received bytes count.
	/// </summary>
	/// <value>The received bytes count.</value>
	public int ReceivedBytesCount { get { return BufferIn.Length; } }

	/// <summary>
	/// Gets the received bytes.
	/// </summary>
	/// <value>The received bytes.</value>
	public string ReceivedBytes { get { return BufferIn; } }

	/// <summary>
	/// Clears the received bytes. 
	/// Warning: This prevents line detection and notification. 
	/// To be used when no \n is expected to avoid keeping unnecessary big amount of data in memory
	/// You should normally not call this function if \n are expected.
	/// </summary>
	public void ClearReceivedBytes ()
	{
		BufferIn = "";
	}

	/// <summary>
	/// Gets the lines count.
	/// </summary>
	/// <value>The lines count.</value>
	public int linesCount { get { return linesIn.Count; } }

	#region Private vars

	// buffer data as they arrive, until a new line is received
	private string BufferIn = "";

	// flag to detect whether coroutine is still running to workaround coroutine being stopped after saving scripts while running in Unity
	private int nCoroutineRunning = 0;

	//added by phoo
	byte[] RxBuffer = new byte[1000];
	ushort usRxLength = 0;

	#endregion

	#region Static vars

	// Only one serial port shared among all instances and living after all instances have been destroyed
	private static SerialPort s_serial;

	// 
	private static List<Serial> s_instances = new List<Serial> ();

	// Enable debug info
	private static bool s_debug = false; // Do not change here. Use EnableDebugInfo on any script instance

	private static float s_lastDataIn = 0;
	private static float s_lastDataCheck = 0;

	#endregion

	void Start ()
	{
		StartCoroutine (ReadSerialLoopWin_Gyro ());
		// print ("Serial Start ");
	}

	void OnValidate ()
	{
		if (RememberLines < 0)
			RememberLines = 0;
	}

	void OnEnable ()
	{
		//		print("Serial OnEnable");
		//		if (s_serial != null)
		//			print ("serial IsOpen: " + s_serial.IsOpen);
		//		else
		//			print ("no serial: ");

		s_instances.Add (this);

		if (EnableDebugInfos && !s_debug) {
			Debug.LogWarning("Serial debug informations enabled by " + this);
			s_debug = true;
		}

		checkOpen (9600);

	}

	void OnDisable ()
	{
		//print("Serial OnDisable");
		s_instances.Remove (this);
	}

	public void OnApplicationQuit ()
	{

		if (s_serial != null) {
			if (s_serial.IsOpen) {
				print ("closing serial port");
				s_serial.Close ();
			}

			s_serial = null;
		}

	}
	private const float lowPassFilterFactor = 0.2f;
	void Update ()
	{
		//print ("Serial Update");

		if (s_serial != null && s_serial.IsOpen) {
			if (nCoroutineRunning == 0) {

				//print ("starting ReadSerialLoop coroutine");

				switch (Application.platform) {

				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsWebPlayer:

					s_serial.ReadTimeout = 1;

					// Each instance has its own coroutine but only one will be active
					break;

				default:
					// Each instance has its own coroutine but only one will be active
					StartCoroutine (ReadSerialLoop ());
					break;

				}
			} else {
				if (nCoroutineRunning > 1)
					print (nCoroutineRunning + " coroutines in " + name);

				nCoroutineRunning = 0; 
			}
		}

		double TimeElapse = (System.DateTime.Now - TimeStart).TotalMilliseconds / 1000;

		string Text = System.DateTime.Now.ToLongTimeString() + "\r\n"
			+ ChipTime[0].ToString() + "-" + ChipTime[1].ToString() + "-" + ChipTime[2].ToString() + "\r\n" + ChipTime[3].ToString() + ":" + ChipTime[4].ToString() + ":" + ChipTime[5].ToString() + "." + ChipTime[6].ToString() + "\r\n"
			+ TimeElapse.ToString("f3") + "\r\n\r\n"
			+ a[0].ToString("f2") + " g\r\n" 
			+ a[1].ToString("f2") + " g\r\n" 
			+ a[2].ToString("f2") + " g\r\n\r\n"
			+ w[0].ToString("f2") + " °/s\r\n"
			+ w[1].ToString("f2") + " °/s\r\n"
			+ w[2].ToString("f2") + " °/s\r\n\r\n"
			+ Angle[0].ToString("f2") + " °\r\n"
			+ Angle[1].ToString("f2") + " °\r\n"
			+ Angle[2].ToString("f2") + " °\r\n\r\n" 
			+ h[0].ToString("f0") + " mG\r\n" 
			+ h[1].ToString("f0") + " mG\r\n" 
			+ h[2].ToString("f0") + " mG\r\n\r\n" 
			+ Temperature.ToString("f2") + " ℃\r\n" 
			+ Pressure.ToString("f0") + " Pa\r\n" 
			+ Altitude.ToString("f2") + " m\r\n\r\n"
			+ (Longitude / 10000000).ToString("f0") + "°" + ((double)(Longitude % 10000000)/1e5).ToString("f5") + "'\r\n"
			+(Latitude / 10000000).ToString("f0") + "°" + ((double)(Latitude % 10000000)/1e5).ToString("f5") + "'\r\n"
			+ GPSHeight.ToString("f1") + " m\r\n"
			+ GPSYaw.ToString("f1") + " °\r\n"
			+ GroundVelocity.ToString("f3") + " km/h";
		
		MapHeadingObj.transform.rotation = Quaternion.Euler(0, 0, -(float)Angle[2]);
		delta_angle = temp_angle + (float)Angle [2];
		float angle = (float)(OnlineMapsUtils.Deg2Rad * delta_angle);
		float d = (float)(Mathf.Sqrt (Screen.width * Screen.width / 4 + Screen.height * Screen.height / 4));
		float l = Mathf.Abs ((float)(2 * d * Mathf.Sin (angle / 2.0f)));
		float delta_curX = Mathf.Abs (l * Mathf.Cos (angle));
		if(delta_angle > 0)
			AirStickers.localPosition += new Vector3 (delta_curX, 0, 0);
		else
			AirStickers.localPosition -= new Vector3 (delta_curX, 0, 0);
		temp_angle = -(float)Angle [2];
	}

	public IEnumerator ReadSerialLoop ()
	{

		while (true) {

			if (!enabled) {
				//print ("behaviour not enabled, stopping coroutine");
				yield break; 
			}

			//print("ReadSerialLoop ");
			nCoroutineRunning++; 

			try {
				s_lastDataCheck = Time.time;
				while (s_serial.BytesToRead > 0) {  // BytesToRead crashes on Windows -> use ReadLine or ReadByte in a Thread or Coroutine


					string serialIn = s_serial.ReadExisting ();

					// Dispatch new data to each instance
					foreach (Serial inst in s_instances) {
						inst.receivedData (serialIn);
					}

					s_lastDataIn = s_lastDataCheck;
				}

			} catch (System.Exception e) {
				print ("System.Exception in serial.ReadExisting: " + e.ToString ());
			}

			yield return null;
		}

	}
	//added by phoo
	double[] a = new double[4], w = new double[4], h = new double[4], Angle = new double[4], Port = new double[4];
	double Temperature, Pressure, Altitude,  GroundVelocity, GPSYaw, GPSHeight;
	long Longitude, Latitude;

	private double[] LastTime = new double[10];
	short sRightPack = 0;
	short [] ChipTime = new short[7];
	private System.DateTime TimeStart = System.DateTime.Now;

	private void DecodeData(byte[] byteTemp)
	{
		double[] Data = new double[4];
		double TimeElapse = (System.DateTime.Now - TimeStart).TotalMilliseconds / 1000;

		Data[0] = System.BitConverter.ToInt16(byteTemp, 2);
		Data[1] = System.BitConverter.ToInt16(byteTemp, 4);
		Data[2] = System.BitConverter.ToInt16(byteTemp, 6);
		Data[3] = System.BitConverter.ToInt16(byteTemp, 8);
		sRightPack++;
		switch (byteTemp[1])
		{
		case 0x50:
			//Data[3] = Data[3] / 32768 * double.Parse(textBox9.Text) + double.Parse(textBox8.Text);
			ChipTime[0] = (short)(2000 + byteTemp[2]);
			ChipTime[1] = byteTemp[3];
			ChipTime[2] = byteTemp[4];
			ChipTime[3] = byteTemp[5];
			ChipTime[4] = byteTemp[6];
			ChipTime[5] = byteTemp[7];
			ChipTime[6] = System.BitConverter.ToInt16(byteTemp, 8);


			break;
		case 0x51:
			//Data[3] = Data[3] / 32768 * double.Parse(textBox9.Text) + double.Parse(textBox8.Text);
			Temperature = Data[3] / 100.0;
			Data[0] = Data[0] / 32768.0 * 16;
			Data[1] = Data[1] / 32768.0 * 16;
			Data[2] = Data[2] / 32768.0 * 16;

			a[0] = Data[0];
			a[1] = Data[1];
			a[2] = Data[2];
			a[3] = Data[3];
			if ((TimeElapse - LastTime[1]) < 0.1) return;
			LastTime[1] = TimeElapse;

			break;
		case 0x52:
			//Data[3] = Data[3] / 32768 * double.Parse(textBox9.Text) + double.Parse(textBox8.Text);
			Temperature = Data[3] / 100.0;
			Data[0] = Data[0] / 32768.0 * 2000;
			Data[1] = Data[1] / 32768.0 * 2000;
			Data[2] = Data[2] / 32768.0 * 2000;
			w[0] = Data[0];
			w[1] = Data[1];
			w[2] = Data[2];
			w[3] = Data[3];

			if ((TimeElapse-LastTime[2])<0.1) return;
			LastTime[2] = TimeElapse;
			break;
		case 0x53:
			//Data[3] = Data[3] / 32768 * double.Parse(textBox9.Text) + double.Parse(textBox8.Text);
			Temperature = Data[3] / 100.0;
			Data[0] = Data[0] / 32768.0 * 180;
			Data[1] = Data[1] / 32768.0 * 180;
			Data[2] = Data[2] / 32768.0 * 180;
			Angle[0] = Data[0];
			Angle[1] = Data[1];
			Angle[2] = Data[2];
			Angle[3] = Data[3];
			if ((TimeElapse-LastTime[3])<0.1) return;
			LastTime[3] = TimeElapse;
			break;
		case 0x54:
			//Data[3] = Data[3] / 32768 * double.Parse(textBox9.Text) + double.Parse(textBox8.Text);
			Temperature = Data[3] / 100.0;
			h[0] = Data[0];
			h[1] = Data[1];
			h[2] = Data[2];
			h[3] = Data[3];
			if ((TimeElapse - LastTime[4]) < 0.1) return;
			LastTime[4] = TimeElapse;
			break;
		case 0x55:
			Port[0] = Data[0];
			Port[1] = Data[1];
			Port[2] = Data[2];
			Port[3] = Data[3];

			break;

		case 0x56:
			Pressure = System.BitConverter.ToInt32(byteTemp, 2);
			Altitude = (double)System.BitConverter.ToInt32(byteTemp, 6) / 100.0;

			break;

		case 0x57:
			Longitude = System.BitConverter.ToInt32(byteTemp, 2);
			Latitude  = System.BitConverter.ToInt32(byteTemp, 6);

			break;

		case 0x58:
			GPSHeight = (double)System.BitConverter.ToInt16(byteTemp, 2) / 10.0;
			GPSYaw = (double)System.BitConverter.ToInt16(byteTemp, 4) / 10.0;
			GroundVelocity = System.BitConverter.ToInt16(byteTemp, 6)/1e3;

			break;
		default:
			break;
		}   
	}
	delegate void UpdateData(byte[] byteData);//声明一个委托

	public IEnumerator ReadSerialLoopWin_Gyro ()
	{

		while (true) {

			if (!enabled) {
				//print ("behaviour not enabled, stopping coroutine");
				yield break; 
			}

			//print("ReadSerialLoopWin ");
			nCoroutineRunning++; 
			//print ("nCoroutineRunning: " + nCoroutineRunning);

			byte[] byteTemp = new byte[1000];

			try
			{
				ushort usLength=0;
				try
				{
					usLength = (ushort)s_serial.Read(RxBuffer, usRxLength, 700);
				}
				catch (System.Exception err)
				{
					//MessageBox.Show(err.Message);
					//return;
				}
				usRxLength += usLength;
				while (usRxLength >= 11)
				{
					RxBuffer.CopyTo(byteTemp, 0);
					if (!((byteTemp[0] == 0x55) & ((byteTemp[1] & 0x50)==0x50)))
					{
						for (int i = 1; i < usRxLength; i++) RxBuffer[i - 1] = RxBuffer[i];
						usRxLength--;
						continue;
					}
					if (((byteTemp[0]+byteTemp[1]+byteTemp[2]+byteTemp[3]+byteTemp[4]+byteTemp[5]+byteTemp[6]+byteTemp[7]+byteTemp[8]+byteTemp[9])&0xff)==byteTemp[10])
						DecodeData(byteTemp);
					for (int i = 11; i < usRxLength; i++) RxBuffer[i - 11] = RxBuffer[i];
					usRxLength -= 11;
				}

				yield return  new WaitForSeconds(0.01f);
//				System.Thread.Sleep(10);
			}
			finally
			{
			}  

			yield return null;
		}

	}

	public IEnumerator ReadSerialLoopWin ()
	{

		while (true) {

			if (!enabled) {
				//print ("behaviour not enabled, stopping coroutine");
				yield break; 
			}

			//print("ReadSerialLoopWin ");
			nCoroutineRunning++; 
			//print ("nCoroutineRunning: " + nCoroutineRunning);

			string serialIn = "";
			try {
				while (true) {  // BytesToRead crashes on Windows -> use ReadLine or ReadByte in a Thread or Coroutine
					char c = (char)s_serial.ReadByte();
					serialIn += c;

					//serialIn += s_serial.ReadLine();
				}

			} catch (System.TimeoutException) {
				//print ("System.TimeoutException in serial.ReadLine: " + te.ToString ());
			} catch (System.Exception e) {
				print ("System.Exception in serial.ReadLine: " + e.ToString ());
			}

			if (serialIn.Length > 0) {

				//Debug.Log("just read some data: " + serialIn);
				// Dispatch new data to each instance
				foreach (Serial inst in s_instances) {
					inst.receivedData (serialIn);
				}
			}

			yield return null;
		}

	}

	/// return all received lines and clear them
	/// Useful if you need to process all the received lines, even if there are several since last call
	public List<string> GetLines (bool keepLines = false)
	{

		List<string> lines = new List<string> (linesIn);

		if (!keepLines)
			linesIn.Clear ();

		return lines;
	}

	/// return only the last received line and clear them all
	/// Useful when you need only the last received values and can ignore older ones
	public string GetLastLine (bool keepLines = false)
	{

		string line = "";
		if (linesIn.Count > 0)
			line = linesIn [linesIn.Count - 1];

		if (!keepLines)
			linesIn.Clear ();

		return line;
	}

	/// <summary>
	/// Send data to the serial port.
	/// </summary>
	public static void Write (string message)
	{
		if (checkOpen ())
			s_serial.Write (message);
	}

	/// <summary>
	/// Send data to the serial port and append a new line character (\n)
	/// </summary>
	public static void WriteLn (string message = "")
	{
		s_serial.Write (message + "\n");
	}

	/// <summary>
	/// Act as if the serial port has received data followed by a new line.
	/// </summary>
	public void SimulateDataReceptionLn(float data) {
		foreach (Serial inst in s_instances) {
			inst.receivedData(data + "\n");
		}
	}

	/// <summary>
	/// Act as if the serial port has received data followed by a new line.
	/// </summary>
	public void SimulateDataReceptionLn(string data) {
		foreach (Serial inst in s_instances) {
			inst.receivedData(data + "\n");
		}
	}

	/// <summary>
	/// Verify if the serial port is opened and opens it if necessary
	/// </summary>
	/// <returns><c>true</c>, if port is opened, <c>false</c> otherwise.</returns>
	/// <param name="portSpeed">Port speed.</param>
	public static bool checkOpen (int portSpeed = 9600)
	{

		if (s_serial == null) {

			string portName = GetPortName ();

			if (portName == "") {
				print ("Error: Couldn't find serial port.");
				return false;
			} else {
				if (s_debug)
					print("Opening serial port: " + portName);
			}

			s_serial = new SerialPort (portName, portSpeed);

			s_serial.Open ();
			//print ("default ReadTimeout: " + s_serial.ReadTimeout);
			//s_serial.ReadTimeout = 10;

			// clear input buffer from previous garbage
			s_serial.DiscardInBuffer ();
		}

		return s_serial.IsOpen;
	}

	// Data has been received, do what this instance has to do with it
	protected void receivedData (string data)
	{

		if (NotifyData) {
			SendMessage ("OnSerialData", data);
		}

		// Detect lines
		if (NotifyLines || NotifyValues) {

			// prepend pending buffer to received data and split by line
			string [] lines = (BufferIn + data).Split ('\n');

			// If last line is not empty, it means the line is not complete (new line did not arrive yet), 
			// We keep it in buffer for next data.
			int nLines = lines.Length;
			BufferIn = lines [nLines - 1];

			// Loop until the penultimate line (don't use the last one: either it is empty or it has already been saved for later)
			for (int iLine = 0; iLine < nLines - 1; iLine++) {
				string line = lines [iLine];
				//Debug.Log ("Received a line: " + line);

				// skip first line 
				if (!FirstLineReceived) {
					FirstLineReceived = true;

					if (SkipFirstLine) {
						if (EnableDebugInfos) {
							Debug.Log("First line skipped: " + line);
						}
						continue;
					}
				}

				// Buffer line
				if (RememberLines > 0) {
					linesIn.Add (line);

					// trim lines buffer
					int overflow = linesIn.Count - RememberLines;
					if (overflow > 0) {
						print ("Serial removing " + overflow + " lines from lines buffer. Either consume lines before they are lost or set RememberLines to 0.");
						linesIn.RemoveRange (0, overflow);
					}
				}

				// notify new line
				if (NotifyLines) {
					SendMessage ("OnSerialLine", line);
				}

				// Notify values
				if (NotifyValues) {
					string [] values = line.Split (ValuesSeparator);
					SendMessage ("OnSerialValues", values);
				}

			}
		}
	}

	static string GetPortName ()
	{

		string[] portNames;

		switch (Application.platform) {

		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXDashboardPlayer:
		case RuntimePlatform.LinuxPlayer:

			portNames = System.IO.Ports.SerialPort.GetPortNames ();

			if (portNames.Length == 0) {
				portNames = System.IO.Directory.GetFiles ("/dev/");                
			}

			foreach (string portName in portNames) {                                
				if (portName.StartsWith ("/dev/tty.usb") || portName.StartsWith ("/dev/ttyUSB"))
					return portName;
			}                
			return "";

		default: // Windows

			portNames = System.IO.Ports.SerialPort.GetPortNames ();

			// Defaults to last port in list (most chance to be an Arduino port)
			if (portNames.Length > 0)
				return portNames [portNames.Length - 1];
			else
				return "";
		}
	}

	void OnGUI() {

		// Show debug only if enabled and by the first instance to avoid overwrite same data
		if (s_debug && this == s_instances[0]) {
			GUILayout.Label("Serial last data: " + s_lastDataIn + " (last check: " + s_lastDataCheck + ")");
		}
	}

}
