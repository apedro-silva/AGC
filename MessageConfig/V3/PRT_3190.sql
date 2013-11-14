use besaswitch
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_3190'
select @MessageDirection=1
delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS pmsg="3190 - Pedido de Operação Comerciante Pendente - Parser">
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
        <P v="#c#1" />
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
    <EL n="IdPropr">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#IdPropr" />
        <P v="#c#7" />
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
    <EL n="DtHora">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#DtHora" />
        <P v="#c#14" />
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
    <EL n="LocTerm">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#LocTerm" />
        <P v="#c#40" />
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
    <EL n="Conta">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#Conta" />
        <P v="#c#15" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="CodTrn">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#CodTrn" />
        <P v="#c#2" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="TotOper">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#TotOper" />
        <P v="#c#5" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="MontanteRecebido">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#MontanteRecebido" />
        <P v="#c#13" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalMontanteRecebido" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="ComEmi">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ComEmi" />
        <P v="#c#11" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalComEmi" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="ComProp">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ComProp" />
        <P v="#c#11" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalComProp" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>

    <EL n="ComRep">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ComRep" />
        <P v="#c#11" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalComRep" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>


    <EL n="ComMb">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ComMb" />
        <P v="#c#11" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalComMb" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>


    <EL n="MontanteRecebido2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#MontanteRecebido2" />
        <P v="#c#13" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#MontanteRecebido2" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>


    <EL n="ComEmi2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ComEmi2" />
        <P v="#c#11" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalComEmi2" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="ComProp2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ComProp2" />
        <P v="#c#11" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalComProp2" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>


    <EL n="ComRep2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ComRep2" />
        <P v="#c#11" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalComRep2" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>


    <EL n="ComMb2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ComMb2" />
        <P v="#c#11" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="Sinal2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalComMb2" />
        <P v="#c#1" />
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


    <EL n="TotCli">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#Montante1" />
        <P v="#c#13" />
        <P v="#c#1" />
      </Fv>
    </EL>
    
    <EL n="SinalTotCli">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalMontante1" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="ValCom">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ValCom" />
        <P v="#c#13" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="SinalValCom">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalValCom" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="TotCli2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#Montante2" />
        <P v="#c#13" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="SinalTotCli2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalMontante2" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>

    <EL n="ValCom2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#ValCom2" />
        <P v="#c#13" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="SinalValCom2">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#SinalValCom2" />
        <P v="#c#1" />
        <P v="#c#1" />
      </Fv>
    </EL>

  </MS>

  <FNS>
    <f i="0" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValGenDecoding, ExpandMsgFns" />
  </FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId and MessageDirection=@MessageDirection
