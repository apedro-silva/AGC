using System;
using System.Text;
using System.Configuration;
using SF.Expand.Core.Orch;
using SF.Expand.Business;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// Perform communications with the Host
    /// </summary>
    public class CallHost :OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {

            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;

            try
            {
                // if these fields doesn't exist there's no message
                if (State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true) == "")
                    return;

                string originalResponseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("OriginalResponseCode", true);

                // If we already have a switch response
                if (originalResponseCode != null)
                    return;

                byte[] requestMsg = State.OrchWrkData.GetWrkData().ReadNodeBuffer("FlexCubeRequest", true);

                State.OrchWrkData.AddToObjBucket("DataEnvioFC", DateTime.Now);

                string TipoTerminal = State.OrchWrkData.GetWrkData().ReadNodeValue("TipoTerm", true);
                string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);

                // Due to Flexcube POS Interface limitations these transactions will go to the ATM Interface
                if ("001,002,010,011,034".IndexOf(CodTrn) < 0)
                    TipoTerminal = "A";

                byte[] responseMsg = CallFlexCube(State, TipoTerminal, requestMsg);
                if (responseMsg == null)
                    return;

                string responseStr = Encoding.Default.GetString(responseMsg);

                State.OrchWrkData.AddToObjBucket("DataRecepcaoFC", DateTime.Now);

                byte[] Buff = null;
                Buff = Encoding.Default.GetBytes(Encoding.Default.GetString(responseMsg, 2, responseMsg.Length - 2));
                State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, Buff);
                State.OrchWrkData.GetWrkData().WriteNodeBuffer("FlexCubeResponse", Buff, true);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("FlexCube:CallHost",exp);
            }
        }

        private byte[] CallFlexCube(OrchPipeComponent.ComponentState State, string TerminalType, byte[] requestMsg)
        {
            string ServerIP=null;
            int ServerPort=0;
            int ServerTimeout = 0;

            System.Configuration.AppSettingsReader s = new AppSettingsReader();
            // ATM
            if (TerminalType == "A")
            {
                ServerIP = (string)s.GetValue("ATMServerIP", typeof(string));
                ServerPort = (int)s.GetValue("ATMServerPort", typeof(int));
                ServerTimeout = (int)s.GetValue("ATMServerTimeout", typeof(int));
            }
            //POS
            else if (TerminalType == "B")
            {
                ServerIP = (string)s.GetValue("POSServerIP", typeof(string));
                ServerPort = (int)s.GetValue("POSServerPort", typeof(int));
                ServerTimeout = (int)s.GetValue("POSServerTimeout", typeof(int));
            }
            else
            {
                throw new BusinessException(string.Format("Invalid Terminal Type <{0}>", TerminalType));
            }

            try
            {
                ClientSocket fcSocket = new ClientSocket();
                fcSocket.CreateTcpClient(ServerIP, ServerPort);

                byte[] responseMsg = fcSocket.TcpClientSendAndReceive(requestMsg, ServerTimeout * 1000);
                return responseMsg;
            }
            catch (Exception exp)
            {
                string error = exp.Message;
                State.LastError = new BusinessException("FlexCube Off-Line");
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "FlexCube Off-Line", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "-2", true);
                return null;
            }
        }
    }
}
