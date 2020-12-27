using System;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using Lantis.EntityComponentSystem;

namespace Lantis.Network
{
	public class NetUdpComponent : ComponentEntity
	{
		private Action<byte[], Socket, string, int> reciveMessageCall;
		private Action sucessCall;
		private Action exceptionCall;
		public bool run;
		public Socket udpSocket;

		public static object[] ParamCreate(string ip, int port, Action<byte[], Socket, string, int> reciveMessageCall, Action sucessCall, Action exceptionCall)
		{
			return new object[] { ip, port, reciveMessageCall, sucessCall, exceptionCall };
		}

		public override void OnAwake(params object[] paramsData)
        {
            base.OnAwake(paramsData);

			SafeRun(delegate
			{
				var ip = paramsData[0] as string;
				var port = (int)paramsData[1];
				reciveMessageCall = (Action<byte[], Socket, string, int>)paramsData[2];
				sucessCall = (Action)paramsData[3];
				exceptionCall = (Action)paramsData[4];
				UdpSubmit.BindUdpComponent(this);
				UdpLineParkTool.BindUdpComponent(this);
				udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				udpSocket.ReceiveBufferSize = (32 * 1024);
				udpSocket.SendBufferSize = (32 * 1024);
				var entity = GetEntity<Entity>();

				if (reciveMessageCall == null)
				{
					var messageDriver = entity.GetComponent<NetMessageDriverComponents>();

					if (messageDriver != null)
					{
						reciveMessageCall = messageDriver.OnReciveMessage;
					}
					else
					{
						Logger.Error("you can't use message recive,becuse you must be add NetMessageDriverComponents befrom current component added!");
					}
				}

				BindAddress();
				OnSocketSuccess();
			});
        }

		public void Start()
		{
			SafeRun(delegate
			{
				run = true;

				var tnew = new Thread(new ThreadStart(delegate
				{
					while (run)
					{
						ReciveDateUdp(udpSocket);
					}
				}));
				tnew.Start();
			});

		}

		/// <summary>
		/// 绑定端点  启动侦听
		/// </summary>
		public void BindAddress()
		{
			SafeRun(delegate
			{
				var ipPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 10810);

				try
				{
					udpSocket.Bind(ipPoint);
				}
				catch
				{
					OnSocketExpection();
				}
			});
		}


		/// <summary>
		/// UDP接收数据
		/// </summary>
		/// <param name="clientSocket"></param>
		private void ReciveDateUdp(Socket clientSocket)
		{
			var readByte = new byte[clientSocket.ReceiveBufferSize];
			var iep = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
			var ep = iep as System.Net.EndPoint;

			while (true)
			{
				try
				{
					var reciveCount = clientSocket.ReceiveFrom(readByte, ref ep);
					var msgBuf = new byte[reciveCount];
					Buffer.BlockCopy(readByte, 0, msgBuf, 0, reciveCount);
					var udpPark = UdpParkTool.InstancePark(msgBuf);

					if (udpPark == null)
					{
						//封包解析失败
						return;
					}

					var ipStr = (ep as System.Net.IPEndPoint).Address.ToString();
					var port = (ep as System.Net.IPEndPoint).Port;
					var msgComDate = UdpLineParkTool.AddPark(ipStr, port, udpPark);

					if (msgComDate == null)
					{
						continue;
					}

					var msgByte = msgComDate;
					var msgType = BitConverter.ToInt32(msgByte, 0);

					if (msgType == 1)
					{
						var dateMsg = new byte[msgByte.Length - 4];
						Buffer.BlockCopy(msgByte, 4, dateMsg, 0, dateMsg.Length);
						var suc = new UdpSubmitComplate();
						suc.SetData(dateMsg);

						if (suc.complate == 1)
						{
							UdpSubmit.RemoveSubmit(suc.parkGroupCode);
						}
						else
						{
							var usd = UdpSubmit.GetSubmit(suc.parkGroupCode);

							if (suc.getList != null && usd != null)
							{
								var parkList = new List<UdpPark>();

								for (int i = 0; i < usd.parkList.Count; ++i)
								{
									var isFind = false;
									usd.createTime = DateTime.Now;
									var sendPark = usd.parkList[i];

									for (int getIndex = 0; getIndex < suc.getList.Count; ++getIndex)
									{
										if (sendPark._ParkIndex == suc.getList[getIndex])
										{
											isFind = true;
											break;
										}
									}

									if (!isFind)
									{
										parkList.Add(sendPark);
										SendParkList(parkList, ipStr, port);
									}
								}
							}
						}
						continue;
					}
					else
					{
						if (ComplateRecorder.CanRecive(udpPark._ParkGroupCode))
						{
							//发送确认包
							UdpLineParkTool.SendComplate(ipStr, port, udpPark._ParkGroupCode, 1, null);
						}
					}

					//处理消息调度
					if (reciveMessageCall != null)
					{
						reciveMessageCall(msgByte, clientSocket, ipStr, port);
					}
				}
				catch
				{
				}
				finally
				{ 
				}
			}

		}

		/// 发送消息到客户端UDP 
		/// </summary>
		/// <typeparam name="T">发送的消息类型</typeparam>
		/// <param name="ClientSocket">接受消息的客户端套接字</param>
		/// <param name="Date">发送的消息的实例</param>
		/// <param name="messageType">消息的类型</param>
		public void SendMessageUdpCompalte(string ip, int port, byte[] bufferDate, int messageType)
		{
			var typeDate = System.BitConverter.GetBytes((int)messageType);
			var lenght = typeDate.Length + bufferDate.Length;
			var sendBuffer = new byte[lenght];
			typeDate.CopyTo(sendBuffer, 0);
			bufferDate.CopyTo(sendBuffer, 4);
			var parkList = UdpParkTool.Separate(sendBuffer, 512, 0);
			SendParkList(parkList, ip, port);
		}

		/// <summary>
		/// 发送消息到客户端UDP 
		/// </summary>
		/// <typeparam name="T">发送的消息类型</typeparam>
		/// <param name="ClientSocket">接受消息的客户端套接字</param>
		/// <param name="Date">发送的消息的实例</param>
		/// <param name="messageType">消息的类型</param>
		public void SendMessageUdp(string ip, int port, byte[] bufferDate, int messageType)
		{
			var typeDate = System.BitConverter.GetBytes((int)messageType);
			var lenght = typeDate.Length + bufferDate.Length;
			var sendBuffer = new byte[lenght];
			typeDate.CopyTo(sendBuffer, 0);
			bufferDate.CopyTo(sendBuffer, 4);
			var parkList = UdpParkTool.Separate(sendBuffer, 512, 1);
			SendParkList(parkList, ip, port);
		}

		public void SendParkList(List<UdpPark> parkList, string ip, int port)
		{
			for (int index = 0; index < parkList.Count; index++)
			{
				var sendMessageBuf = parkList[index]._MsgDate;
				udpSocket.SendTo(sendMessageBuf, (int)sendMessageBuf.Length, SocketFlags.None, new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port));
			}
		}


		public void StopSocket()
		{
			run = false;
		}


		public void CloseSocket()
		{
			run = false;
			SafeClose(udpSocket);
		}

		/// <summary>  
		/// Close the socket safely.  
		/// </summary>  
		/// <param name="socket">The socket.</param>  
		public void SafeClose(Socket socket)
		{
			if (socket == null)
			{
				return;
			}

			try
			{
				socket.Shutdown(SocketShutdown.Both);
			}
			catch (Exception e)
			{
			}

			try
			{
				socket.Close();
			}
			catch (Exception e)
			{
			}
		}

		private void OnSocketSuccess()
		{
			SafeRun(delegate 
			{
				if (sucessCall != null)
				{
					sucessCall();
				}
			});
		}

		private void OnSocketExpection()
		{
			SafeRun(delegate
			{
				if (exceptionCall != null)
				{
					exceptionCall();
				}
			});
		}
	}
}

