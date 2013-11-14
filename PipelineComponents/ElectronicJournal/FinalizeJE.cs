using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.PipelineComponents
{
    public class FinalizeJE : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            FinalizeElectronicJournal(State, Params);
        }

        private void FinalizeElectronicJournal(OrchPipeComponent.ComponentState State, string[] Params)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand dbCommand = db.GetStoredProcCommand("ActualizaJE");
            string JEIdName = Params[0];
            string CodTrn=string.Empty;
            string Montante = "";
            try
            {
                string prtResponseMsg = null;
                string requestBuf = null;
                string responseBuf = null;
                string textoErro = null;
                string numeroConta = null;
                int codigoErro;
                string codResp = State.OrchWrkData.GetWrkData().ReadNodeValue("CodResp", true);
                string responseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);

                byte[] prtResponseBuff = (byte[])State.OrchWrkData.GetWrkData().ReadNodeBuffer("PRTResponse", true);

                if (Params.Length == 2)
                    CodTrn = Params[1];
                else
                    CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn");

                if (prtResponseBuff != null)
                    prtResponseMsg = Encoding.Default.GetString(prtResponseBuff);

                string infoTransacao = State.OrchWrkData.GetWrkData().ReadNodeValue("TextoErro", true);
                if (infoTransacao == null)
                    infoTransacao = "OK";
                textoErro = State.IsInError ? State.LastError.Message : infoTransacao;
                codigoErro = State.IsInError || (codResp != null && codResp != "0" && codResp != "ND") || (responseCode != null && responseCode != "00") ? 1 : 0;

                db.AddInParameter(dbCommand, "JE", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue(JEIdName));
                db.AddInParameter(dbCommand, "Servico", DbType.String, CodTrn);
                db.AddInParameter(dbCommand, "Sequencia", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue("SeqMov",true));

                if (State.OrchWrkData.GetWrkData().ReadNodeValue("Conta", true) != null)
                    numeroConta = State.OrchWrkData.GetWrkData().ReadNodeValue("Conta", true);
                else
                    numeroConta = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroContaDebito", true);

                db.AddInParameter(dbCommand, "NumeroConta", DbType.String, numeroConta);

                string NumeroCartao;
                if (State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroCartao",true)!=null)
                    NumeroCartao = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroCartao", true);
                else
                    NumeroCartao = State.OrchWrkData.GetWrkData().ReadNodeValue("BIN", true) + State.OrchWrkData.GetWrkData().ReadNodeValue("ExBin", true) + State.OrchWrkData.GetWrkData().ReadNodeValue("NumCar", true);
                db.AddInParameter(dbCommand, "NumeroCartao", DbType.String, NumeroCartao);

                Montante = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2", true);
                if (Montante != null)
                {
                    Montante = Montante.PadLeft(13, '0');
                    Montante = Montante.Substring(0, 11) + "." + Montante.Substring(11);
                }
                db.AddInParameter(dbCommand, "Montante", DbType.String, Montante);
                db.AddInParameter(dbCommand, "Estado", DbType.Int16, 1);
                db.AddInParameter(dbCommand, "CodigoErro", DbType.Int32, codigoErro);
                db.AddInParameter(dbCommand, "TextoErro", DbType.String, textoErro);
                db.AddInParameter(dbCommand, "NrIdResp", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("NrIdResp",true));
                db.AddInParameter(dbCommand, "PRTResposta", DbType.String, prtResponseMsg);

                db.AddInParameter(dbCommand, "MessageType", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("MessageType", true));
                db.AddInParameter(dbCommand, "STAN", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("SystemTraceAuditNumber", true));
                db.AddInParameter(dbCommand, "TransmissionDateTime", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("TransmissionDateTime", true));
                db.AddInParameter(dbCommand, "AcquiringInstitutionIdentificationCode", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("AcquiringInstitutionIdentificationCode", true));

                db.AddInParameter(dbCommand, "AplicPdd", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPdd", true));
                db.AddInParameter(dbCommand, "IdLog", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IdLog", true));
                db.AddInParameter(dbCommand, "NrLog", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("NrLog", true));

                byte[] requestMsg = State.OrchWrkData.GetWrkData().ReadNodeBuffer("FlexCubeRequest", true);
                if (requestMsg != null)
                    requestBuf = Encoding.Default.GetString(requestMsg);
                byte[] responseMsg = State.OrchWrkData.GetWrkData().ReadNodeBuffer("FlexCubeResponse", true);
                if (responseMsg != null)
                    responseBuf = Encoding.Default.GetString(responseMsg);

                db.AddInParameter(dbCommand, "FlexCubePedido", DbType.String, requestBuf);
                db.AddInParameter(dbCommand, "FlexCubeResposta", DbType.String, responseBuf);

                Object DataEnvioFC = null;
                Object DataRecepcaoFC = null;
                if (State.OrchWrkData.GetFromObjBucket("DataEnvioFC")!=null)
                    DataEnvioFC = (DateTime)State.OrchWrkData.GetFromObjBucket("DataEnvioFC");

                if (State.OrchWrkData.GetFromObjBucket("DataRecepcaoFC")!=null)
                    DataRecepcaoFC = (DateTime)State.OrchWrkData.GetFromObjBucket("DataRecepcaoFC");

                db.AddInParameter(dbCommand, "DataEnvioFC", DbType.DateTime, DataEnvioFC);
                db.AddInParameter(dbCommand, "DataRecepcaoFC", DbType.DateTime, DataRecepcaoFC);
                db.AddInParameter(dbCommand, "CodResp", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CodResp", true));
                db.AddInParameter(dbCommand, "ResponseCode", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true));
                db.AddInParameter(dbCommand, "NumeroOperacoes", DbType.Int32, State.OrchWrkData.GetWrkData().ReadNodeValue("TotOper", true));

                db.AddInParameter(dbCommand, "FicheiroDRCC", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("FicheiroDRCC", true));
                db.AddInParameter(dbCommand, "RetrievalReferenceNumber", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("RetrievalReferenceNumber", true));
                db.AddInParameter(dbCommand, "TipoServico", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("TipoServico", true));
                db.AddInParameter(dbCommand, "TipoTerminal", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("TipoTerm", true));
                db.AddInParameter(dbCommand, "CodigoMoeda", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("CodMoeda", true));
                db.AddInParameter(dbCommand, "CodigoTerminal", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IdTerminal", true));
                db.AddInParameter(dbCommand, "BancoApoio", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("BanApoio", true));
                db.AddInParameter(dbCommand, "DataHora", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("DtHora", true));
                db.AddInParameter(dbCommand, "DescricaoServico", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("DescricaoServico", true));

                db.AddInParameter(dbCommand, "AplicPddOriginal", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("AplicPddOriginal", true));
                db.AddInParameter(dbCommand, "IdLogOriginal", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IdLogOriginal", true));
                db.AddInParameter(dbCommand, "NrLogOriginal", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("NrLogOriginal", true));
                db.AddInParameter(dbCommand, "Situacao", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("Situacao", true));
                db.AddInParameter(dbCommand, "SinalMontante", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("SinalMontante", true));
                db.AddInParameter(dbCommand, "RubricaContabilistica", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("RubricaContabilistica", true));
                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("ElectronicJournal:FinalizeElectronicJournal", exp);
            }
        }
    }
}
