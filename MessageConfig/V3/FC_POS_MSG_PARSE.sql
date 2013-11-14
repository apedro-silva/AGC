use besaswitch
go

declare @trnId varchar(25)
declare @MessageDirection tinyint
select @trnId = 'FC_POS_MSG_PARSE'
select @MessageDirection=1

delete from Exp_MessagesConfigCache where messageName=@trnId and MessageDirection=@MessageDirection

--Parser
insert into Exp_MessagesConfigCache values(@trnId, @MessageDirection, 0, '
<MSG_CONFIG>
	<MS pmsg="FlexCube POS Message Parser">
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
      <Fv fi="3">
        <P v="#uv#fc-BITMAP" />
        <P v="#c#32" />
        <P v="#c#1" />
      </Fv>
    </EL>
    <EL n="sz_PrimaryAccountNumber">
      <Fv fi="2">
        <P v="#wa#" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#2" />
        <P v="#c#LLVAR" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="PrimaryAccountNumber">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-PrimaryAccountNumber" />
        <P v="#cfx#preceding-sibling::*[1]" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#2" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="ProcessingCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-ProcessingCode" />
        <P v="#c#6" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#3" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="TransactionAmount">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-TransactionAmount" />
        <P v="#c#12" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#4" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="SettlementAmount">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-SettlementAmount" />
        <P v="#c#12" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#5" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="CardholderBillingAmount">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-CardholderBillingAmount" />
        <P v="#c#12" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#6" />
        <P v="#c#UNPACKED" />
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
    <EL n="CardholderBillingFeeAmount">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-CardholderBillingFeeAmount" />
        <P v="#c#8" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#8" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="SettlementConversionRate">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-SettlementConversionRate" />
        <P v="#c#8" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#9" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="CardholderBillingConversionRate">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-CardholderBillingConversionRate" />
        <P v="#c#8" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#10" />
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
    <EL n="LocalTransactionTime">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-LocalTransactionTime" />
        <P v="#c#6" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#12" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="LocalTransactionDate">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-LocalTransactionDate" />
        <P v="#c#4" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#13" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="SettlementDate">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-SettlementDate" />
        <P v="#c#4" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#15" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="CaptureDate">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-CaptureDate" />
        <P v="#c#4" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#17" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    
    
    <EL n="MerchantType">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-MerchantType" />
        <P v="#c#4" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#18" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    
    <EL n="POSConditionCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-POSConditionCode" />
        <P v="#c#2" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#25" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>


    <EL n="TransactionFeeAmount">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-TransactionFeeAmount" />
        <P v="#c#8" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#28" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="SettlementFeeAmount">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-SettlementFeeAmount" />
        <P v="#c#8" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#29" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="TransactionProcessingFeeAmount">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-TransactionProcessingFeeAmount" />
        <P v="#c#8" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#30" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="SettlementProcessingFeeAmount">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-SettlementProcessingFeeAmount" />
        <P v="#c#8" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#31" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="sz_AcquiringInstitutionIdentificationCode">
      <Fv fi="2">
        <P v="#wa#" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#32" />
        <P v="#c#LLVAR" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="AcquiringInstitutionIdentificationCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-AcquiringInstitutionIdentificationCode" />
        <P v="#cfx#preceding-sibling::*[1]" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#32" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="sz_ForwardingInstitutionIdentificationCode">
      <Fv fi="2">
        <P v="#wa#" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#33" />
        <P v="#c#LLVAR" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="ForwardingInstitutionIdentificationCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-ForwardingInstitutionIdentificationCode" />
        <P v="#cfx#preceding-sibling::*[1]" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#33" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="RetrievalReferenceNumber">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-RetrievalReferenceNumber" />
        <P v="#c#12" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#37" />
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
    <EL n="CardAcceptorTerminalIdentification">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-CardAcceptorTerminalIdentification" />
        <P v="#c#8" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#41" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="CardAcceptorIdentificationCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-CardAcceptorIdentificationCode" />
        <P v="#c#15" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#42" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="CardAcceptorName">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-CardAcceptorName" />
        <P v="#c#40" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#43" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="TransactionCurrencyCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-TransactionCurrencyCode" />
        <P v="#c#3" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#49" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="SettlementCurrencyCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-SettlementCurrencyCode" />
        <P v="#c#3" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#50" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="CardholderBillingCurrencyCode">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-CardholderBillingCurrencyCode" />
        <P v="#c#3" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#51" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="sz_AdditionalAmounts">
      <Fv fi="2">
        <P v="#wa#" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#54" />
        <P v="#c#LLLVAR" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="AdditionalAmounts">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-AdditionalAmounts" />
        <P v="#cfx#preceding-sibling::*[1]" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#54" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="sz_ReservedPrivate">
      <Fv fi="2">
        <P v="#wa#" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#60" />
        <P v="#c#LLLVAR" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="ReservedPrivate">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-ReservedPrivate" />
        <P v="#cfx#preceding-sibling::*[1]" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#60" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="OriginalDataElements">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-OriginalDataElements" />
        <P v="#c#42" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#90" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="ReplacementAmounts">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-ReplacementAmounts" />
        <P v="#c#56" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#95" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>

    <EL n="sz_FromAccountNumber">
      <Fv fi="2">
        <P v="#wa#" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#102" />
        <P v="#c#LLVAR" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="FromAccountNumber">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-FromAccountNumber" />
        <P v="#cfx#preceding-sibling::*[1]" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#102" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="sz_ToAccountNumber">
      <Fv fi="2">
        <P v="#wa#" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#103" />
        <P v="#c#LLVAR" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="ToAccountNumber">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-ToAccountNumber" />
        <P v="#cfx#preceding-sibling::*[1]" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#103" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>

    <EL n="PreauthorisationChargebackData">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-PreauthorisationChargebackData" />
        <P v="#c#41" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#126" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>


    <EL n="sz_Movements">
      <Fv fi="2">
        <P v="#wa#" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#127" />
        <P v="#c#LLLVAR" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>
    <EL n="Movements">
      <Fv fi="1">
        <P v="#c#ascii" />
        <P v="#uv#fc-Movements" />
        <P v="#cfx#preceding-sibling::*[1]" />
        <P v="#uv#fc-BITMAP" />
        <P v="#c#127" />
        <P v="#c#UNPACKED" />
      </Fv>
    </EL>

  </MS>

	<FNS>
		<f i="0" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValGenDecoding, ExpandMsgFns" />
    <f i="1" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValISO8583, ISO8583MsgFns" />
    <f i="2" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValVLISO8583, ISO8583MsgFns" />
    <f i="3" ty="PVal" tn="SF.Expand.Core.Connectivity.FnParseValWindows1252, ISO8583MsgFns" />
  </FNS>
</MSG_CONFIG>
')

select * from Exp_MessagesConfigCache
where messageName=@trnId
and MessageDirection=@MessageDirection