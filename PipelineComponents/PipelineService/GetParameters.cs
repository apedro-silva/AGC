using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// 
    /// </summary>
    public class GetParameters : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            // Get Service Parameters
            GetServiceParameters(State, Params);
            GetSpecificParameters(State, Params);
        }

        private void GetServiceParameters(OrchPipeComponent.ComponentState State, string[] Params)
        {
            bool CheckSignal = true;
            try
            {
                string CodTrn;
                if (Params.Length == 0)
                    CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
                else
                {
                    CodTrn = Params[0];
                    CheckSignal = false;
                }
                string CodMsg = State.OrchWrkData.GetWrkData().ReadNodeValue("CodMsg",true);
                string IndSt = State.OrchWrkData.GetWrkData().ReadNodeValue("IndStr", true);

                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("ObtemParametrosServico");
                db.AddInParameter(dbCommand, "CodMsg", DbType.String, CodMsg);
                db.AddInParameter(dbCommand, "Servico", DbType.String, CodTrn);
                db.AddInParameter(dbCommand, "IndSt", DbType.String, IndSt);

                db.AddInParameter(dbCommand, "AplicPdd", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPdd", true));
                db.AddInParameter(dbCommand, "IdLog", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IdLog", true));
                db.AddInParameter(dbCommand, "NrLog", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("NrLog", true));

                db.AddInParameter(dbCommand, "AplicPddOriginal", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPddOriginal", true));
                db.AddInParameter(dbCommand, "IdLogOriginal", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IdLogOriginal", true));
                db.AddInParameter(dbCommand, "NrLogOriginal", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("NrLogOriginal", true));

                DataSet srvDS = db.ExecuteDataSet(dbCommand);

                using (DataTable trnParameters = srvDS.Tables[0])
                {
                    DataRow dr = trnParameters.Rows[0];

                    string DescricaoServico = dr[1].ToString().Trim();
                    string TipoServico = dr[2].ToString().Trim();
                    string ISOMessageType = dr[4].ToString().Trim();
                    string ISOProcessingCode = dr[5].ToString().Trim();
                    string PRTParser = dr[6].ToString().Trim();
                    string PRTConstructor = dr[7].ToString().Trim();
                    string FlexCubeParser = dr[8].ToString().Trim();
                    string FlexCubeConstructor = dr[9].ToString().Trim();
                    string ServiceResponseTypeName = dr[10].ToString().Trim();
                    string MessageTypeOriginal = dr[11].ToString().Trim();
                    string STANOriginal = GetColumnValue(dr[12]);
                    string TransmissionDateTimeOriginal = dr[13].ToString().Trim();
                    string AcquiringInstitutionIdentificationCodeOriginal = dr[14].ToString().Trim();
                    string CodigoBanco = dr[15].ToString().Trim();
                    string CodigoMoeda = dr[16].ToString().Trim();
                    string CodigoTerminal = dr[17].ToString();
                    string NumeroContaDebito = dr[18].ToString().Trim();
                    string NumeroContaCredito = dr[19].ToString().Trim();
                    string ServiceRequestTypeName = dr[20].ToString().Trim();
                    string AbreviaturaServico = dr[21].ToString().Trim();
                    string CodigoMoedaDRCC = dr[22].ToString().Trim();
                    string RetrievalReferenceNumber = dr[23].ToString().Trim();
                    string RubricaContabilistica = dr[24].ToString().Trim();
                    string OriginalJe = GetColumnValue(dr[25]);
                    string OriginalResponseCode = GetColumnValue(dr[26]);
                    string OriginalResponse = GetColumnValue(dr[27]);
                    string CodResp = GetColumnValue(dr[28]);

                    State.OrchWrkData.GetWrkData().WriteNodeValue("RetrievalReferenceNumber", RetrievalReferenceNumber, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoMoedaDRCC", CodigoMoedaDRCC, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("DescricaoServico", DescricaoServico, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("AbreviaturaServico", AbreviaturaServico, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TipoServico", TipoServico, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("MessageType", ISOMessageType, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ProcessingCode", ISOProcessingCode, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("PRTParser", PRTParser, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("PRTConstructor", PRTConstructor, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("FlexCubeParser", FlexCubeParser, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("FlexCubeConstructor", FlexCubeConstructor, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ServiceResponseTypeName", ServiceResponseTypeName, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ServiceRequestTypeName", ServiceRequestTypeName, true);

                    // Information about Reversal
                    // MessageType,STAN, TransmissionDateTime, AcquiringInstitutionIdentificationCode
                    State.OrchWrkData.GetWrkData().WriteNodeValue("MessageTypeOriginal", MessageTypeOriginal, true);

                    State.OrchWrkData.GetWrkData().WriteNodeValue("TransmissionDateTimeOriginal", TransmissionDateTimeOriginal, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("AcquiringInstitutionIdentificationCodeOriginal", AcquiringInstitutionIdentificationCodeOriginal, true);

                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoBanco", CodigoBanco, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoMoeda", CodigoMoeda, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodigoTerminal", CodigoTerminal, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebito", NumeroContaDebito, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCredito", NumeroContaCredito, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("RubricaContabilistica", RubricaContabilistica, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("NrIdResp", "00000000000000", true);

                    DeleteNode(State, "OriginalJE");
                    DeleteNode(State, "OriginalSwitchResponse");
                    DeleteNode(State, "OriginalResponseCode");
                    DeleteNode(State, "fc-ResponseCode");
                    DeleteNode(State, "CodResp");

                    if (OriginalJe != null)
                    {
                        State.OrchWrkData.GetWrkData().WriteNodeValue("OriginalJE", OriginalJe, true);

                        if (OriginalResponseCode != null && OriginalResponseCode.Equals("00"))
                            State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Transacção já submetida com o ID=" + OriginalJe, true);
                    }

                    // This is a Reversal and STANOriginal belongs to Original or
                    // This is a Repeat and STANOriginal belongs to Original
                    if (STANOriginal != null)
                        State.OrchWrkData.GetWrkData().WriteNodeValue("STANOriginal", STANOriginal, true);

                    if (OriginalResponse != null)
                        State.OrchWrkData.GetWrkData().WriteNodeValue("OriginalSwitchResponse", OriginalResponse, true);

                    if (OriginalResponseCode != null)
                    {
                        State.OrchWrkData.GetWrkData().WriteNodeValue("OriginalResponseCode", OriginalResponseCode, true);
                        State.OrchWrkData.GetWrkData().WriteNodeValue("fc-ResponseCode", OriginalResponseCode, true);
                    }
                    if (CodResp != null)
                        State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", CodResp, true);

                    if (CheckSignal)
                    {
                        string Sinal = State.OrchWrkData.GetWrkData().ReadNodeValue("SinalMontante2", true);
                        if (Sinal == "D")
                        {
                            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCredito", NumeroContaCredito, true);
                            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebito", NumeroContaDebito, true);
                        }
                        else
                        {
                            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCredito", NumeroContaDebito, true);
                            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebito", NumeroContaCredito, true);
                        }
                    }
                }


                if (srvDS.Tables.Count > 1)
                {
                    using (DataTable exchangeTable = srvDS.Tables[1])
                    {
                        DataSet exchangeDS = exchangeTable.DataSet;
                        string exchangeXml = exchangeDS.GetXml();
                        if (exchangeXml.IndexOf("<Table1>") >= 0)
                        {
                            exchangeXml = "<Cambios>" + exchangeXml.Substring(exchangeXml.IndexOf("<Table1>")).Replace("</NewDataSet>", "") + "</Cambios>";
                            State.OrchWrkData.GetWrkData().WriteNodeValue("Cambios", exchangeXml, true);
                        }
                    }
                }

                if (srvDS.Tables.Count > 2)
                {
                    using (DataTable serviceTable = srvDS.Tables[2])
                    {
                        DataSet serviceDS = serviceTable.DataSet;
                        string serviceXml = serviceDS.GetXml();
                        if (serviceXml.IndexOf("<Table2>") >= 0)
                        {
                            serviceXml = "<Servicos>" + serviceXml.Substring(serviceXml.IndexOf("<Table2>")).Replace("</NewDataSet>", "") + "</Servicos>";
                            State.OrchWrkData.GetWrkData().WriteNodeValue("Servicos", serviceXml, true);
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
            }
        }

        private void GetSpecificParameters(OrchPipeComponent.ComponentState State, string[] Params)
        {
            string CodTrn;
            if (Params.Length == 0)
                CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
            else
            {
                CodTrn = Params[0];
            }
            if ("009,022,023,035,T4,T5".IndexOf(CodTrn) >= 0)
            {
                string rubricaContabilistica = State.OrchWrkData.GetWrkData().ReadNodeValue("RubricaContabilistica", true);
                string TerminalType = State.OrchWrkData.GetWrkData().ReadNodeValue("TipoTerm", true);
                string RC = GetPaymentServicesCode(TerminalType, rubricaContabilistica);

                SetHostAccount(State, RC, "SinalMontante2");

            }
        }
        private string GetPaymentServicesCode(string terminalType, string paymentCode)
        {
            switch (terminalType)
            {
                case "A": paymentCode = "A" + paymentCode; break;
                case "B": paymentCode = "P" + paymentCode; break;
                default: paymentCode = "O" + paymentCode; break;

            }
            return paymentCode;
        }

        private void SetHostAccount(OrchPipeComponent.ComponentState State, string RubricaContabilistica, string SinalMontante)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("ObtemRubricaContabilistica");
                db.AddInParameter(dbCommand, "RubricaContabilistica", DbType.String, RubricaContabilistica);

                State.OrchWrkData.GetWrkData().WriteNodeValue("RubricaContabilistica", RubricaContabilistica, true);

                DataSet srvDS = db.ExecuteDataSet(dbCommand);

                using (DataTable trnParameters = srvDS.Tables[0])
                {
                    if (trnParameters.Rows.Count == 0)
                        throw new BusinessException(string.Format("Rúbrica Contabilística {0} não parametrizada", RubricaContabilistica));

                    DataRow dr = trnParameters.Rows[0];

                    string Rubrica = dr[0].ToString();
                    string Descricao = dr[1].ToString();
                    string NumeroContaDebito = dr[2].ToString();
                    string NumeroContaCredito = dr[3].ToString();

                    string Sinal = State.OrchWrkData.GetWrkData().ReadNodeValue(SinalMontante, true);
                    if (Sinal == "D")
                    {
                        State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCredito", NumeroContaCredito, true);
                        State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebito", NumeroContaDebito, true);
                    }
                    else
                    {
                        State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCredito", NumeroContaDebito, true);
                        State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebito", NumeroContaCredito, true);
                    }

                }

            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
            }
        }

        private void DeleteNode(OrchPipeComponent.ComponentState State, string nodeName)
        {

            if (State.OrchWrkData.GetWrkData().GetNodeByName(nodeName) != null)
                State.OrchWrkData.GetWrkData().DeleteNode(nodeName);

        }

        private string GetColumnValue(object column)
        {
            if (column == System.DBNull.Value)
                return null;
            if (column.ToString().Trim().Equals(""))
                return null;

            return column.ToString().Trim();
        }
    }
}
