using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    public class ECHOService : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            // Create record log
            InitializeJE initializeJE = new InitializeJE();
            initializeJE.RunComponent(OrchWrkData, new string[] { "JE" });

            // Get service parameters
            GetParameters getParameters = new GetParameters();
            getParameters.RunComponent(OrchWrkData, new string[] { });

            // Construct host message
            MessageConstructor messageConstructor = new MessageConstructor();
            messageConstructor.RunComponent(OrchWrkData, new string[] { "#uv#FlexCubeConstructor", "FlexCubeRequest" });

            // Call Host
            CallHost callHost = new CallHost();
            callHost.RunComponent(OrchWrkData, new string[] { });

            // Parse host response
            MessageParser messageParser = new MessageParser();
            messageParser.RunComponent(OrchWrkData, new string[] { "#uv#FlexCubeParser" });

            // Check PRT Status
            GetPRTStatus getPRTStatus = new GetPRTStatus();
            getPRTStatus.RunComponent(OrchWrkData, new string[] { });

            // Update record log
            FinalizeJE finalizeJE = new FinalizeJE();
            finalizeJE.RunComponent(OrchWrkData, new string[] { "JE" });
        }
    }
}
