use [BesaSwitch]
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_1263'
select @MessageDirection=0
select * from Exp_MessagesConfigCache
where messageName=@trnId and MessageDirection=@MessageDirection

delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS cmsg="1263 - RESPOSTA CONSULTA NIB">
    <BL n="Header">
      <EL n="CodMsg">
        <Fv fi="0">
          <P v="#c#1263" />
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
        <Fv fi="2">
          <P v="#uv#ContaSAN1" />
          <P v="#c#15" />
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
      <BL n="0CN-consulta a NIB destinatário">
        <Fr fi="3">
          <P v="#uv#CodTrn" />
          <P v="#c#0CN" />
        </Fr>
        <BL n="Header">
          <EL n="CompDadosV">
            <Fv fi="0">
              <P v="#c#0046" />
            </Fv>
          </EL>
          <EL n="VerDadosV">
            <Fv fi="0">
              <P v="#c#00" />
            </Fv>
          </EL>
        </BL>
        <BL n="Dados Titular">
          <EL n="NomeTit">
            <Fv fi="2">
              <P v="#uv#NomeTitular" />
              <P v="#c#40" />
              <P v="#c# " />
            </Fv>
          </EL>
        </BL>
      </BL>
    </BL>
	
  </MS>

  <FNS>
    <f i="0" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCII, ExpandMsgFns" />
    <f i="1" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstAmountCurrency, SibsMsgsFns" />
    <f i="2" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCIIFixedLen, SibsMsgsFns" />
    <f i="3" ty="CRep" tn="MsgTransFunctions.FnConstRepConditional, MsgTransFunctions" />
  </FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId and MessageDirection=@MessageDirection
