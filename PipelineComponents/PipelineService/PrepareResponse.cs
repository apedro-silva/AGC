using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// Prepare Response
    /// </summary>
    public class PrepareResponse : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            string originalResponseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("OriginalResponseCode", true);

            // If we already have a switch response
            if (originalResponseCode != null)
                return;

            string Erro = State.OrchWrkData.GetWrkData().ReadNodeValue("Erro", true);

            // Repeated Message from PRT check if we have a response to PRT
            if (Erro == "-1")
            {
                GetResponseFromThisMessage(State);
                if (State.OrchWrkData.GetWrkData().ReadNodeValue("PRTResponse", true)!=null)
                    return;
            }

            try
            {
                ProcessFlexCubeResponse(State);
                ProcessServiceResponse(State);
                SetResponseDefaults(State);

                string CodResp = State.OrchWrkData.GetWrkData().ReadNodeValue("CodResp", true);

                // If Pipeline is in error set CodResp with 6-Erro aplicacional
                if (State.IsInError && CodResp == null)
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "6", true);
                else if (CodResp == null)
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "0", true);
            }
            catch (Exception exp)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "4", true);
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

        private void GetResponseFromThisMessage(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("ObtemRespostaPRT");

                string AplicPDD = State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPdd", true);
                string IdLog = State.OrchWrkData.GetWrkData().ReadNodeValue("IdLog", true);
                string NrLog = State.OrchWrkData.GetWrkData().ReadNodeValue("NrLog", true);

                db.AddInParameter(dbCommand, "AplicPdd", DbType.String, AplicPDD);
                db.AddInParameter(dbCommand, "IdLog", DbType.String, IdLog);
                db.AddInParameter(dbCommand, "NrLog", DbType.String, NrLog);
                db.AddOutParameter(dbCommand, "RespostaPRT", DbType.String, 1200);
                db.AddOutParameter(dbCommand, "CodResp", DbType.String, 2);

                db.ExecuteNonQuery(dbCommand);
                if (db.GetParameterValue(dbCommand, "RespostaPRT").ToString() != "")
                {
                    byte[] PRTResponse = Encoding.ASCII.GetBytes(db.GetParameterValue(dbCommand, "RespostaPRT").ToString());
                    State.OrchWrkData.GetWrkData().WriteNodeBuffer("PRTResponse", PRTResponse, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", db.GetParameterValue(dbCommand, "CodResp").ToString(), true);
                }
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

        private void SetResponseDefaults(OrchPipeComponent.ComponentState State)
        {
            string CodResp;

            //Numero de Identificação da Resposta.Identificação da transacção no sistema do Banco.
            string NrIdResp = State.OrchWrkData.GetWrkData().ReadNodeValue("RetrievalReferenceNumber");
            NrIdResp = NrIdResp.PadLeft(14, '0');
            State.OrchWrkData.GetWrkData().WriteNodeValue("NrIdResp", NrIdResp, true);

            State.OrchWrkData.GetWrkData().WriteNodeValue("CodRespAd", "000", true);
            WD2EMISField(State, "Conta", "ContaSAN1", 15, false, false);

            CodResp = State.OrchWrkData.GetWrkData().ReadNodeValue("CodResp", true);

            if (CodResp != "0")
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("ContaSAN1", "000000000000000", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodMoeda", "000", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("ContaSAN2", "000000000000000", true);
            }

        }

        private void ProcessFlexCubeResponse(OrchPipeComponent.ComponentState State)
        {
            string CodResp;
            string ResponseCode;

            // if these fields doesn't exist there's no message
            if (State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true) == "")
                return;

            ResponseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
            switch (ResponseCode)
            {
                case "00": CodResp = "0"; break;
                case "05": CodResp = "4"; break;
                case "06": CodResp = "4"; break;
                case "15": CodResp = "4"; break;
                case "30": CodResp = "4"; break;
                case "31": CodResp = "4"; break;
                case "40": CodResp = "4"; break;
                case "51": CodResp = "4"; break;
                case "53": CodResp = "4"; break;
                case "61": CodResp = "4"; break;
                case "65": CodResp = "4"; break;
                case "94": CodResp = "0"; break; // Duplicate transmission of ISO Message
                case "95": CodResp = "4"; break;
                case "96": CodResp = "4"; break;
                default: CodResp = "6"; break;
            }
            State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", CodResp, true);

            if (ResponseCode == null)
                return;

            SetIsoResponseMessage(State, ResponseCode);
            ProcessAdditionalAmounts(State);
            ProcessMovements(State);
        }

        private void SetIsoResponseMessage(OrchPipeComponent.ComponentState State, string ISOResponseCode)
        {
            if (ISOResponseCode == "0")
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("ISOResponseMessage", "OK", true);
                return;
            }
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("ObtemMensagemISO");
                db.AddInParameter(dbCommand, "ISOResponseCode", DbType.String, ISOResponseCode);
                db.AddOutParameter(dbCommand, "ISOResponseMessage", DbType.String, 50);

                db.ExecuteNonQuery(dbCommand);

                State.OrchWrkData.GetWrkData().WriteNodeValue("ISOResponseMessage", db.GetParameterValue(dbCommand, "ISOResponseMessage").ToString(), true);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

        private void ProcessMovements(ComponentState State)
        {
            string Movements = State.OrchWrkData.GetWrkData().ReadNodeValue("fc-Movements", true);
            if (Movements==null)
                return;

            ParseMessageField(State, Movements, "FC_Movements_PARSE");

        }

        private void ProcessAdditionalAmounts(ComponentState State)
        {
            string AdditionalAmounts = State.OrchWrkData.GetWrkData().ReadNodeValue("fc-AdditionalAmounts", true);
            if (AdditionalAmounts==null)
                return;
            ParseMessageField(State, AdditionalAmounts, "FC_Amounts_PARSE");

            WD2EMISField(State, "DHMsg", 0, 8, "DataSaldo", false);

            WD2EMISField(State, "NetAvailableBalance", "SaldoDisp", 11, false, false);
            WD2EMISField(State, "NetAvailableBalanceCurrency", "MoedaSaldoDisp", 3, false, false);
            WD2EMISField(State, "NetAvailableBalanceSign", "SinalSaldoDisp", false);

            WD2EMISField(State, "LedgerBalance", "SaldoCont", 11, false, false);
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

        private void ProcessServiceResponse(OrchPipeComponent.ComponentState State)
        {
            // call service specific component
            string ServiceResponseTypeName = State.OrchWrkData.GetWrkData().ReadNodeValue("ServiceResponseTypeName", true);

            //Service do not have specific response
            if (ServiceResponseTypeName == "")
                return;

            Type myType = Type.GetType(ServiceResponseTypeName);
            try
            {
                myType.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { State});
            }
            catch(Exception exp)
            {
                State.LastError = new BusinessException("PipelineService:PrepareResponse", exp);
            }
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
