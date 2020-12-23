using System;
using System.Collections.Generic;
using System.Text;

namespace Lantis.Network
{
	/// <summary>
	/// UDP应答包
	/// </summary>
	public class UdpSubmitComplate
	{
		/// <summary>
		/// 消息包编码
		/// </summary>
		public long parkGroupCode;
		/// <summary>
		/// 1-完成 0-失败
		/// </summary>
		public byte complate;
		/// <summary>
		/// 失败的列表索引
		/// </summary>
		public List<int> getList = new List<int>();

		/// <summary>
		/// 获取字节
		/// </summary>
		/// <returns></returns>
		public byte[] GetBytes()
		{
			List<byte> dataBuf = new List<byte>();
			byte[] codeBuf = BitConverter.GetBytes(parkGroupCode);
			dataBuf.Add(codeBuf[0]);
			dataBuf.Add(codeBuf[1]);
			dataBuf.Add(codeBuf[2]);
			dataBuf.Add(codeBuf[3]);
			dataBuf.Add(codeBuf[4]);
			dataBuf.Add(codeBuf[5]);
			dataBuf.Add(codeBuf[6]);
			dataBuf.Add(codeBuf[7]);
			dataBuf.Add(complate);
			byte[] countList = BitConverter.GetBytes(getList.Count);

			for (int i = 0; i < countList.Length; ++i)
			{
				dataBuf.Add(countList[i]);
			}

			for (int i = 0; i < getList.Count; ++i)
			{
				byte[] indexBuf = BitConverter.GetBytes(getList[i]);
				dataBuf.Add(indexBuf[0]);
				dataBuf.Add(indexBuf[1]);
				dataBuf.Add(indexBuf[2]);
				dataBuf.Add(indexBuf[3]);
			}

			return dataBuf.ToArray();
		}

		/// <summary>
		/// 设置数据
		/// </summary>
		public void SetData(byte[] data)
		{
			parkGroupCode = BitConverter.ToInt64(data, 0);
			complate = data[8];

			if (complate == 0)
			{
				int indexCount = BitConverter.ToInt32(data, 9);

				for (int i = 0; i < indexCount; ++i)
				{
					int index = BitConverter.ToInt32(data, i * 4 + 13);
					getList.Add(index);
				}
			}
		}
	}

}
