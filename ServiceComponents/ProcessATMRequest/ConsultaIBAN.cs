using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;

namespace SoftFinanca.BESA
{
    public class ConsultaIBAN
    {
        public ConsultaIBAN(OrchPipeComponent.ComponentState State)
        {
            if (State.IsInError)
                return;
            try
            {
                string nib = State.OrchWrkData.GetWrkData().ReadNodeValue("NIBDestinatario", true);
                string Conta = nib.Substring(8, 11);
                State.OrchWrkData.GetWrkData().WriteNodeValue("Conta", Conta, true);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

    }
}
