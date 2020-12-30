using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Network
{
	/// <summary>
	/// UDP工具
	/// </summary>
	public class UdpParkTool
	{
		private static object lockObj = new object();
		/// <summary>
		/// 
		/// </summary>
		private static long id = 0;

		public static long GetLongId()
		{
			long value = 0;
			lock (lockObj)
			{
				id++;
				value = id;
			}
			return value;
		}

		/// <summary>
		/// 分割数据为包
		/// </summary>
		/// <param name="msgBuf"></param>
		/// <param name="bufferLenght"></param>
		/// <returns></returns>
		public static List<UdpPark> Separate(byte[] msgBuf, int bufferLenght, byte needComplate)
		{
			int messageLenght = bufferLenght - 17;
			int Toald = msgBuf.Length / messageLenght;
			int Remainder = msgBuf.Length % messageLenght;

			if (Remainder > 0)
			{
				Toald++;
			}

			var Rd = new Random();
			var parkGroupCode = GetLongId();
			var parkList = new List<UdpPark>();

			//循环拆包
			for (int parkIndex = 0; parkIndex < Toald; parkIndex++)
			{
				//0标志 不结束
				int endTag = 0;
				byte[] chunk = null;

				if (Remainder > 0 && (parkIndex + 1) >= Toald)
				{
					//结束包
					chunk = new byte[Remainder + 17];
					endTag = 1;

					byte[] groupCodebuf = BitConverter.GetBytes(parkGroupCode);
					byte[] indexBuf = BitConverter.GetBytes(parkIndex);
					byte[] endBuf = BitConverter.GetBytes(endTag);
					Buffer.BlockCopy(groupCodebuf, 0, chunk, 0, groupCodebuf.Length);
					Buffer.BlockCopy(indexBuf, 0, chunk, 8, indexBuf.Length);
					Buffer.BlockCopy(endBuf, 0, chunk, 12, endBuf.Length);
					chunk[16] = needComplate;
					Buffer.BlockCopy(msgBuf, parkIndex * messageLenght, chunk, 17, Remainder);
				}
				else
				{
					chunk = new byte[bufferLenght];
					byte[] groupCodebuf = BitConverter.GetBytes(parkGroupCode);
					byte[] indexBuf = BitConverter.GetBytes(parkIndex);
					byte[] endBuf = BitConverter.GetBytes(endTag);
					Buffer.BlockCopy(groupCodebuf, 0, chunk, 0, groupCodebuf.Length);
					Buffer.BlockCopy(indexBuf, 0, chunk, 8, indexBuf.Length);
					Buffer.BlockCopy(endBuf, 0, chunk, 12, endBuf.Length);
					chunk[16] = needComplate;
					Buffer.BlockCopy(msgBuf, parkIndex * messageLenght, chunk, 17, messageLenght);
				}

				parkList.Add(new UdpPark(parkGroupCode, parkIndex, endTag, needComplate, chunk));
			}

			return parkList;
		}

		/// <summary>
		/// 组合从网洛接收到的数据包
		/// </summary>
		/// <param name="sourceParkList"></param>
		/// <returns></returns>
		public static byte[] Combination(List<UdpPark> sourceParkList)
		{
			if (sourceParkList == null)
			{
				return null;
			}

			if (sourceParkList.Count == 0)
			{
				return null;
			}

			UdpPark firstDate = sourceParkList.Find(item => item._ParkIndex == 0);
			UdpPark endParkDate = sourceParkList.Find(item => item._ParkEndTag == 1);

			//验证完整性
			if (endParkDate == null || firstDate == null)
			{
				if (endParkDate == null)
				{
					//DebugLoger.LogError("验证完整状态失败 endParkDate null");
				}

				if (firstDate == null)
				{
					//DebugLoger.LogError("验证完整状态失败 firstDate null");
				}
				//验证不了
				return null;
			}

			if ((endParkDate._ParkIndex + 1) > sourceParkList.Count)
			{
				//索引加1 数据包不足
				//DebugLoger.LogError("验证完整状态失败 二");
				return null;
			}

			int msgLenght = firstDate._MsgDate.Length;
			//全部数据包长度  数据条数-1 个 数据长度 加上尾包长度
			int messageCount = msgLenght * (sourceParkList.Count - 1) + endParkDate._MsgDate.Length;
			byte[] messageDate = new byte[messageCount];

			for (int parkIndex = 0; parkIndex < sourceParkList.Count; parkIndex++)
			{
				//循环组合
				UdpPark udpPark = sourceParkList[parkIndex];
				///直接按照索引对齐包数据
				udpPark._MsgDate.CopyTo(messageDate, udpPark._ParkIndex * msgLenght);
			}

			return messageDate;
		}

		/// <summary>
		/// 通过数据 实例化包  从网络接收到的数据就用来实例化包
		/// </summary>
		/// <param name="buffer"></param>
		public static UdpPark InstancePark(byte[] buffer)
		{
			if (buffer == null || buffer.Length < 16)
			{
				return null;
			}

			long parkGroupCode = BitConverter.ToInt64(buffer, 0);//8位
			int parkIndex = BitConverter.ToInt32(buffer, 8);//4位
			int parkEndTag = BitConverter.ToInt32(buffer, 12);//4位
			byte needComplate = buffer[buffer[16]];
			int remainderCount = buffer.Length - 17;
			byte[] msgDate = new byte[remainderCount];
			Buffer.BlockCopy(buffer, 17, msgDate, 0, remainderCount);
			UdpPark up = new UdpPark(parkGroupCode, parkIndex, parkEndTag, needComplate, msgDate);

			return up;
		}
	}
}
