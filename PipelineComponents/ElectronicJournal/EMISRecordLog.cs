using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using SF.Expand.Business;
using SF.Expand.Core.Orch;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// Switch component that writes to JeRegistoEmis Table.
    /// </summary>
    public class EMISRecordLog
    {
        /// <summary>
        /// DoEMISRecordLog
        /// </summary>
        /// <param name="State">The component State</param>
        /// <param name="Params">The component params</param>
        public static void DoEMISRecordLog(OrchPipeComponent.ComponentState State, string[] Params)
        {
            try
            {
                string JEIdName  = Params[0];
                string JeOriginal = null;
                string SinalTransaccao=null;
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("CriaJeRegistoEMIS");
                string TipoRegisto = State.OrchWrkData.GetWrkData().ReadNodeValue("TipReg", true);
                string JEFicheiroEMIS = State.OrchWrkData.GetWrkData().ReadNodeValue("JEFicheiroEMIS", true);
                string DescricaoOperacao = State.OrchWrkData.GetWrkData().ReadNodeValue("DescricaoOperacao", true);

                string NumeroContaDebito = State.OrchWrkData.GetWrkData().ReadNodeValue(Params[2], true);
                string NumeroContaCredito = State.OrchWrkData.GetWrkData().ReadNodeValue(Params[3], true);
                if (Params[4]!=null)
                    SinalTransaccao = State.OrchWrkData.GetWrkData().ReadNodeValue(Params[4], true);
                string Montante = getAmountValue(State.OrchWrkData.GetWrkData().ReadNodeValue(Params[1], true));
                string montPDst = getAmountValue(State.OrchWrkData.GetWrkData().ReadNodeValue("MontPDst", true));
                string montRDst = getAmountValue(State.OrchWrkData.GetWrkData().ReadNodeValue("MontRDst", true));
                string montanteAdicional = getAmountValue(State.OrchWrkData.GetWrkData().ReadNodeValue("MontanteAdicional", true));

                JeOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue(JEIdName, true);

                string RubricaContabilistica = State.OrchWrkData.GetWrkData().ReadNodeValue("RubricaContabilistica", true);
                string Situacao = State.OrchWrkData.GetWrkData().ReadNodeValue("SituacaoRegistoEMIS", true);
                string Registo = State.OrchWrkData.GetWrkData().ReadNodeValue("EMISFileRecord", true);
                string EstadoRegisto = State.OrchWrkData.GetWrkData().ReadNodeValue("EstadoRegisto", true);
                string TipoProcesso = State.OrchWrkData.GetWrkData().ReadNodeValue("TpProcTrn", true);
                string ModoEnvio = State.OrchWrkData.GetWrkData().ReadNodeValue("ModEnvTrn", true);

                db.AddInParameter(dbCommand, "JE", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue(JEIdName));
                db.AddInParameter(dbCommand, "JEFicheiroEMIS", DbType.String, JEFicheiroEMIS);
                db.AddInParameter(dbCommand, "TipoRegisto", DbType.String, TipoRegisto);
                db.AddInParameter(dbCommand, "TipoProcesso", DbType.String, TipoProcesso);
                db.AddInParameter(dbCommand, "ModoEnvio", DbType.String, ModoEnvio);
                db.AddInParameter(dbCommand, "DescricaoOperacao", DbType.String, DescricaoOperacao);
                db.AddInParameter(dbCommand, "NumeroContaDebito", DbType.String, NumeroContaDebito);
                db.AddInParameter(dbCommand, "NumeroContaCredito", DbType.String, NumeroContaCredito);
                db.AddInParameter(dbCommand, "Montante", DbType.String, Montante);
                db.AddInParameter(dbCommand, "MontPDst", DbType.String, montPDst);
                db.AddInParameter(dbCommand, "MontRDst", DbType.String, montRDst);
                db.AddInParameter(dbCommand, "MontanteAdicional", DbType.String, montanteAdicional);

                db.AddInParameter(dbCommand, "RubricaContabilistica", DbType.String, RubricaContabilistica);
                db.AddInParameter(dbCommand, "Situacao", DbType.Int16, Situacao);
                db.AddInParameter(dbCommand, "Estado", DbType.Int16, EstadoRegisto);
                db.AddInParameter(dbCommand, "Registo", DbType.String, Registo);
                db.AddInParameter(dbCommand, "SinalTransaccao", DbType.String, SinalTransaccao);
                db.AddInParameter(dbCommand, "JeOriginal", DbType.Int32, JeOriginal);
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                throw new BusinessException("ElectronicJournal:EMISRecodLog.DoEMISRecordLog Exception->" + exp.Message);
            }
        }

        /// <summary>
        /// UpdateEMISRecordLog
        /// </summary>
        /// <param name="State">The component State</param>
        /// <param name="Params">The component params</param>
        public static void UpdateEMISRecordLog(OrchPipeComponent.ComponentState State, string[] Params)
        {
            try
            {
                string JEIdName = Params[0];
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("ActualizaJeRegistoEMIS");

                db.AddInParameter(dbCommand, "JE", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue(JEIdName));
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                throw new BusinessException("ElectronicJournal:EMISRecodLog.DoEMISRecordLog Exception->" + exp.Message);
            }
        }
        public static void DoSystemLog(string message)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("DoSystemLog");

                db.AddInParameter(dbCommand, "pMessage", DbType.String, message);
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                throw new BusinessException("ElectronicJournal:EMISRecodLog.DoSystemLog Exception->" + exp.Message);
            }
        }


        private static string getAmountValue(string montante)
        {
            if (montante != null)
            {
                montante = montante.PadLeft(13, '0');
                montante = montante.Substring(0, 11) + "." + montante.Substring(11);
            }
            return montante;
        }

        
    }
}
