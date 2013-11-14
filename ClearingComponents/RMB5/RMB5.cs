using System;
using System.IO;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using System.Globalization;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.Clearing
{
    public class RMB5 : OrchPipeComponent
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
                String TIPREG = string.Empty;
                String TOTREG = string.Empty;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() == "") continue;

                    TIPREG = line.Substring(0, 1);
                    ProcessLine(State, line);
                    if (TIPREG == "9")
                    {
                        TOTREG = line.Substring(1, 8);
                        if (i != Convert.ToInt32(TOTREG))
                            throw new BusinessException("ERMB.ProcessFile->TOTREG <> Registos no ficheiro!");
                        break;
                    }
                    if (TIPREG == "1")
                        i++;
                }
                
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
                finJE.RunComponent(State.OrchWrkData, new string[] { "JEFicheiroEMIS", "V04" });
            }

        }
        private void ProcessLine(OrchPipeComponent.ComponentState State, string line)
        {
            if (line.Length != 69)
                throw new BusinessException(string.Format("ProcessLine->Invalid Record length <{0}> !", line.Length));

            string TIPREG = line.Substring(0, 1);
            string FICH = line.Substring(1, 4);

            switch (TIPREG)
            {
                // header 
                case "0":
                    break;
                // registos de detalhe
                case "1": ProcessFileTrailer(State, line, FICH); 
                    break;
                // trailler
                case "9":
                    string TOTDEB = line.Substring(9, 16);
                    string TOTCRED = line.Substring(25, 16);
                    double dblDeb = Convert.ToDouble(FormatTotal(TOTDEB));
                    double dblCre = Convert.ToDouble(FormatTotal(TOTCRED));
                    double dblRes;
                    dblRes = dblCre - dblDeb;
                    string RelatorioERMB = State.OrchWrkData.GetWrkData().ReadNodeValue("RelatorioERMB", true);
                    RelatorioERMB = RelatorioERMB + "TRAILER" + ":\t\t" + GetTotalFormattedValue(TOTDEB) + "\t\t\t" + GetTotalFormattedValue(TOTCRED) + "\r\n";
                    RelatorioERMB = RelatorioERMB + "TOTAL A RECEBER(+) / PAGAR (-) NOSTRO" + ":\t\t\t" + GetTotalFormattedValue(dblRes.ToString());
                    State.OrchWrkData.GetWrkData().WriteNodeValue("RelatorioERMB", RelatorioERMB, true);
                    break;
                default: return;
            }
        }

        private string GetTotalFormattedValue(string inValue)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            double dblValue = Convert.ToDouble(FormatTotal(inValue));
            string outValue = dblValue.ToString("N", nfi);
            outValue = "                    ".Substring(0, 20-outValue.Length) + outValue;
            return outValue;
        }

        private void ProcessFileTrailer(OrchPipeComponent.ComponentState State, string ERMBLine, string FICH)
        {
            string TIPREG = string.Empty;
            string TOTREG = string.Empty;
            string TOTDEB = string.Empty;
            string TOTCRED = string.Empty;
            string ERMBTOTREG = ERMBLine.Substring(21, 8);
            string ERMBTOTDEB = ERMBLine.Substring(29, 16);
            string ERMBTOTCRED = ERMBLine.Substring(45, 16);
            string FileName = string.Empty;

            string RelatorioERMB = State.OrchWrkData.GetWrkData().ReadNodeValue("RelatorioERMB", true);

            switch (FICH)
            {
                case "DST5": FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("EDSTFileName"); break;
                case "ORI5": FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("EORIFileName"); break;
                case "CLN5": FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("CCLNFileName"); break;
                case "MOV5": FileName = State.OrchWrkData.GetWrkData().ReadNodeValue("MOVEFileName"); break;
                default:
                    throw new BusinessException(string.Format("ProcessLine->Invalid FICH Type <{0}> !", FICH));
            }
                

            // abrir ficheiro no directorio FICH
            using (StreamReader sr = new StreamReader(FileName))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim() == "") continue;

                    TIPREG = line.Substring(0, 1);
                    if (TIPREG == "9")
                    {
                        TOTREG = line.Substring(1, 8);
                        TOTDEB = line.Substring(9, 16);
                        TOTCRED = line.Substring(25, 16);
                        break;
                    }
                }
            }

            RelatorioERMB = RelatorioERMB + FICH + "...:\t\t" + GetTotalFormattedValue(TOTDEB) + "\t\t\t" + GetTotalFormattedValue(TOTCRED) + "\r\n";
            State.OrchWrkData.GetWrkData().WriteNodeValue("RelatorioERMB", RelatorioERMB, true);

            // comparar TOTREG com FICH.Trailer.TOTREG
            if (FormatTotal(TOTREG) != FormatTotal(ERMBTOTREG))
                throw new BusinessException("ERMB.ProcessFile->TOTREG no fihceiro ERMB <> TOTREG no ficheiro <" + FICH + ">!");

            // comparar TOTDEB com FICH.Trailer.TOTDEB
            if (FormatTotal(TOTDEB) != FormatTotal(ERMBTOTDEB))
                throw new BusinessException("ERMB.ProcessFile->TOTDEB no ficheiro ERMB <> TOTDEB no ficheiro <" + FICH + ">!");

            // comparar TOTCRED com FICH.Trailer.TOTCRED
            if (FormatTotal(TOTCRED) != FormatTotal(ERMBTOTCRED))
                throw new BusinessException("ERMB.ProcessFile->TOTCRED no ficheiro ERMB <> TOTCRED no ficheiro <" + FICH + ">!");
        }
        private string FormatTotal(string inTotal)
        {
            return inTotal.Replace(' ', '0');
        }
        private string GetFileName(string FilePath)
        {
            string FileName = string.Empty;
            try
            {
                FileName = FilePath.Substring(FilePath.LastIndexOf('\\') + 1);
            }
            catch (Exception)
            {
                throw;
            }
            return FileName;
        }
    }
}
