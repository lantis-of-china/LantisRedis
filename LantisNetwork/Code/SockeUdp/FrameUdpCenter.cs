using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Network
{
	public class FrameUdpCenter
	{
		public static void StartRun()
		{
			ComplateRecorder.Start();
			UdpSubmit.Start();
			UdpLineParkTool.Start();
		}

		public static void Close()
		{
			ComplateRecorder.Close();
			UdpSubmit.Close();
			UdpLineParkTool.Close();
		}
	}

}
