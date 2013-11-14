use besaswitch
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_1251'
select @MessageDirection=0
delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--Constructor
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS cmsg="1200-Resposta de Consulta de Saldos e Movimentos">
    <BL n="Message">
      <EL n="CodMsg">
        <Fv fi="0">
          <P v="#c#1251" />
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
		<Fv fi="6">
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
		<Fv fi="6">
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

	<BL n="Movimentos">
		<Fr fi="1">
			<P v="#uxc#Movements://r/Movement" />
		</Fr>
		<Fv fi="2">
			<P v="#uxc#Movements://r/Movement" />
			<P v="#c#2" />
		</Fv>
		
		<BL n="bloco">
			<EL n="RefMov">
				<Fv fi="3">
					<P v="#uxt#Movements://r/Movement[#index#]/TransactionLiteral" />
					<P v="#c#3" />
				</Fv>
			</EL>
		
			<EL n="DescMov">
				<Fv fi="4">
					<P v="#uxt#Movements://r/Movement[#index#]/TransactionLiteral" />
					<P v="#uxt#Movements://r/Movement[#index#]/DebitCreditFlag" />
					<P v="#uxt#Movements://r/Movement[#index#]/TxnAmountSign" />
					<P v="#c#15" />
				</Fv>
			</EL>
		
			<EL n="DataMov">
				<Fv fi="3">
					<P v="#uxt#Movements://r/Movement[#index#]/ValueDate" />
					<P v="#c#8" />
				</Fv>
			</EL>
		
			<EL n="ValorMov">
				<Fv fi="5">
					<P v="#uxt#Movements://r/Movement[#index#]/TxnAmount" />
					<P v="#c#13" />
					<P v="#uv#NetAvailableBalanceCurrency" />
					<P v="#uv#CodigoMoeda" />
				</Fv>
			</EL>
		
			<EL n="Sinal">
				<Fv fi="7">
					<P v="#uxt#Movements://r/Movement[#index#]/DebitCreditFlag" />
					<P v="#uxt#Movements://r/Movement[#index#]/TxnAmountSign" />
				</Fv>
			</EL>
			
			<EL n="CodMoeda">
				<Fv fi="0">
					<P v="#uv#CodMoeda" />
				</Fv>
			</EL>
		
		
		</BL>						
	</BL>						
</BL>	
</MS>
	
<FNS>
	<f i="0" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCII, ExpandMsgFns" />
	<f i="1" ty="CRep" tn="MsgTransFunctions.FnConstNRep, MsgTransFunctions" />
	<f i="2" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCIIFixedLen, SibsMsgsFns" />
	<f i="3" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCIIFixedLenByIndex, SibsMsgsFns" />
	<f i="4" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValDescMovByIndex, SibsMsgsFns" />
	<f i="5" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstAmountCurrencyByIndex, SibsMsgsFns" />
	<f i="6" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstAmountCurrency, SibsMsgsFns" />
	<f i="7" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCIIDebitCreditByIndex, SibsMsgsFns" />
</FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId
and MessageDirection=@MessageDirection