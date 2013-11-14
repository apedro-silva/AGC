using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SF.Expand.Switch.SwitchServices
{
    /// <summary>
    /// 
    /// </summary>
    public class ECSVtoEMIS
    {
        private StreamWriter ECSVFileStream = null;
        private ClearingHelper helper = null;
        private int ProcessedRows = 0;
        private decimal SumAvailableBalance = 0;
        private const int HeaderLength = 46;
        private const int RecordLength = 52;
        private const int TrailerLength = 41;

        /// <summary>
        /// 
        /// </summary>
        public ECSVtoEMIS()
        {
            helper = new ClearingHelper();
        }
        /// <summary>
        /// 
        /// </summary>
        private void WriteHeader()
        {
            FileHeaderECSV header = new FileHeaderECSV(HeaderLength);
            header.FICH = "ECSV";
            header.VERIFICH = "00";
            header.IDFICHANT = helper.GetIdFichAnt("ECSV");
            header.IDFICH = helper.GetIdFich("ECSV");
            header.DATAVALOR = helper.GetDataValor();
            ECSVFileStream.Write(header.GetHeader());
            ECSVFileStream.WriteLine();
        }
        
        private int WriteRecords()
        {
            RecordECSV1 record = new RecordECSV1(RecordLength);
            ProcessedRows = 0;

            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCommand = db.GetStoredProcCommand("dbo.GetAccountYesterdayBalance");
            IDataReader myReader = db.ExecuteReader(dbCommand);
            string accountNum;
            decimal availableBalance;
            DateTime bookDate;
            decimal bookBalance;

        	// SELECT AccountNum, AvailableBalance, BookDate, BookBalance, IDCurrency, IBAN, ShortName FROM dbo.AccountYesterdayBalance
            while (myReader.Read())
            {
                accountNum = (string)myReader.GetValue(0);
                availableBalance = (decimal)myReader.GetValue(1);
                bookDate = (DateTime)myReader.GetValue(2);
                bookBalance = (decimal)myReader.GetValue(3);

                record.CONTA = accountNum.PadLeft(15, '0');
                string svesp = decimal.Round(availableBalance, 2).ToString();
                svesp = svesp.Replace(",", "");
                svesp = svesp.Replace("-", "");
                record.SVESP = svesp.PadLeft(13, '0');
                SumAvailableBalance += availableBalance;
                if (availableBalance >= 0)
                    record.SINAL1 = "C";
                else
                    record.SINAL1 = "D";
                record.DTSCONT = helper.GetDateYMD(bookDate);
                string scont = decimal.Round(bookBalance, 2).ToString();
                scont = scont.Replace(",", "");
                scont = scont.Replace("-", "");
                record.SCONT = scont.PadLeft(13, '0');
                if (bookBalance >= 0)
                    record.SINAL2 = "C";
                else
                    record.SINAL2= "D";
                ECSVFileStream.Write(record.GetECSVRecord1());
                ECSVFileStream.WriteLine();
                ProcessedRows++;
            }
            myReader.Close();
            helper.CloseReader();
            return ProcessedRows;
        }
        
        private void WriteTrailer()
        {
            FileTrailerECSV trailer = new FileTrailerECSV(TrailerLength);
            trailer.TOTREG = ProcessedRows.ToString().PadLeft(8, '0');
            string sum = decimal.Round(SumAvailableBalance, 2).ToString();
            sum = sum.Replace(",", "");
            sum = sum.Replace("-", "");
            trailer.TOTDEB = "D" + sum.PadLeft(15, '0');
            trailer.TOTCRED = "C" + sum.PadLeft(15, '0');
            ECSVFileStream.Write(trailer.GetTrailer());
            ECSVFileStream.WriteLine();
        }
        
        public void YesterdayBalanceToEMIS()
        {
            string path = helper.GetPath("ECSV");
            string fileName = helper.GetIdFich("ECSV");
            decimal seqNum = helper.GetSeqNum("ECSV");
            int records = 0;
            int ret = 0;
            try
            {
                Directory.CreateDirectory(path + "\\Temp");
            }
            catch (Exception) { };

            try
            {
                string tempPath = path + "\\Temp\\ECSV" + fileName + ".txt";
                string finalPath = path + "\\ECSV" + fileName + ".txt";

                ECSVFileStream = new StreamWriter(new FileStream(tempPath, FileMode.Create));
                WriteHeader();
                records = WriteRecords();
                WriteTrailer();
                ECSVFileStream.Close();
                MoveFile2Done(tempPath, finalPath);
                if (records > 0)
                {
                    ret = helper.UpdateECSVToEMIS(fileName, seqNum + 1);
                    return;
                }
                else
                {
                    if (File.Exists(finalPath))
                        File.Delete(finalPath);
                    return;
                }
            }
            catch (Exception ex)
            {
                if (ECSVFileStream!=null)
                    ECSVFileStream.Close();
                throw new Exception("Erro em YesterdayBalanceToEMIS->"+ex.Message, ex);
            }
        }
        private void MoveFile2Done(string tempFileName, string finalFileName)
        {
            try
            {
                File.Move(tempFileName, finalFileName);
            }
            catch (Exception) { throw; };
        }

    }
}
