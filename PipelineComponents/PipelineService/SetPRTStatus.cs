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
    /// Set PRT Status
    /// </summary>
    public class SetPRTStatus : OrchPipeComponent
    {

        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;

            UpdatePRTStatus(State);

        }
        private void UpdatePRTStatus(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");

                DbCommand dbCommand = db.GetStoredProcCommand("ActualizaEstadoPRT");
                db.AddInParameter(dbCommand, "Estado", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("PRTStatus", true));
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("PipelineService:SetPRTStatus", exp);
            }
        }
    }
}
