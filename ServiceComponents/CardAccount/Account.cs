using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace SF.Expand.Switch.ServiceComponents
{
    /// <summary>
    /// Gets information about a valid account number. This class calls sql stored procedure residente in 
    /// a database defined in the FlexCube connection string.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets the account name for a valid account number. This class calls the PR_GETCUSTOMERNAME sql stored procedure residente in 
        /// a database defined in the FlexCube connection string.
        /// </summary>
        /// <param name="AccountNumber">The account number.</param>
        /// <returns>CustomerName</returns>
        public string GetAccountName(string AccountNumber)
        {
            string FullCustomerName = "";
            try
            {
                Database db = DatabaseFactory.CreateDatabase("FlexCube");
                DbCommand dbCommand = db.GetStoredProcCommand("PR_GETCUSTOMERNAME");
                db.AddInParameter(dbCommand, "pAccountNumber", DbType.String, AccountNumber);
                db.AddOutParameter(dbCommand, "pFullCustomerName", DbType.String, 500);
                db.ExecuteNonQuery(dbCommand);

                Object objCustomerName = db.GetParameterValue(dbCommand, "pFullCustomerName");
                if (objCustomerName!=System.DBNull.Value)
                    FullCustomerName = (string)objCustomerName;

                return FullCustomerName;
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }
        /// <summary>
        /// Gets the account info for a valid account number. This class calls the PR_GETCUSTOMERINFO sql stored procedure residente in 
        /// a database defined in the FlexCube connection string.
        /// </summary>
        /// <param name="AccountNumber">The account number.</param>
        /// <param name="FullCustomerName">Full name of the customer.</param>
        /// <param name="BranchCode">The branch code.</param>
        /// <param name="CCY">The CCY.</param>
        /// <param name="AccountClass">The account class.</param>
        /// <param name="IBAN">The IBAN.</param>
        /// 
        public void GetAccountInfo(string AccountNumber, out string FullCustomerName, out string BranchCode, out string CCY, out string AccountClass, out string IBAN)
        {
            FullCustomerName = "";
            BranchCode = "";
            CCY = "";
            AccountClass = "";
            IBAN = "";

            try
            {
                Database db = DatabaseFactory.CreateDatabase("FlexCube");
                DbCommand dbCommand = db.GetStoredProcCommand("PR_GETCUSTOMERINFO");
                db.AddInParameter(dbCommand, "pAccountNumber", DbType.String, AccountNumber);
                db.AddOutParameter(dbCommand, "pFullCustomerName", DbType.String, 500);
                db.AddOutParameter(dbCommand, "pBranchCode", DbType.String, 3);
                db.AddOutParameter(dbCommand, "pCCY", DbType.String, 3);
                db.AddOutParameter(dbCommand, "pAccountClass", DbType.String, 6);
                db.AddOutParameter(dbCommand, "pIBAN", DbType.String, 35);
                db.AddOutParameter(dbCommand, "pNODR", DbType.String, 35);
                db.ExecuteNonQuery(dbCommand);

                Object objParameter;
                objParameter = db.GetParameterValue(dbCommand, "pFullCustomerName");
                if (objParameter != System.DBNull.Value)
                    FullCustomerName = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pBranchCode");
                if (objParameter != System.DBNull.Value)
                    BranchCode = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pCCY");
                if (objParameter != System.DBNull.Value)
                    CCY = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pAccountClass");
                if (objParameter != System.DBNull.Value)
                    AccountClass = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pIBAN");
                if (objParameter != System.DBNull.Value)
                    IBAN = (string)objParameter;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Gets the account info for a valid account number. This class calls the PR_GETCUSTOMERINFO sql stored procedure residente in 
        /// a database defined in the FlexCube connection string.
        /// </summary>
        /// <param name="AccountNumber">The account number.</param>
        /// <param name="FullCustomerName">Full name of the customer.</param>
        /// <param name="BranchCode">The branch code.</param>
        /// <param name="CCY">The CCY.</param>
        /// <param name="AccountClass">The account class.</param>
        /// <param name="IBAN">The IBAN.</param>
        /// <param name="AccountState">The Account State.</param>
        /// 
        public void GetAccountInfoExtended(string AccountNumber, out string FullCustomerName, out string BranchCode, out string CCY, out string AccountClass, out string IBAN, out string AccountState)
        {
            FullCustomerName = "";
            BranchCode = "";
            CCY = "";
            AccountClass = "";
            IBAN = "";
            AccountState = "";

            try
            {
                Database db = DatabaseFactory.CreateDatabase("FlexCube");
                DbCommand dbCommand = db.GetStoredProcCommand("PR_GETCUSTOMERINFO");
                db.AddInParameter(dbCommand, "pAccountNumber", DbType.String, AccountNumber);
                db.AddOutParameter(dbCommand, "pFullCustomerName", DbType.String, 500);
                db.AddOutParameter(dbCommand, "pBranchCode", DbType.String, 3);
                db.AddOutParameter(dbCommand, "pCCY", DbType.String, 3);
                db.AddOutParameter(dbCommand, "pAccountClass", DbType.String, 6);
                db.AddOutParameter(dbCommand, "pIBAN", DbType.String, 35);
                db.AddOutParameter(dbCommand, "pNODR", DbType.String, 35);
                db.ExecuteNonQuery(dbCommand);

                Object objParameter;
                objParameter = db.GetParameterValue(dbCommand, "pFullCustomerName");
                if (objParameter != System.DBNull.Value)
                    FullCustomerName = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pBranchCode");
                if (objParameter != System.DBNull.Value)
                    BranchCode = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pCCY");
                if (objParameter != System.DBNull.Value)
                    CCY = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pAccountClass");
                if (objParameter != System.DBNull.Value)
                    AccountClass = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pIBAN");
                if (objParameter != System.DBNull.Value)
                    IBAN = (string)objParameter;

                objParameter = db.GetParameterValue(dbCommand, "pNODR");
                if (objParameter != System.DBNull.Value)
                    AccountState = (string)objParameter;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
