using System;
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
    public class ProcessEXCHFile : OrchPipeComponent
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
                State.LastError = new BusinessException("EXCH.ProcessLine", exp);
            }
            if (sr != null)
                sr.Close();
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
                DbCommand dbCommand = db.GetStoredProcCommand("SetExchangeRates");
                db.AddInParameter(dbCommand, "CCY1", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CCY1"));
                db.AddInParameter(dbCommand, "CCY2", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CCY2"));
                db.AddInParameter(dbCommand, "RateType", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("RateType"));
                db.AddInParameter(dbCommand, "MidRate", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("MidRate"));
                db.AddInParameter(dbCommand, "BuySpread", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("BuySpread"));
                db.AddInParameter(dbCommand, "SaleSpread", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("SaleSpread"));
                db.AddInParameter(dbCommand, "BuyRate", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("BuyRate"));
                db.AddInParameter(dbCommand, "SaleRate", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("SaleRate"));
                //db.AddInParameter(dbCommand, "DRCCRate", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("DRCCRate"));

                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

        private void DoParseLine(ComponentState State, string line)
        {
            int currIndex;
            string CCY1 = GetField(line, 0, out currIndex);
            string CCY2= GetField(line, currIndex + 1, out currIndex);
            string RateType = GetField(line, currIndex + 1, out currIndex);
            string MidRate = GetField(line, currIndex + 1, out currIndex);
            string BuyPread = GetField(line, currIndex + 1, out currIndex);
            string SaleSpread = GetField(line, currIndex + 1, out currIndex);
            string BuyRate = GetField(line, currIndex + 1, out currIndex);
            string SaleRate = GetField(line, currIndex + 1, out currIndex);
            //string DRCCRate = GetField(line, currIndex + 1, out currIndex); if (DRCCRate == "") DRCCRate = "0";

            State.OrchWrkData.GetWrkData().WriteNodeValue("CCY1", CCY1, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("CCY2", CCY2, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("RateType", RateType, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("MidRate", MidRate, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("BuySpread", BuyPread, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("SaleSpread", SaleSpread, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("BuyRate", BuyRate, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("SaleRate", SaleRate, true);
            //State.OrchWrkData.GetWrkData().WriteNodeValue("DRCCRate", DRCCRate, true);
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
            }
            return outField;
        }
    }
}
