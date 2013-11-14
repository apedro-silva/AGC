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
    /// Statistics
    /// </summary>
    public class Statistics : OrchPipeComponent
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

            UpdateStatistics(State);
        }

        private void UpdateStatistics(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);

                DbCommand dbCommand = db.GetStoredProcCommand("ActualizaEstatisticas");
                db.AddInParameter(dbCommand, "NumeroConta", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("Conta", true));

                if (CodTrn == "33")
                {
                    db.AddInParameter(dbCommand, "Comerciante", DbType.String, "1");
                    db.AddInParameter(dbCommand, "NumeroOperacoes", DbType.Int16, State.OrchWrkData.GetWrkData().ReadNodeValue("TotOper", true));
                    string Montante = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante", true);
                    if (Montante != null)
                    {
                        Montante = Montante.PadLeft(13, '0');
                        Montante = Montante.Substring(0, 11) + "." + Montante.Substring(11);
                    }
                    db.AddInParameter(dbCommand, "ValorOperacoes", DbType.Decimal, Montante);
                }
                else
                {
                    db.AddInParameter(dbCommand, "Comerciante", DbType.String, null);
                    db.AddInParameter(dbCommand, "NumeroOperacoes", DbType.Int16, 1);
                    string Montante = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2", true);
                    if (Montante != null)
                    {
                        Montante = Montante.PadLeft(13, '0');
                        Montante = Montante.Substring(0, 11) + "." + Montante.Substring(11);
                    }
                    db.AddInParameter(dbCommand, "ValorOperacoes", DbType.Decimal, Montante);
                }
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("PipelineService:Statistics", exp);
            }
        }

    }
}
