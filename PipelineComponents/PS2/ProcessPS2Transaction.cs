using System;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using SF.Expand.Switch.PipelineComponents;

namespace SF.Expand.Switch.SwitchServices
{
    public class ProcessPS2Transaction : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new OrchPipeComponent.ComponentState(OrchWrkData, Params);
            if (State.IsInError)
                return;
            DoFundsTransfer(State);
        }
        private void DoFundsTransfer(OrchPipeComponent.ComponentState State)
        {
            string NumeroContaDebito = State.OrchWrkData.GetWrkData().ReadNodeValue("ContaDebito", true);
            string NumeroContaCredito = State.OrchWrkData.GetWrkData().ReadNodeValue("ContaCredito", true);

            PrepareFundsTransfer(State);

            InitializeJE initJe = new InitializeJE();
            initJe.RunComponent(State.OrchWrkData, new string[] { "JE" });

            string Montante = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2", true);
            if (Montante == null || double.Parse(Montante) == 0)
            {
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Montante Nulo", true);
                State.LastError = new BusinessException(string.Format("PS2.ProcessPS2Transaction:Montante Nulo"));
            }

            if (!State.IsInError)
            {
                GetParameters getParams = new GetParameters();
                getParams.RunComponent(State.OrchWrkData, new string[] { });
            }

            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaDebito", NumeroContaDebito, true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("NumeroContaCredito", NumeroContaCredito, true);
            if (!State.IsInError)
            {
                DoFlexCubeTransaction(State);
            }

            FinalizeJE finJE = new FinalizeJE();
            finJE.RunComponent(State.OrchWrkData, new string[] { "JE" });
        }
        private void PrepareFundsTransfer(OrchPipeComponent.ComponentState State)
        {
            // prepara Transferencia Bancária Ordenante
            State.OrchWrkData.GetWrkData().WriteNodeValue("IndStr", "0", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("TipoTerm", "A", true);
            //State.OrchWrkData.GetWrkData().WriteNodeValue("DtHora", GetDateHour(), true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("OnlineBatch", "B", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("Erro", "0", true);
            State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "OK", true);
        }
        private void DoFlexCubeTransaction(OrchPipeComponent.ComponentState State)
        {
            PrepareConstructor prepConst = new PrepareConstructor();
            prepConst.RunComponent(State.OrchWrkData, new string[] { });

            if (!State.IsInError)
            {
                MessageConstructor msgConst = new MessageConstructor();
                msgConst.RunComponent(State.OrchWrkData, new string[] { "#uv#FlexCubeConstructor", "FlexCubeRequest" });

            }
            if (!State.IsInError)
            {
                CallHost callHost = new CallHost();
                callHost.RunComponent(State.OrchWrkData, new string[] { });
            }

            if (!State.IsInError)
            {
                MessageParser msgParser = new MessageParser();
                msgParser.RunComponent(State.OrchWrkData, new string[] { "#uv#FlexCubeParser" });
            }
            if (!State.IsInError)
            {
                string ResponseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("fc-ResponseCode", true);
            }

        }
        private string GetDateHour()
        {
            DateTime dt = DateTime.Now;

            string DataHora = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            return DataHora;
        }

    }
}
