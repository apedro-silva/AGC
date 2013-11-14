using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Services;
using SF.Expand.Core.Orch;
using SF.Expand.Switch.Clearing;
using SF.Expand.Notification;

/// <summary>
/// Summary description for MGV5Service
/// </summary>
[WebService(Namespace = "http://sf.expand.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class MGV5Service : System.Web.Services.WebService
{

    public MGV5Service()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string RunFileLogger(string FilePath)
    {
        string errorMsg = "OK";
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();

        try
        {
            // A Pipeline é só para ter um WorkData!
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);
            WrkData.GetWrkData().WriteNodeValue("EMISFileName", FilePath, true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro recebido", true);
            FileLogger fileLog = new FileLogger();
            fileLog.RunComponent(WrkData, null);
            if (WrkData.GetWrkData().ReadNodeValue("Erro", true) == "1")
                errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        return errorMsg;
    }

    [WebMethod]
    public string RunErrorFileLogger(string FilePath)
    {
        string errorMsg = "OK";
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();

        try
        {
            // A Pipeline é só para ter um WorkData!
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);
            WrkData.GetWrkData().WriteNodeValue("EMISFileName", FilePath, true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro recebido", true);
            ErrorFileLogger fileLog = new ErrorFileLogger();
            fileLog.RunComponent(WrkData, null);
            if (WrkData.GetWrkData().ReadNodeValue("Erro", true) == "1")
                errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        return errorMsg;
    }
    [WebMethod]
    public string RunErrorFileProcessor(string FilePath)
    {
        string errorMsg;
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Transacção em processamento", true);
            WrkData.GetWrkData().WriteNodeValue("Erro", "0", true);

            StreamReader sr = new StreamReader(FilePath, Encoding.ASCII);
            WrkData.AddToObjBucket("FileStreamReader", sr);


            SF.Expand.Switch.Clearing.EERR eerr = new EERR();
            eerr.RunComponent(WrkData, null);
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


    [WebMethod]
    public string RunMGV5FilesProcessor(string FilePath, string RMB5FileName, string CLN5FileName, string DST5FileName, string ORI5FileName, string MOV5FileName)
    {
        string errorMsg;
        string rmb5JEFicheiroEMIS=string.Empty;
        string ori5JEFicheiroEMIS = string.Empty;
        string mov5JEFicheiroEMIS = string.Empty;
        string dst5JEFicheiroEMIS = string.Empty;
        string cln5JEFicheiroEMIS = string.Empty;

        try
        {
            MGV5Files mgv5File = new MGV5Files();

            errorMsg = mgv5File.RunRMB5FilePipeline("RMB5", FilePath, RMB5FileName, DST5FileName, ORI5FileName, CLN5FileName, MOV5FileName, out rmb5JEFicheiroEMIS);
            if (errorMsg == "Ficheiro processado")
                errorMsg = mgv5File.RunCLN5Pipeline("CLN5", FilePath, CLN5FileName, out cln5JEFicheiroEMIS);
            if (errorMsg == "Ficheiro processado")
                errorMsg = mgv5File.RunDST5Pipeline("DST5", FilePath, DST5FileName, out dst5JEFicheiroEMIS);
            if (errorMsg == "Ficheiro processado")
                errorMsg = mgv5File.RunMOV5Pipeline("MOV5", FilePath, MOV5FileName, out mov5JEFicheiroEMIS);
            if (errorMsg == "Ficheiro processado")
                errorMsg = mgv5File.RunORI5Pipeline("ORI5", FilePath, ORI5FileName, out ori5JEFicheiroEMIS);

            if (errorMsg != "Ficheiro processado")
            {
                FinalizeClearing finalizeClearing = new FinalizeClearing();
                finalizeClearing.InvalidateClearing(rmb5JEFicheiroEMIS, ori5JEFicheiroEMIS, dst5JEFicheiroEMIS, mov5JEFicheiroEMIS, cln5JEFicheiroEMIS);
                NotificationService.Send("Clearing.File", "Ficheiros de compensação carregados com sucesso!", null);
            }
            else 
                errorMsg = ProcessJeEmisRecords("55");
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        return errorMsg;
    }


    [WebMethod]
    public string ProcessJeEmisRecords(string situacao)
    {
        string errorMsg;
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Transacção em processamento", true);
            WrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            WrkData.GetWrkData().WriteNodeValue("Situacao", situacao, true);

            ProcessJEEmisRecords processJEEmisRecords = new ProcessJEEmisRecords();
            processJEEmisRecords.RunComponent(WrkData, null);
            errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            NotificationService.Send("Clearing.Processing", "Erro no processamento da compensação:" +  errorMsg, null);
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return errorMsg;
    }
    [WebMethod]
    public string ReprocessJeRecord(int Je)
    {
        string newJe = null;
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Transacção em processamento", true);
            WrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            WrkData.GetWrkData().WriteNodeValue("Je2Reprocess", Je.ToString(), true);

            ReprocessJeRecord reprocessJeRecord = new ReprocessJeRecord();
            reprocessJeRecord.RunComponent(WrkData, null);
            newJe = WrkData.GetWrkData().ReadNodeValue("JE", true);
        }
        catch (Exception exp)
        {
            string errorMsg;
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return newJe;
    }

}

