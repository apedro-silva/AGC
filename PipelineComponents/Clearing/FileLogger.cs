using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Threading;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// FileLogger class. Switch Component that performs the Log of the Clearing process
    /// </summary>
    public class FileLogger : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The OrchWrkData.</param>
        /// <param name="Params">The Params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);
            State.ResetLastError();
            StreamReader sr=null;

            try
            {
                InitializeJE initJe = new InitializeJE();
                State.OrchWrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
                initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });

                State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "00", true);

                string FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("EMISFileName");
                sr = OpenEmisFile(FileName);
                string line;
                string TipoRegisto;

                // le ficheiro para obter informação do trailler para logar
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() == "") continue;
                    TipoRegisto = line.Substring(0, 1);
                    if (TipoRegisto == "0")
                    {
                        State.OrchWrkData.GetWrkData().WriteNodeValue("EMISFileHeader", line, true);
                        DoParseLine(State, line, "EMIS_TIPREG_0");

                        string Ficheiro = State.OrchWrkData.GetWrkData().ReadNodeValue("Fich", true);
                        switch (Ficheiro)
                        {
                            case "ERMB": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "E4", true); break;
                            case "EORI": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "E1", true); break;
                            case "EDST": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "E3", true); break;
                            case "CCLN": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "E2", true); break;
                            case "RMB5": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "V04", true); break;
                            case "ORI5": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "V01", true); break;
                            case "DST5": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "V03", true); break;
                            case "CLN5": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "V02", true); break;
                            case "MOV5": State.OrchWrkData.GetWrkData().WriteNodeValue("CodTrn", "V05", true); break;
                            default: break;
                        }
                        State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
                    }
                    else if (TipoRegisto == "9")
                    {
                        State.OrchWrkData.GetWrkData().WriteNodeValue("EMISFileTrailler", line, true);
                        DoParseLine(State, line, "EMIS_TIPREG_9");
                    }
                }

                InitializeEMISFileLog fileLog = new InitializeEMISFileLog();
                fileLog.RunComponent(State.OrchWrkData, null);

                State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro recebido", true);
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
                finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
            }

        }

        /// <summary>
        /// Opens the emis file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The File StreamReader</returns>
        private StreamReader OpenEmisFile(string filePath)
        {
            StreamReader sr=null;
            String line = string.Empty;
            int i = 0;
            while (i < 5)
            {
                try
                {
                    sr = new StreamReader(filePath, Encoding.ASCII);
                    break;
                }
                catch (Exception) {  }
                Thread.Sleep(3000);
                i++;
            }
            return sr;

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
            byte[] EMISLine = Encoding.ASCII.GetBytes(line);
            State.OrchWrkData.AddToObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE, EMISLine);
            MessageParser msgParser = new MessageParser();
            msgParser.RunComponent(State.OrchWrkData, new string[] { "#c#" + ParseMessageName });
            if (State.IsInError) throw new Exception(State.LastError.ToString());
        }

    }
}
