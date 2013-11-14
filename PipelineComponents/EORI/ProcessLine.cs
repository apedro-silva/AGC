using System;
using System.Text;
using System.IO;
using System.Data.Common;
using System.Data;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    public class ProcessEORILine :OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;


            StreamReader sr = (StreamReader)State.OrchWrkData.GetFromObjBucket("FileStreamReader");
            try
            {
                string line = string.Empty;

                Boolean processResult = false;
                string TIPREG = "";
                while ((line = sr.ReadLine()) != null)
                {
                    CleanWorkData(State);

                    if (line.Trim() == "") continue;
                    // para o JE
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EMISFileRecord", line, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("RubricaContabilistica", "", true);

                    // Set message for Message parser
                    byte[] EORILine = Encoding.ASCII.GetBytes(line);
                    State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, EORILine);

                    TIPREG = line.Substring(0, 1);
                    DoParseLine(State, line, "EORI_TIPREG_" + TIPREG);

                    processResult = ProcessTransaction(State, line);

                    if (!processResult) break;
                }

                DeleteWorkDataNode(State, "EMISFileRecord");

                // actualiza Log do Ficheiro
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro processado", true);
                State.ResetLastError();
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
            }
            finally
            {
                FinalizeFileJE finJE = new FinalizeFileJE();
                finJE.RunComponent(State.OrchWrkData, new string[] { "JEFicheiroEMIS", "E1" });

                if (sr != null)
                    sr.Close();
            }
            return;
        }

        private bool ProcessTransaction(OrchPipeComponent.ComponentState State, string line)
        {
            FinalizeJE finJE = new FinalizeJE();
            InitializeJE initJe = new InitializeJE();
            EMISRecordLog recordLog = new EMISRecordLog();

            bool result = false;
            string TIPREG = line.Substring(0, 1);
            switch (TIPREG)
            {
                // if TIPREG=2 -> Totais de Levantamentos
                case "2":
                    string AGATM = State.OrchWrkData.GetWrkData().ReadNodeValue("AgAtm", true);
                    DoFundsTransfer("TA", State, AGATM, "ImportLev", "SinalImportLev");
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "TA", true);
                    result = true;
                    break;

                // if TIPREG=3 -> Tarifas
                case "3":
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "TF", true);
                    if (IsTarintPresent(State, "TarInt1", "ValTarInt1"))
                        DoFundsTransfer("TF", State, State.OrchWrkData.GetWrkData().ReadNodeValue("TarInt1", true), "ValTarInt1", "SinalValTarInt1");

                    if (IsTarintPresent(State, "TarInt2", "ValTarInt2"))
                        DoFundsTransfer("TF", State, State.OrchWrkData.GetWrkData().ReadNodeValue("TarInt2", true), "ValTarInt2", "SinalValTarInt2");

                    if (IsTarintPresent(State, "TarInt3", "ValTarInt3"))
                        DoFundsTransfer("TF", State, State.OrchWrkData.GetWrkData().ReadNodeValue("TarInt3", true), "ValTarInt3", "SinalValTarInt3");
                    
                    result = true;
                    break;
                case "9":
                    // Loga registo
                    initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "EH", true);
                    recordLog.RunComponent(State.OrchWrkData, new string[] { "JE", "Montante2", "NumeroContaDebito", "NumeroContaCredito", null});
                    finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    break;
                default:
                    // Loga registo
                    initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "00", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Tipo registo " + TIPREG + " no EDST não suportado", true);
                    recordLog.RunComponent(State.OrchWrkData, new string[] { "JE", "Montante2", "NumeroContaDebito", "NumeroContaCredito", null});
                    finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    result = true;
                    break;
            }
            return result;
        }
        private Boolean IsTarintPresent(OrchPipeComponent.ComponentState State, string fieldNameToCalculate, string valueToCalculate)
        {
            string ValTarInt = State.OrchWrkData.GetWrkData().ReadNodeValue(valueToCalculate, true);
            string TARINT = State.OrchWrkData.GetWrkData().ReadNodeValue(fieldNameToCalculate, true);
            if (TARINT == null || TARINT == "0" || TARINT.Trim() == "")
                return false;
            else
                return true;
        }

        private string CalculateTaxes(string ValTarint1, string ValTarint2, string ValTarint3)
        {
            long lResult=0, lValtarint1=0, lValtarint2=0, lValtarint3=0;
            if (ValTarint1 != null)
                lValtarint1 = Convert.ToInt64(ValTarint1);
            if (ValTarint2 != null)
                lValtarint2 = Convert.ToInt64(ValTarint2);
            if (ValTarint3 != null)
                lValtarint3 = Convert.ToInt64(ValTarint3);
            lResult = lValtarint1 + lValtarint2 + lValtarint3;
            return (Convert.ToString(lResult,10).PadLeft(12,'0'));
        }

        private void DoParseLine(ComponentState State, string line, string ParseMessageName)
        {
            // Set message for Message parser
            byte[] EDSTLine = Encoding.ASCII.GetBytes(line);
            State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, EDSTLine);
            MessageParser msgParser = new MessageParser();
            msgParser.RunComponent(State.OrchWrkData, new string[] { "#c#" + ParseMessageName });
            if (State.IsInError) return;
        }

        private void DoFundsTransfer(string CodTrn, OrchPipeComponent.ComponentState State, string RubricaContabilistica, string WDValueField, string SinalMontante)
        {

            FinalizeJE finJE = new FinalizeJE();
            InitializeJE initJe = new InitializeJE();
            EMISRecordLog recordLog = new EMISRecordLog();

            DeleteWorkDataNode(State, "RetrievalReferenceNumber");

            // Para FlexCube só existe um Montante
            State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", State.OrchWrkData.GetWrkData().ReadNodeValue(WDValueField, true), true);

            // afectar FC Msg Constructor
            State.OrchWrkData.GetWrkData().WriteNodeValue("TipoTerm", "A", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", CodTrn, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);

            string DtHora = State.OrchWrkData.GetWrkData().ReadNodeValue("DthSuperv", true);
            if (DtHora == "000000000000")
                DtHora = State.OrchWrkData.GetWrkData().ReadNodeValue("DataFicheiroEMIS", true) + "00";

            DtHora = DtHora + "00";
            State.OrchWrkData.GetWrkData().WriteNodeValue("DtHora", DtHora, true);

            // Loga registo
            initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });

            DoFlexCubeTransaction(State, RubricaContabilistica, SinalMontante);

            recordLog.RunComponent(State.OrchWrkData, new string[] { "JE", "Montante2", "NumeroContaDebito", "NumeroContaCredito", SinalMontante });
            finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });

            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCreditoTrn", State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroContaCredito", true), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebitoTrn", State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroContaDebito", true), true);

            State.ResetLastError();
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
                        throw new BusinessException(string.Format("Código de terminal {0} não parametrizado no AGC", RubricaContabilistica));

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
                State.LastError = new BusinessException("EORI.ProcessLine.SetHostAccount", exp);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
            }
        }

        private void CleanWorkData(ComponentState State)
        {
            DeleteWorkDataNode(State, "EstadoRegisto");
            DeleteWorkDataNode(State, "SituacaoRegistoEMIS");
            DeleteWorkDataNode(State, "CodTrn");
            DeleteWorkDataNode(State, "NumeroContaDebitoTrn");
            DeleteWorkDataNode(State, "NumeroContaCreditoTrn");
            DeleteWorkDataNode(State, "NumeroContaDebito");
            DeleteWorkDataNode(State, "NumeroContaCredito");
            DeleteWorkDataNode(State, "TransactionCurrencyCode");
            DeleteWorkDataNode(State, "FromAccountNumber");
            DeleteWorkDataNode(State, "ToAccountNumber");
            DeleteWorkDataNode(State, "MessageTypeOriginal");
            DeleteWorkDataNode(State, "STANOriginal");
            DeleteWorkDataNode(State, "TransmissionDateTimeOriginal");
            DeleteWorkDataNode(State, "AcquiringInstitutionIdentificationCodeOriginal");
            DeleteWorkDataNode(State, "RetrievalReferenceNumber");
            DeleteWorkDataNode(State, "CardAcceptorName");
            DeleteWorkDataNode(State, "TipoTerm");

            DeleteWorkDataNode(State, "DescricaoAbreviaturaServico");
            DeleteWorkDataNode(State, "DescricaoServico");
            DeleteWorkDataNode(State, "ContaParaFlexCube");
            DeleteWorkDataNode(State, "Conta");
            DeleteWorkDataNode(State, "CardAcceptorTerminalIdentification");
            DeleteWorkDataNode(State, "SystemTraceAuditNumber");
            DeleteWorkDataNode(State, "AcquiringInstitutionIdentificationCode");
            DeleteWorkDataNode(State, "ForwardingInstitutionIdentificationCode");
            DeleteWorkDataNode(State, "PrimaryAccountNumber");
            DeleteWorkDataNode(State, "BanApoio");
            DeleteWorkDataNode(State, "CodigoBanco");

            DeleteWorkDataNode(State, "IdTerminal");
            DeleteWorkDataNode(State, "CodigoTerminal");

            DeleteWorkDataNode(State, "Montante2");
            DeleteWorkDataNode(State, "TransactionAmount");
            DeleteWorkDataNode(State, "OriginalDataElements");
            DeleteWorkDataNode(State, "fc-ResponseCode");
            DeleteWorkDataNode(State, "NrIdResp");

            DeleteWorkDataNode(State, "AplicPdd");
            DeleteWorkDataNode(State, "IdLog");
            DeleteWorkDataNode(State, "NrLog");
            DeleteWorkDataNode(State, "MessageType");
            DeleteWorkDataNode(State, "SystemTraceAuditNumber");
            DeleteWorkDataNode(State, "TransmissionDateTime");
            DeleteWorkDataNode(State, "AcquiringInstitutionIdentificationCode");
            DeleteWorkDataNode(State, "FlexCubeRequest");
            DeleteWorkDataNode(State, "FlexCubeResponse");
            DeleteWorkDataNode(State, "BIN");
            DeleteWorkDataNode(State, "ExBin");
            DeleteWorkDataNode(State, "NumCar");
            DeleteWorkDataNode(State, "SeqMov");
            DeleteWorkDataNode(State, "Servico");
        }
        private void DeleteWorkDataNode(ComponentState State, string WorkDataField)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(WorkDataField) != null)
                State.OrchWrkData.GetWrkData().DeleteNode(WorkDataField);
        }

        private void DoFlexCubeTransaction(OrchPipeComponent.ComponentState State, string RubricaContabilistica, string SinalMontante)
        {
            string WDValue = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2", true);
            if (WDValue == null || Convert.ToInt32(WDValue) == 0)
                return;


            if (!State.IsInError)
            {
                string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
                GetParameters getParams = new GetParameters();
                getParams.RunComponent(State.OrchWrkData, new string[] { CodTrn });
            }

            if (!State.IsInError && RubricaContabilistica != null)
            {
                SetHostAccount(State, RubricaContabilistica, SinalMontante);
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

            State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "1", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("SituacaoRegistoEMIS", "1", true);

        }
    }
}
