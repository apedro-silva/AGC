using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.PipelineDecision
{
    /// <summary>
    /// The CheckCard Switch component performs the validation of the card number associated with the current transaction.
    /// </summary>
    public class CheckCard : OrchPipeDecision
    {
        /// <summary>
        /// Runs the decision component.
        /// </summary>
        /// <param name="OrchWrkData">The OrchWrkData.</param>
        /// <param name="Params">The Params.</param>
        /// <returns>
        /// 	<c>true</c> if [is card valid]; otherwise, <c>false</c>.
        /// </returns>
        public override bool RunDecision(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeDecision.DecisionState State = new DecisionState(OrchWrkData, Params);
            if (State.IsInError)
                return false;

            if ((State.OrchWrkData.GetWrkData().ReadNodeValue("NumCar", true) == null) &&
                (State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroCartao", true) == null))
                return true;

            string CodTrn = State.OrchWrkData.GetWrkData().ReadNodeValue("CodTrn", true);
                
            // verify card in database
            bool CardValid = IsCardValid(State);
            string EstadoCartao = State.OrchWrkData.GetWrkData().ReadNodeValue("EstadoCartao", true);

            if (((EstadoCartao != "1") && ((CodTrn == "71") || (CodTrn == "071"))) || CardValid)
                return true;

            State.LastError = new BusinessException("Cartão Inválido ou Data Validade Expirada!");
            return false;
        }
        /// <summary>
        /// Determines whether is card valid by calling SP ObtemEstadoCartao.
        /// </summary>
        /// <param name="State">The state.</param>
        /// <returns>
        /// 	<c>true</c> if [is card valid]; otherwise, <c>false</c>.
        /// </returns>
        private Boolean IsCardValid(OrchPipeDecision.DecisionState State)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("BESASwitch");
                DbCommand dbCommand = db.GetStoredProcCommand("ObtemEstadoCartao");
				string NumeroCartao=null;

                if (State.OrchWrkData.GetWrkData().ReadNodeValue("NumCar", true) != null)
                    NumeroCartao = State.OrchWrkData.GetWrkData().ReadNodeValue("BIN",true) + State.OrchWrkData.GetWrkData().ReadNodeValue("ExBin",true) + State.OrchWrkData.GetWrkData().ReadNodeValue("NumCar",true);
                else if (State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroCartao", true) != null)
                    NumeroCartao = State.OrchWrkData.GetWrkData().ReadNodeValue("NumeroCartao", true);

                string NumeroConta = State.OrchWrkData.GetWrkData().ReadNodeValue("Conta", true);
                db.AddInParameter(dbCommand, "NumeroCartao", DbType.String, NumeroCartao.Trim());
                db.AddInParameter(dbCommand, "NumeroConta", DbType.String, NumeroConta.Trim());
                db.AddOutParameter(dbCommand, "EstadoCartao", DbType.Int32, 2);
                db.ExecuteNonQuery(dbCommand);

                int EstadoCartao = (int) db.GetParameterValue(dbCommand, "EstadoCartao");
                State.OrchWrkData.GetWrkData().WriteNodeValue("EstadoCartao", EstadoCartao.ToString(), true);

                return (EstadoCartao == 0);

                //if (EstadoCartao == 0)
                //    return true;
                //else
                //{
                //    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "4", true);
                //    State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", "Cartão Inválido ou Data Validade Expirada!", true);
                //    return false;
                //}
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException("CheckCard.IsCardValid", exp);
                return false; 
            }
        }

    }
}
