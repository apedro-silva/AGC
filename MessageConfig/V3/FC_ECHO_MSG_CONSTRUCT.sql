use besaswitch
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'FC_ECHO_MSG_CONSTRUCT'
select @MessageDirection=0
delete from Exp_MessagesConfigCache where messageName=@trnId  and MessageDirection=@MessageDirection

--Constructor
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
<MS cmsg="Flex Cube ECHO Message Construct">
	<BL n="Message">
      <EL n="MessageType">
      <Fv fi="0">
        <P v="#c#ISO016000010" />
      </Fv>
	</EL>
      <EL n="MessageType">
      <Fv fi="0">
        <P v="#uv#MessageType" />
      </Fv>
    </EL>
    <EL n="BITMAP">
      <Fv fi="1">
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>

    <EL n="TransmissionDateTime_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#7" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="TransmissionDateTime">
      <Fv fi="0">
        <P v="#uv#TransmissionDateTime" />
      </Fv>
    </EL>

    <EL n="SystemTraceAuditNumber_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#11" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="SystemTraceAuditNumber">
      <Fv fi="0">
        <P v="#uv#SystemTraceAuditNumber" />
      </Fv>
    </EL>

    <EL n="NetworkManagementInformationCode_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#70" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="NetworkManagementInformationCode">
      <Fv fi="0">
        <P v="#c#301" />
      </Fv>
    </EL>

   </BL>


    </MS>

	<FNS>
		<f i="0" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCII, ExpandMsgFns" />
    <f i="1" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValISO8583BitMap, ISO8583MsgFns" />
    <f i="2" ty="CTrf" tn="SF.Expand.Core.Connectivity.FnConstTrfISO8583BitMap, ISO8583MsgFns" />
  </FNS>
</MSG_CONFIG>')

select * from Exp_MessagesConfigCache
where messageName=@trnId
and MessageDirection=@MessageDirection