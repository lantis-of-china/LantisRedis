using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace LantisNetwork
{
    public class MessageSenderManager
    {
        public static object lockSelf = new object();
        public static Dictionary<Socket, SenderState> sendMap = new Dictionary<Socket, SenderState>();
        
        /// <summary>
        /// 添加一个发送
        /// </summary>
        /// <param name="sender"></param>
        static public void AddSender(MessageSender sender)
        {
            SenderState senderQueue = null;

            lock (lockSelf)
            {
                if (sendMap.ContainsKey(sender.clientSocket))
                {
                    senderQueue = sendMap[sender.clientSocket];
                }
                else
                {
                    senderQueue = new SenderState();
                    sendMap.Add(sender.clientSocket, senderQueue);
                }
            }

            sender.bindSenderState = senderQueue;
            senderQueue.Enqueue(sender);
            senderQueue.Send();
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="socket"></param>
        static public void RemoveSender(Socket socket)
        {
            lock (lockSelf)
            {
                if (sendMap.ContainsKey(socket))
                {
                    SenderState senderState = sendMap[socket];
                    senderState.Close();
                    sendMap.Remove(socket);
                }
            }
        }
    }

}
