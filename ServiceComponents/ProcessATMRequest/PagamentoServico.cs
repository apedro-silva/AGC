using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;

namespace SoftFinanca.BESA
{
    public class PagamentoServico
    {
        public PagamentoServico(OrchPipeComponent.ComponentState State)
        {
            if (State.IsInError)
                return;
            try
            {
                string TipoTerminal = State.OrchWrkData.GetWrkData().ReadNodeValue("TipoTerm", true);

                // Rejects if not comming from an ATM. Flexcube does not support Funds Transfer on POS
                if (!TipoTerminal.Equals("A"))
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("fc-ResponseCode", "40", true);
                    throw new Exception("Pagamento Serviços não suportado em POS!");
                }
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

    }
}
