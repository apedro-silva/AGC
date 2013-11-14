using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    public class PRTStatusService : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            // Create record log
            InitializeJE initializeJE = new InitializeJE();
            initializeJE.RunComponent(OrchWrkData, new string[] { "JE" });

            // Set PRT Status
            SetPRTStatus setPrtStatus = new SetPRTStatus();
            setPrtStatus.RunComponent(OrchWrkData, new string[] { });

            // Update record log
            FinalizeJE finalizeJE = new FinalizeJE();
            finalizeJE.RunComponent(OrchWrkData, new string[] { "JE" });
        }
    }
}
