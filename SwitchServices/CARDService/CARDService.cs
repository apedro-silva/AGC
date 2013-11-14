using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    /// <summary>
    /// The CARD Service Switch pipeline component, runs a transaction, 
    /// including communications with the host. 
    /// </summary>
    public class CARDService : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">OrchWrkData</param>
        /// <param name="Params">Params</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            // Create record log
            InitializeJE initializeJE = new InitializeJE();
            initializeJE.RunComponent(OrchWrkData, new string[] { "JE" });

            // Get service parameters
            GetParameters getParameters = new GetParameters();
            getParameters.RunComponent(OrchWrkData, new string[] { });

            // Prepare for host message construction
            PrepareConstructor prepareConstructor = new PrepareConstructor();
            prepareConstructor.RunComponent(OrchWrkData, new string[] { });

            // Construct host message
            MessageConstructor messageConstructor = new MessageConstructor();
            messageConstructor.RunComponent(OrchWrkData, new string[] { "#c#FC_ATM_MSG_CONSTRUCT", "FlexCubeRequest" });

            // Call Host
            CallHost callHost = new CallHost();
            callHost.RunComponent(OrchWrkData, new string[] {  });

            // Parse host response
            MessageParser messageParser = new MessageParser();
            messageParser.RunComponent(OrchWrkData, new string[] { "#c#FC_ATM_MSG_PARSE" });

            // Prepare for service message construction
            PrepareResponse prepareResponse = new PrepareResponse();
            prepareResponse.RunComponent(OrchWrkData, new string[] {  });

            // Construct service response
            messageConstructor.RunComponent(OrchWrkData, new string[] { "#uv#PRTConstructor", "PRTResponse" });

            // Update record log
            FinalizeJE finalizeJE = new FinalizeJE();
            finalizeJE.RunComponent(OrchWrkData, new string[] { "JE" });
        }
    }
}
