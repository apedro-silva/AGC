using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace SF.Expand.Switch.PipelineComponents
{
    class eXPandTrace
    {
        private const string FILE_NAME = "MessageTranslator.txt";
        public static void TraceMsg(string msg)
        {
            AppDomain myDomain = AppDomain.CurrentDomain;

            using (StreamWriter sw = File.AppendText(myDomain.RelativeSearchPath + @"\" + FILE_NAME))
            {
                sw.WriteLine("{0}-{1}-{2}", DateTime.Now, Assembly.GetCallingAssembly().GetName().Name, msg);
                sw.Close();
            }
        }

    }
}
