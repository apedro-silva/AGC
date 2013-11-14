using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Threading;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.PipelineComponents
{
    public class InitFiles2EmisLog : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            State.ResetLastError();

            try
            {
                InitializeJE initJe = new InitializeJE();
                State.OrchWrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
                initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });
            }
            catch (Exception)
            {
            }
        }

    }
}
