use besaswitch
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'FC_ATM_MSG_CONSTRUCT'
select @MessageDirection=0
delete from Exp_MessagesConfigCache where messageName=@trnId  and MessageDirection=@MessageDirection

--Constructor
--<?xml version="1.0" encoding="ISO-8859-1"?>

insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
	<MS cmsg="Flex Cube ATM Message Construct">
    <BL n="Message">
      <EL n="ISOHeader">
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
    <EL n="PrimaryAccountNumber_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#2" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="PrimaryAccountNumber">
      <Fv fi="3">
        <P v="#uv#PrimaryAccountNumber" />
        <P v="#c#LLVAR" />
      </Fv>
    </EL>

    <EL n="ProcessingCode_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#3" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="ProcessingCode">
      <Fv fi="0">
        <P v="#uv#ProcessingCode" />
      </Fv>
    </EL>
    <EL n="TransactionAmount_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#4" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="TransactionAmount">
      <Fv fi="0">
        <P v="#uv#TransactionAmount" />
      </Fv>
    </EL>

    <EL n="SettlementAmount_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#5" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="SettlementAmount">
      <Fv fi="0">
        <P v="#uv#SettlementAmount" />
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

    <EL n="SettlementConversionRate_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#9" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="SettlementConversionRate">
      <Fv fi="0">
        <P v="#uv#SettlementConversionRate" />
      </Fv>
    </EL>
    <EL n="CardholderBillingConversionRate_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#10" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="CardholderBillingConversionRate">
      <Fv fi="0">
        <P v="#uv#CardholderBillingConversionRate" />
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
    <EL n="LocalTransactionTime_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#12" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="LocalTransactionTime">
      <Fv fi="0">
        <P v="#uv#LocalTransactionTime" />
      </Fv>
    </EL>
    <EL n="LocalTransactionDate_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#13" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="LocalTransactionDate">
      <Fv fi="0">
        <P v="#uv#LocalTransactionDate" />
      </Fv>
    </EL>
    <EL n="SettlementDate_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#15" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="SettlementDate">
      <Fv fi="0">
        <P v="#uv#SettlementDate" />
      </Fv>
    </EL>
    <EL n="CaptureDate_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#17" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="CaptureDate">
      <Fv fi="0">
        <P v="#uv#CaptureDate" />
      </Fv>
    </EL>
    <EL n="TransactionFeeAmount_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#28" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="TransactionFeeAmount">
      <Fv fi="0">
        <P v="#uv#TransactionFeeAmount" />
      </Fv>
    </EL>
    <EL n="SettlementFeeAmount_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#29" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="SettlementFeeAmount">
      <Fv fi="0">
        <P v="#uv#SettlementFeeAmount" />
      </Fv>
    </EL>
    <EL n="TransactionProcessingFeeAmount_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#30" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="TransactionProcessingFeeAmount">
      <Fv fi="0">
        <P v="#uv#TransactionProcessingFeeAmount" />
      </Fv>
    </EL>
    <EL n="SettlementProcessingFeeAmount_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#31" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="SettlementProcessingFeeAmount">
      <Fv fi="0">
        <P v="#uv#SettlementProcessingFeeAmount" />
      </Fv>
    </EL>
    <EL n="AcquiringInstitutionIdentificationCode_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#32" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="AcquiringInstitutionIdentificationCode">
      <Fv fi="3">
        <P v="#uv#AcquiringInstitutionIdentificationCode" />
        <P v="#c#LLVAR" />
      </Fv>
    </EL>
    <EL n="ForwardingInstitutionIdentificationCode_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#33" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="ForwardingInstitutionIdentificationCode">
      <Fv fi="3">
        <P v="#uv#ForwardingInstitutionIdentificationCode" />
        <P v="#c#LLVAR" />
      </Fv>
    </EL>

    <EL n="RetrievalReferenceNumber_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#37" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="RetrievalReferenceNumber">
      <Fv fi="0">
        <P v="#uv#RetrievalReferenceNumber" />
      </Fv>
    </EL>
    <EL n="CardAcceptorTerminalIdentification_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#41" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="CardAcceptorTerminalIdentification">
      <Fv fi="0">
        <P v="#uv#CardAcceptorTerminalIdentification" />
      </Fv>
    </EL>
    <EL n="CardAcceptorIdentificationCode_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#42" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="CardAcceptorIdentificationCode">
      <Fv fi="0">
        <P v="#uv#CardAcceptorIdentificationCode" />
      </Fv>
    </EL>


      <EL n="CardAcceptorName_BitMap">
        <Ft fi="2">
          <P v="#cfx#following-sibling::*" />
          <P v="#cfx#//EL[@n=''BITMAP'']" />
          <P v="#c#43" />
          <P v="#c#UNPACKED" />
        </Ft>
      </EL>
      <EL n="CardAcceptorName">
        <Fv fi="6">
          <P v="#c#iso-8859-1" />
          <P v="#uv#CardAcceptorName" />
        </Fv>
      </EL>

    <EL n="TransactionCurrencyCode_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#49" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="TransactionCurrencyCode">
      <Fv fi="0">
        <P v="#uv#TransactionCurrencyCode" />
      </Fv>
    </EL>
    <EL n="SettlementCurrencyCode_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#50" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="SettlementCurrencyCode">
      <Fv fi="0">
        <P v="#uv#SettlementCurrencyCode" />
      </Fv>
    </EL>
    <EL n="ReservedPrivate_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#60" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="ReservedPrivate">
      <Fv fi="3">
        <P v="#uv#ReservedPrivate" />
        <P v="#c#LLLVAR" />
      </Fv>
    </EL>
    <EL n="OriginalDataElements_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#90" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="OriginalDataElements">
      <Fv fi="0">
        <P v="#uv#OriginalDataElements" />
      </Fv>
    </EL>
    <EL n="ReplacementAmounts_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#95" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="ReplacementAmounts">
      <Fv fi="0">
        <P v="#uv#ReplacementAmounts" />
      </Fv>
    </EL>
    <EL n="FromAccountNumber_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#102" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="FromAccountNumber">
      <Fv fi="3">
        <P v="#uv#FromAccountNumber" />
        <P v="#c#LLVAR" />
      </Fv>
    </EL>

    <EL n="ToAccountNumber_BitMap">
      <Ft fi="2">
        <P v="#cfx#following-sibling::*" />
        <P v="#cfx#//EL[@n=''BITMAP'']" />
        <P v="#c#103" />
        <P v="#c#UNPACKED" />
      </Ft>
    </EL>
    <EL n="ToAccountNumber">
      <Fv fi="3">
        <P v="#uv#ToAccountNumber" />
        <P v="#c#LLVAR" />
      </Fv>
    </EL>

   </BL>


    </MS>

	<FNS>
		<f i="0" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValASCII, ExpandMsgFns" />
    <f i="1" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValISO8583BitMap, ISO8583MsgFns" />
    <f i="2" ty="CTrf" tn="SF.Expand.Core.Connectivity.FnConstTrfISO8583BitMap, ISO8583MsgFns" />
    <f i="3" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValEncVLISO8583, ISO8583MsgFns" />
    <f i="4" ty="CTrf" tn="SF.Expand.Core.Connectivity.FnConstTrfFlexCubeBuffSize, ISO8583MsgFns" />
    <f i="5" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValFlexCubeHeader, ISO8583MsgFns" />
		<f i="6" ty="CVal" tn="SF.Expand.Core.Connectivity.FnConstValGenEncoding, ExpandMsgFns" />
  </FNS>
</MSG_CONFIG>')

select * from Exp_MessagesConfigCache
where messageName=@trnId
and MessageDirection=@MessageDirection