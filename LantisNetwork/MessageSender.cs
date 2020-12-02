using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lantis.Pool;

namespace Lantis.Network
{
	public delegate void ExceCall(string call);

    public class MessageSender : LantisPoolInterface
    {
		public SenderState bindSenderState;
		public bool write;
		public Socket clientSocket; 
        public byte[] sendBuffer;
        public int sendRecord;
        public ExceCall exceCall;

        public void OnPoolSpawn()
        {
        }

        public void OnPoolDespawn()
        {
        }


        public MessageSender()
        {

        }

        public MessageSender(Socket client, byte[] buffer)
        {
            SenderStar(client, buffer);
        }

        public void SenderStar(Socket client, byte[] buffer)
        {
            exceCall = ReceiveException;
            clientSocket = client;
            sendBuffer = buffer;
            sendRecord = 0;
        }

        public void BeginSendMsg()
        {
            if(clientSocket != null && clientSocket.Connected)
            {
                try
                {
                    clientSocket.BeginSend(sendBuffer, sendRecord, sendBuffer.Length - sendRecord, SocketFlags.None, SendCallBack, clientSocket);
                }
                catch (Exception e) 
                {
					if (exceCall != null)
                    {
                        exceCall(e.ToString());
                    }

                    exceCall = null;
                }
            }
            else
            {
                if (exceCall != null)
                {
                    exceCall("null");
                }

                exceCall = null;
            }
        }

        public void SendCallBack(IAsyncResult ar)
        {
            try
            {
                int i = clientSocket.EndSend(ar);
                sendRecord += i;

                if(sendRecord < sendBuffer.Length)
                {
                    BeginSendMsg();
                }
                else
                {
					bindSenderState.SendEnd();
					bindSenderState.Send();
				}
            }
            catch(Exception e)
            {
				if (exceCall != null)
                {
                    exceCall(e.ToString());
                }

                exceCall = null;
            }
        }

        public void ReceiveException(string ex)
        {
			MessageSenderManager.RemoveSender(clientSocket);

			if (write)
			{
			}
		}
    }
}
