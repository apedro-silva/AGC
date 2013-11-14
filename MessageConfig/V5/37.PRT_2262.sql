use [BesaSwitch]
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_2262'
select @MessageDirection=0
select * from Exp_MessagesConfigCache
where messageName=@trnId and MessageDirection=@MessageDirection

delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS cmsg="2262 - RESPOSTA AO PEDIDO DE OPERAÇÃO COM CARTÃO (EMISSOR)">
    <BL n="Header">
      <EL n="CodMsg">
        <Fv fi="0">
          <P v="#c#2262" />
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
