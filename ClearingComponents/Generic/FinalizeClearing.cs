using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SF.Expand.Core.Orch;
using SF.Expand.Business;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

namespace SF.Expand.Switch.Clearing
{
    public class FinalizeClearing : OrchPipeComponent
    {
        public override void RunComponent(IOrchWrkData OrchWrkData, string[] Params)
        {
            OrchPipeComponent.ComponentState State = new ComponentState(OrchWrkData, Params);

            StreamReader sr = (StreamReader)State.OrchWrkData.GetFromObjBucket("FileStreamReader");
            if (sr!=null)
                sr.Close();
        }

        public void InvalidateClearing(string rmb5JeFicheiroEmis, string ori5JeFicheiroEmis, string dst5JeFicheiroEmis, string mov5JeFicheiroEmis, string cln5JeFicheiroEmis)
        {
            Database db = DatabaseFactory.CreateDatabase("BESASwitch");
            DbCommand dbCommand = db.GetStoredProcCommand("InvalidateClearing");
            if (rmb5JeFicheiroEmis == "")
                rmb5JeFicheiroEmis = null;

            if (ori5JeFicheiroEmis == "")
                ori5JeFicheiroEmis = null;

            if (dst5JeFicheiroEmis == "")
                dst5JeFicheiroEmis = null;

            if (mov5JeFicheiroEmis == "")
                mov5JeFicheiroEmis = null;

            if (cln5JeFicheiroEmis == "")
                cln5JeFicheiroEmis = null;


            db.AddInParameter(dbCommand, "rmb5JeFicheiroEmis", DbType.Int32, rmb5JeFicheiroEmis);
            db.AddInParameter(dbCommand, "ori5JeFicheiroEmis", DbType.Int32, ori5JeFicheiroEmis);
            db.AddInParameter(dbCommand, "dst5JeFicheiroEmis", DbType.Int32, dst5JeFicheiroEmis);
            db.AddInParameter(dbCommand, "mov5JeFicheiroEmis", DbType.Int32, mov5JeFicheiroEmis);
            db.AddInParameter(dbCommand, "cln5JeFicheiroEmis", DbType.Int32, cln5JeFicheiroEmis);

            db.ExecuteNonQuery(dbCommand);
        }

    }
}
