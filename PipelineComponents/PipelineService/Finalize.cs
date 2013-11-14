using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// Finalize
    /// </summary>
    public class Finalize : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            byte[] msgOut=null;
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            try
            {
                System.Configuration.AppSettingsReader appReader = new AppSettingsReader();
                string inDebug = (string)appReader.GetValue("DEBUG", typeof(string));

                byte[] PRTHeader = ConstructPRTHeader(State);

                string originalResponseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("OriginalResponseCode", true);
                string originalSwitchResponse = State.OrchWrkData.GetWrkData().ReadNodeValue("OriginalSwitchResponse", true);
                if (originalResponseCode!=null && originalSwitchResponse != null)
                {
                    byte[] PRTResponse = Encoding.ASCII.GetBytes(originalSwitchResponse);
                    State.OrchWrkData.GetWrkData().WriteNodeBuffer("PRTResponse", PRTResponse, true);
                }

                byte[] TransactionResponde = (byte[])State.OrchWrkData.GetWrkData().ReadNodeBuffer("PRTResponse", true);
                if (inDebug == "ON" && (TransactionResponde == null || TransactionResponde.Length == 0))
                {
                    string TextoErro = State.OrchWrkData.GetWrkData().ReadNodeValue("TextoErro", true);
                    TransactionResponde = Encoding.Default.GetBytes(TextoErro);

                    msgOut = ConstructPRTBody(PRTHeader, TransactionResponde);
                }
                else if (TransactionResponde != null && TransactionResponde.Length != 0)
                {
                    msgOut = ConstructPRTBody(PRTHeader, TransactionResponde);
                }
                State.OrchWrkData.AddToObjBucket("SWITCHResponse", msgOut);
            }
            catch (Exception exp)
            {
                State.OrchWrkData.AddToObjBucket("SWITCHResponse", msgOut);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.StackTrace, true);
            }
        }

        private byte[] ConstructPRTBody(byte[] PRTHeader, byte[] TransactionResponde)
        {
            int msg_len;
            byte[] msgOut = null;
            msg_len = PRTHeader.Length + TransactionResponde.Length + 2;
            msgOut = new byte[msg_len];
            msgOut[0] = (byte)(msg_len / 256);
            msgOut[1] = (byte)(msg_len % 256);

            Array.Copy(PRTHeader, 0, msgOut, 2, PRTHeader.Length);
            Array.Copy(TransactionResponde, 0, msgOut, 14, TransactionResponde.Length);
            return msgOut;
        }

        private byte[] ConstructPRTHeader(OrchPipeComponent.ComponentState State)
        {
            byte[] prtHeader = new byte[12];

            string MaquinaOrigem = State.OrchWrkData.GetWrkData().ReadNodeValue("MaquinaOrigem");
            string MaquinaDestino = State.OrchWrkData.GetWrkData().ReadNodeValue("MaquinaDestino");
            string CodigoAplicacao = State.OrchWrkData.GetWrkData().ReadNodeValue("CodigoAplicacao");

            string NumeroTerminalLogico = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroTerminalLogico");
            string NumeroSequenciaMsg = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroSequenciaMsg");

            Array.Copy(Encoding.Default.GetBytes(MaquinaOrigem), prtHeader,3);
            Array.Copy(Encoding.Default.GetBytes(MaquinaDestino), 0, prtHeader, 3, 3);

            Array.Copy(Encoding.Default.GetBytes(CodigoAplicacao), 0, prtHeader, 6, 2);
            prtHeader[8]=0;
            Array.Copy(Encoding.Default.GetBytes(NumeroTerminalLogico), 0, prtHeader, 9, 1);
            Array.Copy(Encoding.Default.GetBytes(NumeroSequenciaMsg), 0, prtHeader, 10, 2);

            return prtHeader;
        }
    }
}
