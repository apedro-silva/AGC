using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Configuration;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SF.Expand.Switch.SwitchServices
{
    /// <summary>
    /// ClearingHelper
    /// </summary>
    public class ClearingHelper
    {
        IDataReader myReader;
        /// <summary>
        /// 
        /// </summary>
        public ClearingHelper()
        {
            //SELECT FileCode, SeqNum, LastFileRef, Path, CurrentStep FROM dbo.FilesToEMIS
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCommand = db.GetSqlStringCommand("SELECT FileCode, SeqNum, LastFileRef, Path, CurrentStep FROM dbo.FilesToEMIS where FileCode='ECSV'");
            myReader = db.ExecuteReader(dbCommand);
            myReader.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileCode"></param>
        /// <returns></returns>
        public string GetIdFichAnt(string fileCode)
        {
            return (string)myReader.GetValue(2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileCode"></param>
        /// <returns></returns>
        public string GetIdFich(string fileCode)
        {
            string date = DateTime.Today.ToString();
            string year = date.Substring(6, 4);
            string month = date.Substring(3, 2);
            string day = date.Substring(0, 2);
            decimal seqdec = GetSeqNum(fileCode);
            seqdec++;
            string seq = seqdec.ToString().PadLeft(3, '0');
            StringBuilder idfichant = new StringBuilder();
            idfichant.Append(year);
            idfichant.Append(month);
            idfichant.Append(day);
            idfichant.Append(seq);
            return idfichant.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDataValor()
        {
            DateTime t = DateTime.Now;
            string dataValor = "00" + t.Hour.ToString().PadLeft(2, '0') + t.Minute.ToString().PadLeft(2, '0') + t.Second.ToString().PadLeft(2, '0');
            return dataValor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string GetDateYMD(DateTime dt)
        {
            string year = dt.Year.ToString();
            string month = dt.Month.ToString().PadLeft(2, '0');
            string day = dt.Day.ToString().PadLeft(2, '0');
            StringBuilder sb = new StringBuilder();
            sb.Append(year);
            sb.Append(month);
            sb.Append(day);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileCode"></param>
        /// <returns></returns>
        public string GetPath(string fileCode)
        {
            string path = (string)myReader.GetValue(3);
            if (path.LastIndexOf("\\") == path.Length - 1)
                path = path.Substring(0, path.Length - 1);

            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRef"></param>
        /// <param name="seqNum"></param>
        /// <returns></returns>
        public int UpdateECSVToEMIS(string fileRef, decimal seqNum)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCommand = db.GetStoredProcCommand("dbo.UpdateECSVToEMIS");
            db.AddInParameter(dbCommand, "@FileRef", DbType.String, fileRef);
            db.AddInParameter(dbCommand, "@SeqNum", DbType.Int32, seqNum);
            return db.ExecuteNonQuery(dbCommand);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileCode"></param>
        /// <returns></returns>
        public decimal GetSeqNum(string fileCode)
        {
            DateTime dToday = DateTime.Today;
            if (myReader == null) return 0;
            if (Decimal.Parse((string)myReader.GetValue(2)) == 0) return 0;
            string lastFileRef = (string)myReader.GetValue(2);
            DateTime dSeqNum = DateTime.ParseExact(lastFileRef.Substring(0, 8), "yyyyMMdd", null);
            if (dSeqNum.CompareTo(dToday) < 0)
                return 0;
            else
                return (decimal)myReader.GetValue(1);
        }
        /// <summary>
        /// CloseReader
        /// </summary>
        public void CloseReader()
        {
            myReader.Close();
        }

    }
}
