using System;
using System.Collections.Generic;
using KMBL.StepupAuthentication;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;

namespace KMBLNetNbanking
{
    public class MasterEventsFetch
    {
        //Events Master data Fetch

        public int EVENT_ID { get; set; }

        public string EVENT_NAME { get; set; }
    }
}