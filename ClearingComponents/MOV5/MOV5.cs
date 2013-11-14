using System;
using System.Text;
using System.IO;
using System.Data.Common;
using System.Data;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Globalization;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.Clearing
{
    public class MOV5 : OrchPipeComponent
    {
        private int nrLogSeq = 0;
        private string sessionId = null;

        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;

            string FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("InputFileName");
            StreamReader sr = new StreamReader(FileName, Encoding.ASCII);
            String line = sr.ReadLine();
            try
            {
                InitializeJE initJe = new InitializeJE();
                FinalizeJE finJE = new FinalizeJE();

                string TIPREG = "";

                while ((line = sr.ReadLine()) != null)
                {
                    State.ResetLastError();
                    CleanWorkData(State);
                    if (line.Trim() == "") continue;
                    TIPREG = line.Substring(0, 1);

                    State.OrchWrkData.GetWrkData().WriteNodeValue("EMISFileRecord", line, true);

                    // Loga registo
                    initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });

                    DoParseLine(State, line, "MOV5_TIPREG_" + TIPREG);

                    if (sessionId==null)
                        sessionId = State.OrchWrkData.GetWrkData().ReadNodeValue("IdLog", true);

                    if (State.IsInError)
                    {
                        finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                        continue;
                    }

                    ProcessTransaction(State, line);

                    // repõe dados do cliente
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ModEnvTrn", State.OrchWrkData.GetWrkData().ReadNodeValue("ModEnv", true), true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TpProcTrn", State.OrchWrkData.GetWrkData().ReadNodeValue("TpProc", true), true);

                    RecallTransactionField(State, "AplicPddOriginalTrn", "AplicPddOriginal");
                    RecallTransactionField(State, "IdLogOriginalTrn", "IdLogOriginal");
                    RecallTransactionField(State, "NrLogOriginalTrn", "NrLogOriginal");

                    RecallTransactionField(State, "AplicPddTrn", "AplicPdd");
                    RecallTransactionField(State, "IdLogTrn", "IdLog");
                    RecallTransactionField(State, "NrLogTrn", "NrLog");

                    RecallTransactionField(State, "fc-ResponseCodeTrn", "fc-ResponseCode");
                    RecallTransactionField(State, "FlexCubeRequestTrn", "FlexCubeRequest", true);
                    RecallTransactionField(State, "FlexCubeResponseTrn", "FlexCubeResponse", true);
                    RecallTransactionField(State, "TextoErroTrn", "TextoErro");
                    RecallTransactionField(State, "RubricaContabilisticaTrn", "RubricaContabilistica");
                    RecallTransactionField(State, "SystemTraceAuditNumberTrn", "SystemTraceAuditNumber");
                    RecallTransactionField(State, "EstadoRegistoTrn", "EstadoRegisto");
                    RecallTransactionField(State, "RetrievalReferenceNumberTrn", "RetrievalReferenceNumber");
                    RecallTransactionField(State, "TipoServicoTrn", "TipoServico");
                    RecallTransactionField(State, "TipoTermTrn", "TipoTerm");
                    RecallTransactionField(State, "DescricaoServicoTrn", "DescricaoServico");
                    RecallTransactionField(State, "BanApoioTrn", "BanApoio");
                    RecallTransactionField(State, "SituacaoRegistoEMISTrn", "SituacaoRegistoEMIS");

                    EMISRecordLog.DoEMISRecordLog(State, new string[] { "JE", "Montante", "NumeroContaDebitoTrn", "NumeroContaCreditoTrn", "SinalMontante" });
                    finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                }
                DeleteWorkDataNode(State, "Comerciante");

                // Anula transacções Real-time não presentes neste ficheiro
                ReverseTransactionsNotConfirmed(State);

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
                if (sr != null)
                    sr.Close();
                FinalizeFileJE finJE = new FinalizeFileJE();
                finJE.RunComponent(State.OrchWrkData, new string[] { "JEFicheiroEMIS", "V05" });
            }
            return;
        }

        private void ReverseTransactionsNotConfirmed(ComponentState State)
        {
            InitializeJE initJe = new InitializeJE();
            FinalizeJE finJE = new FinalizeJE();

            // Obtem lista de transacções 1100 em Real-time não confirmados neste ficheiro
            DataSet notConfirmedDS = Get1100NotConfirmed(State);

            // Percorre lista de transacções a anular
            foreach (DataRow myRow in notConfirmedDS.Tables[0].Rows)
            {
                try
                {
                    CleanWorkData(State);

                    // Parsa registo 1100 para workdata
                    string Registo = myRow[0].ToString();
                    string PRTCodMsg = Registo.Substring(0, 4);
                    DoParseLine(State, Registo, "PRT_" + PRTCodMsg);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", myRow[1].ToString(), true);

                    // Inicia Log da transacção
                    initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });

                    //Anula transacção
                    ReverseTransaction(State);
                }
                catch (Exception exp)
                {
                    State.LastError = new BusinessException(exp.Message);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
                }
                finally
                {
                    // Actualiza Log da transacção
                    finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    State.ResetLastError();
                }
            }
        }

        private void ReverseTransaction(ComponentState State)
        {
            string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);

            // obtem parametros transacção original
            string AplicPddOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPdd", true);
            string IdLogOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("IdLog", true);
            string NrLogOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("NrLog", true);

            // Altera AplicPdd para novo valor o que permite uma nova transacção
            State.OrchWrkData.GetWrkData().WriteNodeValue("AplicPdd", "X", true);

            // Queremos uma anulação
            State.OrchWrkData.GetWrkData().DeleteNode("CodMsg");
            State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "1", true);

            // Vamos anular a 1100 original
            State.OrchWrkData.GetWrkData().WriteNodeValue("AplicPddOriginal", AplicPddOriginal, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("IdLogOriginal", IdLogOriginal, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("NrLogOriginal", NrLogOriginal, true);

            DoFlexCubeTransaction(State, CodTrn, null, "Montante2", false, null, null);
            //State.OrchWrkData.GetWrkData().WriteNodeValue("SituacaoRegistoEMIS", State.OrchWrkData.GetWrkData().ReadNodeValue("EstadoRegisto", true), true);
        }

        private DataSet Get1100NotConfirmed(ComponentState State)
        {
            Database db = DatabaseFactory.CreateDatabase("BESASwitch");
            DbCommand dbCommand = db.GetStoredProcCommand("Obtem1100NaoConfirmadas");
            string JEFicheiroEMIS = State.OrchWrkData.GetWrkData().ReadNodeValue("JEFicheiroEMIS", true);
            db.AddInParameter(dbCommand, "JEFicheiroEMIS", DbType.String, JEFicheiroEMIS);
            return db.ExecuteDataSet(dbCommand);
        }

        private bool ProcessTransaction(OrchPipeComponent.ComponentState State, string line)
        {
            bool result = true;
            string TIPREG = line.Substring(0, 1);

            string CODTRN = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
            string TPROC = State.OrchWrkData.GetWrkData().ReadNodeValue("TpProc", true);
            string MODENV = State.OrchWrkData.GetWrkData().ReadNodeValue("ModEnv", true);
            string TIPOTERMINAL = State.OrchWrkData.GetWrkData().ReadNodeValue("TipoTerm", true);

            string RC = string.Empty;

            switch (TIPREG)
            {
                // if TIPREG=3 -> apoio a comerciantes e empresas com pagamento serviços
                // se TIPREG=3
                case "3":
                    // Funds Transfer always go to the ATM Server
                    State.OrchWrkData.GetWrkData().WriteNodeValue("BanApoio", "0000", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);

                    GetNetValue(State, "Montante", "SinalMontante", "C", "MontanteAdicional", "SinalMontanteAdicional", "D");
                    string montante = State.OrchWrkData.GetWrkData().ReadNodeValue("MontanteFinal", true);
                    decimal decMontante = Convert.ToDecimal(montante);

                    if (decMontante>0)
                        DoFlexCubeTransaction(State, CODTRN, null, "Montante", true, "SinalMontante", null);

                    SetMainTransactionKeys(State);

                    // keep main record information
                    StoreTransactionFields(State);

                    break;
                case "4":
                    // keep main record information
                    StoreTransactionFields(State);

                    // Funds Transfer always go to the ATM Server

                    State.OrchWrkData.GetWrkData().WriteNodeValue("BanApoio", "0000", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TipoTerm", TIPOTERMINAL, true);

                    GetNetValue(State, "MontPDst", "SinalMontPDst", "D", "MontRDst", "SinalMontRDst", "C");

                    switch (CODTRN)
                    {
                        case "033": DoFundsTransfer(State, "T3", null, "MontanteFinal", "SinalMontanteFinal");
                                    break;
                        case "035": DoFundsTransfer(State, "T5", null, "MontRDst", "SinalMontRDst");
                                    DoFundsTransfer(State, "T4", null, "MontPDst", "SinalMontPDst");
                                    break;
                        default:    break; 
                    }

                    break;

                case "5":
                    // keep main record information
                    StoreTransactionFields(State);

                    string Montante = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante", true);
                    string SISTEMA = State.OrchWrkData.GetWrkData().ReadNodeValue("Sistema", true);
                    string SISTPAG = State.OrchWrkData.GetWrkData().ReadNodeValue("SistPag", true);
                    string AVCUTOFF = State.OrchWrkData.GetWrkData().ReadNodeValue("AvCutOff", true);
                    string BPAGApoio = State.OrchWrkData.GetWrkData().ReadNodeValue("BPAGApoio", true);
                    string TarSibs = State.OrchWrkData.GetWrkData().ReadNodeValue("TarSibs", true);
                    TarSibs = TarSibs.Trim();

                    string servico = "TXX";

                    if ((SISTEMA == "2" || SISTEMA == "9" ) && SISTPAG == "9" && AVCUTOFF == "2")
                    {
                        servico = "T7";
                    }
                    else if ((SISTEMA == "2" || SISTEMA == "9") && SISTPAG == "C" && AVCUTOFF == "2" && BPAGApoio == "2")
                    {
                        servico = "T8";
                    }
                    else if ((SISTEMA == "2" || SISTEMA == "9") && SISTPAG == "C" && AVCUTOFF == "2" && BPAGApoio == "3")
                    {
                        servico = "T9";
                    }
                    else if ((SISTEMA == "2") && SISTPAG == " " && AVCUTOFF == "2" && BPAGApoio == "2" && TarSibs=="G51")
                    {
                        servico = "T10";
                    }
                    else if ((SISTEMA == "2") && SISTPAG == " " && AVCUTOFF == "2" && BPAGApoio == "2" && TarSibs == "G61")
                    {
                        servico = "T13";
                    }
                    else if ((SISTEMA == "9") && SISTPAG == "C" && AVCUTOFF == "2" && BPAGApoio == "6")
                    {
                        servico = "T11";
                    }
                    else

                    GetNetValue(State, "Montante", "SinalMontante", "D", "MontanteAdicional", "SinalMontanteAdicional", "C");
                    DoFundsTransfer(State, servico, null, "Montante", "SinalMontante");

                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "EE1", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErroTrn", "OK", true);

                    break;
                case "9":
                    int TOTREG = Convert.ToInt32(State.OrchWrkData.GetWrkData().ReadNodeValue("TotReg", true));
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "EI1", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErroTrn", "OK", true);
                    result = false;
                    break;
                default:
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TipReg", TIPREG, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "00", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErroTrn", "Tipo registo " + TIPREG + " no MOV5 não suportado", true);
                    break;
            }
            return result;
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

        private void StoreTransactionFields(ComponentState State)
        {
            StoreTransactionField(State, "fc-ResponseCode", "fc-ResponseCodeTrn");
            StoreTransactionField(State, "FlexCubeRequest", "FlexCubeRequestTrn", true);
            StoreTransactionField(State, "FlexCubeResponse", "FlexCubeResponseTrn", true);
            StoreTransactionField(State, "NumeroContaCredito", "NumeroContaCreditoTrn");
            StoreTransactionField(State, "NumeroContaDebito", "NumeroContaDebitoTrn");

            StoreTransactionField(State, "AplicPddOriginal", "AplicPddOriginalTrn");
            StoreTransactionField(State, "IdLogOriginal", "IdLogOriginalTrn");
            StoreTransactionField(State, "NrLogOriginal", "NrLogOriginalTrn");

            StoreTransactionField(State, "AplicPdd", "AplicPddTrn");
            StoreTransactionField(State, "IdLog", "IdLogTrn");
            StoreTransactionField(State, "NrLog", "NrLogTrn");

            StoreTransactionField(State, "TextoErro", "TextoErroTrn");
            StoreTransactionField(State, "RubricaContabilistica", "RubricaContabilisticaTrn");
            StoreTransactionField(State, "EstadoRegisto", "EstadoRegistoTrn");
            StoreTransactionField(State, "RetrievalReferenceNumber", "RetrievalReferenceNumberTrn");
            StoreTransactionField(State, "SystemTraceAuditNumber", "SystemTraceAuditNumberTrn");
            StoreTransactionField(State, "IndStr", "IndStrTrn");
            StoreTransactionField(State, "TipoServico", "TipoServicoTrn");
            StoreTransactionField(State, "TipoTerm", "TipoTermTrn");
            StoreTransactionField(State, "DescricaoServico", "DescricaoServicoTrn");
            StoreTransactionField(State, "SituacaoRegistoEMIS", "SituacaoRegistoEMISTrn");
            StoreTransactionField(State, "BanApoio", "BanApoioTrn");
        }


        private void DoFundsTransfer(ComponentState State, string CodTrn, string RubricaContabilistica, string WDValueField, string SinalMontante)
        {
            SetTransactionKeys(State);

            string WDValue = State.OrchWrkData.GetWrkData().ReadNodeValue(WDValueField, true);
            // se montante=0 não vai ao FlexCube
            if (WDValue == null || Convert.ToInt32(WDValue) == 0)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
                return;
            }
            DeleteWorkDataNode(State, "AplicPddOriginal");
            DeleteWorkDataNode(State, "IdLogOriginal");
            DeleteWorkDataNode(State, "NrLogOriginal");

            DeleteWorkDataNode(State, "RetrievalReferenceNumber");

            // Funds Transfer always go to the ATM Server
            State.OrchWrkData.GetWrkData().WriteNodeValue("BanApoio", "0000", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);

            // Loga registo
            InitializeJE initJe = new InitializeJE();
            initJe.RunComponent(State.OrchWrkData, new string[] { "JEFundsTransfer" });

            DoFlexCubeTransaction(State, CodTrn, RubricaContabilistica, WDValueField, false, SinalMontante, "JEFundsTransfer");

            EMISRecordLog.DoEMISRecordLog(State, new string[] { "JEFundsTransfer", WDValueField, "NumeroContaDebito", "NumeroContaCredito", SinalMontante });
            FinalizeJE finJE = new FinalizeJE();
            finJE.RunComponent(State.OrchWrkData, new string[] { "JEFundsTransfer", CodTrn });
            State.ResetLastError();
        }

        private void DoFlexCubeTransaction(OrchPipeComponent.ComponentState State, string CodTrn, string RubricaContabilistica, string WDValueField, bool OffLine, string SinalMontante, string JEWorkDataField)
        {
            string WDValue = State.OrchWrkData.GetWrkData().ReadNodeValue(WDValueField, true);

            // Para FlexCube só existe um Montante
            State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", WDValue, true);

            //if (WDValue == null || Convert.ToInt32(WDValue) == 0)
            //{
            //    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErroTrn", "OK", true);
            //    return;
            //}

            if (!State.IsInError)
            {
                GetParameters getParams = new GetParameters();
                getParams.RunComponent(State.OrchWrkData, new string[] { CodTrn });
            }

            if (!State.IsInError && OffLine)
            {
                string MessageType = State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true);
                if (MessageType != "" && MessageType.Substring(1, 1) == "2")
                {
                    MessageType = MessageType.Substring(0, 2) + "2" + MessageType.Substring(3);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("MessageType", MessageType, true);
                }
            }

            if (RubricaContabilistica == null)
                RubricaContabilistica = State.OrchWrkData.GetWrkData().ReadNodeValue("RubricaContabilistica", true);

            if (!State.IsInError && RubricaContabilistica != "")
            {
                SetHostAccount(State, RubricaContabilistica, SinalMontante);
            }

            if (!State.IsInError)
            {
                PrepareConstructor prepConst = new PrepareConstructor();
                prepConst.RunComponent(State.OrchWrkData, new string[] { JEWorkDataField });
            }

            bool IsTrnInDB = IsTransactionInDB(State, CodTrn);
            string CODTRN = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
            string TPROC = State.OrchWrkData.GetWrkData().ReadNodeValue("TpProc", true);
            string MODENV = State.OrchWrkData.GetWrkData().ReadNodeValue("ModEnv", true);

            // submete ao FlexCube transacções recebidas em Real-Time e não presentes na base de dados
            if (TransactionProcessed(TPROC, MODENV, CODTRN) && !IsTrnInDB)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("SituacaoRegistoEMIS", "55", true);
            }
            //Submete ao FlexCube transacções não recebidas em real-time
            else if (TransactionNotProcessed(TPROC, MODENV, CODTRN) && !IsTrnInDB)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("SituacaoRegistoEMIS", "55", true);
            }

            string DtHora = State.OrchWrkData.GetWrkData().ReadNodeValue("DtHora", true);
            if (DtHora == "19700101000000")
                DtHora = State.OrchWrkData.GetWrkData().ReadNodeValue("DataFicheiroEMIS", true) + "00";

            DtHora = DtHora + "00";
            State.OrchWrkData.GetWrkData().WriteNodeValue("DtHora", DtHora, true);

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

        private Boolean IsTransactionInDB(OrchPipeComponent.ComponentState State, string CodTrn)
        {
            try
            {
                string AplicPDD = State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPdd", true);
                string IdLog = State.OrchWrkData.GetWrkData().ReadNodeValue("IdLog", true);
                string NrLog = State.OrchWrkData.GetWrkData().ReadNodeValue("NrLog", true);

                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("VerificaTransaccaoEMIS");
                db.AddInParameter(dbCommand, "AplicPDD", DbType.String, AplicPDD);
                db.AddInParameter(dbCommand, "IdLog", DbType.String, IdLog);
                db.AddInParameter(dbCommand, "NrLog", DbType.String, NrLog);
                db.AddInParameter(dbCommand, "CodTrn", DbType.String, CodTrn);
                db.AddOutParameter(dbCommand, "JE", DbType.Int64, 6);
                db.ExecuteDataSet(dbCommand);
                string JE = db.GetParameterValue(dbCommand, "JE").ToString();
                if (JE != "")
                    return true;
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "MOV5.ProcessLine.IsTransactionInDB exception->" + exp.Message, true);
            }
            return false;
        }

        private void CleanWorkData(ComponentState State)
        {
            DeleteWorkDataNode(State, "TipoServico");
            DeleteWorkDataNode(State, "IndStr");
            DeleteWorkDataNode(State, "EstadoRegisto");
            DeleteWorkDataNode(State, "SituacaoRegistoEMIS");
            DeleteWorkDataNode(State, "CodTrn");
            DeleteWorkDataNode(State, "TpProc");
            DeleteWorkDataNode(State, "ModEnv");
            DeleteWorkDataNode(State, "TpProcTrn");
            DeleteWorkDataNode(State, "ModEnvTrn");
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
            DeleteWorkDataNode(State, "RubricaContabilistica");
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
            DeleteWorkDataNode(State, "Montante");
            DeleteWorkDataNode(State, "MontanteComissoesBanco");
            
            DeleteWorkDataNode(State, "ComEmi");
            DeleteWorkDataNode(State, "ComProp");
            DeleteWorkDataNode(State, "TaxaCli");
            DeleteWorkDataNode(State, "ValCom");
            DeleteWorkDataNode(State, "ComEmiA");
            DeleteWorkDataNode(State, "ComPropA");
            DeleteWorkDataNode(State, "ComEmiB");
            DeleteWorkDataNode(State, "ComPropB");
            DeleteWorkDataNode(State, "TaxaCliA");
            DeleteWorkDataNode(State, "TaxaCliB");
            DeleteWorkDataNode(State, "TotCli");
            DeleteWorkDataNode(State, "TotCliA");
            DeleteWorkDataNode(State, "TotCliB");
            DeleteWorkDataNode(State, "MontPDst");
            DeleteWorkDataNode(State, "MontRDst");

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
            DeleteWorkDataNode(State, "TarSibs");
            DeleteWorkDataNode(State, "TarInt1");
            DeleteWorkDataNode(State, "TarInt2");
        }

        private void SetTransactionKeys(OrchPipeComponent.ComponentState State)
        {
            string aplicPdd = State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPdd", true);
            if (aplicPdd == null || aplicPdd.Trim() == "")
                aplicPdd = "AGC";

            string nrLog = State.OrchWrkData.GetWrkData().ReadNodeValue("NrLog", true);
            if (nrLog == null || nrLog.Trim() == "" || nrLog.Equals("00000000"))
                nrLog = "AGC";

            nrLogSeq += 1;
            State.OrchWrkData.GetWrkData().WriteNodeValue("NrLog", "M" + nrLogSeq.ToString().PadLeft(7, '0'), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("IdLog", sessionId, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("AplicPdd", aplicPdd, true);
        }
        /// <summary>
        /// SetMainTransactionKeys : 033 have NrLog, 035 service does not have NrLog
        /// </summary>
        /// <param name="State">ComponentState State</param>
        private void SetMainTransactionKeys(OrchPipeComponent.ComponentState State)
        {
            string aplicPdd = State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPdd", true);
            if (aplicPdd == null || aplicPdd.Trim() == "")
                aplicPdd = "AGC";

            string nrLog = State.OrchWrkData.GetWrkData().ReadNodeValue("NrLog", true);
            if (nrLog == null || nrLog.Trim() == "" || nrLog.Equals("00000000"))
                nrLogSeq += 1;
            else
                nrLogSeq = int.Parse(nrLog);

            State.OrchWrkData.GetWrkData().WriteNodeValue("NrLog", nrLogSeq.ToString().PadLeft(8, '0'), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("IdLog", sessionId, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("AplicPdd", aplicPdd, true);
        }

        private void StoreTransactionField(ComponentState State, string WorkDataField, string TrnField)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(WorkDataField) != null)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue(TrnField, State.OrchWrkData.GetWrkData().ReadNodeValue(WorkDataField, true), true);
                State.OrchWrkData.GetWrkData().DeleteNode(WorkDataField);
            }
        }
        private void StoreTransactionField(ComponentState State, string WorkDataField, string TrnField, bool NodeBuffer)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(WorkDataField) != null)
            {
                State.OrchWrkData.GetWrkData().WriteNodeBuffer(TrnField, State.OrchWrkData.GetWrkData().ReadNodeBuffer(WorkDataField, true), true);
                State.OrchWrkData.GetWrkData().DeleteNode(WorkDataField);
            }
        }


        private void RecallTransactionField(ComponentState State, string TrnField, string WorkDataField)
        {
            DeleteWorkDataNode(State, WorkDataField);
            if (State.OrchWrkData.GetWrkData().GetNodeByName(TrnField) != null)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue(WorkDataField, State.OrchWrkData.GetWrkData().ReadNodeValue(TrnField, true), true);
                State.OrchWrkData.GetWrkData().DeleteNode(TrnField);
            }
        }
        private void RecallTransactionField(ComponentState State, string TrnField, string WorkDataField, bool NodeBuffer)
        {
            DeleteWorkDataNode(State, WorkDataField);
            if (State.OrchWrkData.GetWrkData().GetNodeByName(TrnField) != null)
            {
                State.OrchWrkData.GetWrkData().WriteNodeBuffer(WorkDataField, State.OrchWrkData.GetWrkData().ReadNodeBuffer(TrnField, true), true);
                State.OrchWrkData.GetWrkData().DeleteNode(TrnField);
            }
        }

        private void DeleteWorkDataNode(ComponentState State, string WorkDataField)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(WorkDataField) != null)
                State.OrchWrkData.GetWrkData().DeleteNode(WorkDataField);
        }
        private void SetTotalsValues(OrchPipeComponent.ComponentState State)
        {
            string TOTDEB = State.OrchWrkData.GetWrkData().ReadNodeValue("ValorTrailerDebito", true);
            string TOTCRED = State.OrchWrkData.GetWrkData().ReadNodeValue("ValorTrailerCredito", true);
            string Tipo1Credito = State.OrchWrkData.GetWrkData().ReadNodeValue("ValorOperacoesTipo1Credito", true);
            string Tipo1Debito = State.OrchWrkData.GetWrkData().ReadNodeValue("ValorOperacoesTipo1Debito", true);
            string Tipo3Credito = State.OrchWrkData.GetWrkData().ReadNodeValue("ValorOperacoesTipo3Credito", true);
            string Tipo3Debito = State.OrchWrkData.GetWrkData().ReadNodeValue("ValorOperacoesTipo3Debito", true);
            string Tipo6Credito = State.OrchWrkData.GetWrkData().ReadNodeValue("ValorOperacoesTipo6Credito", true);
            string Tipo6Debito = State.OrchWrkData.GetWrkData().ReadNodeValue("ValorOperacoesTipo6Debito", true);

            string RelatorioEDST = "OPERACOES NOSSOS CLIENTES + COMISSOES:" + GetTotalFormattedValue(Tipo1Debito) + "\t" + GetTotalFormattedValue(Tipo1Credito) + "\r\n";
            RelatorioEDST = RelatorioEDST + "FECHO POS NOSSOS COMERCIANTES + COMISSOES:" + GetTotalFormattedValue(Tipo3Debito) + "\t" + GetTotalFormattedValue(Tipo3Credito) + "\r\n";
            RelatorioEDST = RelatorioEDST + "CONTA FLOAT:" + GetTotalFormattedValue(Tipo6Debito) + "\t" + GetTotalFormattedValue(Tipo6Credito) + "\r\n";
            RelatorioEDST = RelatorioEDST + "TRAILER" + ":" + GetTotalFormattedValue(TOTDEB) + "\t" + GetTotalFormattedValue(TOTCRED);
            State.OrchWrkData.GetWrkData().WriteNodeValue("RelatorioEDST", RelatorioEDST, true);
        }

        private string GetTotalFormattedValue(string inValue)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            double dblValue = Convert.ToDouble(FormatTotal(inValue));
            string outValue = dblValue.ToString("N", nfi);
            //outValue = "                    ".Substring(0, 20 - outValue.Length) + outValue;
            return outValue;
        }
        private string FormatTotal(string inTotal)
        {
            return inTotal.Replace(' ', '0');
        }
        private void DoParseLine(ComponentState State, string line, string ParseMessageName)
        {
            // Set message for Message parser
            byte[] MOV5Line = Encoding.ASCII.GetBytes(line);
            State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, MOV5Line);
            MessageParser msgParser = new MessageParser();
            msgParser.RunComponent(State.OrchWrkData, new string[] { "#c#" + ParseMessageName });
        }


        private bool TransactionNotProcessed(string TPROC, string MODENV, string CODTRN)
        {
            // Transacção enviada em Real-Time
            if (TPROC == "1" || MODENV == "1" || MODENV == "2")
                return false;

            if (CODTRN == "03" || CODTRN == "04" || CODTRN == "05" || CODTRN == "003" || CODTRN == "004" || CODTRN == "005")
                return false;

            return true;
        }

        private bool TransactionProcessed(string TPROC, string MODENV, string CODTRN)
        {
            // Transacção enviada em Real-Time e não é consultas
            if ((TPROC == "1" || MODENV == "1" || MODENV == "2") && (CODTRN != "03" && CODTRN != "04" && CODTRN != "05" && CODTRN != "003" && CODTRN != "004" && CODTRN != "005"))
                return true;

            return false;
        }

        private void GetNetValue(OrchPipeComponent.ComponentState State, string firstValue, string firstValueSign, string firstOperationType, string secondValue, string secondValueSign, string secondOperationType)
        {
            decimal decFinalAmount = 0;
            string firstAmount = State.OrchWrkData.GetWrkData().ReadNodeValue(firstValue, true);
            decimal decFirstAmount = Convert.ToDecimal(firstAmount);
            string firstAmountSign = State.OrchWrkData.GetWrkData().ReadNodeValue(firstValueSign, true);

            string secondAmount = State.OrchWrkData.GetWrkData().ReadNodeValue(secondValue, true);
            decimal decSecondAmount = Convert.ToDecimal(secondAmount);
            string secondAmountSign = State.OrchWrkData.GetWrkData().ReadNodeValue(secondValueSign, true);

            if (decFirstAmount > decSecondAmount)
                State.OrchWrkData.GetWrkData().WriteNodeValue("SinalMontanteFinal", firstAmountSign, true);
            else
                State.OrchWrkData.GetWrkData().WriteNodeValue("SinalMontanteFinal", secondAmountSign, true);

            if (firstAmountSign != firstOperationType)
                decFirstAmount *= -1;
            if (secondAmountSign != secondOperationType)
                decSecondAmount *= -1;

            decFinalAmount = decFirstAmount - decSecondAmount;

            if (decFinalAmount < 0)
                decFinalAmount *= -1;

            State.OrchWrkData.GetWrkData().WriteNodeValue("MontanteFinal", decFinalAmount.ToString().PadLeft(13, '0'), true);
        }

    }
}
