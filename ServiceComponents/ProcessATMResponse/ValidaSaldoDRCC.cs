using System;
using System.Collections.Generic;
using System.Text;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.Xml;
using System.Globalization;

namespace SoftFinanca.BESA
{
    public class ValidaSaldoDRCC
    {
        public ValidaSaldoDRCC(OrchPipeComponent.ComponentState State)
        {
            if (State.IsInError)
                return;
            ValidateAccountBalance(State);
        }
        private void ValidateAccountBalance(OrchPipeComponent.ComponentState State)
        {
            try
            {
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

                XmlNode myCurrencyNode = null;
                decimal finalValue=0;
                string Montante = State.OrchWrkData.GetWrkData().ReadNodeValue("Montante2", true);
                string SaldoDisponivel = State.OrchWrkData.GetWrkData().ReadNodeValue("NetAvailableBalance", true);
                string MoedaSaldoDisponivel = State.OrchWrkData.GetWrkData().ReadNodeValue("NetAvailableBalanceCurrency", true);
                string MoedaDRCC = State.OrchWrkData.GetWrkData().ReadNodeValue("CodigoMoedaDRCC", true);

                decimal debitValue = Convert.ToDecimal(Montante.Insert(Montante.Length - 2, "."),nfi);

                if (MoedaSaldoDisponivel == MoedaDRCC)
                {
                    finalValue = Convert.ToDecimal(SaldoDisponivel.Insert(SaldoDisponivel.Length - 2, "."),nfi);
                }
                else
                {
                    string exchangeXml = State.OrchWrkData.GetWrkData().ReadNodeValue("Cambios", true);
                    if (exchangeXml == "")
                        throw new BusinessException("ProcessATMResponse.ValidaSaldoDRCC::Exchange table has no values!");

                    XmlDocument exchangeDom = new XmlDocument();
                    try
                    {
                        exchangeDom.LoadXml(exchangeXml);
                    }
                    catch (Exception exp)
                    {
                        throw new BusinessException("ProcessATMResponse.ValidaSaldoDRCC::Error parser currency xml! " + exp.Message);
                    }
                    string ratePath = "//Table1[CCYCode1='" + MoedaSaldoDisponivel + "' and CCYCode2='" + MoedaDRCC + "']";
                    myCurrencyNode = exchangeDom.SelectSingleNode(ratePath);

                    // If we get a node than we have conversion
                    if (myCurrencyNode != null)
                    {
                        int decOriginal = Convert.ToInt16(myCurrencyNode.SelectSingleNode("NoOfDecimal1").InnerText);

                        decimal accountBalance = Convert.ToDecimal(SaldoDisponivel.Insert(SaldoDisponivel.Length - decOriginal, "."),nfi);
                        decimal rateValue = Convert.ToDecimal(myCurrencyNode.SelectSingleNode("Rate").InnerText,nfi);
                        finalValue = accountBalance * rateValue;
                    }
                    else //if (myCurrencyNode == null)
                    {
                        ratePath = "//Table1[CCYCode1='" + MoedaDRCC + "' and CCYCode2='" + MoedaSaldoDisponivel + "']";
                        myCurrencyNode = exchangeDom.SelectSingleNode(ratePath);
                        if (myCurrencyNode != null)
                        {
                            int decOriginal = Convert.ToInt16(myCurrencyNode.SelectSingleNode("NoOfDecimal2").InnerText);

                            decimal accountBalance = Convert.ToDecimal(SaldoDisponivel.Insert(SaldoDisponivel.Length - decOriginal, "."),nfi);
                            decimal rateValue = Convert.ToDecimal(myCurrencyNode.SelectSingleNode("Rate").InnerText,nfi);
                            finalValue = accountBalance / rateValue;
                        }
                    }
                    if (myCurrencyNode == null)
                    {
                        State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "S4", true);
                        return;
                    }
                }

                if (finalValue < debitValue)
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "S1", true);
                else
                    State.OrchWrkData.GetWrkData().WriteNodeValue("CodResp", "S3", true);

            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
            }
        }

    }
}
