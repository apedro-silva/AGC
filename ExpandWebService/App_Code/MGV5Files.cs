using System;
using SF.Expand.Core.Orch;
using SF.Expand.Switch.SwitchServices;
using SF.Expand.Switch.Clearing;


/// <summary>
/// Summary description for MGV5Files
/// </summary>
public class MGV5Files
{
    public MGV5Files()
    {
    }
    public string RunORI5Pipeline(string PipelineID, string FilePath, string FileName, out string JeFicheiroEmis)
    {
        string errorMsg;
        JeFicheiroEmis = string.Empty;

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, PipelineID, null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("EMISGetPath", FilePath, true);
            WrkData.GetWrkData().WriteNodeValue("EMISFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("CodTrn", "V01", true);
            WrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro em processamento", true);

            InitializeClearing initialize = new InitializeClearing();
            initialize.RunComponent(WrkData, null);

            JeFicheiroEmis = WrkData.GetWrkData().ReadNodeValue("JEFicheiroEMIS", true);

            ORI5 ori = new ORI5();
            ori.RunComponent(WrkData, null);

            errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return errorMsg;
    }

    public string RunDST5Pipeline(string PipelineID, string FilePath, string FileName, out string JeFicheiroEmis)
    {
        string errorMsg;
        JeFicheiroEmis = string.Empty;

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, PipelineID, null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("EMISGetPath", FilePath, true);
            WrkData.GetWrkData().WriteNodeValue("EMISFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("CodTrn", "V03", true);
            WrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro em processamento", true);

            InitializeClearing initialize = new InitializeClearing();
            initialize.RunComponent(WrkData, null);

            JeFicheiroEmis = WrkData.GetWrkData().ReadNodeValue("JEFicheiroEMIS", true);

            DST5 dst = new DST5();
            dst.RunComponent(WrkData, null);

            errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return errorMsg;
    }

    public string RunCLN5Pipeline(string PipelineID, string FilePath, string FileName, out string JeFicheiroEmis)
    {
        string errorMsg;
        JeFicheiroEmis = string.Empty;

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, PipelineID, null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("EMISGetPath", FilePath, true);
            WrkData.GetWrkData().WriteNodeValue("EMISFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("CodTrn", "V02", true);
            WrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro em processamento", true);

            InitializeClearing initialize = new InitializeClearing();
            initialize.RunComponent(WrkData, null);

            JeFicheiroEmis = WrkData.GetWrkData().ReadNodeValue("JEFicheiroEMIS", true);

            CLN5 ccln = new CLN5();
            ccln.RunComponent(WrkData, null);

            errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return errorMsg;
    }

    public string RunRMB5FilePipeline(string PipelineID, string FilePath, string RMB5FileName, string DST5FileName, string ORI5FileName, string CLN5FileName, string MOV5FileName, out string JeFicheiroEmis)
    {
        string errorMsg = "NOK";

        JeFicheiroEmis = string.Empty;

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, PipelineID, null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", RMB5FileName, true);
            WrkData.GetWrkData().WriteNodeValue("EDSTFileName", DST5FileName, true);
            WrkData.GetWrkData().WriteNodeValue("EORIFileName", ORI5FileName, true);
            WrkData.GetWrkData().WriteNodeValue("CCLNFileName", CLN5FileName, true);
            WrkData.GetWrkData().WriteNodeValue("MOVEFileName", MOV5FileName, true);
            WrkData.GetWrkData().WriteNodeValue("EMISGetPath", FilePath, true);
            WrkData.GetWrkData().WriteNodeValue("EMISFileName", RMB5FileName, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("CodTrn", "V04", true);
            WrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro em processamento", true);

            InitializeClearing initialize = new InitializeClearing();
            initialize.RunComponent(WrkData, null);

            JeFicheiroEmis = WrkData.GetWrkData().ReadNodeValue("JEFicheiroEMIS", true);
            RMB5 rmb = new RMB5();
            rmb.RunComponent(WrkData, null);

            errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);

        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return errorMsg;
    }


    internal string RunMOV5Pipeline(string PipelineID, string FilePath, string FileName, out string JeFicheiroEmis)
    {
        string errorMsg;
        JeFicheiroEmis = string.Empty;

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, PipelineID, null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("EMISGetPath", FilePath, true);
            WrkData.GetWrkData().WriteNodeValue("EMISFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("CodTrn", "V05", true);
            WrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro em processamento", true);

            InitializeClearing initialize = new InitializeClearing();
            initialize.RunComponent(WrkData, null);

            JeFicheiroEmis = WrkData.GetWrkData().ReadNodeValue("JEFicheiroEMIS", true);

            // Criar component MOVE para tratar ficheiros MOV5
            MOV5 move = new MOV5();
            move.RunComponent(WrkData, null);

            errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return errorMsg;
    }
}
