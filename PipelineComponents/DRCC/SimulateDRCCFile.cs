using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.Xml;
using System.Globalization;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    public class SimulateDRCCFile : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new OrchPipeComponent.ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;

            try
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "EA", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);

                InitializeJE initJE = new InitializeJE();
                initJE.RunComponent(OrchWrkData, new string[] { "#JEDRCC" });

                while (GetDRCCRecord(State))
                {
                    PrepareFundsTransfer(State);
                    DoFlexCubeTransaction(State);
                    UpdateDRCCRecord(State);
                }
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("DRCC.SimulateDRCCFile", exp);
            }
            FinalizeFileJE endJE = new FinalizeFileJE();
            endJE.RunComponent(OrchWrkData, new string[] { "#JEDRCC", "EA" });
        }

        private void ValidateAccountBalance(OrchPipeComponent.ComponentState State)
        {
            try
            {
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

                string Montante = State.OrchWrkData.GetWrkData().ReadNodeValue("MontanteCliente", true);
                string SaldoDisponivel = State.OrchWrkData.GetWrkData().ReadNodeValue("NetAvailableBalance", true);
                string SinalSaldoDisponivel = State.OrchWrkData.GetWrkData().ReadNodeValue("NetAvailableBalanceSign", true);
                string MoedaSaldoDisponivel = State.OrchWrkData.GetWrkData().ReadNodeValue("NetAvailableBalanceCurrency", true);
                string MoedaDRCC = State.OrchWrkData.GetWrkData().ReadNodeValue("CodigoMoedaDRCC", true);
                string DRCCRate = State.OrchWrkData.GetWrkData().ReadNodeValue("DRCCRate", true);

                if (SinalSaldoDisponivel == "D")
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "S1", true); // Saldo Insuficiente
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Saldo Insuficiente", true); // Saldo Insuficiente
                    return;
                }
                decimal debitValue = Convert.ToDecimal(Montante.Insert(Montante.Length - 2, "."), nfi);
                decimal finalAccountBalance = Convert.ToDecimal(SaldoDisponivel.Insert(SaldoDisponivel.Length - 2, "."), nfi);


                if (finalAccountBalance < debitValue)
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "S1", true); // Saldo Insuficiente
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Saldo Insuficiente", true); // Saldo Insuficiente
                }
                else
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "S3", true); // Saldo Insuficiente
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Saldo OK", true); // Saldo OK
                }
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

        private void UpdateDRCCRecord(ComponentState State)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCommand = db.GetStoredProcCommand("UpdateDRCCRecord");

            string IdDRCCRecord = State.OrchWrkData.GetWrkData().ReadNodeValue("IdDRCCRecord");
            string IdDRCCStatus = "1";
            string IdDRCCResponseCode = "0";

            string EstadoRegisto = State.OrchWrkData.GetWrkData().ReadNodeValue("EstadoRegisto", true);
            if (EstadoRegisto == null || EstadoRegisto == "")
                IdDRCCResponseCode = "0";
            else if (EstadoRegisto == "S1")
                IdDRCCResponseCode = "2";
            else if (EstadoRegisto == "S3")
                IdDRCCResponseCode = "1";
            else if (EstadoRegisto != "1")
                IdDRCCResponseCode = "11";

            db.AddInParameter(dbCommand, "IdDRCCRecord", DbType.Int64, IdDRCCRecord);
            db.AddInParameter(dbCommand, "IdDRCCStatus", DbType.Int16, IdDRCCStatus);
            db.AddInParameter(dbCommand, "IdDRCCResponseCode", DbType.Int16, IdDRCCResponseCode);
            db.ExecuteNonQuery(dbCommand);
        }

        private void DoFlexCubeTransaction(OrchPipeComponent.ComponentState State)
        {
            InitializeJE initJe = new InitializeJE();
            initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });
            if (!State.IsInError)
            {
                string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
                GetParameters getParams = new GetParameters();
                getParams.RunComponent(State.OrchWrkData, new string[] { CodTrn });
            }
            if (!State.IsInError)
            {
                PrepareConstructor prepConst = new PrepareConstructor();
                prepConst.RunComponent(State.OrchWrkData, new string[] { });

            }
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
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "0", true);
                    ProcessAdditionalAmounts(State);
                    ValidateAccountBalance(State);
                }
                else
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "1", true);
            }

            FinalizeJE finJE = new FinalizeJE();
            finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
        }

        private Boolean GetDRCCRecord(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("GetDRCCRecord");

                string DRCCFileId = State.OrchWrkData.GetWrkData().ReadNodeValue("DRCCFileId");
                db.AddInParameter(dbCommand, "DRCCFileId", DbType.String, DRCCFileId);
                db.AddInParameter(dbCommand, "DRCCStatusId", DbType.Int16, 4);

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
                    string DRCCRate = dr[9].ToString();

                    State.OrchWrkData.GetWrkData().WriteNodeValue("IdDRCCRecord", IdDRCCRecord, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("AccountCreditNum", AccountCreditNum, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("NumCar", CardNum, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Conta", AccountNum, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("DebitAmountEUR", DebitAmountEUR, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CCYCode", CCYCode, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("MontanteCliente", DebitAmount.Replace(".", "").Replace(",", ""), true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", "000000000000", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoMoeda", CodigoMoeda, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoBanco", CodigoBanco, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("DRCCRate", DRCCRate, true);
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
            // prepara Consulta de Saldos DRCC
            State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "DS", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("TipoTerm", "A", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("CodMoeda", State.OrchWrkData.GetWrkData().ReadNodeValue("CodigoMoeda"), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("DtHora", GetDateHour(), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
        }
        private string GetDateHour()
        {
            DateTime dt = DateTime.Now;

            string DataHora = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            return DataHora;
        }
        private void ProcessAdditionalAmounts(ComponentState State)
        {
            string AdditionalAmounts = State.OrchWrkData.GetWrkData().ReadNodeValue("fc-AdditionalAmounts", true);
            if (AdditionalAmounts == null)
                return;
            ParseMessageField(State, AdditionalAmounts, "FC_Amounts_PARSE");

            WD2EMISField(State, "DHMsg", 0, 8, "DataSaldo", false);

            WD2EMISField(State, "NetAvailableBalance", "SaldoDisp", 13, false, false);
            WD2EMISField(State, "NetAvailableBalanceCurrency", "MoedaSaldoDisp", 3, false, false);
            WD2EMISField(State, "NetAvailableBalanceSign", "SinalSaldoDisp", false);

            WD2EMISField(State, "LedgerBalance", "SaldoCont", 13, false, false);
            WD2EMISField(State, "LedgerBalanceCurrency", "MoedaSaldoCont", 3, false, false);
            WD2EMISField(State, "LedgerBalanceSign", "SinalSaldoCont", false);
        }

        private void WD2EMISField(OrchPipeComponent.ComponentState State, string WDFieldName, string EMISFieldName, bool Mandatory)
        {
            if (Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                throw new BusinessException("Mandatory Field <" + WDFieldName + "> not present on FlexCube message");

            if (!Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                return;

            string wdField = State.OrchWrkData.GetWrkData().ReadNodeValue(WDFieldName);
            State.OrchWrkData.GetWrkData().WriteNodeValue(EMISFieldName, wdField, true);
        }

        private void WD2EMISField(OrchPipeComponent.ComponentState State, string WDFieldName, int srcStartIndex, int lenToCopy, string EMISFieldName, bool Mandatory)
        {
            if (Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                throw new BusinessException("Mandatory Field <" + WDFieldName + "> not present on PRT message");

            if (!Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                return;

            string wdField = State.OrchWrkData.GetWrkData().ReadNodeValue(WDFieldName);
            State.OrchWrkData.GetWrkData().WriteNodeValue(EMISFieldName, wdField.Substring(srcStartIndex, lenToCopy), true);
        }

        private void WD2EMISField(OrchPipeComponent.ComponentState State, string WDFieldName, string EMISFieldName, int destSize, bool IsString, bool Mandatory)
        {
            if (Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                throw new BusinessException("Mandatory Field <" + WDFieldName + "> not present on FlexCube message");

            if (!Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                return;

            string wdField = State.OrchWrkData.GetWrkData().ReadNodeValue(WDFieldName);
            if (wdField.Length > destSize)
                wdField = wdField.Substring(wdField.Length - destSize);
            else
            {
                if (IsString)
                    wdField = wdField.PadRight(destSize, ' ');
                else
                    wdField = wdField.PadLeft(destSize, '0');

            }
            State.OrchWrkData.GetWrkData().WriteNodeValue(EMISFieldName, wdField, true);
        }
        private void ParseMessageField(ComponentState State, string MessageField, string ParseMessageName)
        {
            // Set message for Message parser
            State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, Encoding.ASCII.GetBytes(MessageField));
            MessageParser msgParser = new MessageParser();
            msgParser.RunComponent(State.OrchWrkData, new string[] { "#c#" + ParseMessageName });
        }

    }
}
