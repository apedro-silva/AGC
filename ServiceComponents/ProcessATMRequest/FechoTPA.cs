using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;

namespace SoftFinanca.BESA
{
    public class FechoTPA
    {
        public FechoTPA(OrchPipeComponent.ComponentState State)
        {
            if (State.IsInError)
                return;
            try
            {
                string montante2 = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2", true);
                decimal decMontante = decimal.Parse(montante2);
                string sinalMontante = State.OrchWrkData.GetWrkData().ReadNodeValue("SinalMontante", true);

                string MontanteAdicional = State.OrchWrkData.GetWrkData().ReadNodeValue("MontanteAdicional", true);
                decimal decMontanteAdicional = Convert.ToDecimal(MontanteAdicional);
                string sinalMontanteAdicional = State.OrchWrkData.GetWrkData().ReadNodeValue("SinalMontanteAdicional", true);

                if (decMontante > decMontanteAdicional)
                    State.OrchWrkData.GetWrkData().WriteNodeValue("SinalMontante", sinalMontante, true);
                else
                    State.OrchWrkData.GetWrkData().WriteNodeValue("SinalMontante", sinalMontanteAdicional, true);

                if (sinalMontante == "D")
                    decMontante *= -1;
                if (sinalMontanteAdicional == "C")
                    decMontanteAdicional *= -1;

                decimal decMontanteLiquido = Convert.ToDecimal(decMontante)-Convert.ToDecimal(decMontanteAdicional);

                if (decMontanteLiquido < 0)
                    decMontanteLiquido *= -1;

                State.OrchWrkData.GetWrkData().WriteNodeValue("Montante2", decMontanteLiquido.ToString().PadLeft(13, '0'), true);

                if (decMontanteLiquido == 0)
                {
                    State.OrchWrkData.GetWrkData().WriteNodeValue("MessageType", "");
                    State.OrchWrkData.GetWrkData().WriteNodeValue("FlexCubeConstructor", "");
                }
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

    }
}
