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
    /// Get PRT Status
    /// </summary>
    public class GetPRTStatus : OrchPipeComponent
    {

        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            GetStatus(State);

        }
        private void GetStatus(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");

                DbCommand dbCommand = db.GetStoredProcCommand("ObtemEstadoPRT");
                db.AddOutParameter(dbCommand, "Estado", DbType.String, 3);
                db.ExecuteNonQuery(dbCommand);
                State.OrchWrkData.GetWrkData().WriteNodeValue("PRTStatus", dbCommand.Parameters[0].Value.ToString(), true);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("PipelineService:GetPRTStatus", exp);
            }
        }
    }
}
