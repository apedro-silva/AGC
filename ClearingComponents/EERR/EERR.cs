using System;
using System.Text;
using System.IO;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using SF.Expand.Switch.PipelineComponents;


namespace SF.Expand.Switch.Clearing
{
    /// <summary>
    /// Switch component that processes the EERR file from SIBS/EMIS.
    /// </summary>
    public class EERR : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            InitializeJE initJe = new InitializeJE();
            FinalizeJE finJE = new FinalizeJE();

            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;


            StreamReader sr = (StreamReader)State.OrchWrkData.GetFromObjBucket("FileStreamReader");
            try
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "V07", true);
                initJe.RunComponent(State.OrchWrkData, new string[] { "JEFile" });

                String line = null;
                String TIPREG = null;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    CleanWorkData(State);
                    if (line.Trim() == "") continue;

                    State.OrchWrkData.GetWrkData().WriteNodeValue("EMISFileRecord", line, true);
                    TIPREG = line.Substring(0, 1);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "ER" + TIPREG, true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
                    DoParseLine(State, line, "EERR_TIPREG_" + TIPREG);

                    initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });
                    ProcessCard(State, line);
                    finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });

                    i += 1;
                }
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message);
            }
            finally
            {
                finJE.RunComponent(State.OrchWrkData, new string[] { "JEFile" });
                if (sr != null)
                    sr.Close();
            }
        }

        /// <summary>
        /// Processes a card record from the CCLN file.
        /// </summary>
        /// <param name="State">The ComponentState</param>
        /// <param name="line">The EERR record line</param>
        private void ProcessCard(OrchPipeComponent.ComponentState State, string line)
        {
            string codErro = string.Empty;
            string message2Log = string.Empty;

            string TIPREG = line.Substring(0, 1);
            try
            {
                codErro = State.OrchWrkData.GetWrkData().ReadNodeValue("CodErro", true);
                switch (TIPREG)
                {
                    // o registo de header "TIPREG=0"
                    case "0": break;

                    // o registo de detalhe "TIPREG=1", quando corresponde à informação de que um ficheiro foi processado; 
                    case "1":
                        string hdrSit = State.OrchWrkData.GetWrkData().ReadNodeValue("HdrSit", true);
                        string fich = State.OrchWrkData.GetWrkData().ReadNodeValue("Fich", true);
                        string dataFich = State.OrchWrkData.GetWrkData().ReadNodeValue("DataFich", true);
                        string seqFich = State.OrchWrkData.GetWrkData().ReadNodeValue("SeqFich", true);
                        message2Log = string.Format("Ficheiro {0} de {1} com sequência {2} está {3}", fich, dataFich, seqFich, GetHeaderSituation(hdrSit));
                        State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", message2Log, true);
                        break;

                    // o registo de detalhe "TIPREG=2", é criado sempre que se verifique invalidade num detalhe do ficheiro, 
                    // sendo apresentado o registo do ficheiro enviado pelo Emissor que provocou o erro; 
                    case "2":
                        string BIN = State.OrchWrkData.GetWrkData().ReadNodeValue("BIN", true);
                        string EXBIN= State.OrchWrkData.GetWrkData().ReadNodeValue("EXBIN", true);
                        string NumCar = State.OrchWrkData.GetWrkData().ReadNodeValue("NumCar", true);
                        string conta = State.OrchWrkData.GetWrkData().ReadNodeValue("Conta", true);
                        codErro = State.OrchWrkData.GetWrkData().ReadNodeValue("CodErro", true);
                        message2Log = string.Format("Cartão {0}{1}{2} com conta {3} ", BIN, EXBIN, NumCar, GetCardRecordError(codErro));
                        break;
                    case "9": break;
                    default:
                        State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "00", true);
                        State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Tipo registo " + TIPREG + " no EERR não suportado", true);
                        return;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private object GetCardRecordError(string codErro)
        {
            try
            {
                string RowsAffected = string.Empty;
                string recordErrorDescription = string.Empty;

                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("GetCardRecordError");
                db.AddInParameter(dbCommand, "CardError", DbType.String, codErro);
                db.AddOutParameter(dbCommand, "CardErrorDescription", DbType.String, 100);
                db.ExecuteNonQuery(dbCommand);
                Object recordErrorObj = db.GetParameterValue(dbCommand, "RecordErrorDescription");
                if (recordErrorObj != System.DBNull.Value)
                    recordErrorDescription = (System.String)recordErrorObj;
                return recordErrorDescription;
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
            byte[] EERRLine = Encoding.ASCII.GetBytes(line);
            State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, EERRLine);
            MessageParser msgParser = new MessageParser();
            msgParser.RunComponent(State.OrchWrkData, new string[] { "#c#" + ParseMessageName });
            if (State.IsInError) return;
        }

        /// <summary>
        /// Returns the header situation description.
        /// </summary>
        /// <param name="Situation">The situation.</param>
        private string GetHeaderSituation(string hdrSituation)
        {
            string description = string.Empty;
            switch (hdrSituation)
            {
                case "0": description = "Correcto";  break;
                case "1": description = "com erro de formato"; break;
                case "2": description = "com erro de sequência"; break;
                case "3": description = "com erro de data"; break;
                case "4": description = "com erro do registo de parâmetros"; break;
                case "5": description = "com Header não reconhecido"; break;
                case "6": description = "com Instituição origem inválida"; break;
            }
            return description;
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

    }
}
