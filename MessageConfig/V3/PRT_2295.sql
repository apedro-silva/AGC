use besaswitch
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'PRT_2295'
select @MessageDirection=0
delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
  <MS cmsg="2295-Resposta pedido anulacao de Operacao com NBA">
    <BL n="Message">
      <EL n="CodMsg">
        <Fv fi="0">
          <P v="#c#2295" />
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
    </BL>
  </MS>

  <FNS>
    <f i="0" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCII, ExpandMsgFns" />
  </FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId and MessageDirection=@MessageDirection
