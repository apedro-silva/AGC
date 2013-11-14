use [BesaSwitch]
go


declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_1137'
select @MessageDirection=1
delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS pmsg="1137 - PEDIDO DE OPERAÇÃO COM CARTÃO (ACQUIRER)">
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

      <EL n="TipoCart">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoCart" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="EntAut">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#EntAut" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CodRec">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CodRec" />
          <P v="#c#2" />
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
      
    <BL n="Dados Adicionais Operacoes EMV">
      <EL n="NrSeqPan">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#NrSeqPan" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="DtIniAplic">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#DtIniAplic" />
          <P v="#c#6" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CarTerm">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CarTerm" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CapTermAcq">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CapTermAcq" />
          <P v="#c#3" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="AppCounter">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#AppCounter" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="AppProfile">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#AppProfile" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="UnpNr">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#UnpNr" />
          <P v="#c#4" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="TipoCrip">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#TipoCrip" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="Criptograma">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Criptograma" />
          <P v="#c#8" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="IssApData">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#IssApData" />
          <P v="#c#33" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="Tvr">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Tvr" />
          <P v="#c#5" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CriptInf">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CriptInf" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CvmResults">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CvmResults" />
          <P v="#c#3" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="DfName">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#DfName" />
          <P v="#c#17" />
          <P v="#c#1" />
        </Fv>
      </EL>
    </BL>
      
    <BL n="Dados Seguranca">
      <EL n="ExpP2">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#ExpP2" />
          <P v="#c#1" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="CompP2">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#CompP2" />
          <P v="#c#2" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="Pista2">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#Pista2" />
          <P v="#c#37" />
          <P v="#c#1" />
        </Fv>
      </EL>
      <EL n="PinBlock">
        <Fv fi="0">
          <P v="#c#ascii" />
          <P v="#uv#PinBlock" />
          <P v="#c#8" />
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

    <BL n="Autorizacoes-067,068,089">
      <Fr fi="3">
        <P v="#c#067,068,089" />
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

    <BL n="Cancelamentos-069">
      <Fr fi="3">
        <P v="#c#069" />
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
    <BL n="Pagamento Servicos Compra MB-0P7">
      <Fr fi="3">
        <P v="#c#0P7" />
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
