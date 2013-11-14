using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;

using SF.Expand.Core.Orch;
using SF.Expand.Business;
using SF.Expand.Core.Connectivity;

namespace SF.Expand.Switch.PipelineComponents
{
    /// <summary>
    /// Message Constructor
    /// </summary>
    public class MessageConstructor : OrchPipeComponent
    {
        /// <summary>
        /// Runs the component.
        /// </summary>
        /// <param name="OrchWrkData">The orch WRK data.</param>
        /// <param name="Params">The params.</param>
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            // State is in Error do not create message for FlexCube
            if (State.IsInError && (Params[1] == "FlexCubeRequest"))
                return;

            // Message Repeated from PRT(-1) and we already have a response do not create the message for SIBS
            if ((State.IsInError) && (State.OrchWrkData.GetWrkData().ReadNodeValue("PRTResponse", true) != null))
                return;

            if (Params[1] == "FlexCubeRequest" && State.OrchWrkData.GetWrkData().ReadNodeValue("FlexCubeConstructor", true) == "")
                return;

            string originalResponseCode = State.OrchWrkData.GetWrkData().ReadNodeValue("OriginalResponseCode", true);

            // If we already have a switch response
            if (originalResponseCode != null)
                return;

            try
            {
                State.OrchWrkData.RemoveFromObjBucket(OrchestratorDefs.WRKOBJ_MSG_CONSTRUCT);
                DoConstruct(State);
            }
            catch (Exception exp)
            {
                State.LastError = new BusinessException(exp.Message, exp);
                State.OrchWrkData.GetWrkData().WriteNodeValue("TextoErro", exp.Message, true);
            }
            finally
            {
                byte[] requestMsg = (byte[])State.OrchWrkData.GetFromObjBucket(OrchestratorDefs.WRKOBJ_MSG_CONSTRUCT);
                if (requestMsg!=null)
                    State.OrchWrkData.GetWrkData().WriteNodeBuffer(Params[1], requestMsg, true);
            }
        }
        private void DoConstruct(OrchPipeComponent.ComponentState State)
        {
            if (State.Params.Length < 1)
                throw new BusinessException("PipelineService.MessageConstructor - Insufficient Parameters! Check pipeline configuration!");

            System.Configuration.AppSettingsReader s = new AppSettingsReader();
            string sGetConstMsg = (string)s.GetValue("GetConstMsg", typeof(string));
            string sMessageConstructor = (string)s.GetValue("MessageConstructor", typeof(string));

            string _typeNameGetMsg = sGetConstMsg;
            if (_typeNameGetMsg == null || _typeNameGetMsg.Length == 0)
                throw new BusinessException("PipelineService.MessageConstructor - GetConstMsg Object not defined! Check pipeline configuration (2nd param)!");

            Type _typeGetMsg = null;
            try { _typeGetMsg = Type.GetType(_typeNameGetMsg, true); }
            catch (Exception X)
            {
                throw new BusinessException("PipelineService.MessageConstructor - Error loading GetConstMsg object!", X);
            }

            if (!typeof(IFnGetConstMsg).IsAssignableFrom(_typeGetMsg))
                throw new BusinessException("PipelineService.MessageConstructor - GetConstMsg Object seems to have no implementation of IFnGetConstMsg interface!");

            IFnGetConstMsg GetMsgObj = Activator.CreateInstance(_typeGetMsg, true) as IFnGetConstMsg;
            if (GetMsgObj == null)
                throw new BusinessException("PipelineService.MessageConstructor - Error creating GetConstMsg Object. Returned null!");

            string[] ParamArr = new string[State.Params.Length];
            Array.Copy(State.Params, ParamArr, 1);
            string sMsgName = "";
            try
            {
                MsgException Ret = GetMsgObj.GetConstMsgName(State.OrchWrkData, ParamArr, out sMsgName);
                if (Ret != null)
                {
                    State.LastError = new BusinessException("GetConstMsgName Failed (returned false)!", Ret);
                    return;
                }
            }
            catch (Exception X)
            {
                throw new BusinessException("PipelineService.MessageConstructor - GetConstMsgName thrown an Exception!", X);
            }

            string _typeNameConstObj = sMessageConstructor;
            if (_typeNameConstObj == null || _typeNameConstObj.Length == 0)
                throw new BusinessException("PipelineService.MessageConstructor - MessageConstructor Object not defined! Check pipeline configuration (1st param)!");

            Type _type = null;
            try { _type = Type.GetType(_typeNameConstObj, true); }
            catch (Exception X)
            {
                throw new BusinessException("PipelineService.MessageConstructor - Error loading Contructor object!", X);
            }

            if (!typeof(IMessageConstructor).IsAssignableFrom(_type))
                throw new BusinessException("PipelineService.MessageConstructor - Constructor Object seems to have no implementation of IMessageConstructor interface!");

            IMessageConstructor ConstrObj = Activator.CreateInstance(_type, true) as IMessageConstructor;

            if (ConstrObj == null)
                throw new BusinessException("PipelineService.MessageConstructor - Error creating Constructor Object. Returned null!");

            try
            {
                ConstrObj.LoadMsgConstructorConfiguration(State.OrchWrkData, sMsgName);

                ConstrObj.ConstructMessage(State.OrchWrkData, OrchestratorDefs.WRKOBJ_MSG_CONSTRUCT);
            }
            catch (Exception X)
            {
                throw new BusinessException("PipelineService.MessageConstructor - Message " + sMsgName + " construction Failed!", X);
            }
        }
    }
}
