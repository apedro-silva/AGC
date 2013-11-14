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
    public class InitializeEMISFileLog : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            DoEMISFileLog(State);
        }

        private void DoEMISFileLog(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("CriaJeFicheiroEMIS");
                string Aplicacao = State.OrchWrkData.GetWrkData().ReadNodeValue("Aplic", true);
                string Ficheiro = State.OrchWrkData.GetWrkData().ReadNodeValue("Fich", true);
                string VersaoFicheiro = State.OrchWrkData.GetWrkData().ReadNodeValue("VerFich", true);
                string CodigoBanco = State.OrchWrkData.GetWrkData().ReadNodeValue("CodBan", true);
                string CPD = State.OrchWrkData.GetWrkData().ReadNodeValue("CPD", true);
                string DataFicheiro = State.OrchWrkData.GetWrkData().ReadNodeValue("DataFich", true);
                string SequenciaFicheiro = State.OrchWrkData.GetWrkData().ReadNodeValue("SeqFich", true);
                string DataFicheiroAnterior = State.OrchWrkData.GetWrkData().ReadNodeValue("DataFichAnt", true);
                string SequenciaFicheiroAnterior = State.OrchWrkData.GetWrkData().ReadNodeValue("SeqFichAnt", true);
                string DataValor= State.OrchWrkData.GetWrkData().ReadNodeValue("DataValor", true);
                string CodigoMoeda = State.OrchWrkData.GetWrkData().ReadNodeValue("CodMoeda", true);
                string TotalRegistos = State.OrchWrkData.GetWrkData().ReadNodeValue("TotReg", true);
                string TotalDebito = State.OrchWrkData.GetWrkData().ReadNodeValue("TotDeb", true);
                string TotalCredito = State.OrchWrkData.GetWrkData().ReadNodeValue("TotCred", true);
                string Header = State.OrchWrkData.GetWrkData().ReadNodeValue("EMISFileHeader", true);
                string Trailler = State.OrchWrkData.GetWrkData().ReadNodeValue("EMISFileTrailler", true);

                db.AddInParameter(dbCommand, "JE", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue("JE"));
                db.AddInParameter(dbCommand, "Aplicacao", DbType.String, Aplicacao);
                db.AddInParameter(dbCommand, "Ficheiro", DbType.String, Ficheiro);
                db.AddInParameter(dbCommand, "VersaoFicheiro", DbType.String, VersaoFicheiro);
                db.AddInParameter(dbCommand, "CodigoBanco", DbType.String, CodigoBanco);
                db.AddInParameter(dbCommand, "CPD", DbType.String, CPD);

                db.AddInParameter(dbCommand, "DataFicheiro", DbType.String, DataFicheiro);
                db.AddInParameter(dbCommand, "SequenciaFicheiro", DbType.String, SequenciaFicheiro);
                db.AddInParameter(dbCommand, "DataFicheiroAnterior", DbType.String, DataFicheiroAnterior);
                db.AddInParameter(dbCommand, "SequenciaFicheiroAnterior", DbType.String, SequenciaFicheiroAnterior);
                db.AddInParameter(dbCommand, "DataValor", DbType.String, DataValor);
                db.AddInParameter(dbCommand, "CodigoMoeda", DbType.String, CodigoMoeda);
                db.AddInParameter(dbCommand, "TotalRegistos", DbType.String, TotalRegistos);
                db.AddInParameter(dbCommand, "TotalDebito", DbType.String, TotalDebito);
                db.AddInParameter(dbCommand, "TotalCredito", DbType.String, TotalCredito);
                db.AddInParameter(dbCommand, "Header", DbType.String, Header);
                db.AddInParameter(dbCommand, "Trailler", DbType.String, Trailler);
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                throw new BusinessException("ElectronicJournal:InitializeEMISFileLog.DoEMISFileLog Exception->" + exp.Message);
            }
        }
    }
}
