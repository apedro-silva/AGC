using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;

namespace SoftFinanca
{
    public class PRTSocket
    {
        private Socket _socketClient = null;
        byte[] readBuffer = new byte[1024];

        public PRTSocket()
        {
        }

        public void CreateSocket(string host, int port)
        {
            IPAddress[] IPs = Dns.GetHostAddresses(host);

            Socket s = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                s.Connect(IPs[0], port);
                if (s.Connected)
                    _socketClient = s;
            }
            catch(Exception)
            {
                throw;
            }
        }
        public void CloseSocket()
        {
            if (_socketClient!=null)
                _socketClient.Close();
        }

        public void SocketSend(string prtAPL, string prtSES)
        {
            byte[] prtAPLMsg = Encoding.Default.GetBytes(prtAPL);
            byte[] prtSESMsg = Encoding.Default.GetBytes(prtSES);

            int msg_len = prtAPLMsg.Length + prtSESMsg.Length + 4;
            byte[] sendMsg = new byte[msg_len];

            sendMsg[0] = (byte)(msg_len / 256);
            sendMsg[1] = (byte)(msg_len % 256);
            Array.Copy(prtAPLMsg, 0, sendMsg, 2, prtAPLMsg.Length);
            sendMsg[2 + prtAPLMsg.Length] = 0x0a;
            Array.Copy(prtSESMsg, 0, sendMsg, 2 + prtAPLMsg.Length + 1, prtSESMsg.Length);
            sendMsg[sendMsg.Length - 1] = 0x0a;

            _socketClient.Send(sendMsg);
        }

        public int SocketReceive(byte[] bytes)
        {
            //_socketClient.BeginReceive(readBuffer, 0, 1024, SocketFlags.None, new AsyncCallback(StreamReceiver),null);
            int bytesRecv = _socketClient.Receive(bytes);
            return bytesRecv;
        }
        private void StreamReceiver(IAsyncResult ar)
        {
            int bytesRead;

            // Finish asynchronous read into readBuffer and get number of bytes read.
            bytesRead = _socketClient.EndReceive(ar);

        }

    }
    class myState
    {
    }
}
