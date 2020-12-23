using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Network
{
	/// <summary>
	/// UDP应答管理
	/// </summary>
	public class UdpSubmit
	{
		public static object lockObj = new object();
		/// <summary>
		/// 超时时间Ms
		/// </summary>
		public static int outTime = 3000;
		/// <summary>
		/// 发送过的包
		/// </summary>
		public static Dictionary<long, UdpSubmitData> messagePardList = new Dictionary<long, UdpSubmitData>();
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
		/// 记住应答
		/// </summary>
		/// <param name="data"></param>
		public static void RecordData(List<UdpPark> data, string ip, int port)
		{
			if (data == null || data.Count == 0)
			{
				DebugLoger.LogError("要记录的数据空");
				return;
			}

			UdpSubmitData udpSubmitData = null;
			long key = data[0]._ParkGroupCode;

			lock (lockObj)
			{
				if (messagePardList.ContainsKey(key))
				{
					udpSubmitData = messagePardList[key];
				}
				else
				{
					udpSubmitData = new UdpSubmitData();
					messagePardList.Add(key, udpSubmitData);
				}

				udpSubmitData.parkList = data;
				udpSubmitData.createTime = DateTime.Now;
				udpSubmitData.count = 0;
				udpSubmitData.ip = ip;
				udpSubmitData.port = port;
			}
		}

		/// <summary>
		/// 移除应答
		/// </summary>
		/// <param name="key"></param>
		public static void RemoveSubmit(long key)
		{
			lock (lockObj)
			{
				if (messagePardList.ContainsKey(key))
				{
					messagePardList.Remove(key);
				}
			}
		}

		/// <summary>
		/// 获取应答
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static UdpSubmitData GetSubmit(long key)
		{
			lock (lockObj)
			{
				if (messagePardList.ContainsKey(key))
				{
					return messagePardList[key];
				}
			}

			return null;
		}

		/// <summary>
		/// 清理记录数据
		/// </summary>
		public static void ClearRecordData()
		{
			messagePardList.Clear();
		}

		/// <summary>Update
		/// 刷新应答
		/// </summary>
		public static void UpData()
		{
			List<UdpSubmitData> needSendParkList = new List<UdpSubmitData>();
			List<long> needRemoveLong = new List<long>();
			List<long> messageKeys = null;

			lock (lockObj)
			{
				messageKeys = new List<long>(messagePardList.Keys);
			}

			for (int index = 0; index < messageKeys.Count; ++index)
			{
				UdpSubmitData usd = GetSubmit(messageKeys[index]);

				if (usd != null)
				{
					TimeSpan span = System.DateTime.Now - usd.createTime;

					if (span.TotalMilliseconds > outTime)
					{
						if (usd.count < 4)
						{
							usd.createTime = System.DateTime.Now;
							usd.count++;
							needSendParkList.Add(usd);
						}
						else
						{
							needRemoveLong.Add(messageKeys[index]);
						}
					}
				}
			}

			for (int i = 0; i < needRemoveLong.Count; ++i)
			{
				RemoveSubmit(needRemoveLong[i]);
			}

			for (int i = 0; i < needSendParkList.Count; ++i)
			{
				UdpSubmitData udpParkList = needSendParkList[i];

				if (UdpNetWork.HasInstance())
				{
					UdpNetWork.Instance.SendParkList(udpParkList.parkList, udpParkList.ip, udpParkList.port);
				}
			}
		}
	}

}
