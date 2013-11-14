using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.PipelineComponents
{
    public class FinalizeFileJE : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            FinalizeFileElectronicJournal(State, Params);
        }

        private void FinalizeFileElectronicJournal(OrchPipeComponent.ComponentState State, string[] Params)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCommand = db.GetStoredProcCommand("ActualizaJE");
            string JEIdName = Params[0];
            string CodTrn=string.Empty;

            if (Params.Length == 2)
                CodTrn = Params[1];
            else
                CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn");

            try
            {
                db.AddInParameter(dbCommand, "JE", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue(JEIdName));
                db.AddInParameter(dbCommand, "Servico", DbType.String, CodTrn);

                db.AddInParameter(dbCommand, "Estado", DbType.Int16, 1);
                db.AddInParameter(dbCommand, "CodigoErro", DbType.Int16, State.OrchWrkData.GetWrkData().ReadNodeValue("Erro"));
                db.AddInParameter(dbCommand, "TextoErro", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("TextoErro"));
                db.AddInParameter(dbCommand, "CodResp", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CodResp",true));
                db.AddInParameter(dbCommand, "TipoServico", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("TipoServico", true));
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("ElectronicJournal:FinalizeFileElectronicJournal", exp);
            }
        }
    }
}
