using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    public class ExecuteDRCCFile :OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            string CodigoMoedaConta = string.Empty;
            Boolean doGetParameters = true;
            string RubricaContabilistica=string.Empty;
            OrchPipeComponent.ComponentState State = new OrchPipeComponent.ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;

            try
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "EB", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);

                InitializeJE initJE = new InitializeJE();
                initJE.RunComponent(OrchWrkData, new string[] { "#JEDRCC" });

                while (GetDRCCRecord(State))
                {
                    RubricaContabilistica = string.Empty;
                    CodigoMoedaConta = State.OrchWrkData.GetWrkData().ReadNodeValue("CCYCode", true);
                    if (CodigoMoedaConta != null)
                        RubricaContabilistica = "DRCC" + CodigoMoedaConta;

                    DoFundsTransfer(State, RubricaContabilistica, "Montante2", doGetParameters);
                    UpdateDRCCRecord(State);
                    doGetParameters = false;
                }

                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
                FinalizeFileJE endJE = new FinalizeFileJE();
                endJE.RunComponent(OrchWrkData, new string[] { "#JEDRCC", "EB" });
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("DRCC.ExecuteDRCCFile", exp);
            }
        }

        private void CalculateTotalDRCC(ComponentState State)
        {
            string MontanteConta = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2", true);
            string MontanteTotal = State.OrchWrkData.GetWrkData().ReadNodeValue("MontanteTotalDRCC", true);
            decimal decMontante = Convert.ToDecimal(MontanteConta) + Convert.ToDecimal(MontanteTotal);
            if (decMontante > 0)
                State.OrchWrkData.GetWrkData().WriteNodeValue("MontanteTotalDRCC", decMontante.ToString().PadLeft(13, '0'), true);
        }
        private void UpdateDRCCRecord(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("UpdateDRCCRecord");

                string IdDRCCRecord = State.OrchWrkData.GetWrkData().ReadNodeValue("IdDRCCRecord", true);
                string EstadoRegisto = State.OrchWrkData.GetWrkData().ReadNodeValue("EstadoRegisto", true);
                string IdDRCCResponseCode="0";
                string IdDRCCStatus="1";

                if (EstadoRegisto == null || EstadoRegisto == "") // Não processado
                {
                    IdDRCCResponseCode = "0"; //[Não processado]
                    IdDRCCStatus = "1"; // Importado
                }
                else if (EstadoRegisto == "0") //OK
                {
                    IdDRCCResponseCode = "10"; //Tranferência OK
                    IdDRCCStatus = "3"; // Completo
                }
                else if (EstadoRegisto == "1") // com Erro
                {
                    IdDRCCResponseCode = "6"; //Tranferência com Erro
                    IdDRCCStatus = "1"; // Importado
                }

                db.AddInParameter(dbCommand, "IdDRCCRecord", DbType.Int64, IdDRCCRecord);
                db.AddInParameter(dbCommand, "IdDRCCStatus", DbType.Int16, IdDRCCStatus);
                db.AddInParameter(dbCommand, "IdDRCCResponseCode", DbType.Int16, IdDRCCResponseCode);
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

        private void DoFlexCubeTransaction(OrchPipeComponent.ComponentState State)
        {
            PrepareConstructor prepConst = new PrepareConstructor();
            prepConst.RunComponent(State.OrchWrkData, new string[] { });

            if (!State.IsInError)
            {
                MessageConstructor msgConst = new MessageConstructor();
                msgConst.RunComponent(State.OrchWrkData, new string[] { "#uv#FlexCubeConstructor", "FlexCubeRequest" });

            }
            if (!State.IsInError)
            {
                CallHost callHost = new CallHost();
                callHost.RunComponent(State.OrchWrkData, new string[] { });
            }

            if (!State.IsInError)
            {
                MessageParser msgParser = new MessageParser();
                msgParser.RunComponent(State.OrchWrkData, new string[] { "#uv#FlexCubeParser" });
            }
            if (!State.IsInError)
            {
                string ResponseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
                if (ResponseCode == "00")
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "0", true);
                else
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "1", true);
            }

        }

        private Boolean GetDRCCRecord(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("GetDRCCRecord");

                string DRCCFileId = State.OrchWrkData.GetWrkData().ReadNodeValue("DRCCFileId");
                db.AddInParameter(dbCommand, "DRCCFileId", DbType.String, DRCCFileId);
                db.AddInParameter(dbCommand, "DRCCStatusId", DbType.Int16, 5);

                DataSet srvDS = db.ExecuteDataSet(dbCommand);

                if (srvDS.Tables.Count == 0)
                    return false;

                using (DataTable trnParameters = srvDS.Tables[0])
                {
                    DataRow dr = trnParameters.Rows[0];

                    string IdDRCCRecord = dr[0].ToString();
                    string AccountCreditNum = dr[1].ToString();
                    string CardNum = dr[2].ToString();
                    string AccountNum = dr[3].ToString();
                    string DebitAmountEUR = dr[4].ToString();
                    string CCYCode = dr[5].ToString();
                    string DebitAmount = dr[6].ToString();
                    string CodigoBanco = dr[7].ToString();
                    string CodigoMoeda = dr[8].ToString();

                    State.OrchWrkData.GetWrkData().WriteNodeValue("IdDRCCRecord", IdDRCCRecord, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("AccountCreditNum", AccountCreditNum, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("NumCar", CardNum, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Conta", AccountNum, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("DebitAmountEUR", DebitAmountEUR, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CCYCode", CCYCode, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", DebitAmount.Replace(".", "").Replace(",", ""), true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoMoeda", CodigoMoeda, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoBanco", CodigoBanco, true);
                }
                return true;
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
                return false;
            }
        }

        private void PrepareFundsTransfer(OrchPipeComponent.ComponentState State)
        {
            // prepara Transferencia Bancária Ordenante
            State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "E9", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("TipoTerm", "A", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("DtHora", GetDateHour(), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("CodMoeda", State.OrchWrkData.GetWrkData().ReadNodeValue("CodigoMoeda"), true);
        }
        private void DoFundsTransfer(OrchPipeComponent.ComponentState State, string RubricaContabilistica, string WDValueField, Boolean doGetParameters)
        {
            InitializeJE initJe = new InitializeJE();
            initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });

            PrepareFundsTransfer(State);

            string WDValue = State.OrchWrkData.GetWrkData().ReadNodeValue(WDValueField, true);
            if (WDValue == null || Convert.ToInt32(WDValue) == 0)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Montante Nulo", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "0", true);
                State.LastError = new BusinessException(string.Format("DRCC.ProcessLine:Montante Nulo"));
            }
            if (!State.IsInError && RubricaContabilistica == string.Empty)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Rubrica Contabilistica Inválida", true);
                State.LastError = new BusinessException(string.Format("DRCC.ProcessLine:Rubrica Contabilistica Inválida"));
            }

            if (!State.IsInError)
            {
                GetParameters getParams = new GetParameters();
                getParams.RunComponent(State.OrchWrkData, new string[] { });
            }

            if (!State.IsInError)
            {
                string NumeroCartao = State.OrchWrkData.GetWrkData().ReadNodeValue("NumCar", true);
                NumeroCartao = NumeroCartao.Substring(0, 5) + "********" + NumeroCartao.Substring(13, 2);
                string DescricaoServico = State.OrchWrkData.GetWrkData().ReadNodeValue("DescricaoServico", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("DescricaoServico", DescricaoServico.Replace("%1", NumeroCartao), true);

                // Para FlexCube só existe um Montante
                State.OrchWrkData.GetWrkData().WriteNodeValue("Montante", WDValue, true);
            }

            if (!State.IsInError)
            {
                if (State.OrchWrkData.GetWrkData().GetNodeByName("ContaCredito" + RubricaContabilistica) == null)
                    SetHostAccount(State, RubricaContabilistica, "ContaCredito" + RubricaContabilistica, "ContaDebito" + RubricaContabilistica);
            }

            if (!State.IsInError)
            {
                string NumeroContaCredito = State.OrchWrkData.GetWrkData().ReadNodeValue("ContaCredito" + RubricaContabilistica, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCredito", NumeroContaCredito, true);

                string NumeroContaDebito = State.OrchWrkData.GetWrkData().ReadNodeValue("ContaDebito" + RubricaContabilistica, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebito", NumeroContaDebito, true);

                DoFlexCubeTransaction(State);
            }

            FinalizeJE finJE = new FinalizeJE();
            finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
        }

        private void SetHostAccount(OrchPipeComponent.ComponentState State, string RubricaContabilistica, string ContaCredito, string ContaDebito)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCommand = db.GetStoredProcCommand("ObtemRubricaContabilistica");
            db.AddInParameter(dbCommand, "RubricaContabilistica", DbType.String, RubricaContabilistica);

            DataSet srvDS = db.ExecuteDataSet(dbCommand);

            using (DataTable trnParameters = srvDS.Tables[0])
            {
                if (trnParameters.Rows.Count == 0)
                {
                    State.LastError = new BusinessException(string.Format("DRCC.ProcessLine:SetHostAccount() Undefined Account Information {0}", RubricaContabilistica));
                    return;
                }
                DataRow dr = trnParameters.Rows[0];

                string Rubrica = dr[0].ToString();
                string Descricao = dr[1].ToString();
                string NumeroContaDebito = dr[2].ToString();
                string NumeroContaCredito = dr[3].ToString();

                if (ContaCredito != null)
                    State.OrchWrkData.GetWrkData().WriteNodeValue(ContaCredito, NumeroContaCredito, true);
                if (ContaDebito != null)
                    State.OrchWrkData.GetWrkData().WriteNodeValue(ContaDebito, NumeroContaDebito, true);
            }
        }

        private string GetDateHour()
        {
            DateTime dt = DateTime.Now;

            string DataHora = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            return DataHora;
        }
    }
}
