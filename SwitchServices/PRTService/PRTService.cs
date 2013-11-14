using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Switch.PipelineDecision;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    public class PRTService : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            // Initialize Service parameters
            InitializePipeline initialize = new InitializePipeline();
            initialize.RunComponent(OrchWrkData, new string[] { });

            // Create record log
            InitializeJE initializeJE = new InitializeJE();
            initializeJE.RunComponent(OrchWrkData, new string[] { "JE" });

            // Parse service message
            MessageParser messageParser = new MessageParser();
            messageParser.RunComponent(OrchWrkData, new string[] { "#uv#PRTCodMsg" });

            // Get service parameters
            GetParameters getParameters = new GetParameters();
            getParameters.RunComponent(OrchWrkData, new string[] {  });

            // create component for message construction
            MessageConstructor messageConstructor = new MessageConstructor();

            // Check card 
            CheckCard checkCard = new CheckCard();
            if (checkCard.RunDecision(OrchWrkData, new string[] { }))
            {
                // Prepare for host message construction
                PrepareConstructor prepareConstructor = new PrepareConstructor();
                prepareConstructor.RunComponent(OrchWrkData, new string[] { });

                // Construct host message
                messageConstructor.RunComponent(OrchWrkData, new string[] { "#uv#FlexCubeConstructor", "FlexCubeRequest" });

                // Call Host
                CallHost callHost = new CallHost();
                callHost.RunComponent(OrchWrkData, new string[] { });

                // Parse host response
                messageParser.RunComponent(OrchWrkData, new string[] { "#uv#FlexCubeParser" });
            }
            // Prepare for service message construction
            PrepareResponse prepareResponse = new PrepareResponse();
            prepareResponse.RunComponent(OrchWrkData, new string[] { });

            // Construct service response
            messageConstructor.RunComponent(OrchWrkData, new string[] { "#uv#PRTConstructor", "PRTResponse" });

            // Finalize service flow
            Finalize finalize = new Finalize();
            finalize.RunComponent(OrchWrkData, new string[] { });

            // Update record log
            FinalizeJE finalizeJE = new FinalizeJE();
            finalizeJE.RunComponent(OrchWrkData, new string[] { "JE" });
        }
    }
}
