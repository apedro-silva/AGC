using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;

namespace SoftFinanca.BESA
{
    public class TransferenciaOrdenante
    {
        public TransferenciaOrdenante(OrchPipeComponent.ComponentState State)
        {
            if (State.IsInError)
                return;


        }

    }
}
