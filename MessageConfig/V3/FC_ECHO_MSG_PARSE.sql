use besaswitch
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'FC_ECHO_MSG_PARSE'
select @MessageDirection=1

delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--Parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
	<MS pmsg="Flex Cube ECHO Message Parser">
    <EL n="FlexCubeHeader">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#fc-ISO" />
        <P v="#c#12" />
        <P v="#c#1" />
       </Fv>
    </EL>
    <EL n="MessageType">
      <Fv fi="0">
        <P v="#c#ascii" />
        <P v="#uv#fc-MessageType" />
        <P v="#c#4" />
        <P v="#c#1" />
       </Fv>
    </EL>
    <EL n="BITMAP">
      <Fv fi="2">
        <P v="#uv#fc-BITMAP" />
        <P v="#c#32" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="TransmissionDateTime">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-TransmissionDateTime" />
        <P v="#c#10" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#7" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="SystemTraceAuditNumber">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-SystemTraceAuditNumber" />
        <P v="#c#6" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#11" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>

    <EL n="ResponseCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-ResponseCode" />
        <P v="#c#2" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#39" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>

    <EL n="NetworkManagementInfoCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-NetworkManagementInfoCode" />
        <P v="#c#3" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#70" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>

  </MS>

	<FNS>
		<f i="0" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValGenDecoding, ExpandMsgFns" />
    <f i="1" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValISO8583, ISO8583MsgFns" />
    <f i="2" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValWindows1252, ISO8583MsgFns" />
  </FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId
and MessageDirection=@MessageDirection