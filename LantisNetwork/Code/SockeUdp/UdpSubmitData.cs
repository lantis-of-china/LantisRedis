using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Network
{
	/// <summary>
	/// 需要应答的包
	/// </summary>
	public class UdpSubmitData
	{
		/// <summary>
		/// 地址
		/// </summary>
		public string ip;
		/// <summary>
		/// 端口
		/// </summary>
		public int port;
		/// <summary>
		/// 消息队列
		/// </summary>
		public List<UdpPark> parkList;
		/// <summary>
		/// 发送时间Ms
		/// </summary>
		public DateTime createTime;
		/// <summary>
		/// 发送次数
		/// </summary>
		public int count;
	}

}
