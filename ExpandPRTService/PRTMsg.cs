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
    class PRTMsg
    {
        private byte[] _prtMessage;
        private int _prtBytesRead;
        TcpClient _tcpClient;
        public PRTMsg()
        {
        }

        public PRTMsg(TcpClient inTcpClient, byte[] inPrtMessage, int inPrtBytesRead)
        {
            _prtMessage = inPrtMessage;
            _tcpClient = inTcpClient;
            _prtBytesRead = inPrtBytesRead;
        }

        public void QueuePRTMessage()
        {
            try
            {
                ProcessPRTMessage();
            }
            catch (SocketException)
            {
                throw;
            }
        }

        private void ProcessPRTMessage()
        {
            string localException;
            byte[] responseData = null;
            try
            {
                TraceHelper.WriteDebugFormat(null, "Message received!bytes read={0}", _prtBytesRead);
                byte[] requestData = new byte[_prtBytesRead];
                
                Array.Copy(_prtMessage, requestData, _prtBytesRead);

                // Call ExpandWebService passing the msg received from socket
                ExpandWebService.Service expWS = new ExpandWebService.Service();
                expWS.Url = Properties.Settings.Default.eXpandWebService;
                responseData = expWS.RunSwitchPipeline(requestData);
                TraceHelper.WriteDebugFormat(null, "eXPandWSOutHeaderValue={0}", expWS.eXPandWSOutHeaderValue.eXPandErrorDescription);
                if (responseData != null)
                {
                    string prtHeader = string.Format("{0}{1}-{2:X}{3:X}{4:X}-{5:X}{6:X}{7:X}-{8:X2}{9:X2}-{10:X2}-{11}-{12}{13}", responseData[0], responseData[1], responseData[2], responseData[3], responseData[4], responseData[5], responseData[6], responseData[7], responseData[8], responseData[9], responseData[10], responseData[11], responseData[12], responseData[13]);
                    TraceHelper.WriteDebugFormat(null, "Response EMIS Message Header={0}", prtHeader);

                    string responseDataHex = Convert2Hex(responseData);
                    TraceHelper.WriteDebugFormat(null, "Switch Response={0}", responseDataHex);

                    TraceHelper.WriteDebugFormat(null, "Message sent!");

                    // Return msg received from ExpandWebService to socket
                    lock (_tcpClient.GetStream())
                    {
                        NetworkStream netStream = _tcpClient.GetStream();
                        netStream.Write(responseData, 0, responseData.Length);
                    }
                }
                else
                {
                    TraceHelper.WriteErrorFormat(null, "null response not sent!");
                }
            }
            catch (Exception exp)
            {
                localException = exp.Message;
                TraceHelper.WriteErrorFormat(null, "Exception:{0}", exp.Message);
            }
        }
        private string Convert2Hex(byte[] toConvert)
        {
            StringBuilder outHex = new StringBuilder();
            for (int i = 0; i < toConvert.Length; i++)
            {
                outHex.AppendFormat("{0:X}.", toConvert[i]);
            }
            return outHex.ToString();
        }
    }
}
