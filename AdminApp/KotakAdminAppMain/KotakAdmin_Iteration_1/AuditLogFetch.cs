using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KMBLNetNbanking
{
    public class AuditLogFetch
    {
        public int AuditID {get; set;}

        public string CRN { get; set; }

        public int SessionID { get; set; }

        public int EventID { get; set; }

        public string EventName { get; set; }

        public int StatusID { get; set; }

        public string StatusName { get; set; }

        public string ChannelName { get; set; }

        public string GateNumber { get; set; }

        public string DeviceDetails { get; set; }

        public DateTime CreatedOn { get; set; }

        public int ParentTransactionID { get; set; }

        public int Score { get; set; }

        public int IsCompleted { get; set; }

        public string Version { get; set; }

    }
}