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

namespace SF.Expand.Switch.SwitchServices
{
    public class CURR : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;

            string FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("InputFileName");
            StreamReader sr = OpenFile2Process(FileName);
            try
            {
                string line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    DoParseLine(State, line);
                    SetExchangeRates(State);
                }
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro processado", true);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("CURR.ProcessLine", exp);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            return;

        }

        private StreamReader OpenFile2Process(string fileName)
        {
            int i = 0;
            StreamReader sr = null;
            while (i < 5)
            {
                try
                {
                    sr = new StreamReader(fileName, Encoding.ASCII);
                    break;
                }
                catch (Exception) { }
                Thread.Sleep(3000);
                i++;
            }
            return sr;
        }

        private void SetExchangeRates(OrchPipeComponent.ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("SetCurrencyInformation");
                db.AddInParameter(dbCommand, "CCYCode", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CCYCode"));
                db.AddInParameter(dbCommand, "CCYName", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CCYName"));
                db.AddInParameter(dbCommand, "Country", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("Country"));
                db.AddInParameter(dbCommand, "NumberOfDecimals", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("NumberOfDecimals"));

                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void DoParseLine(ComponentState State, string line)
        {
            int currIndex;
            string CCYCode = GetField(line, 0, out currIndex);
            string CCYName= GetField(line, currIndex + 1, out currIndex);
            string Country = GetField(line, currIndex + 1, out currIndex);
            string NumberOfDecimals = GetField(line, currIndex + 1, out currIndex);

            State.OrchWrkData.GetWrkData().WriteNodeValue("CCYCode", CCYCode, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("CCYName", CCYName, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("Country", Country, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("NumberOfDecimals", NumberOfDecimals, true);
        }

        private string GetField(string line, int startIndex, out int lastIndex)
        {
            string outField = "";
            int length = 0;
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
                throw;
            }
            return outField;
        }
    }
}
