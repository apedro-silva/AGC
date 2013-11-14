using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace SF.Expand.Switch.PipelineComponents
{
    public class ClientSocket
    {
        private Socket _socketClient = null;
        private TcpClient _tcpClient = null;
        public ClientSocket()
        {
        }

        public void CreateClientSocket(string host, int port)
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
            catch (Exception)
            {
                throw;
            }
        }

        public void CreateTcpClient(String server, Int32 port)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                _tcpClient = new TcpClient(server, port);
            }
            catch (ArgumentNullException e)
            {
                throw e;
            }
            catch (SocketException e)
            {
                throw e;
            }
        }

        public void CloseTcpClient()
        {
            _tcpClient.Close();
        }

        public void CloseClientSocket()
        {
            if (_socketClient == null)
                return;
            _socketClient.Close();
        }
        public byte[] SocketClientSendAndReceive(byte[] requestData)
        {
            byte[] responseData = new byte[1024];

            if (_socketClient == null)
                return null;
            _socketClient.Send(requestData, System.Net.Sockets.SocketFlags.None);

            int bytesReceived = _socketClient.Receive(responseData);

            byte[] returnData = new byte[bytesReceived];
            Array.Copy(responseData, returnData, bytesReceived);
            return returnData;
        }

        public byte[] TcpClientSendAndReceive(byte[] message, int Timeout)
        {
            // Get a client stream for reading and writing.
            NetworkStream stream = _tcpClient.GetStream();

            int msg_len = message.Length;
            byte[] sendMsg = new byte[msg_len + 2];

            sendMsg[0] = (byte)(msg_len / 256);
            sendMsg[1] = (byte)(msg_len % 256);

            Array.Copy(message, 0, sendMsg, 2, message.Length);

            // Send the message to the connected TcpServer. 
            stream.Write(sendMsg, 0, sendMsg.Length);

            // Buffer to store the response bytes.
            Byte[] data = new Byte[1024];

            _tcpClient.ReceiveTimeout = Timeout;
            // Read the the TcpServer response bytes.
            Int32 bytesReceived = stream.Read(data, 0, data.Length);

            // prepare buffer to return
            Byte[] returnData = new byte[bytesReceived];
            Array.Copy(data, returnData, bytesReceived);

            // Close everything.
            stream.Close();
            return returnData;
        }
    }
}
