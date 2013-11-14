using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SF.Expand.Switch.PipelineComponents
{
    public class FinalizeEMISFileLog : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            return;

            //DoEMISFileLog(State, Params);
        }

        private void DoEMISFileLog(ComponentState State, string[] Params)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("ActualizaJeFicheiroEMIS");
                string JE = State.OrchWrkData.GetWrkData().ReadNodeValue(Params[0], true);
                string CodResp = State.OrchWrkData.GetWrkData().ReadNodeValue("CodResp", true);

                db.AddInParameter(dbCommand, "JE", DbType.Int32, JE);
                db.AddInParameter(dbCommand, "CodResp", DbType.String, CodResp);
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                throw new BusinessException("ElectronicJournal:FinalizeEMISFileLog.DoEMISFileLog Exception->"+exp.Message);
            }
        }
    }
}
