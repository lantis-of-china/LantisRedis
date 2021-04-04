using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using Lantis.Pool;

namespace Lantis.Network
{
    public class MessageReciver : LantisPoolInterface
    {
		public bool write;
        /// <summary>
        /// 客户套接字
        /// </summary>
        public Socket clientSocket;
        /// <summary>
        /// 接收流
        /// </summary>
        public MemoryStream reciveStream;
        /// <summary>
        /// 长度缓冲区
        /// </summary>
        public byte[] lengthBuf;
        /// <summary>
        /// 消息缓冲区
        /// </summary>
        public byte[] msgBufer;
        /// <summary>
        /// 定义缓冲区大小
        /// </summary>
        public int bufLength;
        /// <summary>
        /// 当前是否接受消息头 消息头为消息长度 4字节
        /// </summary>
        public bool msgHead;
        /// <summary>
        /// 准备接收的消息长度
        /// </summary>
        public long messageLength = 0;
        /// <summary>
        /// 消息回调
        /// </summary>
        private Action<byte[], Socket, string, int> messageDriverCall;
        private Action reciverExeceptionCall;

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
        }

        public MessageReciver()
        {
        }

        public void Start(Socket client, Action<byte[], Socket, string, int> messageCall, Action execeptionCall)
        {
            messageDriverCall = messageCall;
            reciverExeceptionCall = execeptionCall;
            bufLength = 2048;
            clientSocket = client;

            if (reciveStream == null)
            {
                reciveStream = new MemoryStream();
            }

            if (lengthBuf == null)
            {
                lengthBuf = new byte[4];
            }

            if (msgBufer == null)
            {
                msgBufer = new byte[bufLength];
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(BeginReceiveMsgHead));
        }

        /// <summary>
        /// 开始接收消息头
        /// </summary>
        public void BeginReceiveMsgHead(object paramar)
        {
            msgHead = true;

            try
            {
                clientSocket.BeginReceive(lengthBuf, 0, lengthBuf.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception e)
            {
				if (write)
				{
				}
            }
        }

        /// <summary>
        /// 开始接收消息体
        /// </summary>
        public void BeginReciveMsgBody(object paramar)
        {
            msgHead = false;

            try
            {
				int reciveCount = msgBufer.Length;

				//if (reciveStream != null)
				//{
					int needRecive = (int)messageLength - (int)reciveStream.Position;

					if (needRecive >= msgBufer.Length)
					{
						reciveCount = msgBufer.Length;
					}
					else
					{
						reciveCount = needRecive;
					}
				//}

				clientSocket.BeginReceive(msgBufer, 0, reciveCount, SocketFlags.None, ReceiveCallback, null);
            }
            catch(Exception e)
            {
				if (write)
				{
				}
			}
        }

        /// <summary>
        /// 数据接收回调
        /// </summary>
        /// <param name="result"></param>
        public void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int readCount = clientSocket.EndReceive(result);

                if (readCount == 0)
                {
                    throw new Exception();
                }

                if (msgHead)
                {
                    if (readCount != 4)
                    {
                    }

                    reciveStream.Seek(0, SeekOrigin.Begin);
                    messageLength = System.BitConverter.ToInt32(lengthBuf, 0);
                    BeginReciveMsgBody(null);
                }
                else
                {
                    reciveStream.Write(msgBufer, 0, readCount);

                    if (reciveStream.Length >= messageLength)
                    {
                        byte[] msgBuf = reciveStream.ToArray();
                        reciveStream.Seek(0, SeekOrigin.Begin);
                        //reciveStream.Dispose();
                        //reciveStream.Close();
                        //reciveStream = null;
                        BeginReceiveMsgHead(null);

                        if (messageDriverCall != null)
                        {
                            messageDriverCall(msgBuf, clientSocket, (clientSocket.RemoteEndPoint as System.Net.IPEndPoint).Address.ToString(), (clientSocket.RemoteEndPoint as System.Net.IPEndPoint).Port);
                        }
                    }
                    else
                    {
                        BeginReciveMsgBody(null);
                    }
                }
            }
            catch(Exception e)
            {
                ReceiveException();

				if (write)
				{
				}
			}

        }


        public void ReceiveException()
        {
            if (reciverExeceptionCall != null)
            {
                reciverExeceptionCall();
            }
		}
    }
}