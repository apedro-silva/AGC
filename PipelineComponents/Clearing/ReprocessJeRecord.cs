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
    public class ReprocessJeRecord : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            InitializeJE initJe = new InitializeJE();
            FinalizeJE finJE = new FinalizeJE();

            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            // loga novo registo
            initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });

            try
            {
                // coloca registo em situação de "Reprocessar possivel"
                State.OrchWrkData.GetWrkData().WriteNodeValue("Situacao", "1", true);

                //obtem dataset com transacções a processar
                DataSet JeRecordDS = GetJeRecord2Reprocess(State);

                //por cada registo chama FlexCube
                string JE;
                string Montante;
                string CodTrn;
                string TipoTerminal;
                string CodigoTerminal, CodigoMoeda;
                string BancoApoio;
                string DataHora;
                string AplicPdd;
                string IdLog;
                string NrLog;
                string NumeroConta;
                string NumeroCartao, TipoServico;
                string AplicPddOriginal, IdLogOriginal, NrLogOriginal, SinalMontante, RubricaContabilistica;

                foreach (DataRow myRow in JeRecordDS.Tables[0].Rows)
                {
                    CleanWorkData(State);

                    JE = myRow[0].ToString();
                    CodTrn = myRow[1].ToString();
                    NumeroConta = myRow[2].ToString();
                    NumeroCartao = myRow[3].ToString();
                    Montante = myRow[4].ToString();
                    TipoTerminal = myRow[5].ToString();
                    AplicPdd = myRow[6].ToString();
                    IdLog = myRow[7].ToString();
                    NrLog = myRow[8].ToString();
                    CodigoMoeda = myRow[9].ToString();
                    CodigoTerminal = myRow[10].ToString();
                    BancoApoio = myRow[11].ToString();
                    DataHora = myRow[12].ToString();
                    TipoServico = myRow[13].ToString();
                    AplicPddOriginal = myRow[14].ToString();
                    IdLogOriginal = myRow[15].ToString();
                    NrLogOriginal = myRow[16].ToString();
                    SinalMontante = myRow[17].ToString();
                    RubricaContabilistica = myRow[18].ToString();

                    Montante = Montante.Replace(",", "");
                    Montante = Montante.Replace(".", "");

                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", CodTrn, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", Montante, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TipoTerm", TipoTerminal, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodMoeda", CodigoMoeda, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("IdTerminal", CodigoTerminal, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("BanApoio", BancoApoio, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("DtHora", DataHora, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroCartao", NumeroCartao, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Conta", NumeroConta, true);

                    State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", TipoServico, true);

                    State.OrchWrkData.GetWrkData().WriteNodeValue("AplicPddOriginal", AplicPddOriginal, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("IdLogOriginal", IdLogOriginal, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("NrLogOriginal", NrLogOriginal, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("SinalMontante", SinalMontante, true);

                    DoFlexCubeTransaction(State, RubricaContabilistica);

                    State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
                }
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
            }
            finally
            {
                finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
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
            DeleteWorkDataNode(State, "SinalMontante");
        }
        private void DeleteWorkDataNode(ComponentState State, string WorkDataField)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(WorkDataField) != null)
                State.OrchWrkData.GetWrkData().DeleteNode(WorkDataField);
        }

        private void DoFlexCubeTransaction(OrchPipeComponent.ComponentState State, string RubricaContabilistica)
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

            if (RubricaContabilistica == null)
                RubricaContabilistica = State.OrchWrkData.GetWrkData().ReadNodeValue("RubricaContabilistica", true);

            if (!State.IsInError && RubricaContabilistica != "")
            {
                SetHostAccount(State, RubricaContabilistica, "SinalMontante");
            }

            if (!State.IsInError)
            {
                PrepareConstructor prepConst = new PrepareConstructor();
                prepConst.RunComponent(State.OrchWrkData, new string[] { });
            }

            if (!State.IsInError & State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true) != "")
            {
                MessageConstructor msgConst = new MessageConstructor();
                msgConst.RunComponent(State.OrchWrkData, new string[] { "#uv#FlexCubeConstructor", "FlexCubeRequest" });
            }

            if (!State.IsInError & State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true) != "")
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
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Situacao", "0", true); // JePRT Situacao
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Transacção Reprocessada", true);
                }
            }

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
                        throw new BusinessException(string.Format("EDST.ProcessLine:SetHostAccount() Undefined Account Information {0}", RubricaContabilistica));

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
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "EDST.ProcessLine.SetHostAccount exception->" + exp.Message, true);
            }
        }



        private DataSet GetJeRecord2Reprocess(ComponentState State)
        {
            DataSet myDS = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("GetJeRecord2Reprocess");
                db.AddInParameter(dbCommand, "JE", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue("Je2Reprocess", true));

                myDS = db.ExecuteDataSet(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
            return myDS;
        }
    }
}
