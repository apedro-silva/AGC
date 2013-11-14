using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using SF.Expand.Core.Data;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.PipelineComponents
{
    public class InitializeClearing : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            State.ResetLastError();

            try
            {
                string FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("InputFileName");
                StreamReader sr = new StreamReader(FileName, Encoding.ASCII);
                String line = sr.ReadLine();
                State.OrchWrkData.AddToObjBucket("FileStreamReader", sr);
                GetSWITCHConfiguration(State);
                ValidateEMISFile(State, line);

                CheckHeader(State, line);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
            }
            finally
            {
                FinalizeFileJE finJE = new FinalizeFileJE();
                finJE.RunComponent(State.OrchWrkData, new string[] { "JEFicheiroEMIS" });
            }
        }

        private void CheckHeader(OrchPipeComponent.ComponentState State, string line)
        {
            string CodigoBanco = State.OrchWrkData.GetWrkData().ReadNodeValue("CodigoBanco");
            string FICH = line.Substring(2, 4);
            string CODBAN = line.Substring(8, 4);
            string CODMOEDA = line.Substring(43, 3);
            string PipelineID = State.OrchWrkData.GetFromStringsBucket(OrchestratorDefs.WRKVAR_PID);

            if (FICH != PipelineID)
                throw new BusinessException(string.Format("CheckHeader->Invalid FICH <{0}>!", CODBAN));
            if (CODBAN != CodigoBanco)
                throw new BusinessException(string.Format("CheckHeader->Invalid Bank Code <{0}>!", CODBAN));

            State.OrchWrkData.GetWrkData().WriteNodeValue("CodMoeda", CODMOEDA, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("DtHora", GetDateHour(), true);
        }

        private string GetDateHour()
        {
            DateTime dt = DateTime.Now;

            string DataHora = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dt.Year,dt.Month,dt.Day,dt.Hour,dt.Minute,dt.Second);
            return DataHora;
        }

        private void GetSWITCHConfiguration(OrchPipeComponent.ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("ObtemConfiguracaoSWITCH");
                db.AddOutParameter(dbCommand, "CodigoBanco", DbType.String,4);
                db.AddOutParameter(dbCommand, "CodigoMoeda", DbType.String, 3);
                db.AddOutParameter(dbCommand, "CodigoTerminal", DbType.String, 10);

                db.ExecuteNonQuery(dbCommand);
                string CodigoBanco = db.GetParameterValue(dbCommand, "CodigoBanco").ToString();
                State.OrchWrkData.GetWrkData().AppendNode("CodigoBanco", Types.String, CodigoBanco);
                string CodigoMoeda = db.GetParameterValue(dbCommand, "CodigoMoeda").ToString();
                State.OrchWrkData.GetWrkData().AppendNode("CodigoMoeda", Types.String, CodigoMoeda);
                string CodigoTerminal = db.GetParameterValue(dbCommand, "CodigoTerminal").ToString();
                State.OrchWrkData.GetWrkData().AppendNode("CodigoTerminal", Types.String, CodigoTerminal);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }
        private void ValidateEMISFile(ComponentState State, string line)
        {
            Database db = DatabaseFactory.CreateDatabase("BESASwitch");
            string aplicacao = line.Substring(1, 1);
            string ficheiro = line.Substring(2, 4);
            string versaoFicheiro = line.Substring(6, 2);
            string codigoBanco = line.Substring(8, 4);
            string CPD = line.Substring(12, 1);
            string IdFicheiro = line.Substring(13, 11);
            string IdFicheiroAnterior = line.Substring(24, 11);
            string dataValor = line.Substring(35, 8);
            string codigoMoeda = line.Substring(43, 3);

            State.OrchWrkData.GetWrkData().WriteNodeValue("DataFicheiroEMIS", IdFicheiro.Substring(0, 10), true);

            DbCommand dbCommand = db.GetStoredProcCommand("ValidaFicheiroEMIS");
            db.AddInParameter(dbCommand, "Aplicacao", DbType.String, aplicacao);
            db.AddInParameter(dbCommand, "Ficheiro", DbType.String, ficheiro);
            db.AddInParameter(dbCommand, "VersaoFicheiro", DbType.String, versaoFicheiro);
            db.AddInParameter(dbCommand, "CodigoBanco", DbType.String, codigoBanco);
            db.AddInParameter(dbCommand, "CPD", DbType.String, CPD);
            db.AddInParameter(dbCommand, "IdFicheiro", DbType.String, IdFicheiro);
            db.AddInParameter(dbCommand, "IdFicheiroAnterior", DbType.String, IdFicheiroAnterior);
            db.AddInParameter(dbCommand, "DataValor", DbType.String, dataValor);
            db.AddInParameter(dbCommand, "CodigoMoeda", DbType.String, codigoMoeda);
            db.AddOutParameter(dbCommand, "Estado", DbType.Int32, 2);
            db.AddOutParameter(dbCommand, "JEFicheiroEMIS", DbType.Int32, 2);
            db.ExecuteNonQuery(dbCommand);

            string JE = db.GetParameterValue(dbCommand, "JEFicheiroEMIS").ToString();
            State.OrchWrkData.GetWrkData().WriteNodeValue("JEFicheiroEMIS", JE, true);

            int estado = (int)db.GetParameterValue(dbCommand, "Estado");
            if (estado == 1)
                throw new BusinessException("Ficheiro já tratado!");
            else if (estado == 2)
                throw new BusinessException("Ficheiro anterior ainda não tratado!");
        }


    }
}
