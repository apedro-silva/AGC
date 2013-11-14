use [BesaSwitch]
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_3147'
select @MessageDirection=1
select * from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS pmsg="3147 - PEDIDO DE OPERAÇÃO COM CARTÃO (REPRESENTANTE)">
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
		<EL n="SeqMov">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#SeqMov" />
			<P v="#c#2" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="SeqPAN">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#SeqPAN" />
			<P v="#c#1" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="EnvPadrao">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#EnvPadrao" />
			<P v="#c#3" />
			<P v="#c#1" />
		  </Fv>
		</EL>
		<EL n="TipoAut">
		  <Fv fi="0">
			<P v="#c#ascii" />
			<P v="#uv#TipoAut" />
			<P v="#c#2" />
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

    <EL n="CompDadosV">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#CompDadosV" />
        <P v="#c#4" />
        <P v="#c#1" />
      </Fv>
    </EL>


    <BL n="Autorizacao Estrangeiro-012">
      <Fr fi="3">
        <P v="#c#012" />
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
      <EL n="SistPag">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#SistPag" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CodMoeda2">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CodMoeda2" />
          <P v="#c#3" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="Cambio">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Cambio" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ImportOp">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ImportOp" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ValTotal">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ValTotal" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="IdAceitante">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IdAceitante" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="RetRef">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#RetRef" />
          <P v="#c#12" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NatOper">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NatOper" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="PanTbMbNet">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#PanTpMbNet" />
          <P v="#c#16" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NumAut">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumAut" />
          <P v="#c#6" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Devolução Compras Outras-016,026,027,066,095">
      <Fr fi="3">
        <P v="#c#016,026,027,066,095" />
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
      <EL n="IdComRep">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IdComRep" />
          <P v="#c#7" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ProdFin">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ProdFin" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NpCrep">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NpCrep" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="MntOri">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#MntOri" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ModPag">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ModPag" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NrIdResp-O">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NrIdResp-O" />
          <P v="#c#14" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="AplicN-O">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#AplicN-O" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="IdLog-O">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IdLog-O" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NrLog-O">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NrLog-O" />
          <P v="#c#8" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NatOper">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NatOper" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="PanTpMbNet">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#PanTpMbNet" />
          <P v="#c#16" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Cancelamentos-019">
      <Fr fi="3">
        <P v="#c#019" />
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
      <EL n="IdComRep">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IdComRep" />
          <P v="#c#7" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ProdFin">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ProdFin" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NatOper">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NatOper" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NpCrep">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NpCrep" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NumAut-O">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumAut-O" />
          <P v="#c#6" />
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
