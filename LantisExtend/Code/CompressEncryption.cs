using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ComponentAce.Compression.Libs.zlib;
using Lantis;

namespace Lantis.Extend
{

	public unsafe class CompressEncryption
	{
		/// <summary>
		/// 压缩加密
		/// </summary>
		/// <param name="sourceByte"></param>
		/// <returns></returns>
		public unsafe static byte[] CompressEncryptionData(byte[] sourceByte)
		{
			try
			{
				MemoryStream stmOutTemp = new MemoryStream();
				ZOutputStream outZStream = new ZOutputStream(stmOutTemp, zlibConst.Z_DEFAULT_COMPRESSION);
				outZStream.Write(sourceByte, 0, sourceByte.Length);
				outZStream.finish();
				sourceByte = stmOutTemp.ToArray();
				return sourceByte;
			}
			catch (Exception e)
			{
				Logger.Error("压缩数据出现异常 " + e.ToString());
				return null;
			}
		}

		/// <summary>
		/// 解压缩解密
		/// </summary>
		/// <param name="sourceByte"></param>
		/// <returns></returns>
		public static byte[] UnCompressDecompressData(byte[] sourceByte)
		{
			try
			{
				MemoryStream stmOutput = new MemoryStream();
				ZOutputStream outZStream = new ZOutputStream(stmOutput);
				outZStream.Write(sourceByte, 0, sourceByte.Length);
				outZStream.finish();
				sourceByte = stmOutput.ToArray();
				return sourceByte;
			}
			catch (Exception e)
			{
				Logger.Error("解压数据出现异常 " + e.ToString());
				return null;
			}
		}







		private static int offsetCount = 3;
		internal class RecordEncryption
		{
			public byte sameCount;
			public byte value;
		}


		public static byte[] EncryptionMsg(byte[] sourceByte)
		{
			if (sourceByte == null || sourceByte.Length == 0)
			{
				return sourceByte;
			}

			try
			{
				for (Int64 offestIndex = 0; offestIndex < sourceByte.Length - offsetCount; ++offestIndex)
				{
					byte curByte = sourceByte[offestIndex];

					sourceByte[offestIndex] = sourceByte[offestIndex + offsetCount];

					sourceByte[offestIndex + offsetCount] = curByte;
				}

				return sourceByte;
			}
			catch
			{
				return sourceByte;
			}
		}

		public static byte[] UnEncryptionMsg(byte[] encryByte)
		{
			if (encryByte == null || encryByte.Length == 0)
			{
				return encryByte;
			}

			try
			{
				byte[] sourceBuf = encryByte;

				for (Int64 offestIndex = sourceBuf.Length - 1; offestIndex >= offsetCount; --offestIndex)
				{
					byte curByte = sourceBuf[offestIndex];

					sourceBuf[offestIndex] = sourceBuf[offestIndex - 3];

					sourceBuf[offestIndex - 3] = curByte;
				}

				return sourceBuf;
			}
			catch
			{
				return encryByte;
			}
		}
	}
}
