using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using SF.Expand.Trace;

namespace SF.Expand.Switch
{
    public class PRTTcpClient
    {
        public TcpClient prtClient = null;
        private NetworkStream netStream = null;
        public PRTTcpClient()
        {
        }

        public void CreateTcpClient(string host, int port)
        {
            //_tcpClient = new TcpClient(host, port);
            //netStream = _tcpClient.GetStream();
            //return;

            IPAddress[] IPs = Dns.GetHostAddresses(host);
            prtClient = new TcpClient();
            prtClient.Connect(IPs, port);
            netStream = prtClient.GetStream();
        }

        public void CloseTcpClient()
        {
            if (netStream!=null)
                netStream.Close();

            if (prtClient != null)
                prtClient.Close();
        }

        public void TcpClientSend(string prtAPL, string prtSES)
        {
            byte[] prtAPLMsg = Encoding.Default.GetBytes(prtAPL);
            byte[] prtSESMsg = Encoding.Default.GetBytes(prtSES);

            if (!netStream.CanWrite)
                throw new Exception("PRTTcpClient.TcpClientSend->CanWrite=false");

            int msg_len = prtAPLMsg.Length + prtSESMsg.Length + 4;
            byte[] sendMsg = new byte[msg_len];

            sendMsg[0] = (byte)(msg_len / 256);
            sendMsg[1] = (byte)(msg_len % 256);
            Array.Copy(prtAPLMsg, 0, sendMsg, 2, prtAPLMsg.Length);
            sendMsg[2 + prtAPLMsg.Length] = 0x0a;
            Array.Copy(prtSESMsg, 0, sendMsg, 2 + prtAPLMsg.Length + 1, prtSESMsg.Length);
            sendMsg[sendMsg.Length - 1] = 0x0a;

            netStream.Write(sendMsg, 0, msg_len);
        }

        public int TcpClientReceive(byte[] rcvBuffer, int timeout)
        {
            int bytesRead = 0;

            if (!netStream.CanRead)
                throw new Exception("PRTTcpClient.TcpClientReceive->CanRead=false");

            netStream.ReadTimeout = timeout;
            bytesRead = netStream.Read(rcvBuffer, 0, 2);

            int bytes2Read = rcvBuffer[0] * 256 + rcvBuffer[1];
            bytes2Read -= 2;
            int i = 0;
            do
            {
                i = netStream.Read(rcvBuffer, bytesRead, bytes2Read - i);
                bytesRead += i;
            }
            while (netStream.DataAvailable && bytesRead < bytes2Read);
            return bytesRead;
        }

    }
}
