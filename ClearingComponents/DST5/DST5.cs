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
    public class DST5 : OrchPipeComponent
    {
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

                    DoParseLine(State, line, "DST5_TIPREG_" + TIPREG);
                    if (State.IsInError)
                    {
                        finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                        continue;
                    }

                    ProcessTransaction(State, line);

                    // rep�e dados do cliente
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", State.OrchWrkData.GetWrkData().ReadNodeValue("MontanteCliente",true), true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ModEnvTrn", State.OrchWrkData.GetWrkData().ReadNodeValue("ModEnv", true), true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TpProcTrn", State.OrchWrkData.GetWrkData().ReadNodeValue("TpProc", true), true);

                    RecallTransactionField(State, "AplicPddOriginalTrn", "AplicPddOriginal");
                    RecallTransactionField(State, "IdLogOriginalTrn", "IdLogOriginal");
                    RecallTransactionField(State, "NrLogOriginalTrn", "NrLogOriginal");

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
                    RecallTransactionField(State, "SituacaoRegistoEMISTrn", "SituacaoRegistoEMIS");
                    RecallTransactionField(State, "BanApoioTrn", "BanApoio");

                    EMISRecordLog.DoEMISRecordLog(State, new string[] { "JE", "Montante2", "NumeroContaDebitoTrn", "NumeroContaCreditoTrn", "SinalMontante" });
                    finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                }
                DeleteWorkDataNode(State, "Comerciante");

                // Anula transac��es Real-time n�o presentes neste ficheiro
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
                finJE.RunComponent(State.OrchWrkData, new string[] { "JEFicheiroEMIS","V03" });
            }
            return;
        }

        private void ReverseTransactionsNotConfirmed(ComponentState State)
        {
            InitializeJE initJe = new InitializeJE();
            FinalizeJE finJE = new FinalizeJE();

            // Obtem lista de transac��es 1100 em Real-time n�o confirmados neste ficheiro
            DataSet notConfirmedDS = Get1100NotConfirmed(State);

            // Percorre lista de transac��es a anular
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

                    // Inicia Log da transac��o
                    initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });

                    //Anula transac��o
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
                    // Actualiza Log da transac��o
                    finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    State.ResetLastError();
                }
            }
        }

        private void ReverseTransaction(ComponentState State)
        {
            string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);

            // obtem parametros transac��o original
            string AplicPddOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPdd", true);
            string IdLogOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("IdLog", true);
            string NrLogOriginal = State.OrchWrkData.GetWrkData().ReadNodeValue("NrLog", true);

            // Altera AplicPdd para novo valor o que permite uma nova transac��o
            State.OrchWrkData.GetWrkData().WriteNodeValue("AplicPdd", "X", true);

            // Queremos uma anula��o
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
            string MontanteA;
            string ComEmiA;
            string ComPropA;
            string TaxaCliA;
            string TaxaCliB;
            decimal decMontante;
            string TARINT1;
            string TARINT2;

            string CODTRN = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
            string TPROC = State.OrchWrkData.GetWrkData().ReadNodeValue("TpProc", true);
            string MODENV = State.OrchWrkData.GetWrkData().ReadNodeValue("ModEnv", true);

            switch (TIPREG)
            {
                // if TIPREG=1 -> opera��es com cart�es
                case "1":
                // if TIPREG=2 -> opera��es detinadas a nibs
                case "2":
                    MontanteA = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante", true);
                    decMontante = Convert.ToDecimal(MontanteA);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("MontanteCliente", decMontante.ToString().PadLeft(13, '0'), true);

                    DoFlexCubeTransaction(State, CODTRN, null, "MontanteCliente", true, "SinalMontante", null);
                    StoreTransactionFields(State);

                    //Comiss�o do Emissor
                    ComEmiA = State.OrchWrkData.GetWrkData().ReadNodeValue("MntPagoDestinatario", true);
                    decMontante = Convert.ToDecimal(ComEmiA);
                    TARINT1 = State.OrchWrkData.GetWrkData().ReadNodeValue("TarInt1", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ComEmi", decMontante.ToString().PadLeft(13, '0'), true);
                    DoFundsTransfer(State, "T1", TARINT1, "ComEmi", "SinalMntPagoDestinatario");

                    //Comiss�o do Propriet�rio
                    ComPropA = State.OrchWrkData.GetWrkData().ReadNodeValue("MntRecebidoDestinatario", true);
                    decMontante = Convert.ToDecimal(ComPropA);
                    TARINT2 = State.OrchWrkData.GetWrkData().ReadNodeValue("TarInt2", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("ComProp", decMontante.ToString().PadLeft(13, '0'), true);
                    DoFundsTransfer(State, "T2", TARINT2, "ComProp", "SinalMntRecebidoDestinatario");

                    //Taxas de Cliente
                    TaxaCliA = State.OrchWrkData.GetWrkData().ReadNodeValue("MontanteAdicional", true);
                    TaxaCliB = State.OrchWrkData.GetWrkData().ReadNodeValue("TaxaCliB", true);
                    decMontante = Convert.ToDecimal(TaxaCliA) + Convert.ToDecimal(TaxaCliB);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TaxaCli", decMontante.ToString().PadLeft(13, '0'), true);
                    DoFundsTransfer(State, "T6", "TAXCLI", "TaxaCli", "SinalMontanteAdicional");

                    break;

                case "9":
                    int TOTREG = Convert.ToInt32(State.OrchWrkData.GetWrkData().ReadNodeValue("TotReg", true));
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "EF1", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErroTrn", "OK", true);
                    result = false;
                    break;
                default:
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TipReg", TIPREG, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "00", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErroTrn", "Tipo registo " + TIPREG + " no DST5 n�o suportado", true);
                    break;
            }
            return result;
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
            string WDValue = State.OrchWrkData.GetWrkData().ReadNodeValue(WDValueField, true);
            // se montante=0 n�o vai ao FlexCube
            if (WDValue == null || Convert.ToInt32(WDValue) == 0)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
                return;
            }

            DeleteWorkDataNode(State, "RetrievalReferenceNumber");
            DeleteWorkDataNode(State, "AplicPddOriginal");
            DeleteWorkDataNode(State, "IdLogOriginal");
            DeleteWorkDataNode(State, "NrLogOriginal");

            // se n�o existe RubricaContabilistica n�o vai ao FlexCube
            if (RubricaContabilistica == null || RubricaContabilistica == "0" || RubricaContabilistica.Trim() == "")
                return;

            State.OrchWrkData.GetWrkData().WriteNodeValue("RubricaContabilistica", RubricaContabilistica, true);

            // Funds Transfer always go to the ATM Server
            State.OrchWrkData.GetWrkData().WriteNodeValue("TipoTerm", "A", true);
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
            if (WDValue == null || Convert.ToInt32(WDValue) == 0)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
                return;
            }

            // Para FlexCube s� existe um Montante
            State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", WDValue, true);

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

            string CODTRN = CodTrn;
            string TPROC = State.OrchWrkData.GetWrkData().ReadNodeValue("TpProc", true);
            string MODENV = State.OrchWrkData.GetWrkData().ReadNodeValue("ModEnv", true);
            bool IsTrnInDB = IsTransactionInDB(State, CodTrn);
            // submete ao FlexCube transac��es recebidas em Real-Time e n�o presentes na base de dados
            if (TransactionProcessed(TPROC, MODENV, CODTRN) && !IsTrnInDB)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("SituacaoRegistoEMIS", "55", true);
            }
            //Submete ao FlexCube transac��es n�o recebidas em real-time e n�o presentes na base de dados(bug da EMIS)
            else if (TransactionNotProcessed(TPROC, MODENV, CODTRN) && !IsTrnInDB)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("SituacaoRegistoEMIS", "55", true);
            }

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
                        throw new BusinessException(string.Format("R�brica Contabil�stica {0} n�o parametrizada", RubricaContabilistica));

                    DataRow dr = trnParameters.Rows[0];

                    string Rubrica = dr[0].ToString();
                    string Descricao = dr[1].ToString();
                    string NumeroContaDebito = dr[2].ToString();
                    string NumeroContaCredito = dr[3].ToString();

                    string Sinal = State.OrchWrkData.GetWrkData().ReadNodeValue(SinalMontante, true);
                    string IndStr = State.OrchWrkData.GetWrkData().ReadNodeValue("IndStr", true);

                    if (IndStr!=null && IndStr=="0")
                    {
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
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "DST5.ProcessLine.IsTransactionInDB exception->" + exp.Message, true);
            }
            return false;
        }

        private void CleanWorkData(ComponentState State)
        {
            DeleteWorkDataNode(State, "TipoServico");
            DeleteWorkDataNode(State, "BanApoio");
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
            DeleteWorkDataNode(State, "MontanteCliente");
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
            byte[] EDSTLine = Encoding.ASCII.GetBytes(line);
            State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, EDSTLine);
            MessageParser msgParser = new MessageParser();
            msgParser.RunComponent(State.OrchWrkData, new string[] { "#c#" + ParseMessageName });
        }


        private bool TransactionNotProcessed(string TPROC, string MODENV, string CODTRN)
        {
            // Transac��o enviada em Real-Time
            if (TPROC == "1" || MODENV == "1" || MODENV == "2")
                return false;

            if (CODTRN == "03" || CODTRN == "04" || CODTRN == "05" || CODTRN == "003" || CODTRN == "004" || CODTRN == "005")
                return false;

            return true;
        }

        private bool TransactionProcessed(string TPROC, string MODENV, string CODTRN)
        {
            // Transac��o enviada em Real-Time e n�o � consultas
            if ((TPROC == "1" || MODENV == "1" || MODENV == "2") && (CODTRN != "03" && CODTRN != "04" && CODTRN != "05" && CODTRN != "003" && CODTRN != "004" && CODTRN != "005"))
                return true;

            return false;
        }
    }
}
