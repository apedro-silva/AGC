using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SoftFinanca.BESA
{
    public class ConsultaIBAN
    {
        public ConsultaIBAN(OrchPipeComponent.ComponentState State)
        {
            if (State.IsInError)
                return;
            GetIBANShortName(State);
        }
        private void GetIBANShortName(OrchPipeComponent.ComponentState State)
        {
            try
            {
                string Conta = State.OrchWrkData.GetWrkData().ReadNodeValue("Conta", true);

                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("ObtemIBANNome");
                db.AddInParameter(dbCommand, "NumeroConta", DbType.String, Conta);

                DataSet srvDS = db.ExecuteDataSet(dbCommand);

                using (DataTable trnParameters = srvDS.Tables[0])
                {
                    if (trnParameters.Rows.Count == 1)
                    {
                        DataRow dr = trnParameters.Rows[0];

                        string IBAN = dr[0].ToString();
                        string NomeConta = dr[1].ToString();

                        State.OrchWrkData.GetWrkData().WriteNodeValue("IBAN", IBAN, true);
                        State.OrchWrkData.GetWrkData().WriteNodeValue("NomeTitular", NomeConta.PadRight(40, ' '), true);
                        State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "0", true);
                    }
                    else throw new BusinessException("Conta não suportada!");
                }
            }
            catch (Exception exp)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "4", true);
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

    }
}
