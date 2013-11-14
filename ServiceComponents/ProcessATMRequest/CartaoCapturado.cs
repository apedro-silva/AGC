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
    public class CartaoCapturado
    {
        public CartaoCapturado(OrchPipeComponent.ComponentState State)
        {
            if (State.IsInError)
                return;
            try
            {
                string MotCapt = State.OrchWrkData.GetWrkData().ReadNodeValue("MotCapt", true);
                string NumCartao = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroCartao", true);
                string DataHoraCaptura = State.OrchWrkData.GetWrkData().ReadNodeValue("DtHora", true);
                string LocalCaptura = State.OrchWrkData.GetWrkData().ReadNodeValue("LocTerm", true);
                int RowsAffected = ChangeCardCapturedState(NumCartao, MotCapt, LocalCaptura, DataHoraCaptura);
                if (RowsAffected == 0)
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Cartão não existe no AGC", true);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }
        private int ChangeCardCapturedState(string NumCartao, string MotivoCaptura, string LocalCaptura, string DataHoraCaptura)
        {
            try
            {
                int RowsAffected = 0;
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("AlteraSituacaoCartaoCapturado");
                db.AddInParameter(dbCommand, "NumeroCartao", DbType.String, NumCartao);
                db.AddInParameter(dbCommand, "SituacaoCartaoCapturado", DbType.Int16, 0);
                db.AddInParameter(dbCommand, "Motivo", DbType.String, MotivoCaptura);
                db.AddInParameter(dbCommand, "Local", DbType.String, LocalCaptura);
                db.AddInParameter(dbCommand, "DataHoraCaptura", DbType.DateTime, string.Format("{0}-{1}-{2} {3}:{4}:{5}", DataHoraCaptura.Substring(0, 4), DataHoraCaptura.Substring(4, 2), DataHoraCaptura.Substring(6, 2), DataHoraCaptura.Substring(8, 2), DataHoraCaptura.Substring(10, 2), DataHoraCaptura.Substring(12, 2)));
                db.AddOutParameter(dbCommand, "RegistosProcessados", DbType.Int16, 0);
                int rowsAffected = db.ExecuteNonQuery(dbCommand);
                Object objRowsAffected = db.GetParameterValue(dbCommand, "RegistosProcessados");
                if (objRowsAffected != System.DBNull.Value)
                    RowsAffected = (System.Int16)objRowsAffected;
                return RowsAffected;
            }
            catch (Exception exp)
            {
                throw new BusinessException("ChangeCardCapturedState", exp);
            }
        }



    }
}
