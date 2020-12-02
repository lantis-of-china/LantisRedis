using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Network
{
    /// <summary>
    /// 发送状态
    /// </summary>
    public class SenderState : LantisPoolInterface
    {
        public object lockSelf = new object();
        public bool close = false;
        public bool sending = false;
        public Queue<MessageSender> sendQueue = new Queue<MessageSender>();

        public void OnPoolSpawn()
        {
            close = false;
            sending = false;
            sendQueue.Clear();
        }

        public void OnPoolDespawn()
        {
            close = false;
            sending = false;
            sendQueue.Clear();
        }

        /// <summary>
        /// 加入队列
        /// </summary>
        /// <param name="ms"></param>
        public void Enqueue(MessageSender ms)
        {
            lock (lockSelf)
            {
                sendQueue.Enqueue(ms);
            }
        }

        /// <summary>
        /// 数量获取
        /// </summary>
        public int QueueCount
        {
            get
            {
                lock (lockSelf)
                {
                    return sendQueue.Count;
                }
            }
        }

        /// <summary>
        /// 发送关闭
        /// </summary>
        public void SendEnd()
        {
            lock (lockSelf)
            {
                sending = false;
            }

        }

        /// <summary>
        /// 发送
        /// </summary>
        public void Send()
        {
            lock (lockSelf)
            {
                if (!close && !sending && sendQueue.Count > 0)
                {
                    sending = true;
                    MessageSender messageSender = sendQueue.Dequeue();
                    messageSender.BeginSendMsg();
                }
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            lock (lockSelf)
            {
                close = true;
                sending = false;
            }
        }
    }

}
