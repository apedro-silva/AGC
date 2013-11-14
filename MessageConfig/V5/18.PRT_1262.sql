use [BesaSwitch]
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_1262'
select @MessageDirection=0
select * from Exp_MessagesConfigCache
where messageName=@trnId and MessageDirection=@MessageDirection

delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS cmsg="1262 - RESPOSTA A CONSULTA SALDOS E MOVIMENTOS(EMISSOR)">
    <BL n="Header">
      <EL n="CodMsg">
        <Fv fi="0">
          <P v="#c#1262" />
        </Fv>
      </EL>
      <EL n="VerMsg">
        <Fv fi="0">
          <P v="#uv#VerMsg" />
        </Fv>
      </EL>
      <EL n="AplicPdd">
        <Fv fi="0">
          <P v="#uv#AplicPdd" />
        </Fv>
      </EL>
      <EL n="IdLog">
        <Fv fi="0">
          <P v="#uv#IdLog" />
        </Fv>
      </EL>
      <EL n="NrLog">
        <Fv fi="0">
          <P v="#uv#NrLog" />
        </Fv>
      </EL>
    </BL>
    <BL n="Dados da resposta">
      <EL n="CodResp">
        <Fv fi="0">
          <P v="#uv#CodResp" />
        </Fv>
      </EL>
      <EL n="NrIdResp">
        <Fv fi="0">
          <P v="#uv#NrIdResp" />
        </Fv>
      </EL>
    </BL>
    <BL n="Dados adicionais resposta">
      <EL n="CodRespAd">
        <Fv fi="0">
          <P v="#uv#CodRespAd" />
        </Fv>
      </EL>
    </BL>

    <BL n="Dados saldos conta">
      <EL n="Conta-SAN1">
        <Fv fi="0">
          <P v="#uv#ContaSAN1" />
        </Fv>
      </EL>
      <EL n="CodMoeda">
        <Fv fi="0">
          <P v="#uv#CodMoeda" />
        </Fv>
      </EL>
      <EL n="SaldoDisponivelE">
        <Fv fi="2">
          <P v="#uv#SaldoDisp" />
          <P v="#c#11" />
        </Fv>
      </EL>
      <EL n="SinalSaldoDisp">
        <Fv fi="2">
          <P v="#uv#SinalSaldoDisp" />
          <P v="#c#1" />
          <P v="#c#C" />
        </Fv>
      </EL>

      <EL n="SaldoContabilisticoE">
        <Fv fi="2">
          <P v="#uv#SaldoCont" />
          <P v="#c#11" />
        </Fv>
      </EL>
      <EL n="SinalSaldoCont">
        <Fv fi="2">
          <P v="#uv#SinalSaldoCont" />
          <P v="#c#1" />
          <P v="#c#C" />
        </Fv>
      </EL>

      <EL n="DataSaldo">
        <Fv fi="2">
          <P v="#uv#DataSaldo" />
          <P v="#c#8" />
        </Fv>
      </EL>
    </BL>

    <BL n="Dados SIDF Credito">
      <EL n="IndForDad">
        <Fv fi="0">
          <P v="#c#0" />
        </Fv>
      </EL>
      <EL n="TipPagExt">
        <Fv fi="0">
          <P v="#c#0" />
        </Fv>
      </EL>
      <EL n="LimCred">
        <Fv fi="0">
          <P v="#c#0000000" />
        </Fv>
      </EL>
      <EL n="ValCat">
        <Fv fi="0">
          <P v="#c#000000000" />
        </Fv>
      </EL>
      <EL n="Sinal">
        <Fv fi="0">
          <P v="#c#C" />
        </Fv>
      </EL>
      <EL n="CurrBalance">
        <Fv fi="0">
          <P v="#c#000000000" />
        </Fv>
      </EL>
      <EL n="SaldoCash">
        <Fv fi="0">
          <P v="#c#000000000" />
        </Fv>
      </EL>
      <EL n="Sinal">
        <Fv fi="0">
          <P v="#c#C" />
        </Fv>
      </EL>
      <EL n="SaldoComp">
        <Fv fi="0">
          <P v="#c#000000000" />
        </Fv>
      </EL>
      <EL n="Sinal">
        <Fv fi="0">
          <P v="#c#C" />
        </Fv>
      </EL>
    </BL>

    <BL n="Dados variaveis">
      <BL n="003-Consulta Saldos">
        <Fr fi="3">
          <P v="#uv#CodTrn" />
          <P v="#c#003" />
        </Fr>
        <BL n="Header">
          <EL n="CompDadosV">
            <Fv fi="0">
              <P v="#c#0089" />
            </Fv>
          </EL>
          <EL n="VerDadosV">
            <Fv fi="0">
              <P v="#c#00" />
            </Fv>
          </EL>
        </BL>
        <BL n="Dados Saldo Conta">
          <EL n="ContaSAN2">
            <Fv fi="2">
              <P v="#uv#ContaSAN2" />
              <P v="#c#15" />
              <P v="#c#0" />
            </Fv>
          </EL>
          <EL n="CodMoedaSAN2">
            <Fv fi="2">
              <P v="#uv#CodMoedaSAN2" />
              <P v="#c#3" />
              <P v="#c#0" />
            </Fv>
          </EL>
          
          <EL n="SaldoDisponivelSAN2">
            <Fv fi="2">
              <P v="#uv#SaldoDispSAN2" />
              <P v="#c#11" />
              <P v="#c#0" />
            </Fv>
          </EL>
          <EL n="SinalSaldoDispSAN2">
            <Fv fi="2">
              <P v="#uv#SinalSaldoDispSAN2" />
              <P v="#c#1" />
              <P v="#c#C" />
            </Fv>
          </EL>

          <EL n="SaldoContSAN2">
            <Fv fi="2">
              <P v="#uv#SaldoContSAN2" />
              <P v="#c#11" />
              <P v="#c#0" />
            </Fv>
          </EL>
          <EL n="SinalSaldoContSAN2">
            <Fv fi="2">
              <P v="#uv#SinalSaldoContSAN2" />
              <P v="#c#1" />
              <P v="#c#C" />
            </Fv>
          </EL>
          <EL n="DataSaldoSAN2">
            <Fv fi="2">
              <P v="#uv#DataSaldoSAN2" />
              <P v="#c#8" />
              <P v="#c#0" />
            </Fv>
          </EL>
        </BL>
        <BL n="Dados Saldo Conta Credito">
          <EL n="IndSaldos">
            <Fv fi="0">
              <P v="#c#100" />
            </Fv>
          </EL>
          <EL n="NrContaCR">
            <Fv fi="2">
              <P v="#uv#NrContaCR" />
              <P v="#c#15" />
              <P v="#c#0" />
            </Fv>
          </EL>
          <EL n="SaldoDispConta">
            <Fv fi="2">
              <P v="#uv#SaldoDispConta" />
              <P v="#c#11" />
              <P v="#c#0" />
            </Fv>
          </EL>
          <EL n="SinalSaldoDispConta">
            <Fv fi="2">
              <P v="#uv#SinalSaldoDispConta" />
              <P v="#c#1" />
              <P v="#c#C" />
            </Fv>
          </EL>
          <EL n="LimCr">
            <Fv fi="2">
              <P v="#uv#LimCr" />
              <P v="#c#7" />
              <P v="#c#0" />
            </Fv>
          </EL>
        </BL>
      </BL>
      <BL n="004-Consulta Movimentos">
        <Fr fi="3">
          <P v="#uv#CodTrn" />
          <P v="#c#004" />
        </Fr>
        <BL n="Header">
          <Ft fi="8">
            <P v="#cfx#.//BL|.//EL" />
          </Ft>
          <Fv fi="2">
            <P v="#wa#" />
            <P v="#c#4" />
            <P v="#c#0" />
          </Fv>

          <EL n="VerDadosV">
            <Fv fi="0">
              <P v="#c#00" />
            </Fv>
          </EL>

        <BL n="Movimentos">
          <Fr fi="4">
            <P v="#uxc#Movements://r/Movement" />
          </Fr>
          <Fv fi="2">
            <P v="#uxc#Movements://r/Movement" />
            <P v="#c#2" />
          </Fv>

          <BL n="bloco">
            <EL n="RefMov">
              <Fv fi="5">
                <P v="#uxt#Movements://r/Movement[#index#]/TransactionLiteral" />
                <P v="#c#3" />
              </Fv>
            </EL>

            <EL n="DescMov">
              <Fv fi="6">
                <P v="#uxt#Movements://r/Movement[#index#]/TransactionLiteral" />
                <P v="#uxt#Movements://r/Movement[#index#]/DebitCreditFlag" />
                <P v="#uxt#Movements://r/Movement[#index#]/TxnAmountSign" />
                <P v="#c#15" />
              </Fv>
            </EL>

            <EL n="DataMov">
              <Fv fi="5">
                <P v="#uxt#Movements://r/Movement[#index#]/ValueDate" />
                <P v="#c#8" />
              </Fv>
            </EL>

            <EL n="ValorMov">
              <Fv fi="7">
                <P v="#uxt#Movements://r/Movement[#index#]/TxnAmount" />
                <P v="#c#13" />
                <P v="#uv#NetAvailableBalanceCurrency" />
                <P v="#uv#CodigoMoeda" />
              </Fv>
            </EL>

            <EL n="Sinal">
              <Fv fi="9">
                <P v="#uxt#Movements://r/Movement[#index#]/DebitCreditFlag" />
                <P v="#uxt#Movements://r/Movement[#index#]/TxnAmountSign" />
              </Fv>
            </EL>
          </BL>
        </BL>
        </BL>

      </BL>
    </BL>

  </MS>

  <FNS>
    <f i="0" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCII, ExpandMsgFns" />
    <f i="1" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstAmountCurrency, SibsMsgsFns" />
    <f i="2" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCIIFixedLen, SibsMsgsFns" />
    <f i="3" ty="CRep" tn="MsgTransFunctions.FnConstRepConditional, MsgTransFunctions" />
    <f i="4" ty="CRep" tn="MsgTransFunctions.FnConstNRep, MsgTransFunctions" />
    <f i="5" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCIIFixedLenByIndex, SibsMsgsFns" />
    <f i="6" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValDescMovByIndex, SibsMsgsFns" />
    <f i="7" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstAmountCurrencyByIndex, SibsMsgsFns" />
    <f i="8" ty="CTrf" tn="SF.Expand.Core.Connectivity.FnConstTrfBuffSizeMultiple, ExpandMsgFns" />
    <f i="9" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCIIDebitCreditByIndex, SibsMsgsFns" />
  </FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId and MessageDirection=@MessageDirection
