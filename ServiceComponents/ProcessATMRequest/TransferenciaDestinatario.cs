using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;

namespace SoftFinanca.BESA
{
    public class TransferenciaDestinatario
    {
        public TransferenciaDestinatario(OrchPipeComponent.ComponentState State)
        {
            if (State.IsInError)
                return;
            try
            {
                string nib = State.OrchWrkData.GetWrkData().ReadNodeValue("NIBDestinatario", true);
                string conta = State.OrchWrkData.GetWrkData().ReadNodeValue("Conta", true);

                if (nib != null && conta==null)
                {
                    conta = nib.Substring(8, 11);
                    State.OrchWrkData.GetWrkData().WriteNodeValue("Conta", conta, true);
                }
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

    }
}
