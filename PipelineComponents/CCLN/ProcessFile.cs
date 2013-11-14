using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    /// <summary>
    /// Switch component that processes the CCLN file from SIBS/EMIS.
    /// </summary>
    public class CCLN : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The component OrchWrkData</param>
        /// <param name="Params">The component params</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;


            StreamReader sr = (StreamReader)State.OrchWrkData.GetFromObjBucket("FileStreamReader");
            try
            {
                InitializeJE initJe = new InitializeJE();
                EMISRecordLog recordLog = new EMISRecordLog();
                FinalizeJE finJE = new FinalizeJE();

                String line = null;
                String TIPREG = null;
                String TOTREG = null;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    CleanWorkData(State);
                    if (line.Trim() == "") continue;

                    State.OrchWrkData.GetWrkData().WriteNodeValue("EMISFileRecord", line, true);
                    TIPREG = line.Substring(0, 1);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "C" + TIPREG, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
                    DoParseLine(State, line, "CCLN_TIPREG_" + TIPREG);

                    initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    ProcessCard(State, line);
                    recordLog.RunComponent(State.OrchWrkData, new string[] { "JE", null, null, null, null });
                    finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    if (TIPREG == "9")
                    {
                        TOTREG = line.Substring(1, 8);
                        if (i != Convert.ToInt32(TOTREG))
                            throw new BusinessException("CCLN.ProcessCards->TOTREG <> Registos no ficheiro!");
                        break;
                    }

                    i += 1;
                }
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro processado", true);
                State.ResetLastError();
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("CCLN.ProcessCards", exp);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
            }
            finally
            {
                FinalizeFileJE finJE = new FinalizeFileJE();
                finJE.RunComponent(State.OrchWrkData, new string[] { "JEFicheiroEMIS", "E2" });

                if (sr != null)
                    sr.Close();
            }
        }

        /// <summary>
        /// Processes a card record from the CCLN file.
        /// </summary>
        /// <param name="State">The ComponentState</param>
        /// <param name="line">The CCLN record line</param>
        private void ProcessCard(OrchPipeComponent.ComponentState State, string line)
        {
            int RowsAffected = 0;
            if (line.Length != 163)
                throw new BusinessException(string.Format("ProcessCard->Comprimento registo inválido <{0}> !", line.Length));

            string TIPREG = line.Substring(0, 1);
            try
            {
                if (!State.IsInError)
                {
                    string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
                    GetParameters getParams = new GetParameters();
                    getParams.RunComponent(State.OrchWrkData, new string[] { CodTrn });
                }

                string Bin = State.OrchWrkData.GetWrkData().ReadNodeValue("BIN", true);
                string ExBin = State.OrchWrkData.GetWrkData().ReadNodeValue("ExBin", true);
                string NumCar = State.OrchWrkData.GetWrkData().ReadNodeValue("NumCar", true);
                string NumCartao = Bin + ExBin + NumCar;

                string MotCapt = State.OrchWrkData.GetWrkData().ReadNodeValue("MotCapt", true);
                string SitCC = State.OrchWrkData.GetWrkData().ReadNodeValue("SitCC", true);
                string SitCar = State.OrchWrkData.GetWrkData().ReadNodeValue("SitCar", true);
                string SitCar2 = State.OrchWrkData.GetWrkData().ReadNodeValue("SitCar2", true);
                string LocTerm = State.OrchWrkData.GetWrkData().ReadNodeValue("LocTerm", true);
                string DataHoraAlteracao = State.OrchWrkData.GetWrkData().ReadNodeValue("DtHora", true);
                switch (TIPREG)
                {
                    // if TIPREG=1 -> cartões capturados 
                    case "1":
                        RowsAffected = ChangeCardCapturedState(NumCartao, SitCC, MotCapt, LocTerm, DataHoraAlteracao);
                        break;
                    // if TIPREG=2 -> cartões em lista negra
                    case "2":
                        RowsAffected = ChangeCardState(NumCartao, SitCar2, DataHoraAlteracao);
                        break;
                    case "4": break;
                    case "9": RowsAffected = -1; break;
                    default:
                        State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "00", true);
                        State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Tipo registo " + TIPREG + " no EDST não suportado", true);
                        return;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (RowsAffected == -1)
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
                else if (RowsAffected == 0)
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Cartão não processado", true);
                else
                    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Situação de Cartão alterada", true);
            }
        }

        /// <summary>
        /// Cleans the work data.
        /// </summary>
        /// <param name="State">The state.</param>
        private void CleanWorkData(ComponentState State)
        {
            DeleteWorkDataNode(State, "BIN");
            DeleteWorkDataNode(State, "ExBin");
            DeleteWorkDataNode(State, "NumCar");
            DeleteWorkDataNode(State, "SitCar");
            DeleteWorkDataNode(State, "SitCar2");
            DeleteWorkDataNode(State, "MotCapt");
            DeleteWorkDataNode(State, "SitCC");
            DeleteWorkDataNode(State, "LocTerm");
            DeleteWorkDataNode(State, "TarSibs");
            DeleteWorkDataNode(State, "CPD");
            DeleteWorkDataNode(State, "DataExp");
            DeleteWorkDataNode(State, "ModIns");
            DeleteWorkDataNode(State, "Senha");
            DeleteWorkDataNode(State, "DtHora");
            DeleteWorkDataNode(State, "ModEnv");
            DeleteWorkDataNode(State, "NrLog");
            DeleteWorkDataNode(State, "IdLog");
            DeleteWorkDataNode(State, "AplicPdd");
            DeleteWorkDataNode(State, "TipReg");
            DeleteWorkDataNode(State, "TextoErro");
        }

        /// <summary>
        /// Deletes the work data node.
        /// </summary>
        /// <param name="State">The state.</param>
        /// <param name="WorkDataField">The work data field.</param>
        private void DeleteWorkDataNode(ComponentState State, string WorkDataField)
        {
            if (State.OrchWrkData.GetWrkData().GetNodeByName(WorkDataField) != null)
                State.OrchWrkData.GetWrkData().DeleteNode(WorkDataField);
        }

        /// <summary>
        /// Changes the state of the card captured.
        /// </summary>
        /// <param name="NumCartao">The num cartao.</param>
        /// <param name="SitCC">The sit CC.</param>
        /// <param name="MotCapt">The mot capt.</param>
        /// <param name="Local">The local.</param>
        /// <param name="DataHora">The data hora.</param>
        /// <returns>RowsAffected</returns>
        private int ChangeCardCapturedState(string NumCartao, string SitCC, string MotCapt, string Local, string DataHora)
        {
            try
            {
                int RowsAffected = 0;
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("AlteraSituacaoCartaoCapturado");
                db.AddInParameter(dbCommand, "NumeroCartao", DbType.String, NumCartao);
                db.AddInParameter(dbCommand, "SituacaoCartaoCapturado", DbType.Int16, Convert.ToInt16(SitCC));
                db.AddInParameter(dbCommand, "Motivo", DbType.String, MotCapt);
                db.AddInParameter(dbCommand, "Local", DbType.String, Local);
                db.AddInParameter(dbCommand, "DataHoraCaptura", DbType.DateTime, string.Format("{0}-{1}-{2} {3}:{4}:{5}", DataHora.Substring(0, 4), DataHora.Substring(4, 2), DataHora.Substring(6, 2), DataHora.Substring(8, 2), DataHora.Substring(10, 2), DataHora.Substring(12, 2)));
                db.AddOutParameter(dbCommand, "RegistosProcessados", DbType.Int16,0);
                int rowsAffected = db.ExecuteNonQuery(dbCommand);
                Object objRowsAffected = db.GetParameterValue(dbCommand, "RegistosProcessados");
                if (objRowsAffected != System.DBNull.Value)
                    RowsAffected = (System.Int16)objRowsAffected;
                return RowsAffected;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Changes the state of the card.
        /// </summary>
        /// <param name="NumCartao">The num cartao.</param>
        /// <param name="SitCar">The sit car.</param>
        /// <param name="DataHora">The data hora.</param>
        /// <returns>RowsAffected</returns>
        private int ChangeCardState(string NumCartao, string SitCar, string DataHora)
        {
            try
            {
                int RowsAffected = 0;
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("AlteraSituacaoCartao");
                db.AddInParameter(dbCommand, "NumeroCartao", DbType.String, NumCartao);
                db.AddInParameter(dbCommand, "SituacaoCartao", DbType.String, SitCar);
                db.AddInParameter(dbCommand, "DataHoraAlteracao", DbType.DateTime, string.Format("{0}-{1}-{2} {3}:{4}", DataHora.Substring(0, 4), DataHora.Substring(4, 2), DataHora.Substring(6, 2), DataHora.Substring(8, 2), DataHora.Substring(10, 2)));
                db.AddOutParameter(dbCommand, "RegistosProcessados", DbType.Int16, 0);
                int rowsAffected = db.ExecuteNonQuery(dbCommand);
                Object objRowsAffected = db.GetParameterValue(dbCommand, "RegistosProcessados");
                if (objRowsAffected != System.DBNull.Value)
                    RowsAffected = (System.Int16)objRowsAffected;
                return RowsAffected;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Does the parse line.
        /// </summary>
        /// <param name="State">The state.</param>
        /// <param name="line">The line.</param>
        /// <param name="ParseMessageName">Name of the parse message.</param>
        private void DoParseLine(ComponentState State, string line, string ParseMessageName)
        {
            // Set message for Message parser
            byte[] EDSTLine = Encoding.ASCII.GetBytes(line);
            State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, EDSTLine);
            MessageParser msgParser = new MessageParser();
            msgParser.RunComponent(State.OrchWrkData, new string[] { "#c#" + ParseMessageName });
            if (State.IsInError) return;
        }

    }
}
