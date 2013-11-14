using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using SF.Expand.Core.Data;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.PipelineComponents
{
    public class ProcessJEEmisRecords :OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            FinalizeJE finJE = new FinalizeJE();
            EMISRecordLog recordLog = new EMISRecordLog();
            InitializeJE initJe = new InitializeJE();

            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            //obtem dataset com transacções a processar
            DataSet JeEmisDS = GetJeEmisRecords(State);

            //por cada registo chama FlexCube
            string JE;
            string NumeroContaDebito;
            string NumeroContaCredito;
            string Montante;
            string CodTrn;
            string TipoTerminal;
            string TipoServico;
            string CodigoMoeda;
            string CodigoTerminal;
            string BancoApoio;
            string DataHora;
            string AplicPdd;
            string IdLog;
            string NrLog;
            string AplicPddOriginal;
            string IdLogOriginal;
            string NrLogOriginal;
            string NumeroConta;
            string NumeroCartao;
            string SinalMontante;
            string JEFicheiroEMIS;
            string TipoRegisto, TipoProcesso, ModoEnvio, DescricaoOperacao, RubricaContabilistica, Registo;

            foreach (DataRow myRow in JeEmisDS.Tables[0].Rows)
            {
                CleanWorkData(State);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Situacao", "1", true);

                JE = myRow[0].ToString();
                CodTrn = myRow[1].ToString();
                NumeroContaDebito = myRow[2].ToString();
                NumeroContaCredito = myRow[3].ToString();
                Montante = myRow[4].ToString();
                TipoTerminal = myRow[5].ToString();
                TipoServico = myRow[6].ToString();

                AplicPdd = myRow[7].ToString();
                IdLog = myRow[8].ToString();
                NrLog = myRow[9].ToString();

                CodigoMoeda = myRow[10].ToString();
                CodigoTerminal = myRow[11].ToString();
                BancoApoio = myRow[12].ToString();
                DataHora = myRow[13].ToString();
                NumeroConta = myRow[14].ToString();
                NumeroCartao = myRow[15].ToString();
                SinalMontante = myRow[16].ToString();
                JEFicheiroEMIS = myRow[17].ToString();
                TipoRegisto = myRow[18].ToString();
                TipoProcesso = myRow[19].ToString();
                ModoEnvio = myRow[20].ToString();
                DescricaoOperacao = myRow[21].ToString();
                RubricaContabilistica = myRow[22].ToString();
                Registo = myRow[23].ToString();
                AplicPddOriginal = myRow[24].ToString();
                IdLogOriginal = myRow[25].ToString();
                NrLogOriginal = myRow[26].ToString();

                Montante = Montante.Replace(",", "");
                Montante = Montante.Replace(".", "");

                State.OrchWrkData.GetWrkData().WriteNodeValue("JeOriginal", JE, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", CodTrn, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", Montante, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebitoTrn", NumeroContaDebito, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCreditoTrn", NumeroContaCredito, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TipoTerm", TipoTerminal, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", TipoServico, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodMoeda", CodigoMoeda, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("IdTerminal", CodigoTerminal, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("BanApoio", BancoApoio, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("DtHora", DataHora, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroCartao", NumeroCartao, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("SinalMontante", SinalMontante, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("JEFicheiroEMIS", JEFicheiroEMIS, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TipReg", TipoRegisto, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TpProcTrn", TipoProcesso, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("ModEnvTrn", ModoEnvio, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("DescricaoOperacao", DescricaoOperacao, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("EMISFileRecord", Registo, true);

                State.OrchWrkData.GetWrkData().WriteNodeValue("AplicPdd", AplicPdd, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("IdLog", IdLog, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("NrLog", NrLog, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("AplicPddOriginal", AplicPddOriginal, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("IdLogOriginal", IdLogOriginal, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("NrLogOriginal", NrLogOriginal, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "1", true);

                initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });
                DoFlexCubeTransaction(State);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Conta", NumeroConta, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("RubricaContabilistica", RubricaContabilistica, true);
                recordLog.RunComponent(State.OrchWrkData, new string[] { "JE", "Montante2", "NumeroContaDebito", "NumeroContaCredito", "SinalMontante" });

                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
                finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });

                if (State.IsInError) break;
            }

        }

        private void DoFlexCubeTransaction(OrchPipeComponent.ComponentState State)
        {
            string Montante = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2", true);
            if (Montante == null || Convert.ToInt32(Montante) == 0)
                return;

            if (!State.IsInError)
            {
                string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
                GetParameters getParams = new GetParameters();
                getParams.RunComponent(State.OrchWrkData, new string[] { CodTrn });
            }

            if (!State.IsInError)
            {
                string MessageType = State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true);
                if (MessageType != "" && MessageType.Substring(1, 1) == "2")
                {
                    MessageType = MessageType.Substring(0, 2) + "2" + MessageType.Substring(3);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("MessageType", MessageType, true);
                }
            }

            RecallTransactionField(State, "NumeroContaDebitoTrn", "NumeroContaDebito");
            RecallTransactionField(State, "NumeroContaCreditoTrn", "NumeroContaCredito");

            if (!State.IsInError)
            {
                PrepareConstructor prepConst = new PrepareConstructor();
                prepConst.RunComponent(State.OrchWrkData, new string[] { });
            }

            if (!State.IsInError && State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true) != "")
            {
                MessageConstructor msgConst = new MessageConstructor();
                msgConst.RunComponent(State.OrchWrkData, new string[] { "#uv#FlexCubeConstructor", "FlexCubeRequest" });
            }

            if (!State.IsInError && State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true) != "")
            {
                CallHost callHost = new CallHost();
                callHost.RunComponent(State.OrchWrkData, new string[] { });
            }

            if (!State.IsInError && State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true) != "")
            {
                MessageParser msgParser = new MessageParser();
                msgParser.RunComponent(State.OrchWrkData, new string[] { "#uv#FlexCubeParser" });
                string ResponseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
                if (ResponseCode == "00")
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoRegisto", "0", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Situacao", "0", true);
                }
            }

            // coloca registo no estado "Tratado"
            SetJeEmisRecordProcessed(State);
        }

        private DataSet GetJeEmisRecords(OrchPipeComponent.ComponentState State)
        {
            DataSet myDS = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("GetJeEmisRecords2Process");

                myDS = db.ExecuteDataSet(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
            return myDS;
        }

        private void SetJeEmisRecordProcessed(OrchPipeComponent.ComponentState State)
        {
            // Updates JeRegistoEmis Situacao=0, Estado=EstadoRegisto
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("SetJeEmisRecordsProcessed");
                db.AddInParameter(dbCommand, "JE", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue("JE",true));
                db.AddInParameter(dbCommand, "EstadoRegisto", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("EstadoRegisto", true));

                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

        private void StoreTransactionField(ComponentState State, string WorkDataField, string TrnField)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(WorkDataField) != null)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue(TrnField, State.OrchWrkData.GetWrkData().ReadNodeValue(WorkDataField, true), true);
                State.OrchWrkData.GetWrkData().DeleteNode(WorkDataField);
            }

        }

        private void RecallTransactionField(ComponentState State, string TrnField, string WorkDataField)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(TrnField) != null)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue(WorkDataField, State.OrchWrkData.GetWrkData().ReadNodeValue(TrnField, true), true);
                State.OrchWrkData.GetWrkData().DeleteNode(TrnField);
            }

        }

        private void CleanWorkData(ComponentState State)
        {
            DeleteWorkDataNode(State, "EstadoRegisto");
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

    }
}
