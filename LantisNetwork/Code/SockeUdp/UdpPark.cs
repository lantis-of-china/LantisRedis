using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Lantis.Network
{
	/// <summary>
	/// 封包信息
	/// </summary>
	public class UdpPark
	{
		//包的分组码
		public long _ParkGroupCode;
		//包的索引
		public int _ParkIndex;
		//包结束标志
		public int _ParkEndTag;
		//需要1-游戏消息(需要应答) 0-应答消息(应答消息)
		public byte _NeedComplate;
		//二进制数据
		public byte[] _MsgDate;

		public UdpPark(long parkGroupCode, int parkIndex, int parkEndTag, byte needComplate, byte[] msgDate)
		{
			_ParkGroupCode = parkGroupCode;
			_ParkIndex = parkIndex;
			_ParkEndTag = parkEndTag;
			_NeedComplate = needComplate;
			_MsgDate = msgDate;
		}
	}
}
