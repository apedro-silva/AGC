use [BesaSwitch]
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_3136'
select @MessageDirection=1
delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS pmsg="3136 - PEDIDO INFORMAÇÃO SITUAÇÃO DE CARTÃO PENDENTE (ACQUIRER)">
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

    <BL n="Dados Cartao">
      <EL n="CompPAN">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CompPAN" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="PAN">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumeroCartao" />
          <P v="#c#19" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="DataExp">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#DataExp" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Dados lista negra papel">
      <EL n="TipoCart">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoCart" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="LcNac">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#LcNac" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="DtLcNac">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#DtLcNac" />
          <P v="#c#8" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="Visca">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Visca" />
          <P v="#c#2" />
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

    <BL n="Alteracao Situacao Cartao Acquirer-172">
      <Fr fi="3">
        <P v="#c#172" />
        <P v="#uv#CodTrn" />
        <P v="#uv#CompDadosV" />
      </Fr>
      <EL n="VerDadosV">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#VerDadosV" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NSitCar">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NSitCar" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ModIns">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ModIns" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

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
