using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    public class ProcessDRCCFile :OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            string[] localParams = new string[1];
            int parseFile=0;
            string FileName=null;
            StreamReader sr=null;
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;

            State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "E5", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            
            InitializeJE initJE = new InitializeJE();
            localParams[0] = "JE";
            initJE.RunComponent(OrchWrkData, localParams);

            try
            {
                FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("InputFileName");
                sr = new StreamReader(FileName, Encoding.ASCII);
                string line = string.Empty;
                int NumRegistosTipoUm = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "")
                        continue;
                    string TIPREG = line.Substring(0, 1);
                    DoParseLine(State, line, "DRCC_TIPREG_" + TIPREG);

                    switch (TIPREG)
                    {
                        case "0": InsertDRCCFile(State); parseFile = parseFile | 0x1; break;
                        case "1": InsertDRCCRecord(State); parseFile = parseFile | 0x2; NumRegistosTipoUm += 1; break;
                        case "9": UpdateDRCCFile(State); parseFile = parseFile | 0x4; break;
                    }
                }

                string TotalRegistosTipoUm = State.OrchWrkData.GetWrkData().ReadNodeValue("TotReg", true);

                if (Convert.ToInt32(TotalRegistosTipoUm) != NumRegistosTipoUm)
                {
                    DeleteDRCCFileAndRecords(State);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "4", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "SS", true);
                    throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. O ficheiro está mal formatado");
                }

                if (parseFile == 0x7)
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "0", true);
                else
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "4", true);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "SS", true);
                    throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. O ficheiro está mal formatado");
                }
                if (sr != null)
                    sr.Close();
                if (parseFile == 0x7 && FileName != null)
                    MoveFile2Done(FileName);
            }
            catch (FileNotFoundException)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "ERRO: Carregamento do ficheiro DRCC não efectuado. Ficheiro inexistente", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "2", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "SS", true);
            }
            catch (BusinessException exp)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "SS", true);
            }
            catch (Exception exp)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "3", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "SS", true);
            }
            finally
            {
                if (sr !=null)
                    sr.Close();
            }
            // limpa estes campos para irem para o LOG
            if (State.OrchWrkData.GetWrkData().GetNodeByName("Conta") != null)
                State.OrchWrkData.GetWrkData().DeleteNode("Conta");
            if (State.OrchWrkData.GetWrkData().GetNodeByName("NumCar") != null)
                State.OrchWrkData.GetWrkData().DeleteNode("NumCar");
            if (State.OrchWrkData.GetWrkData().GetNodeByName("Montante2") != null)
                State.OrchWrkData.GetWrkData().DeleteNode("Montante2");

            State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "S0", true);
            FinalizeJE endJE = new FinalizeJE();
            endJE.RunComponent(OrchWrkData, localParams);
        }

        private void MoveFile2Done(string FileName)
        {
            string myPath = FileName.Substring(0, FileName.LastIndexOf('\\') + 1);
            try
            {
                Directory.CreateDirectory(myPath + "\\Done");
            }
            catch (Exception)
            {
            }
            string DestFileName = FileName.Substring(FileName.LastIndexOf('\\') + 1);
            File.Move(FileName, myPath + "Done\\" + DestFileName + "." + DateTime.Now.Ticks.ToString());
        }

        private void InsertDRCCRecord(ComponentState State)
        {
            try
            {
                System.Configuration.AppSettingsReader s = new AppSettingsReader();
                string FlexCubeServicesURL = (string)s.GetValue("FlexCubeServices", typeof(string));
                string NumeroConta = State.OrchWrkData.GetWrkData().ReadNodeValue("Conta",true);
                string BranchCode;
                string CCY;
                string AccountClass;
                string IBAN;

                FlexCubeServices.Service flexCube = new FlexCubeServices.Service();
                flexCube.Url = FlexCubeServicesURL;

                string CustomerName = flexCube.GetAccountInfo(NumeroConta, out BranchCode, out CCY, out AccountClass, out IBAN);

                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("InsertDRCCRecord");
                db.AddInParameter(dbCommand, "IDFileDRCC", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IDFileDRCC"));
                db.AddInParameter(dbCommand, "AccountCreditNum", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("NumCtaCred"));
                db.AddInParameter(dbCommand, "CardNum", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("NumCar"));
                db.AddInParameter(dbCommand, "AccountNum", DbType.String, NumeroConta);
                db.AddInParameter(dbCommand, "DebitAmountEUR", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2"));
                db.AddInParameter(dbCommand, "ValueDate", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("DataDeb"));
                db.AddInParameter(dbCommand, "CCYCode", DbType.String, CCY);
                db.AddInParameter(dbCommand, "CustomerName", DbType.String, CustomerName);

                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {
                throw;
                //throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. Erro no InsertDRCCRecord");
            }
        }

        private void DeleteDRCCFileAndRecords(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("DeleteDRCCFileAndRecords");
                db.AddInParameter(dbCommand, "IDFileDRCC", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IDFileDRCC"));

                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {
                throw;
                //throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. Erro no InsertDRCCRecord");
            }
        }
        private void UpdateDRCCFile(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("UpdateDRCCFile");
                db.AddInParameter(dbCommand, "IDFileDRCC", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("IDFileDRCC"));
                db.AddInParameter(dbCommand, "TotalRecs", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("TotReg"));
                db.AddInParameter(dbCommand, "TotalAmount", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("TotValor"));

                db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception)
            {
                throw;
                //throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. Erro no UpdateDRCCFile");
            }
        }

        private void InsertDRCCFile(ComponentState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DbCommand dbCommand = db.GetStoredProcCommand("InsertDRCCFile");
                db.AddInParameter(dbCommand, "FileDate", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("Data"));
                db.AddInParameter(dbCommand, "FileSequence", DbType.String, State.OrchWrkData.GetWrkData().ReadNodeValue("Sequencia"));
                db.AddInParameter(dbCommand, "Forced", DbType.Int16, State.OrchWrkData.GetWrkData().ReadNodeValue("ForcedFile"));
                db.AddOutParameter(dbCommand, "IDFileDRCC", DbType.Int64, 18);
                db.ExecuteNonQuery(dbCommand);
                string IDFileDRCC = db.GetParameterValue(dbCommand, "IDFileDRCC").ToString();
                if (IDFileDRCC == "-1") // Ficheiro duplicado ainda por tratar
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "1", true);
                    throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. Ficheiro ainda não tratado");
                }
                if (IDFileDRCC == "-2") // ficheiro Pendente
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "6", true);
                    throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. Existe um ficheiro pendente");
                }
                if (IDFileDRCC == "-3") // ficheiro duplicado já tratado
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "7", true);
                    throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. Ficheiro já tratado");
                }
                State.OrchWrkData.GetWrkData().WriteNodeValue("IDFileDRCC", IDFileDRCC, true);

            }
            catch (Exception)
            {
                throw;
                //throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. Erro no InsertDRCCFile");
            }
        }

        private void DoParseLine(ComponentState State, string line, string ParseMessageName)
        {
            try
            {
                // Set message for Message parser
                byte[] DRCCLine = Encoding.ASCII.GetBytes(line);
                State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, DRCCLine);
                MessageParser msgParser = new MessageParser();
                msgParser.RunComponent(State.OrchWrkData, new string[] { "#c#" + ParseMessageName});
            }
            catch (Exception)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "5", true);
                throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. Erro no DoParseLine");
            }
            if (State.IsInError)
            {
                State.ResetLastError();
                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "6", true);
                throw new BusinessException("ERRO: Carregamento do ficheiro DRCC não efectuado. O ficheiro está mal formatado");
            }
        }

    }
}
