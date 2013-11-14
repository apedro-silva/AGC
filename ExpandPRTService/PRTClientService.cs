using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Data.Common;
using SF.Expand.Trace;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SF.Expand.Switch
{
    public partial class PRTClientService : ServiceBase
    {
        private const string baseName = "ExpandPRTService.V5";
        private Thread prtReaderThread;
        PRTTcpClient prtTcpClient;
        public PRTClientService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            TraceHelper.Initialize();
            TraceHelper.WriteDebug("Starting...");

            try
            {
                if (Properties.Settings.Default.PRTIp != "")
                {
                    prtReaderThread = new Thread(new ThreadStart(StartPRTReader));
                    prtReaderThread.Start();
                }
            }
            catch (Exception exp)
            {
                TraceHelper.WriteErrorFormat(null, "Exception:{0}", exp.Message);
                SetPrtStatus("NOK");
            }
        }

        protected override void OnStop()
        {
            TraceHelper.WriteDebug("Stopping...");
            try
            {
                prtTcpClient.CloseTcpClient();
                if (prtReaderThread.IsAlive)
                    prtReaderThread.Abort();
            }
            catch (Exception exp)
            {
                TraceHelper.WriteErrorFormat(null, "Exception:{0}", exp.Message);
            }
            SetPrtStatus("NOK");
        }
        private void StartPRTReader()
        {
            int ZeroBytesReadCounter = 0;
            Thread msgThread;
            byte[] prtMessage = new byte[1024];
            ExpandWebService.Service expWS = new ExpandWebService.Service();

            while (true)
            {
                ZeroBytesReadCounter = 0;
                try
                {
                    expWS.Url = Properties.Settings.Default.eXpandWebService;
                    TraceHelper.WriteDebugFormat(null, "IP={0} #Port={1}", Properties.Settings.Default.PRTIp, Properties.Settings.Default.PRTPort.ToString());
                    prtTcpClient = new PRTTcpClient();

                    prtTcpClient.CreateTcpClient(Properties.Settings.Default.PRTIp, Properties.Settings.Default.PRTPort);
                    TraceHelper.WriteDebugFormat(null, "SessionMsg={0}#{1}", Properties.Settings.Default.PRTAPL, Properties.Settings.Default.PRTSES);
                    prtTcpClient.TcpClientSend(Properties.Settings.Default.PRTAPL, Properties.Settings.Default.PRTSES);
                    int bytesReceived = prtTcpClient.TcpClientReceive(prtMessage, Properties.Settings.Default.SleepOnError * 1000);
                    TraceHelper.WriteDebugFormat(null, "PrtResponse={0}", Encoding.Default.GetString(prtMessage, 2, bytesReceived - 2));

                    if (bytesReceived == 0)
                        throw new Exception("PRT Receptor returned 0 bytes");

                    // Call ExpandWebService to report PRT OK
                    SetPrtStatus("OK");

                    //keep reading messages and put them in a separate thread
                    while (ZeroBytesReadCounter<5)
                    {
                        TraceHelper.WriteDebugFormat(null, "Waiting for EMIS Message");
                        bytesReceived = prtTcpClient.TcpClientReceive(prtMessage, Timeout.Infinite);
                        TraceHelper.WriteDebugFormat(null, "EMIS Message received bytes read={0}", bytesReceived.ToString());
                        if (bytesReceived > 0)
                        {
                            ZeroBytesReadCounter = 0;
                            string prtHeader = string.Format("{0}{1}-{2:X}{3:X}{4:X}-{5:X}{6:X}{7:X}-{8:X2}{9:X2}-{10:X2}-{11}-{12}{13}", prtMessage[0], prtMessage[1], prtMessage[2], prtMessage[3], prtMessage[4], prtMessage[5], prtMessage[6], prtMessage[7], prtMessage[8], prtMessage[9], prtMessage[10], prtMessage[11], prtMessage[12], prtMessage[13]);
                            TraceHelper.WriteDebugFormat(null, "Received EMIS Message Header={0}", prtHeader);
                            PRTMsg prtMsg = new PRTMsg(prtTcpClient.prtClient, prtMessage, bytesReceived);
                            msgThread = new Thread(new ThreadStart(prtMsg.QueuePRTMessage));
                            msgThread.Start();
                        }
                        else
                            ZeroBytesReadCounter += 1;
                    }
                }
                catch (Exception exp)
                {
                    TraceHelper.WriteErrorFormat(null, "Exception:{0}", exp.Message);
                }
                finally
                {
                    SetPrtStatus("NOK");
                    int sleepTimeout = Properties.Settings.Default.SleepOnError * 100;
                    TraceHelper.WriteDebugFormat(null, "Sleeping... {0} seconds", sleepTimeout.ToString());
                    Thread.Sleep(Properties.Settings.Default.SleepOnError * 100);
                    if (prtTcpClient!=null)
                        prtTcpClient.CloseTcpClient();
                    prtTcpClient = null;
                }
            }// While (true)
        }
        private void SetPrtStatus(string prtStatus)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");

                DbCommand dbCommand = db.GetStoredProcCommand("ActualizaEstadoPRT");
                db.AddInParameter(dbCommand, "Estado", DbType.String, prtStatus);
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                TraceHelper.WriteErrorFormat(null, "Exception:{0}", exp.Message);
            }

        }
   }
}
