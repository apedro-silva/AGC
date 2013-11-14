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
    public class InitializeJE : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            State.ResetLastError();

            // Perform journalization of request msg
            State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            InitializeElectronicJounal(State, Params[0]);
        }

        private void InitializeElectronicJounal(OrchPipeComponent.ComponentState State, string JEIdName)
        {
            Database db=null;
            DbCommand dbCommand=null;
            try
            {
                string EMISRequest = State.OrchWrkData.GetWrkData().ReadNodeValue("EMISRequestMessage", true);
                if (EMISRequest==null)
                    EMISRequest = State.OrchWrkData.GetWrkData().ReadNodeValue("EMISFileRecord", true);

                db = DatabaseFactory.CreateDatabase("BESASwitch");
                dbCommand = db.GetStoredProcCommand("InicializaJE");

                string PipelineID = State.OrchWrkData.GetFromStringsBucket(OrchestratorDefs.WRKVAR_PID);
                db.AddInParameter(dbCommand, "PipelineID", DbType.String, PipelineID);
                db.AddInParameter(dbCommand, "Estado", DbType.Int16, 0);
                db.AddInParameter(dbCommand, "PedidoPRT", DbType.String, EMISRequest);
                db.AddInParameter(dbCommand, "OnlineBatch", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("OnlineBatch", true));
                db.AddInParameter(dbCommand, "TextoErro", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("TextoErro", true));
                db.AddOutParameter(dbCommand, "JE", DbType.Int32, 8);
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "-1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
            }
            finally
            {
                string JE = db.GetParameterValue(dbCommand, "JE").ToString();
                State.OrchWrkData.GetWrkData().WriteNodeValue(JEIdName, JE, true);
            }
        }
    }
}
