using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;

namespace SF.Expand.Switch.PipelineComponents
{
    public class DoNothing : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {

        }
    }
}
