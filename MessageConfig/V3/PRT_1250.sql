use besaswitch
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_1250'
select @MessageDirection=0
delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--Constructor
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS cmsg="1250-Resposta de Consulta de Saldos">
    <BL n="Message">
      <EL n="CodMsg">
        <Fv fi="0">
          <P v="#c#1250" />
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
      <EL n="CodRespAd">
        <Fv fi="0">
          <P v="#c#000" />
        </Fv>
      </EL>
      <EL n="Conta-SAN1">
        <Fv fi="0">
          <P v="#uv#ContaSAN1" />
        </Fv>
      </EL>
      <EL n="CodMoeda-SAN1">
        <Fv fi="0">
          <P v="#uv#CodMoeda" />
        </Fv>
      </EL>

      <EL n="SaldoDisp-SAN1">
		<Fv fi="1">
			<P v="#uv#SaldoDisp" />
			<P v="#c#13" />
			<P v="#uv#NetAvailableBalanceCurrency" />
			<P v="#uv#CodigoMoeda" />
		</Fv>
      </EL>
      
      <EL n="Sinal">
        <Fv fi="2">
          <P v="#uv#SinalSaldoDisp" />
          <P v="#c#1" />		
        </Fv>
      </EL>
      <EL n="SaldoCont-SAN1">
		<Fv fi="1">
			<P v="#uv#SaldoCont" />
			<P v="#c#13" />
			<P v="#uv#LedgerBalanceCurrency" />
			<P v="#uv#CodigoMoeda" />
		</Fv>
      </EL>
      <EL n="Sinal">
        <Fv fi="2">
          <P v="#uv#SinalSaldoCont" />
          <P v="#c#1" />		
        </Fv>
      </EL>
      <EL n="DataSaldo-SAN1">
        <Fv fi="2">
          <P v="#uv#DataSaldo" />
          <P v="#c#8" />		
        </Fv>
      </EL>

      <EL n="Conta-SAN2">
        <Fv fi="0">
          <P v="#uv#ContaSAN2" />
        </Fv>
      </EL>
      <EL n="CodMoeda-SAN2">
        <Fv fi="0">
          <P v="#uv#CodMoeda" />
        </Fv>
      </EL>
      <EL n="SaldoDisp-SAN2">
        <Fv fi="0">
          <P v="#c#0000000000000" />
        </Fv>
      </EL>
      <EL n="Sinal">
        <Fv fi="0">
          <P v="#c#0" />
        </Fv>
      </EL>
      <EL n="SaldoCont-SAN2">
        <Fv fi="0">
          <P v="#c#0000000000000" />
        </Fv>
      </EL>
      <EL n="Sinal">
        <Fv fi="0">
          <P v="#c#0" />
        </Fv>
      </EL>
      <EL n="DataSaldo-SAN2">
        <Fv fi="0">
          <P v="#c#00000000" />
        </Fv>
      </EL>
    </BL>
  </MS>

  <FNS>
    <f i="0" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCII, ExpandMsgFns" />
    <f i="1" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstAmountCurrency, SibsMsgsFns" />    
    <f i="2" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCIIFixedLen, SibsMsgsFns" />        
  </FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId
and MessageDirection=@MessageDirection