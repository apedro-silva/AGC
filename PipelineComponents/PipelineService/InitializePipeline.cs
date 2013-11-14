using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// Initialize Pipeline
    /// </summary>
    public class InitializePipeline : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            State.ResetLastError();

            // PRTMsg = Header(14 bytes) + Transaction
            byte[] PRTMsg = (byte[])State.OrchWrkData.GetFromObjBucket("PRTClientRequest");
            byte[] TransactionMsg = new byte[PRTMsg.Length - 14];

            // Get Transaction message
            Array.Copy(PRTMsg, 14, TransactionMsg, 0, PRTMsg.Length-14);

            // Set message for Message parser
            string ola = Encoding.Default.GetString(TransactionMsg);
            State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, TransactionMsg);

            // Set MessageName for Parser
            string PRTCodMsg = Encoding.Default.GetString(TransactionMsg, 0, 4);
            State.OrchWrkData.GetWrkData().WriteNodeValue("PRTCodMsg", "PRT_" + PRTCodMsg, true);

            // Set CodMsg for Journalization
            State.OrchWrkData.GetWrkData().WriteNodeValue("CodMsg", PRTCodMsg, true);

            ParsePRTHeader(State, PRTMsg);
        }
        private void ParsePRTHeader(OrchPipeComponent.ComponentState State, byte[] requestMsg)
        {
            State.OrchWrkData.GetWrkData().WriteNodeValue("ComprimentoMsgEMIS", Encoding.Default.GetString(requestMsg, 0, 2), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("MaquinaOrigem", Encoding.Default.GetString(requestMsg, 2, 3), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("MaquinaDestino", Encoding.Default.GetString(requestMsg, 5, 3), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoAplicacao", Encoding.Default.GetString(requestMsg, 8, 2), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("TipoMensagem", Encoding.Default.GetString(requestMsg, 10, 1), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroTerminalLogico", Encoding.Default.GetString(requestMsg, 11, 1), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroSequenciaMsg", Encoding.Default.GetString(requestMsg, 12, 2), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("EMISRequestMessage", Encoding.Default.GetString(requestMsg, 14, requestMsg.Length-14), true);
        }
    }
}
