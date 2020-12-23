using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Network
{
	/// <summary>
	/// 流线包组合工具
	/// </summary>
	public class UdpLineParkGroup
	{
		/// <summary>
		/// 包号
		/// </summary>
		public long _ParkGroupCode;
		/// <summary>
		/// Ip地址
		/// </summary>
		public string _IpString;
		/// <summary>
		/// 端口号
		/// </summary>
		public int _Port;
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime _CreateTime;
		/// <summary>
		/// 包列
		/// </summary>
		public List<UdpPark> ParkList;
		/// <summary>
		/// 重抓次数
		/// </summary>
		public int _ReGetCount;
	}
}
