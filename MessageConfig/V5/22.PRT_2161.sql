use [BESASwitch]
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_2161'
select @MessageDirection=1
delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS pmsg="2161 - PEDIDO DE ANULACAO DE OPERACAO COM CARTAO (EMISSOR)">
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
 	<BL n="Levantamento-001">
		<Fr fi="3">
			<P v="#c#001" />
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
  </BL>

	<BL n="Consulta Saldos-003">
		<Fr fi="3">
			<P v="#c#003" />
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
    <EL n="ContaSan2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ContaSan2" />
        <P v="#c#15" />
        <P v="#c#1" />
      </Fv>
    </EL>
  </BL>

    <BL n="Alteracao PIN-005">
      <Fr fi="3">
        <P v="#c#005" />
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
      <EL n="PVV2">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#PVV2" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="PVV3">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#PVV3" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="PVV2Anterior">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#PVV2Anterior" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="PVV3Anterior">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#PVV3Anterior" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>


    <BL n="Aviso Deposito Numerario/Valores-006,007">
      <Fr fi="3">
        <P v="#c#006,007" />
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
      <EL n="RefDep">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#RefDep" />
          <P v="#c#3" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Transferencia entre contas do cartao-008">
      <Fr fi="3">
        <P v="#c#008" />
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
      <EL n="ContaDestino">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ContaDestino" />
          <P v="#c#15" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Pagamento Servicos Compra MB-009,0P0,0P7">
      <Fr fi="3">
        <P v="#c#009,0P0,0P7" />
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
      <EL n="Entidade">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Entidade" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="RefPag">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#RefPag" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="TipoSer">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoSer" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="BanApoio2">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#BanApoio2" />
          <P v="#c#4" />
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
    </BL>

    <BL n="Compra MB-010">
      <Fr fi="3">
        <P v="#c#010" />
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
      <EL n="IndTipPag">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IndTipPag" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CrePrestCli">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CrePrestCli" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidelizacaoId">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidelizacaoId" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidPontosReb">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidPonstosReb" />
          <P v="#c#7" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidPontosDis">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidPontosDis" />
          <P v="#c#7" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidValorPonto">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidValorPonto" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidValorRem">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidValorRem" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

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

    <BL n="Compras Outras Vertentes-015">
      <Fr fi="3">
        <P v="#c#015" />
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
      <EL n="IndTipPag">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IndTipPag" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CrePrestCli">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CrePrestCli" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidelizacaoId">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidelizacaoId" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidPontosReb">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidPontosReb" />
          <P v="#c#7" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidPontosDis">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidPontosDis" />
          <P v="#c#7" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidValorPonto">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidValorPonto" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="FidValorRem">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#FidValorRem" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Devolucao Compras Outras-016,026,027,066,076,094,095">
      <Fr fi="3">
        <P v="#c#016,026,027,066,076,094,095" />
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

    <BL n="Autorizacoes-017,018,039,067,068,089">
      <Fr fi="3">
        <P v="#c#017,018,039,067,068,089" />
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
    </BL>

    <BL n="Cancelamentos-019,069">
      <Fr fi="3">
        <P v="#c#019,069" />
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

    <BL n="Servico Especial nao bancario-023">
      <Fr fi="3">
        <P v="#c#023" />
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
      <EL n="Entidade">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Entidade" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="DescSe">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#DescSe" />
          <P v="#c#12" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="BanApoio2">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#BanApoio2" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Deposito bancario nao confirmado-036">
      <Fr fi="3">
        <P v="#c#036" />
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
      <EL n="RefDep">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#RefDep" />
          <P v="#c#3" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="MontInd">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#MontInd" />
          <P v="#c#13" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Transferencia bancaria ordenante-037">
      <Fr fi="3">
        <P v="#c#037" />
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
      <EL n="NibDest">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NibDest" />
          <P v="#c#21" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CredDest">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CredDest" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Pagamento Letra Recibo-038">
      <Fr fi="3">
        <P v="#c#038" />
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
      <EL n="NumEfeito">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumEfeito" />
          <P v="#c#21" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Transferencia bancaria destinatario-052">
      <Fr fi="3">
        <P v="#c#052" />
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
      <EL n="NibOrd">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NibOrd" />
          <P v="#c#21" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NomeCli">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NomeCli" />
          <P v="#c#27" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Compra Credito-065">
      <Fr fi="3">
        <P v="#c#065" />
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
      <EL n="ModPag">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ModPag" />
          <P v="#c#3" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Comunicacao Captura Cartao-071">
      <Fr fi="3">
        <P v="#c#071" />
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
      <EL n="MotCapt">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#MotCapt" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>


    <BL n="Alteracao Situacao Cartao-072">
      <Fr fi="3">
        <P v="#c#072" />
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
      <EL n="Senha">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Senha" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

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

    <BL n="Emissao Cheques-502">
      <Fr fi="3">
        <P v="#c#502" />
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
      <EL n="TipoLoCheque">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoLoCheque" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NumCheqEmi">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumCheqEmi" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Deposito Notas com validacao-506,536,537">
      <Fr fi="3">
        <P v="#c#506,536,537" />
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
      <EL n="RefDepVal">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#RefDepVal" />
          <P v="#c#15" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="IndAccao">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IndAccao" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NIB">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NIB" />
          <P v="#c#21" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="TotDepCat2">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TotDepCat2" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="TotDepCat3">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TotDepCat3" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="TotDepCat4">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TotDepCat4" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NumOcorr">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumOcorr" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CatNotas">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CatNotas" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="DenomIn-E">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#DenomIn-E" />
          <P v="#c#3" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NumNotDep">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumNotDep" />
          <P v="#c#3" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Deposito Cheques com validacao-507">
      <Fr fi="3">
        <P v="#c#507" />
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
      <EL n="RefDepVal">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#RefDepVal" />
          <P v="#c#15" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NIB">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NIB" />
          <P v="#c#21" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NumOcorr">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumOcorr" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="MontChqInd">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#MontChqInd" />
          <P v="#c#9" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="Zib">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Zib" />
          <P v="#c#8" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NConta">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NConta" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NCheque">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NCheque" />
          <P v="#c#10" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="TipoCheque">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoCheque" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Confirmacao Emissao Cheques-532">
      <Fr fi="3">
        <P v="#c#532" />
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
      <EL n="TipoLoChq">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoLoChq" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CustoChq">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CustoChq" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ImpSelo">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ImpSelo" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NumChqEmi">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumChqEmi" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="Zib">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Zib" />
          <P v="#c#8" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NConta">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NConta" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NCheque">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NCheque" />
          <P v="#c#10" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="TipoCheque">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoCheque" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Anulacao Emissao Cheques-533">
      <Fr fi="3">
        <P v="#c#533" />
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
      <EL n="TipoLoChq">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoLoChq" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CustoChq">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CustoChq" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ImpSelo">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ImpSelo" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NumChqAnl">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NumChqAnl" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="Zib">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Zib" />
          <P v="#c#8" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NConta">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NConta" />
          <P v="#c#11" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NCheque">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NCheque" />
          <P v="#c#10" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="TipoCheque">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoCheque" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>


    <BL n="Pedido Cheques-ODB">
      <Fr fi="3">
        <P v="#c#ODB" />
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
      <EL n="TipoChq">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoChq" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NChq">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NChq" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="LocEntChq">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#LocEntChq" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>

    <BL n="Dados Operacao Original">
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
          <P v="#uv#AplicPddOriginal" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="IdLog-O">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IdLogOriginal" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="NrLog-O">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NrLogOriginal" />
          <P v="#c#8" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="DhMsg-O">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#DhMsg-O" />
          <P v="#c#14" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="MotAnul">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#MotAnul" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="ScriptRes">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ScriptRes" />
          <P v="#c#21" />
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
