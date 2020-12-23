using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Network
{
	/// <summary>
	/// 完成管理器 过滤多包作用
	/// </summary>
	public class ComplateRecorder
	{
		public static object lockObj = new object();

		public static Dictionary<long, long> complateRecorder = new Dictionary<long, long>();

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
		/// 是否能接收
		/// </summary>
		/// <returns></returns>
		public static bool CanRecive(long id)
		{
			lock (lockObj)
			{
				if (complateRecorder.ContainsKey(id))
				{
					return false;
				}
			}

			return true;
		}
		/// <summary>
		/// 添加记录
		/// </summary>
		/// <param name="id"></param>
		public static void AddRecorder(long id)
		{
			lock (lockObj)
			{
				if (!complateRecorder.ContainsKey(id))
				{
					complateRecorder.Add(id, DateTime.Now.Ticks);
				}
			}
		}

		/// <summary>
		/// 移除记录
		/// </summary>
		/// <param name="id"></param>
		public static void RemoverRecorder(long id)
		{
			lock (lockObj)
			{
				if (complateRecorder.ContainsKey(id))
				{
					complateRecorder.Remove(id);
				}
			}
		}

		/// <summary>
		/// 获取记录
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static long GetRecorder(long id)
		{
			lock (lockObj)
			{
				if (complateRecorder.ContainsKey(id))
				{
					return complateRecorder[id];
				}
			}
			return 0;
		}

		/// <summary>
		/// 更新清理
		/// </summary>
		public static void UpData()
		{
			List<long> keys = null;
			lock (lockObj)
			{
				keys = new List<long>(complateRecorder.Keys);
			}

			long timeOut = UdpSubmit.outTime * 5;
			for (int i = 0; i < keys.Count; ++i)
			{
				long timeTicks = GetRecorder(keys[i]);
				if (timeTicks != 0)
				{
					long timeSencend = (DateTime.Now.Ticks - timeTicks) / 10000000;
					if (timeSencend > timeOut)
					{
						RemoverRecorder(keys[i]);
					}
				}
			}
		}
	}

}
