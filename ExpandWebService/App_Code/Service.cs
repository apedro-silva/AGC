using System;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;
using SF.Expand.Core.Orch;
using SF.Expand.Switch.ServiceComponents;
using SF.Expand.Switch.SwitchServices;
using SF.Expand.Switch.PipelineComponents;

/// <summary>
/// SF.Expand.Switch
/// </summary>
[WebService(Namespace = "http://sf.expand.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    /// <summary>
    /// 
    /// </summary>
    public eXPandWSInHeader eXPandInHeader;
    /// <summary>
    /// 
    /// </summary>
    public eXPandWSOutHeader eXPandOutHeader;

    /// <summary>
    /// Initializes a new instance of the <see cref="Service"/> class.
    /// </summary>
    public Service()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }


    /// <summary>
    /// Gets the state of the flex cube.
    /// </summary>
    /// <param name="FlexCubeResponseCode">The flex cube response code.</param>
    /// <returns></returns>
    private string GetFlexCubeState(string FlexCubeResponseCode)
    {
        if (FlexCubeResponseCode == "00")
            return "OK";
        else
            return "NOK";
    }

    /// <summary>
    /// Helloes the world.
    /// </summary>
    /// <returns></returns>
    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public string HelloWorld()
    {
        return "OK from ExpandWebService";
    }

    /// <summary>
    /// Runs the switch pipeline.
    /// </summary>
    /// <param name="prtClientMsg">The PRT client MSG.</param>
    /// <param name="switchResponse">The switch response.</param>
    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public void RunSwitchPipeline(byte[] prtClientMsg, out byte[] switchResponse)
    {
        //Cria Output Soap Headers
        eXPandOutHeader = new eXPandWSOutHeader();

        switchResponse = new byte[0];
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "SWITCH", null);
            WrkData.AddToObjBucket("PRTClientRequest", prtClientMsg);

            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "O", true);

            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Transacção em processamento", true);

            PRTService prtService = new PRTService();
            prtService.RunComponent(WrkData, null);

            eXPandOutHeader.eXPandErrorCode = WrkData.GetWrkData().ReadNodeValue("Erro", true);
            eXPandOutHeader.eXPandErrorDescription = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
            switchResponse = (byte[])WrkData.GetFromObjBucket("SWITCHResponse");
        }
        catch (Exception exp)
        {
            string errorMsg;
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            eXPandOutHeader.eXPandErrorCode = "Pipeline-Exception";
            eXPandOutHeader.eXPandErrorDescription = errorMsg;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
    }
    /// <summary>
    /// Runs the card pipeline.
    /// </summary>
    /// <param name="ServiceCode">The service code.</param>
    /// <param name="AccountNumber">The account number.</param>
    /// <param name="Amount">The amount.</param>
    /// <param name="CurrencyCode">The currency code.</param>
    /// <returns></returns>
    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public string RunCardPipeline(string ServiceCode, string AccountNumber, string Amount, string CurrencyCode)
    {
        //Cria Output Soap Headers
        eXPandOutHeader = new eXPandWSOutHeader();

        // Call Pipeline
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "Card", null);

        try
        {
            DateTime dt = DateTime.Now;

            string DataHora = string.Format("{0}{1}{2}{3}{4}{5}", dt.Year.ToString().PadLeft(4, '0'), dt.Month.ToString().PadLeft(2, '0'), dt.Day.ToString().PadLeft(2, '0'), dt.Hour.ToString().PadLeft(2, '0'), dt.Minute.ToString().PadLeft(2, '0'), dt.Second.ToString().PadLeft(2, '0')); 

            WrkData.GetWrkData().WriteNodeValue("Conta", AccountNumber.PadLeft(15,'0'), true);
            WrkData.GetWrkData().WriteNodeValue("Montante2", Amount.Replace(",", "").Replace(".", ""), true);
            WrkData.GetWrkData().WriteNodeValue("CodTrn", ServiceCode, true);
            WrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
            WrkData.GetWrkData().WriteNodeValue("DtHora", DataHora, true);
            WrkData.GetWrkData().WriteNodeValue("CodMoeda", CurrencyCode, true);
            WrkData.GetWrkData().WriteNodeValue("TipoTerm", "A", true);

            CARDService cardService = new CARDService();
            cardService.RunComponent(WrkData, null);


            string Erro = WrkData.GetWrkData().ReadNodeValue("CodResp", true);
            if (Erro == "0")
            {
                eXPandOutHeader.eXPandErrorCode = WrkData.GetWrkData().ReadNodeValue("Erro", true);
                eXPandOutHeader.eXPandErrorDescription = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
            }
            else
            {
                string ISOResponseCode = WrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
                if (ISOResponseCode != null)
                {
                    eXPandOutHeader.eXPandErrorCode = WrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
                    eXPandOutHeader.eXPandErrorDescription = WrkData.GetWrkData().ReadNodeValue("ISOResponseMessage", true);
                }
                else
                {
                    eXPandOutHeader.eXPandErrorCode = "1";
                    eXPandOutHeader.eXPandErrorDescription = "ExpandWebService Pipeline Error";
                }
            }
            return WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            string errorMsg;
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            eXPandOutHeader.eXPandErrorCode = "Pipeline-Exception";
            eXPandOutHeader.eXPandErrorDescription = errorMsg;
            return errorMsg;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
    }

    /// <summary>
    /// Runs the flex cube files processor.
    /// </summary>
    /// <param name="FileName">Name of the file.</param>
    /// <returns></returns>
    [WebMethod]
    public string RunFlexCubeFilesProcessor(string FileName)
    {
        string errorMsg = "OK";
        try
        {
            string FileType = FileName.Substring(FileName.LastIndexOf('\\') + 1).Substring(0, 4);

            switch (FileType)
            {
                case "ECSV": errorMsg = RunECSVProcessor(FileName); break;
                case "CURR": errorMsg = RunCURRProcessor(FileName); break;
                case "TAXA": errorMsg = RunTAXAProcessor(FileName); break;
                default:
                    break;
            }
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

    /// <summary>
    /// Runs the ECSV processor.
    /// </summary>
    /// <param name="FileName">Name of the file.</param>
    /// <returns></returns>
    public string RunECSVProcessor(string FileName)
    {
        string errorMsg = "OK";

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro em processamento", true);

            InitializeJE initJe = new InitializeJE();
            initJe.RunComponent(WrkData, new string[] { "JEClearing" });

            ProcessECSVFile ecsv = new ProcessECSVFile();
            ecsv.RunComponent(WrkData, null);

            FinalizeFileJE finJE = new FinalizeFileJE();
            finJE.RunComponent(WrkData, new string[] { "JEClearing", "E6" });

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

    /// <summary>
    /// Runs the CURR processor.
    /// </summary>
    /// <param name="FileName">Name of the file.</param>
    /// <returns></returns>
    public string RunCURRProcessor(string FileName)
    {
        string errorMsg = "OK";

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro em processamento", true);

            InitializeJE initJe = new InitializeJE();
            initJe.RunComponent(WrkData, new string[] { "JEClearing" });

            CURR curr = new CURR();
            curr.RunComponent(WrkData, null);

            FinalizeFileJE finJE = new FinalizeFileJE();
            finJE.RunComponent(WrkData, new string[] { "JEClearing", "E7" });
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

    /// <summary>
    /// Runs the TAXA processor.
    /// </summary>
    /// <param name="FileName">Name of the file.</param>
    /// <returns></returns>
    public string RunTAXAProcessor(string FileName)
    {
        string errorMsg = "OK";
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", FileName, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Ficheiro em processamento", true);

            InitializeJE initJe = new InitializeJE();
            initJe.RunComponent(WrkData, new string[] { "JEClearing" });

            ProcessEXCHFile exch = new ProcessEXCHFile();
            exch.RunComponent(WrkData, null);

            FinalizeFileJE finJE = new FinalizeFileJE();
            finJE.RunComponent(WrkData, new string[] { "JEClearing", "E8" });

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

    /// <summary>
    /// Runs the DRCC process file.
    /// </summary>
    /// <param name="FilePathName">Name of the file path.</param>
    /// <param name="ForcedFile">if set to <c>true</c> [forced file].</param>
    /// <returns></returns>
    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public string RunDRCCProcessFile(string FilePathName, Boolean ForcedFile)
    {
        string result = "OK";
        //Cria Output Soap Headers
        eXPandOutHeader = new eXPandWSOutHeader();

        // Call Pipeline
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();

        try
        {
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DRCCProcessFile", null);
            WrkData.GetWrkData().WriteNodeValue("InputFileName", FilePathName, true);
            if (ForcedFile)
                WrkData.GetWrkData().WriteNodeValue("ForcedFile", "1", true);
            else
                WrkData.GetWrkData().WriteNodeValue("ForcedFile", "0", true);

            string FileName = FilePathName.Substring(FilePathName.LastIndexOf('\\') + 1);
            WrkData.GetWrkData().WriteNodeValue("FicheiroDRCC", FileName, true);

            ProcessDRCCFile drccProcessFile = new ProcessDRCCFile();
            drccProcessFile.RunComponent(WrkData, null);
            string Erro = WrkData.GetWrkData().ReadNodeValue("Erro", true);
            if (Erro == "0")
                result = "OK";
            else if (Erro == "1")
                result = "NOK_DUP_OK";
            else if (Erro == "2")
                result = "NOK_FNF";
            else if (Erro == "4")
                result = "NOK_BFT";
            else if (Erro == "6")
                result = "NOK_PEN";
            else if (Erro == "7")
                result = "NOK_DUP_NOK";
            else 
                result = "NOK";
            
            eXPandOutHeader.eXPandErrorCode = Erro;
            eXPandOutHeader.eXPandErrorDescription = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            result = "NOK";
            string errorMsg = "";
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            eXPandOutHeader.eXPandErrorCode = "Pipeline-Exception";
            eXPandOutHeader.eXPandErrorDescription = errorMsg;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return result;
    }

    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public string RunDRCCExecute(int DRCCFileId)
    {
        string errorMsg = "OK";
        //Cria Output Soap Headers
        eXPandOutHeader = new eXPandWSOutHeader();

        // Call Pipeline
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();

        try
        {
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DRCC", null);
            WrkData.GetWrkData().WriteNodeValue("DRCCFileId", DRCCFileId.ToString(), true);

            ExecuteDRCCFile drccExecute = new ExecuteDRCCFile();
            drccExecute.RunComponent(WrkData, null);

            eXPandOutHeader.eXPandErrorCode = WrkData.GetWrkData().ReadNodeValue("Erro", true);
            eXPandOutHeader.eXPandErrorDescription = errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            eXPandOutHeader.eXPandErrorCode = "Pipeline-Exception";
            eXPandOutHeader.eXPandErrorDescription = errorMsg;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return errorMsg;
    }

    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public string RunDRCCSimulate(int DRCCFileId)
    {
        string errorMsg = "OK";
        //Cria Output Soap Headers
        eXPandOutHeader = new eXPandWSOutHeader();

        // Call Pipeline
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();

        try
        {
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DRCCSimulation", null);
            WrkData.GetWrkData().WriteNodeValue("DRCCFileId", DRCCFileId.ToString(), true);

            SimulateDRCCFile drccProcessNextLine = new SimulateDRCCFile();
            drccProcessNextLine.RunComponent(WrkData, null);

            eXPandOutHeader.eXPandErrorCode = WrkData.GetWrkData().ReadNodeValue("Erro", true);
            eXPandOutHeader.eXPandErrorDescription = errorMsg = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            eXPandOutHeader.eXPandErrorCode = "Pipeline-Exception";
            eXPandOutHeader.eXPandErrorDescription = errorMsg;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
        return errorMsg;
    }

    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public void RunEchoPipeline(out string FlexCubeAtmState, out string FlexCubePosState, out string PRTState)
    {
        FlexCubeAtmState = "NOK";
        FlexCubePosState = "NOK";
        PRTState = "NOK";

        //Cria Output Soap Headers
        eXPandOutHeader = new eXPandWSOutHeader();

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "ECHO", null);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "O", true);
            WrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
            WrkData.GetWrkData().WriteNodeValue("CodTrn", "ET", true);
            WrkData.GetWrkData().WriteNodeValue("TipoTerm", "A", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Transacção em processamento", true);
            
            ECHOService echoService = new ECHOService();
            echoService.RunComponent(WrkData, null);

            string FlexCubeResponseCode = WrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
            FlexCubeAtmState = GetFlexCubeState(FlexCubeResponseCode);

            WrkData.GetWrkData().WriteNodeValue("TipoTerm", "B", true);

            if (WrkData.GetWrkData().GetNodeByName("fc-ResponseCode") != null)
                WrkData.GetWrkData().DeleteNode("fc-ResponseCode");

            echoService.RunComponent(WrkData,null);

            FlexCubeResponseCode = WrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
            FlexCubePosState = GetFlexCubeState(FlexCubeResponseCode);

            string PRTStatus = WrkData.GetWrkData().ReadNodeValue("PRTStatus", true);
            if (PRTStatus == "OK")
            {
                PRTState = "OK";
            }

            eXPandOutHeader.eXPandErrorCode = WrkData.GetWrkData().ReadNodeValue("Erro", true);
            eXPandOutHeader.eXPandErrorDescription = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            string errorMsg;
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            eXPandOutHeader.eXPandErrorCode = "Pipeline-Exception";
            eXPandOutHeader.eXPandErrorDescription = errorMsg;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
    }

    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public void RunPRTStatusPipeline(string PRTStatus)
    {
        //Cria Output Soap Headers
        eXPandOutHeader = new eXPandWSOutHeader();

        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        try
        {
            // Call Pipeline
            IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "PRTStatus", null);
            WrkData.GetWrkData().WriteNodeValue("PRTStatus", PRTStatus, true);
            WrkData.GetWrkData().WriteNodeValue("OnlineBatch", "O", true);
            WrkData.GetWrkData().WriteNodeValue("CodTrn", "PS", true);
            WrkData.GetWrkData().WriteNodeValue("TextoErro", "Transacção em processamento", true);

            PRTStatusService prtStatusService = new PRTStatusService();
            prtStatusService.RunComponent(WrkData, null);

            eXPandOutHeader.eXPandErrorCode = WrkData.GetWrkData().ReadNodeValue("Erro", true);
            eXPandOutHeader.eXPandErrorDescription = WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
        }
        catch (Exception exp)
        {
            string errorMsg;
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            eXPandOutHeader.eXPandErrorCode = "Pipeline-Exception";
            eXPandOutHeader.eXPandErrorDescription = errorMsg;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
    }

    [SoapHeader("eXPandInHeader", Direction = SoapHeaderDirection.In)]
    [SoapHeader("eXPandOutHeader", Direction = SoapHeaderDirection.Out)]
    [WebMethod]
    public string GetAccountName(string AccountNumber)
    {
        string AccountName = "";

        Account acc = new Account();
        AccountName = acc.GetAccountName(AccountNumber);
        return AccountName;
    }


    private string GetFileType(string FileName)
    {
        string s = "";

        // Open the stream and read header line.
        using (StreamReader sr = File.OpenText(FileName))
        {
            s = sr.ReadLine();
            s = s.Substring(2, 4);
        }

        return s;
    }




    [WebMethod]
    public string RunPS2Transaction(string ServiceId, string CardNumber, string DebitAccountNumber, string CreditAccountNumber, string Amount, string CurrencyCode, string ProcessingDate)
    {
        // Call Pipeline
        IOrchestrator eXPandOrch = WebServiceIntegrator.GetOrchestrator();
        IOrchWrkData WrkData = WebServiceIntegrator.GetWrkData(eXPandOrch, "DUMMY", null);

        try
        {
            DateTime dt = DateTime.Now;

            string DataHora = string.Format("{0}{1}{2}{3}", ProcessingDate.ToString(), "00", "00", "00");

            WrkData.GetWrkData().WriteNodeValue("CodTrn", ServiceId, true);

            DebitAccountNumber = DebitAccountNumber.Trim().PadLeft(11, '0');
            CreditAccountNumber = CreditAccountNumber.Trim().PadLeft(11, '0');
            WrkData.GetWrkData().WriteNodeValue("NumeroCartao", CardNumber, true);
            WrkData.GetWrkData().WriteNodeValue("Conta", DebitAccountNumber, true);
            WrkData.GetWrkData().WriteNodeValue("ContaDebito", DebitAccountNumber, true);
            WrkData.GetWrkData().WriteNodeValue("ContaCredito", CreditAccountNumber, true);
            WrkData.GetWrkData().WriteNodeValue("Montante2", Amount.Replace(",", "").Replace(".", ""), true);
            WrkData.GetWrkData().WriteNodeValue("CodMoeda", CurrencyCode, true);
            WrkData.GetWrkData().WriteNodeValue("DtHora", DataHora, true);

            ProcessPS2Transaction ps2 = new ProcessPS2Transaction();
            ps2.RunComponent(WrkData, null);
			string textoErro=WrkData.GetWrkData().ReadNodeValue("TextoErro", true);
			if (textoErro!="OK")
				return textoErro;
			else
				return WrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
        }
        catch (Exception exp)
        {
            string errorMsg;
            if (exp.InnerException != null)
                errorMsg = exp.Source + " : " + exp.Message + " : " + exp.InnerException.Message;
            else
                errorMsg = exp.Source + " : " + exp.Message;
            return errorMsg;
        }
        finally
        {
            eXPandOrch.FinalizePipe();
        }
    }

}
