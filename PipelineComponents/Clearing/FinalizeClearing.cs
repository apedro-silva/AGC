using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.PipelineComponents
{
    public class FinalizeClearing : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            StreamReader sr = (StreamReader)State.OrchWrkData.GetFromObjBucket("FileStreamReader");
            if (sr!=null)
                sr.Close();
        }
    }
}
