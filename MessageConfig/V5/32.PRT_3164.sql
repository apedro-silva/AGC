use [BesaSwitch]
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_3164'
select @MessageDirection=1
delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS pmsg="3164 - PEDIDO DE OPERAÇÃO COMERCIANTE (FECHO TPA) PENDENTE">
	<BL n="MASTER">
		<EL n="CodMsg">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#CodMsg" />
			<P v="#c#4" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="VerMsg">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#VerMsg" />
			<P v="#c#2" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="AplicPdd">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#AplicPdd" />
			<P v="#c#2" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="IdLog">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#IdLog" />
			<P v="#c#4" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="NrLog">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#NrLog" />
			<P v="#c#8" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="DHMsg">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#DHMsg" />
			<P v="#c#14" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="CodTrn">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#CodTrn" />
			<P v="#c#3" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="CodMoeda">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#CodMoeda" />
			<P v="#c#3" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="TipoTerm">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#TipoTerm" />
			<P v="#c#1" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="BanApoio">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#BanApoio" />
			<P v="#c#4" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="IdTerminal">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#IdTerminal" />
			<P v="#c#10" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="CapTerminal">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#CapTerminal" />
			<P v="#c#1" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="DtHora">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#DtHora" />
			<P v="#c#14" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="LocTerm">
		  <Fv fi="1">
			<P v="#c#1252" />
			<P v="#uv#LocTerm" />
			<P v="#c#40" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="MCC">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#MCC" />
			<P v="#c#4" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="CodPais">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#CodPais" />
			<P v="#c#3" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="IdPropr">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#IdPropr" />
			<P v="#c#7" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="IdPerLocal">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#IdPerLocal" />
			<P v="#c#3" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="NumRegLoc">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#NumRegLoc" />
			<P v="#c#5" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="DistConc">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#DistConc" />
			<P v="#c#4" />
			<P v="#c#1" />
		  </Fv>
		</EL>

    <EL n="Conta">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#Conta" />
			<P v="#c#15" />
			<P v="#c#1" />
		  </Fv>
		</EL>
    
    <BL n="Dados comerciante">
      <EL n="IdComEmp">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IdComEmp" />
          <P v="#c#7" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="IdEst">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IdEst" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Montantes Operacao">
      <EL n="Montante">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Montante2" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="SinalMontante">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#SinalMontante" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="MontanteAdicional">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#MontanteAdicional" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="SinalMontanteAdicional">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#SinalMontanteAdicional" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Dados operacao comerciante">
      <EL n="TotOperE">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TotOperE" />
          <P v="#c#7" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

  </BL>

    <EL n="CompDadosV">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#CompDadosV" />
        <P v="#c#4" />
        <P v="#c#1" />
      </Fv>
    </EL>
  </MS>

  <FNS>
    <f i="0" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValGenDecoding, ExpandMsgFns" />
    <f i="1" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValGenDecodingCP, ExpandMsgFns" />
    <f i="2" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValGetRemainingSize, ExpandMsgFns" />
    <f i="3" ty="PRep" tn="SF.Expand.Core.Connectivity.FnParseNRepBlock, ExpandMsgFns" />
  </FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId and MessageDirection=@MessageDirection
