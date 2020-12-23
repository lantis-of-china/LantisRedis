using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Network
{
	/// <summary>
	/// Udp分包 组包线管理器
	/// </summary>
	public class UdpLineParkTool
	{
		public static object lockObj = new object();
		/// <summary>
		/// 组包管线
		/// </summary>
		public static Dictionary<long, UdpLineParkGroup> udpLineParkManager = new Dictionary<long, UdpLineParkGroup>();
		/// <summary>
		/// 运行
		/// </summary>
		public static bool run;

		/// <summary>
		/// 开始
		/// </summary>
		public static void Start()
		{
			run = true;

			new Thread(() =>
			{
				while (run)
				{
					UpData();

					Thread.Sleep(500);
				}
			}).Start();
		}

		/// <summary>
		/// 关闭
		/// </summary>
		public static void Close()
		{
			run = false;
		}

		/// <summary>
		/// 添加到数据包流线分包管理器中 并且查看包是否可以组合  如果能组合就返回组合后数据  否则空
		/// </summary>
		/// <param name="ipStr"></param>
		/// <param name="port"></param>
		/// <param name="udpPark"></param>
		/// <returns></returns>
		public static byte[] AddPark(string ipStr, int port, UdpPark udpPark)
		{
			byte[] bufferByte = null;

			lock (lockObj)
			{
				UdpLineParkGroup udpLineParkGroup = null; //udpLineParkManager.Find(item => item._IpString == ipStr && item._Port == port && item._ParkGroupCode == udpPark._ParkGroupCode);

				if (udpLineParkManager.ContainsKey(udpPark._ParkGroupCode))
				{
					udpLineParkGroup = udpLineParkManager[udpPark._ParkGroupCode];
				}

				if (udpLineParkGroup == null)
				{
					//创建新的
					udpLineParkGroup = new UdpLineParkGroup();
					udpLineParkGroup._IpString = ipStr;
					udpLineParkGroup._Port = port;
					udpLineParkGroup._ParkGroupCode = udpPark._ParkGroupCode;
					udpLineParkGroup._CreateTime = DateTime.Now;
					udpLineParkGroup._ReGetCount = 0;
					udpLineParkGroup.ParkList = udpLineParkGroup.ParkList == null ? new List<UdpPark>() : udpLineParkGroup.ParkList;
					udpLineParkManager.Add(udpLineParkGroup._ParkGroupCode, udpLineParkGroup);
				}

				if (udpLineParkGroup.ParkList.Find(item => item._ParkIndex == udpPark._ParkIndex) != null)
				{
					return null;
				}

				udpLineParkGroup.ParkList.Add(udpPark);
				bufferByte = UdpParkTool.Combination(udpLineParkGroup.ParkList);

				if (bufferByte != null)
				{
					udpLineParkManager.Remove(udpLineParkGroup._ParkGroupCode);
				}

			}

			return bufferByte;
		}

		/// <summary>
		/// 获取已经
		/// </summary>
		/// <param name="groupCode"></param>
		/// <returns></returns>
		public static UdpLineParkGroup GetPark(long groupCode)
		{
			UdpLineParkGroup udpLineParkGroup = null; //udpLineParkManager.Find(item => item._IpString == ipStr && item._Port == port && item._ParkGroupCode == udpPark._ParkGroupCode);
			
			lock (lockObj)
			{
				if (udpLineParkManager.ContainsKey(groupCode))
				{
					udpLineParkGroup = udpLineParkManager[groupCode];
				}
			}
			return udpLineParkGroup;
		}


		/// <summary>
		/// 清理数据
		/// </summary>
		public static void Clear()
		{
			lock (lockObj)
			{
				udpLineParkManager.Clear();
			}
		}
		/// <summary>
		/// 更新数据
		/// </summary>
		public static void UpData()
		{
			List<long> keys = null;

			lock (lockObj)
			{
				keys = new List<long>(udpLineParkManager.Keys);
			}

			List<UdpLineParkGroup> sendParks = new List<UdpLineParkGroup>();
			List<long> removeParkKeys = new List<long>();

			for (int i = 0; i < keys.Count; ++i)
			{
				long key = keys[i];

				lock (lockObj)
				{
					if (udpLineParkManager.ContainsKey(key))
					{
						UdpLineParkGroup udpGroup = udpLineParkManager[key];

						if ((DateTime.Now - udpGroup._CreateTime).TotalMilliseconds > UdpSubmit.outTime)
						{
							if (udpGroup._ReGetCount < 4)
							{
								udpGroup._ReGetCount++;
								sendParks.Add(udpGroup);
							}
							else
							{
								removeParkKeys.Add(key);
							}
						}
					}
				}
			}

			for (int i = 0; i < removeParkKeys.Count; ++i)
			{
				lock (lockObj)
				{
					if (udpLineParkManager.ContainsKey(removeParkKeys[i]))
					{
						udpLineParkManager.Remove(removeParkKeys[i]);
					}
				}
			}

			for (int i = 0; i < sendParks.Count; ++i)
			{
				UdpLineParkGroup udpGroup = sendParks[i];
				List<int> indexList = new List<int>();

				if (udpGroup.ParkList != null)
				{
					for (int ind = 0; ind < udpGroup.ParkList.Count; ++ind)
					{
						indexList.Add(udpGroup.ParkList[ind]._ParkIndex);
					}
				}

				udpGroup._CreateTime = DateTime.Now;
				SendComplate(udpGroup._IpString, udpGroup._Port, udpGroup._ParkGroupCode, 0, indexList);
			}

			sendParks.Clear();
		}

		/// <summary>
		/// 发送完成数据
		/// </summary>
		/// <param name="code">包编码</param>
		/// <param name="complate">0没有完成 1完成</param>
		/// <param name="indexs">完成接收的Index</param>
		public static void SendComplate(string ip, int port, long code, byte complate, List<int> indexs)
		{
			UdpSubmit.UdpSubmitComplate complateData = new UdpSubmit.UdpSubmitComplate();
			complateData.parkGroupCode = code;
			complateData.complate = complate;

			if (indexs != null)
			{
				complateData.getList = indexs;
			}

			byte[] complateBuf = complateData.GetBytes();
			UdpNetWork.Instance.SendMessageUdpCompalte(ip, port, complateBuf, 1);
		}
	}

}
