using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using SF.Expand.Core.Connectivity;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// Message Parser
    /// </summary>
    public class MessageParser : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            byte[] msg2Parse = (byte[])State.OrchWrkData.GetFromObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE);

            // If we do not have message to parse return
            if (msg2Parse==null)
                return;

            try
            {
                DoParse(State);
                State.OrchWrkData.RemoveFromObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message,exp);
                State.OrchWrkData.GetWrkData().WriteNodeValue("ErrorText", exp.Message, true);
            }
        }

        private void DoParse(OrchPipeComponent.ComponentState State)
        {
            if (State.Params.Length < 1)
                throw new BusinessException("PipeMsgParserComp - Insufficient Parameters! Check pipeline configuration!");

            System.Configuration.AppSettingsReader s = new AppSettingsReader();
            string sGetConstMsg = (string)s.GetValue("GetParseMsg", typeof(string));
            string sMessageParser = (string)s.GetValue("MessageParser", typeof(string));
            if (sGetConstMsg == "")
                throw new BusinessException("PipeMsgParserComp - GetParseMsg Object not defined! Check Web.config configuration !");
            if (sMessageParser == "")
                throw new BusinessException("PipeMsgParserComp - MessageParser Object not defined! Check Web.config configuration!");

            string _typeNameGetMsg = sGetConstMsg;
            if (_typeNameGetMsg == null || _typeNameGetMsg.Length == 0)
                throw new BusinessException("PipeMsgParserComp - GetParseMsg Object not defined! Check pipeline configuration (2nd param)!");

            Type _typeGetMsg = null;
            try { _typeGetMsg = Type.GetType(_typeNameGetMsg, true); }
            catch (Exception X)
            {
                throw new BusinessException("PipeMsgParserComp - Error loading GetParseMsg object!", X);
            }

            if (!typeof(IFnGetParseMsg).IsAssignableFrom(_typeGetMsg))
                throw new BusinessException("PipeMsgParserComp - GetParseMsg Object seems to have no implementation of IFnGetParseMsg interface!");

            IFnGetParseMsg GetMsgObj = Activator.CreateInstance(_typeGetMsg, true) as IFnGetParseMsg;
            if (GetMsgObj == null)
                throw new BusinessException("PipeMsgParserComp - Error creating GetParseMsg Object. Returned null!");

            byte[] Buffer = State.OrchWrkData.GetFromObjBucket(OrchestratorDefs.WRKOBJ_MSG_PARSE) as byte[];

            string[] ParamArr = new string[State.Params.Length];
            Array.Copy(State.Params, ParamArr, 1);
            string sMsgName = "";
            try
            {
                MsgException Ret = GetMsgObj.GetParseMsgName(State.OrchWrkData, ParamArr, Buffer, out sMsgName);
                if (Ret != null)
                {
                    //State.LastError = new BusinessException("GetParseMsgName Failed (returned false)!", Ret);
                    return;
                }
            }
            catch (Exception X)
            {
                throw new BusinessException("PipeMsgParserComp - GetParseMsgName thrown an Exception!", X);
            }

            string _typeNameParseObj = sMessageParser;
            if (_typeNameParseObj == null || _typeNameParseObj.Length == 0)
                throw new BusinessException("PipeMsgParserComp - MessageParser Object not defined! Check pipeline configuration (1st param)!");

            Type _type = null;
            try { _type = Type.GetType(_typeNameParseObj, true); }
            catch (Exception X)
            {
                throw new BusinessException("PipeMsgParserComp - Error loading Parser object!", X);
            }

            if (!typeof(IMessageParser).IsAssignableFrom(_type))
                throw new BusinessException("PipeMsgParserComp - Parser Object seems to have no implementation of IMessageParser interface!");

            IMessageParser ParserObj = Activator.CreateInstance(_type, true) as IMessageParser;

            try
            {
                ParserObj.LoadMsgParserConfiguration(State.OrchWrkData, sMsgName);

                ParserObj.ParseMessage(State.OrchWrkData, OrchestratorDefs.WRKOBJ_MSG_PARSE);
            }
            catch (Exception X)
            {
                throw new BusinessException("PipeMsgParserComp - Message " + sMsgName + " parsing Failed!", X);
            }
        }
    }
}
