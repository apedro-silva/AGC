using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using System.Xml;
using System.Configuration;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// PrepareConstructor
    /// </summary>
    public class PrepareConstructor:OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            // If pipeline is in error nothing to do
            if (State.IsInError)
                return;

            // Process Service Request specific code
            ProcessServiceRequest(State);

            // if these fields doesn't exist there's nothing to construct
            if (State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true) == "" || State.OrchWrkData.GetWrkData().ReadNodeValue("ProcessingCode", true) == "")
                return;

            // mandatory fields
            try
            {
                string BanApoio = State.OrchWrkData.GetWrkData().ReadNodeValue("BanApoio", true);
                string CodigoBanco = State.OrchWrkData.GetWrkData().ReadNodeValue("CodigoBanco", true);
                string IdTerminal = State.OrchWrkData.GetWrkData().ReadNodeValue("IdTerminal", true);
                string TipoTerminal = State.OrchWrkData.GetWrkData().ReadNodeValue("TipoTerm", true);
                string TipoServico = State.OrchWrkData.GetWrkData().ReadNodeValue("TipoServico", true);

                string CodigoTerminal = State.OrchWrkData.GetWrkData().ReadNodeValue("CodigoTerminal", true);

                WD2FlexCubeField(State, "CodigoBanco", "PrimaryAccountNumber", 4, false, true);
                WD2FlexCubeField(State, "Montante2", "TransactionAmount", 12, false, true);
                WD2FlexCubeField(State, "CodMoeda", "TransactionCurrencyCode", true);
                WD2FlexCubeField(State, "DtHora", 4, "TransmissionDateTime", true);
                WD2FlexCubeField(State, "DtHora", 8, 6, "LocalTransactionTime", true);
                WD2FlexCubeField(State, "DtHora", 4, 4, "LocalTransactionDate", true);
                WD2FlexCubeField(State, "DtHora", 4, 4, "SettlementDate", true);
                WD2FlexCubeField(State, "DtHora", 4, 4, "CaptureDate", true);


                string stanOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("STANOriginal", true);

                // Only Repeats assume Original STAN
                if (stanOriginal!=null && TipoServico.Equals("RES"))
                    WD2FlexCubeField(State, "STANOriginal", "SystemTraceAuditNumber", 6, false, true);

                else if (Params.Length == 0 || Params[0]==null)
                    WD2FlexCubeField(State, "JE", "SystemTraceAuditNumber", 6, false, true);
                else
                    WD2FlexCubeField(State, Params[0], "SystemTraceAuditNumber", 6, false, true);

                WD2FlexCubeField(State, "CodigoBanco", "AcquiringInstitutionIdentificationCode", 4, false, true);
                WD2FlexCubeField(State, "CodigoBanco", "ForwardingInstitutionIdentificationCode", 4, false, true);


                if (IdTerminal == "0000000000")
                    CodigoBanco = "";

                string CardAcceptorTerminalId = CodigoTerminal;
                if (TipoTerminal=="A" && BanApoio == CodigoBanco)
                    CardAcceptorTerminalId = IdTerminal.Substring(4,6);

                CardAcceptorTerminalId = CardAcceptorTerminalId + "  ";
                State.OrchWrkData.GetWrkData().WriteNodeValue("CardAcceptorTerminalIdentification", CardAcceptorTerminalId, true);

                SetFlexCubeAccounts(State);

                string DescricaoServico = State.OrchWrkData.GetWrkData().ReadNodeValue("DescricaoServico", true);
                string cardNumber = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroCartao", true);
                if (cardNumber != null && cardNumber.Length>=16)
                {
                    cardNumber = cardNumber.Substring(11, 5).PadLeft(11, '*');
                    DescricaoServico = DescricaoServico.Replace("%2", cardNumber);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("DescricaoServico", DescricaoServico, true);
                }

                string AbreviaturaServico = State.OrchWrkData.GetWrkData().ReadNodeValue("AbreviaturaServico", true);

                State.OrchWrkData.GetWrkData().WriteNodeValue("DescricaoAbreviaturaServico", AbreviaturaServico + " - " + DescricaoServico, true);
                WD2FlexCubeField(State, "DescricaoAbreviaturaServico", "CardAcceptorName", 40, true, true);

                State.OrchWrkData.GetWrkData().WriteNodeValue("DadosV", "0".PadLeft(30, '0'), true);

                // If TipoServico='ANL' then it's a reversal
                // so lets create Original Data Elements
                if (TipoServico == "ANL")
                {
                    string MessageTypeOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("MessageTypeOriginal", true);
                    string STANOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("STANOriginal", true);
                    string TransmissionDateTimeOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("TransmissionDateTimeOriginal", true);
                    string AcquiringInstitutionIdentificationCodeOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("AcquiringInstitutionIdentificationCodeOriginal", true);

                    if (STANOriginal == "")
                    {
                        throw new BusinessException(string.Format("Transacção Original em falta"));
                    }
                    string OriginalDataElements = MessageTypeOriginal + STANOriginal + TransmissionDateTimeOriginal + AcquiringInstitutionIdentificationCodeOriginal.PadLeft(11, '0') + AcquiringInstitutionIdentificationCodeOriginal.PadLeft(11, '0');
                    State.OrchWrkData.GetWrkData().WriteNodeValue("OriginalDataElements", OriginalDataElements, true);
                }

                // Set Retrieval Reference Number.
                // Ticks represents the number of 100-nanosecond intervals that have elapsed since 12:00:00 midnight, January 1, 0001.
                string RetrievalReferenceNumber = State.OrchWrkData.GetWrkData().ReadNodeValue("RetrievalReferenceNumber", true);
                if (RetrievalReferenceNumber==null || RetrievalReferenceNumber == "")
                    RetrievalReferenceNumber = GetRetrievalReferenceNumber();
                State.OrchWrkData.GetWrkData().WriteNodeValue("RetrievalReferenceNumber", RetrievalReferenceNumber, true);

                saveWorkData2Disk(State, RetrievalReferenceNumber);
            }
            catch(Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
            }
        }

        private string GetRetrievalReferenceNumber()
        {
            DateTime centuryBegin = new DateTime(2001, 1, 1);
            DateTime currentDate = DateTime.Now;

            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            return elapsedTicks.ToString().Substring(2,12);
        }

        // Save the workdata xml in a file
        private void saveWorkData2Disk(ComponentState State, string RetrievalReferenceNumber)
        {
            string wrkData = State.OrchWrkData.GetWrkData().GetWrkDataXml();
            System.Xml.XmlDocument xmlDoc = new XmlDocument();

            System.Configuration.AppSettingsReader s = new AppSettingsReader();
            try
            {
                string logFolder = (string)s.GetValue("LogFolder", typeof(string));
                xmlDoc.LoadXml(wrkData);
                xmlDoc.Save(logFolder + @"\" + RetrievalReferenceNumber + ".xml");
            }
            catch (Exception)
            {
            }
        }

        private void SetFlexCubeAccounts(ComponentState State)
        {
            string ContaEMIS = State.OrchWrkData.GetWrkData().ReadNodeValue("Conta", true);
            string NumeroContaDebito = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroContaDebito", true);
            string NumeroContaCredito = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroContaCredito", true);

            if (ContaEMIS != null)
            {
                ContaEMIS = ContaEMIS.PadLeft(15, '0');
                if (ContaEMIS.Substring(0, 4) == "0000")
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ContaParaFlexCube", ContaEMIS.Substring(4, 11), true);
                else
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ContaParaFlexCube", ContaEMIS.Substring(1, 14), true);
            }

            if (ContaEMIS != null && NumeroContaDebito == "" && NumeroContaCredito == "")
            {
                WD2FlexCubeField(State, "ContaParaFlexCube", "FromAccountNumber", true);
            }
            else if (ContaEMIS != null && NumeroContaDebito != "" && NumeroContaCredito == "")
            {
                WD2FlexCubeField(State, "NumeroContaDebito", "FromAccountNumber", true);
                WD2FlexCubeField(State, "ContaParaFlexCube", "ToAccountNumber", true);
            }
            else if (ContaEMIS != null && NumeroContaDebito == "" && NumeroContaCredito != "")
            {
                WD2FlexCubeField(State, "ContaParaFlexCube", "FromAccountNumber", true);
                WD2FlexCubeField(State, "NumeroContaCredito", "ToAccountNumber", true);
            }
            else if (ContaEMIS != null && NumeroContaDebito != "" && NumeroContaCredito != "")
            {
                WD2FlexCubeField(State, "NumeroContaDebito", "FromAccountNumber", true);
                WD2FlexCubeField(State, "NumeroContaCredito", "ToAccountNumber", true);
            }
            else if (ContaEMIS == null && NumeroContaDebito != "" && NumeroContaCredito != "")
            {
                WD2FlexCubeField(State, "NumeroContaDebito", "FromAccountNumber", true);
                WD2FlexCubeField(State, "NumeroContaCredito", "ToAccountNumber", true);
            }
            else if (ContaEMIS == null && NumeroContaDebito != "")
            {
                WD2FlexCubeField(State, "NumeroContaDebito", "FromAccountNumber", true);
            }

            StoreTransactionField(State, "FromAccountNumber", "NumeroContaDebito");
            StoreTransactionField(State, "ToAccountNumber", "NumeroContaCredito");
        }

        private void StoreTransactionField(ComponentState State, string WorkDataField, string TrnField)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(WorkDataField) != null)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue(TrnField, State.OrchWrkData.GetWrkData().ReadNodeValue(WorkDataField, true), true);
            }
        }

        private void ProcessServiceRequest(OrchPipeComponent.ComponentState State)
        {
            // call service specific component
            string ServiceRequestTypeName = State.OrchWrkData.GetWrkData().ReadNodeValue("ServiceRequestTypeName", true);

            //Service do not have specific response
            if (ServiceRequestTypeName==null || ServiceRequestTypeName == "")
                return;

            Type myType = Type.GetType(ServiceRequestTypeName);
            try
            {
                myType.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { State });
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("PipelineService:PrepareConstructor", exp);
            }
        }

        private void WD2FlexCubeField(OrchPipeComponent.ComponentState State, string WDFieldName, string FlexCubeFieldName, bool Mandatory)
        {
            if (Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName)==null)
                throw new BusinessException("Mandatory Field <" + WDFieldName + "> not present on PRT message");

            string wdField = State.OrchWrkData.GetWrkData().ReadNodeValue(WDFieldName);
            State.OrchWrkData.GetWrkData().WriteNodeValue(FlexCubeFieldName, wdField, true);
        }
        private void WD2FlexCubeField(OrchPipeComponent.ComponentState State, string WDFieldName, int srcStartIndex, string FlexCubeFieldName, bool Mandatory)
        {
            if (Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                throw new BusinessException("Mandatory Field <" + WDFieldName + "> not present on PRT message");

            string wdField = State.OrchWrkData.GetWrkData().ReadNodeValue(WDFieldName);
            State.OrchWrkData.GetWrkData().WriteNodeValue(FlexCubeFieldName, wdField.Substring(srcStartIndex),true);
        }
        private void WD2FlexCubeField(OrchPipeComponent.ComponentState State, string WDFieldName, int srcStartIndex, int lenToCopy, string FlexCubeFieldName, bool Mandatory)
        {
            if (Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                throw new BusinessException("Mandatory Field <" + WDFieldName + "> not present on PRT message");

            string wdField = State.OrchWrkData.GetWrkData().ReadNodeValue(WDFieldName);
            State.OrchWrkData.GetWrkData().WriteNodeValue(FlexCubeFieldName, wdField.Substring(srcStartIndex,lenToCopy),true);
        }
        private void WD2FlexCubeField(OrchPipeComponent.ComponentState State, string WDFieldName, string FlexCubeFieldName, int destSize, bool IsString, bool Mandatory)
        {
            if (Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                throw new BusinessException("Mandatory Field <" + WDFieldName + "> not present on PRT message");

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
            State.OrchWrkData.GetWrkData().WriteNodeValue(FlexCubeFieldName, wdField, true);
        }
        private void WD2FlexCubeField(OrchPipeComponent.ComponentState State, string WDFieldName, int srcStartIndex, string FlexCubeFieldName, int destSize, bool IsString, bool Mandatory)
        {
            if (Mandatory && State.OrchWrkData.GetWrkData().GetNodeByName(WDFieldName) == null)
                throw new BusinessException("Mandatory Field <" + WDFieldName + "> not present on PRT message");

            string wdField = State.OrchWrkData.GetWrkData().ReadNodeValue(WDFieldName);
            wdField = wdField.Substring(srcStartIndex);

            if (wdField.Length > destSize)
                wdField = wdField.Substring(wdField.Length - destSize);
            else
            {
                if (IsString)
                    wdField = wdField.PadRight(destSize, ' ');
                else
                    wdField = wdField.PadLeft(destSize, '0');

            }
            State.OrchWrkData.GetWrkData().WriteNodeValue(FlexCubeFieldName, wdField, true);
        }

    }
}
