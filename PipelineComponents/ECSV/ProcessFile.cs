using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.Common;
using System.Data;
using System.Threading;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using SF.Expand.Switch.ServiceComponents;
using SF.Expand.Notification;

namespace SF.Expand.Switch.SwitchServices
{
    /// <summary>
    /// </summary>
    public class ProcessECSVFile : OrchPipeComponent
    {
        /// <summary>
        /// </summary>
        /// <param name="OrchWrkData">email recipients separated by ;</param>
        /// <param name="Params"></param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;

            int index = 1;
            string line = "ProcessECSVFile";

            string FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("InputFileName");
            StreamReader sr = OpenFile2Process(FileName);

            try
            {

                if (sr == null)
                    throw new Exception("Não foi possível ler ficheiro [" + FileName + "]");

                while ((line = sr.ReadLine()) != null)
                {
                    DoParseLine(State, line);
                    InsertAccountYesterdayBalance(State);
                    index++;
                }
                SetAccountYesterdayBalance(State);
                CreateECSV4EMIS();
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro processado", true);
                NotificationService.Send("AccountBalance.Upload", "Ficheiro processado", null);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message + "->" + line, true);
                NotificationService.Send("AccountBalance.Upload", exp.Message + "->" + line, null);

                //Email.Send("apedro.silva@gmail.com", "Processamento do ficheiro de Saldos de Véspera", exp.Message + "->linha " + index);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            return;

        }

        private void CreateECSV4EMIS()
        {
            ECSVtoEMIS yesterdayBalance = new ECSVtoEMIS();
            try
            {
                yesterdayBalance.YesterdayBalanceToEMIS();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private StreamReader OpenFile2Process(string fileName)
        {
            int i = 0;
            StreamReader sr = null;
            while (i < 5)
            {
                try
                {
                    sr = new StreamReader(fileName, Encoding.Default);
                    return sr;
                }
                catch (Exception) {}
                Thread.Sleep(10000);
                i++;
            }
            return sr;
        }

        private void InsertAccountYesterdayBalance(OrchPipeComponent.ComponentState State)
        {
            string balanceSign = string.Empty;
            string AvailableBalance = string.Empty;
            string BookBalance = string.Empty;
            string accountNumber = string.Empty;

            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCommand = db.GetStoredProcCommand("InsertAccountYesterdayBalance");

            accountNumber = State.OrchWrkData.GetWrkData().ReadNodeValue("AccountNumber",true);
            double x = double.Parse(accountNumber);

            db.AddInParameter(dbCommand, "AccountNumber", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("AccountNumber"));

            balanceSign = State.OrchWrkData.GetWrkData().ReadNodeValue("AvailableBalanceSign", true);
            AvailableBalance = State.OrchWrkData.GetWrkData().ReadNodeValue("AvailableBalance");
            AvailableBalance = (balanceSign=="C" ? AvailableBalance : "-" + AvailableBalance);

            balanceSign = State.OrchWrkData.GetWrkData().ReadNodeValue("BookDateBalanceSign", true);
            BookBalance = State.OrchWrkData.GetWrkData().ReadNodeValue("BookBalance");
            BookBalance = (balanceSign == "C" ? BookBalance : "-" + BookBalance);

            db.AddInParameter(dbCommand, "AvailableBalance", DbType.String, AvailableBalance);
            db.AddInParameter(dbCommand, "BookDate", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("BookDate"));
            db.AddInParameter(dbCommand, "BookBalance", DbType.String, BookBalance);
            db.AddInParameter(dbCommand, "CurrencyCode", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CurrencyCode"));
            db.AddInParameter(dbCommand, "ShortName", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("ShortName"));
            db.AddInParameter(dbCommand, "IBAN", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IBAN"));
            db.AddInParameter(dbCommand, "CustomerName", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CustomerName"));
            db.AddInParameter(dbCommand, "MISCode", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("MISCode"));
            db.AddInParameter(dbCommand, "MISDescription", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("MISDescription"));
            db.AddInParameter(dbCommand, "AlternateAccountNumber", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("AlternateAccount"));

            db.ExecuteNonQuery(dbCommand);
        }
        private void SetAccountYesterdayBalance(OrchPipeComponent.ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("SetAccountYesterdayBalance");
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void InitAccountYesterdayBalance(OrchPipeComponent.ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("InitAccountYesterdayBalance");
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {
                throw;
            }
        }


        private string GetNumericValue(string inValue)
        {
            inValue = inValue.PadLeft(18, '0');
            string result = inValue.Substring(0, inValue.Length - 3) + "." + inValue.Substring(inValue.Length - 3);
            return result;

        }

        private void DoParseLine(ComponentState State, string line)
        {
            int currIndex;

            line = line.Replace("&#38;", "");
            string Account = GetField(line, 0, out currIndex);
            string AvailableBalanceSign = GetField(line, currIndex + 1, out currIndex);
            string AvailableBalance = GetField(line, currIndex + 1, out currIndex);
            string BookDate = GetField(line, currIndex + 1, out currIndex);
            string BookBalanceSign = GetField(line, currIndex + 1, out currIndex);
            string BookBalance = GetField(line, currIndex + 1, out currIndex);
            string CurrencyCode = GetField(line, currIndex + 1, out currIndex);
            string IBAN = GetField(line, currIndex + 1, out currIndex);
            string ShortName = GetField(line, currIndex + 1, out currIndex);
            string CustomerName = GetField(line, currIndex + 1, out currIndex);
            string MISCode = GetField(line, currIndex + 1, out currIndex);
            string MISDescription = GetField(line, currIndex + 1, out currIndex);
            string AlternateAccount = GetField(line, currIndex + 1, out currIndex);

            State.OrchWrkData.GetWrkData().WriteNodeValue("AccountNumber", Account, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("AvailableBalanceSign", AvailableBalanceSign, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("AvailableBalance", AvailableBalance, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("BookDate", BookDate, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("BookDateBalanceSign", BookBalanceSign, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("BookBalance", BookBalance, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("CurrencyCode", CurrencyCode, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("IBAN", IBAN, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("ShortName", ShortName, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("CustomerName", CustomerName, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("MISCode", MISCode, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("MISDescription", MISDescription, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("AlternateAccount", AlternateAccount, true);
        }

        private string GetField(string line, int startIndex, out int lastIndex)
        {
            string outField = "";
            int length=0;
            lastIndex = startIndex + length;
            try
            {
                length = line.Substring(startIndex).IndexOf(';');
                lastIndex = startIndex + length;
                if (length == -1)
                {
                    outField = line.Substring(startIndex);
                    lastIndex = lastIndex + outField.Length;
                }
                else
                    outField = line.Substring(startIndex, length);
            }
            catch (Exception)
            {
            }
            return outField;
        }
    }
}
