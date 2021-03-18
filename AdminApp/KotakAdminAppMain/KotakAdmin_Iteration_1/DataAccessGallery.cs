using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using KMBL.StepupAuthentication.CoreComponents.DataAccessHandler;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using KMBLNetNbanking;

namespace KotakAdmin_Iteration_1
{
    public class DashboardSPecificResultData
    {
        public string CRN { get; set; }
        public string ResulstReason { get; set; }
        public DateTime CreatedOn { get; set; }
        public string DeviceDetails { get; set; }
        public string ObjectId { get; set; }
        public string Version { get; set; }
    }

    public class DataAccessGallery
    {
        public string RestOfRecords = "";
        public string LoginValid = "",total_pending = "", total_approved = "", total_rejected = "", mobile_count="", netbanking_count="", branch_count="", mobile_graph_count = "", netbanking_graph_count = "", branch_graph_count = "", total_count="",total_records_faceauth ="";
        public dynamic GestureImage;
        private static string MongoDB_Registration_Databse = System.Configuration.ConfigurationManager.AppSettings["MongoDB_Registration_Databse"];
        private static string MongoDB_Verification_Databse = System.Configuration.ConfigurationManager.AppSettings["MongoDB_Verification_Databse"];
        private static string MongoDBConnectionstring = System.Configuration.ConfigurationManager.AppSettings["MongoDBConnectionstring"];
        public string PasswordKey = "";


        public string SP_GET_PERSON_ID(string CRN)
        {
            var dbManager = new DBManager("DBConnectionweb");

            OracleConnection connection = null;
            List<OracleParameter> parameters = new List<OracleParameter>();
            parameters.Add(dbManager.CreateParameter("v_crn_value", CRN, OracleDbType.NVarchar2));
            parameters.Add(dbManager.CreateParameter("v_DeviceType", OracleDbType.RefCursor, ParameterDirection.Output));
            OracleCommand objCmd = dbManager.GetCommand("SP_GET_PERSON_ID_DE_REGISTER", CommandType.StoredProcedure, parameters.ToArray(), out connection);

            connection.Open();
            OracleDataReader reader = objCmd.ExecuteReader();

            string PersonID = null;

            while (reader.Read())
            {
                PersonID = reader.GetString(0);
            }

            dbManager.CloseConnection(connection);

            return PersonID;
        }



        // Admin Dashboard user count Function
        public List<string> DashboardCountV1(string f, string t)
        {         

            var dbManager = new DBManager("DBConnectionweb");
            OracleDataReader dataReader = null;
            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            if (f == null || f == "")
            {
                parameters.Add(dbManager.CreateParameter("v_from_date", null, OracleDbType.Varchar2));
                f = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
                parameters.Add(dbManager.CreateParameter("v_from_date", f + " 00:00:00", OracleDbType.Varchar2));

            if (t == null || t == "")                
                t = DateTime.Now.ToString("yyyy-MM-dd");
                
            parameters.Add(dbManager.CreateParameter("v_to_date", t + " 23:59:59", OracleDbType.Varchar2));            
            parameters.Add(dbManager.CreateParameter("total_reg", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("mobile_reg_Android", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("mobile_reg_IOS", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("netbanking_reg", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("branch_reg", OracleDbType.Int64, ParameterDirection.Output));
            
            parameters.Add(dbManager.CreateParameter("total_ver", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("mobile_ver_Android", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("mobile_ver_IOS", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("netbanking_ver", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("branch_ver", OracleDbType.Int64, ParameterDirection.Output));

            parameters.Add(dbManager.CreateParameter("latest_update_hour", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("latest_update_minute", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("from_date", OracleDbType.Date, ParameterDirection.Output));
            OracleCommand objCmd = dbManager.GetCommand("SP_DASHBOARD_COUNT_V1", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            connection.Open();
            dataReader = objCmd.ExecuteReader();
            dataReader.Close();
            dbManager.CloseConnection(connection);

            List<string> dcv1 = new List<string>();            
            dcv1.Add(objCmd.Parameters["total_reg"].Value.ToString());
            dcv1.Add(objCmd.Parameters["mobile_reg_Android"].Value.ToString());
            dcv1.Add(objCmd.Parameters["mobile_reg_IOS"].Value.ToString());
            dcv1.Add(objCmd.Parameters["netbanking_reg"].Value.ToString());
            dcv1.Add(objCmd.Parameters["branch_reg"].Value.ToString());
            
            dcv1.Add(objCmd.Parameters["total_ver"].Value.ToString());
            dcv1.Add(objCmd.Parameters["mobile_ver_Android"].Value.ToString());
            dcv1.Add(objCmd.Parameters["mobile_ver_IOS"].Value.ToString());
            dcv1.Add(objCmd.Parameters["netbanking_ver"].Value.ToString());
            dcv1.Add(objCmd.Parameters["branch_ver"].Value.ToString());
            //dcv1.Add((objCmd.Parameters["latest_update_hour"].Value.ToString() == "0") ? objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minutes Ago": objCmd.Parameters["latest_update_hour"].Value.ToString()+" Hours "+objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minutes Ago");
            if (objCmd.Parameters["latest_update_hour"].Value.ToString() == null)
                dcv1.Add("No data found");
            else
                dcv1.Add((objCmd.Parameters["latest_update_hour"].Value.ToString() == "0") ? (objCmd.Parameters["latest_update_minute"].Value.ToString() == "0" || objCmd.Parameters["latest_update_minute"].Value.ToString() == "1") ? objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minute Ago" : objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minutes Ago" :
                ((objCmd.Parameters["latest_update_hour"].Value.ToString() == "1") ? objCmd.Parameters["latest_update_hour"].Value.ToString() + " Hour " : objCmd.Parameters["latest_update_hour"].Value.ToString() + " Hours ") + ((objCmd.Parameters["latest_update_minute"].Value.ToString() == "1" || objCmd.Parameters["latest_update_minute"].Value.ToString() == "0") ? objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minute Ago" : objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minutes Ago"));
            dcv1.Add(objCmd.Parameters["from_date"].Value.ToString());
            return dcv1;
        }



        // Admin Dashboard user count Function
        public List<string> DashboardFailureCount(string f, string t)
        {
            var dbManager = new DBManager("DBConnectionweb");
            OracleDataReader dataReader = null;
            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            if (f == null || f == "")
            {
                parameters.Add(dbManager.CreateParameter("v_from_date", null, OracleDbType.Varchar2));
                f = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
                parameters.Add(dbManager.CreateParameter("v_from_date", f + " 00:00:00", OracleDbType.Varchar2));

            if (t == null || t == "")
                t = DateTime.Now.ToString("yyyy-MM-dd");

            parameters.Add(dbManager.CreateParameter("v_to_date", t + " 23:59:59", OracleDbType.Varchar2));
            parameters.Add(dbManager.CreateParameter("total_reg", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("mobile_reg_Android", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("mobile_reg_IOS", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("netbanking_reg", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("branch_reg", OracleDbType.Int64, ParameterDirection.Output));

            parameters.Add(dbManager.CreateParameter("total_ver", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("mobile_ver_Android", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("mobile_ver_IOS", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("netbanking_ver", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("branch_ver", OracleDbType.Int64, ParameterDirection.Output));

            parameters.Add(dbManager.CreateParameter("latest_update_hour", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("latest_update_minute", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("from_date", OracleDbType.Date, ParameterDirection.Output));
            OracleCommand objCmd = dbManager.GetCommand("SP_DASHBOARD_FAILURE_COUNT", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            connection.Open();
            dataReader = objCmd.ExecuteReader();
            dataReader.Close();
            dbManager.CloseConnection(connection);

            List<string> dcv1 = new List<string>();
            dcv1.Add(objCmd.Parameters["total_reg"].Value.ToString());
            dcv1.Add(objCmd.Parameters["mobile_reg_Android"].Value.ToString());
            dcv1.Add(objCmd.Parameters["mobile_reg_IOS"].Value.ToString());
            dcv1.Add(objCmd.Parameters["netbanking_reg"].Value.ToString());
            dcv1.Add(objCmd.Parameters["branch_reg"].Value.ToString());

            dcv1.Add(objCmd.Parameters["total_ver"].Value.ToString());
            dcv1.Add(objCmd.Parameters["mobile_ver_Android"].Value.ToString());
            dcv1.Add(objCmd.Parameters["mobile_ver_IOS"].Value.ToString());
            dcv1.Add(objCmd.Parameters["netbanking_ver"].Value.ToString());
            dcv1.Add(objCmd.Parameters["branch_ver"].Value.ToString());
            //dcv1.Add((objCmd.Parameters["latest_update_hour"].Value.ToString() == "0") ? objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minutes Ago": objCmd.Parameters["latest_update_hour"].Value.ToString()+" Hours "+objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minutes Ago");
            if (objCmd.Parameters["latest_update_hour"].Value.ToString() == null)
                dcv1.Add("No data found");
            else
                dcv1.Add((objCmd.Parameters["latest_update_hour"].Value.ToString() == "0") ? (objCmd.Parameters["latest_update_minute"].Value.ToString() == "0" || objCmd.Parameters["latest_update_minute"].Value.ToString() == "1") ? objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minute Ago" : objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minutes Ago" :
                ((objCmd.Parameters["latest_update_hour"].Value.ToString() == "1") ? objCmd.Parameters["latest_update_hour"].Value.ToString() + " Hour " : objCmd.Parameters["latest_update_hour"].Value.ToString() + " Hours ") + ((objCmd.Parameters["latest_update_minute"].Value.ToString() == "1" || objCmd.Parameters["latest_update_minute"].Value.ToString() == "0") ? objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minute Ago" : objCmd.Parameters["latest_update_minute"].Value.ToString() + " Minutes Ago"));
            dcv1.Add(objCmd.Parameters["from_date"].Value.ToString());
            return dcv1;
        }

        
        // Admin Dashboard user count Function
        public List<string> DashboardGateDropOffsFailure(string f, string t)
        {
            var dbManager = new DBManager("DBConnectionweb");
            OracleDataReader dataReader = null;
            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            if (f == null || f == "")
            {
                parameters.Add(dbManager.CreateParameter("v_from_date", null, OracleDbType.Varchar2));
                f = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
                parameters.Add(dbManager.CreateParameter("v_from_date", f + " 00:00:00", OracleDbType.Varchar2));

            if (t == null || t == "")
                t = DateTime.Now.ToString("yyyy-MM-dd");

            parameters.Add(dbManager.CreateParameter("v_to_date", t + " 23:59:59", OracleDbType.Varchar2));

            parameters.Add(dbManager.CreateParameter("v_registration_drop_offs", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("v_verification_drop_offs", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("v_gate1_failures", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("v_gate2_failures", OracleDbType.Int64, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("v_gate3_failures", OracleDbType.Int64, ParameterDirection.Output));
            OracleCommand objCmd = dbManager.GetCommand("SP_DASHBOARD_GATE_FAILURE_DROP_OFFS", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            connection.Open();
            dataReader = objCmd.ExecuteReader();
            dataReader.Close();
            dbManager.CloseConnection(connection);
            List<string> dcv1 = new List<string>();
            dcv1.Add(objCmd.Parameters["v_registration_drop_offs"].Value.ToString());
            dcv1.Add(objCmd.Parameters["v_verification_drop_offs"].Value.ToString());
            dcv1.Add(objCmd.Parameters["v_gate1_failures"].Value.ToString());
            dcv1.Add(objCmd.Parameters["v_gate2_failures"].Value.ToString());
            dcv1.Add(objCmd.Parameters["v_gate3_failures"].Value.ToString());
            return dcv1;
        }



        public List<DashboardSPecificResultData> DashboardSPecificResult(int event_id, int resultsid, int channelid, string f, string t)
        {
            var dbManager = new DBManager("DBConnectionweb");
            OracleDataReader dataReader = null;
            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            if (f == null || f == "")
            {
                parameters.Add(dbManager.CreateParameter("v_from_date", null, OracleDbType.Varchar2));
                f = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
                parameters.Add(dbManager.CreateParameter("v_from_date", f + " 00:00:00", OracleDbType.Varchar2));

            if (t == null || t == "")
                t = DateTime.Now.ToString("yyyy-MM-dd");

            parameters.Add(dbManager.CreateParameter("v_to_date", t + " 23:59:59", OracleDbType.Varchar2));
            parameters.Add(dbManager.CreateParameter("v_event_id", event_id, OracleDbType.Int64));
            parameters.Add(dbManager.CreateParameter("v_resultsid", resultsid, OracleDbType.Int64));
            parameters.Add(dbManager.CreateParameter("v_channelid", channelid, OracleDbType.Int64));
            parameters.Add(dbManager.CreateParameter("r_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            OracleCommand objCmd = dbManager.GetCommand("SP_GET_DASHBOARD_SPECIFIC_RESULT", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            connection.Open();
            dataReader = objCmd.ExecuteReader();
            List<DashboardSPecificResultData> dsrd = new List<DashboardSPecificResultData>();
            while (dataReader.Read())
            {
                dsrd.Add(new DashboardSPecificResultData
                {                    
                    CRN = dataReader.IsDBNull(0)? null: dataReader.GetString(0),
                    CreatedOn = dataReader.IsDBNull(1) ?  new DateTime() : dataReader.GetDateTime(1),
                    ResulstReason = dataReader.IsDBNull(2) ? null : dataReader.GetString(2),
                    DeviceDetails = dataReader.IsDBNull(3) ? null : dataReader.GetString(3),
                    ObjectId= dataReader.IsDBNull(4) ? null : dataReader.GetString(4),
                    Version=dataReader.IsDBNull(5) ? "1" : dataReader.GetString(5)
                });
            }
            dataReader.Close();
            dbManager.CloseConnection(connection);
            //RestOfRecords = objCmd.Parameters["v_total_records"].Value.ToString();
            return dsrd;
        }

        public void SP_ClearCRN(string CRN)
        {
            var dbManager = new DBManager("DBConnectionweb");

            var parameters = new List<OracleParameter>();
            parameters.Add(dbManager.CreateParameter("v_CRN", CRN, OracleDbType.NVarchar2));

            dbManager.Delete("SP_CLEAR_CRN", CommandType.StoredProcedure, parameters.ToArray());
        }




        // Master Gate Function
        public List<KMBLNetNbanking.MasterGateFetch> MasterGateFetchlog()
        {
            var dbManager = new DBManager("DBConnectionweb");

            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            List<KMBLNetNbanking.MasterGateFetch> mgf = new List<KMBLNetNbanking.MasterGateFetch>();
            parameters.Add(dbManager.CreateParameter("gate_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var dataReader = dbManager.GetDataReader("SP_GET_GATE_MASTER_DETAILS", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            while (dataReader.Read())
            {
                mgf.Add(new KMBLNetNbanking.MasterGateFetch
                {
                    GATE_NUMBER = dataReader.GetInt32(0)
                });
            }
            dataReader.Close();
            dbManager.CloseConnection(connection);
            return mgf;
        }

        public List<KMBLNetNbanking.MasterStatusFetch> MasterStatusFetchlog()
        {

            var dbManager = new DBManager("DBConnectionweb");

            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            List<KMBLNetNbanking.MasterStatusFetch> msd = new List<KMBLNetNbanking.MasterStatusFetch>();
            parameters.Add(dbManager.CreateParameter("status_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var dataReader = dbManager.GetDataReader("SP_GET_STATUS_MASTER_DETAILS", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            while (dataReader.Read())
            {
                msd.Add(new KMBLNetNbanking.MasterStatusFetch
                {
                    status_id = dataReader.GetInt32(0),
                    status_name = dataReader.GetString(1)
                });
            }
            dataReader.Close();
            dbManager.CloseConnection(connection);
            return msd;
        }


        public List<string> GetVersions()
        {
            var dbManager = new DBManager("DBConnectionweb");
            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            List<string> med = new List<string>();
            parameters.Add(dbManager.CreateParameter("version_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var dataReader = dbManager.GetDataReader("SP_GET_VERSIONS", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            while (dataReader.Read())
            {
                med.Add(dataReader.GetString(0));
            }
            dataReader.Close();
            dbManager.CloseConnection(connection);
            return med;
        }


        // Log Function
        public List<AuditLogFetch> Log(int v, string cf, string ef, string sf, string gf, string f, string t, string version)
        {
            List<KMBLNetNbanking.AuditLogFetch> ad = new List<KMBLNetNbanking.AuditLogFetch>();
            if (f == null && t == null || f == "" && t == "")
            {
                f = DateTime.Now.ToString("yyyy-MM-dd");

                t = DateTime.Now.ToString("yyyy-MM-dd");
            }

            var dbManager = new DBManager("DBConnectionweb");
            OracleDataReader reader = null;
            OracleConnection connection = null;

            var parameters = new List<OracleParameter>();
            parameters.Add(dbManager.CreateParameter("v_offset", v, OracleDbType.Int32));
            parameters.Add(dbManager.CreateParameter("v_no_of_records", 100, OracleDbType.Int32));
            parameters.Add(dbManager.CreateParameter("v_crn", cf, OracleDbType.Varchar2));
            parameters.Add(dbManager.CreateParameter("v_event_id", ef, OracleDbType.Int32));
            parameters.Add(dbManager.CreateParameter("v_status_id", sf, OracleDbType.Int32));
            parameters.Add(dbManager.CreateParameter("v_gate_number", gf, OracleDbType.Int32));
            parameters.Add(dbManager.CreateParameter("v_from_date", f + " 00:00:00", OracleDbType.Varchar2));
            parameters.Add(dbManager.CreateParameter("v_to_date", t + " 23:59:59", OracleDbType.Varchar2));
            parameters.Add(dbManager.CreateParameter("v_version", version, OracleDbType.Varchar2));
            parameters.Add(dbManager.CreateParameter("l_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            parameters.Add(dbManager.CreateParameter("v_total_records", OracleDbType.Int32, ParameterDirection.Output));

            OracleCommand objCmd = dbManager.GetCommand("SP_GET_AUDIT_LOG_DETAILS", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            connection.Open();
            reader = objCmd.ExecuteReader();
            while (reader.Read())
            {
                int get_score = 0;
                string failed_at = "";
                if (reader.IsDBNull(13))
                {
                    get_score = 0;
                }
                else
                {
                    get_score = reader.GetInt32(13);
                }
                if (reader.IsDBNull(15))
                {
                    failed_at = "";
                }
                else
                {
                    failed_at = reader.GetString(15);
                }
                var AuditID = reader.GetInt32(0);
                ad.Add(new KMBLNetNbanking.AuditLogFetch
                {
                    AuditID = reader.GetInt32(0),
                    CRN = reader.GetString(1),
                    EventName = reader.GetString(2),
                    EventID = reader.GetInt32(3),
                    SessionID = reader.GetInt32(4),
                    StatusID = reader.GetInt32(5),
                    StatusName = reader.GetString(6),
                    GateNumber = failed_at,
                    ParentTransactionID = reader.GetInt32(8),
                    ChannelName = reader.GetString(9),
                    DeviceDetails = reader.GetString(11),
                    CreatedOn = reader.GetDateTime(12),
                    Score = get_score,
                    IsCompleted = reader.GetInt32(14),
                    Version= reader.IsDBNull(15)?"1": reader.GetString(15)

                });
            }
            reader.Close();
            dbManager.CloseConnection(connection);
            RestOfRecords = objCmd.Parameters["v_total_records"].Value.ToString();
            return ad;
        }

        // MasterEvents Function
        public List<KMBLNetNbanking.MasterEventsFetch> MasterEventsFetchlog()
        {
            var dbManager = new DBManager("DBConnectionweb");

            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            List<KMBLNetNbanking.MasterEventsFetch> med = new List<KMBLNetNbanking.MasterEventsFetch>();
            parameters.Add(dbManager.CreateParameter("event_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var dataReader = dbManager.GetDataReader("SP_GET_EVENTS_MASTER_DETAILS", CommandType.StoredProcedure, parameters.ToArray(), out connection);
            while (dataReader.Read())
            {
                med.Add(new KMBLNetNbanking.MasterEventsFetch
                {
                    EVENT_ID = dataReader.GetInt32(0),
                    EVENT_NAME = dataReader.GetString(1)
                });
            }
            dataReader.Close();
            dbManager.CloseConnection(connection);
            return med;
        }


        public List<KMBLNetNbanking.AuditLogExpandFetch> AuditLogExpandFetch(string audit_id, string event_name)
        {

            var dbManager = new DBManager("DBConnectionweb");
            ImageGallery img;
            OracleConnection connection = null;
            var parameters = new List<OracleParameter>();
            List<KMBLNetNbanking.AuditLogExpandFetch> aed = new List<KMBLNetNbanking.AuditLogExpandFetch>();
            parameters.Add(dbManager.CreateParameter("v_parent_transaction_id", audit_id, OracleDbType.Int64));
            parameters.Add(dbManager.CreateParameter("g_cursor", OracleDbType.RefCursor, ParameterDirection.Output));
            var reader = dbManager.GetDataReader("SP_GET_EVENT_DETAILS", CommandType.StoredProcedure, parameters.ToArray(), out connection);

            while (reader.Read())
            {
                string base64img = "", GetJSONResponse = "", GetStatusName = "";
                int GetAuditID = 0, GetGateName = 0;
                if (event_name == "Registration")
                {
                    img = new ImageGallery(MongoDBConnectionstring, MongoDB_Registration_Databse);
                }
                else
                {
                    img = new ImageGallery(MongoDBConnectionstring, MongoDB_Verification_Databse);
                }

                if (reader.IsDBNull(1))
                {
                    GetAuditID = 0;
                }
                else
                {
                    GetAuditID = reader.GetInt32(0);
                }
                if (reader.IsDBNull(1))
                {
                    base64img = "../../assets/images/empty_person.png";
                }
                else
                {
                    base64img = "data:image/jpeg;base64," + Convert.ToBase64String(img.Get(reader.GetString(1)));
                }
                if (reader.IsDBNull(2))
                {
                    GetGateName = 0;
                }
                else
                {
                    GetGateName = reader.GetInt32(2);
                }
                if (reader.IsDBNull(3))
                {
                    GetJSONResponse = "";
                }
                else
                {
                    GetJSONResponse = reader.GetString(3);
                }
                if (reader.IsDBNull(4))
                {
                    GetStatusName = "";
                }
                else
                {
                    GetStatusName = reader.GetString(4);
                }


                aed.Add(new KMBLNetNbanking.AuditLogExpandFetch
                {
                    AUDIT_ID = GetAuditID,
                    Image_get = base64img,
                    GATE_NAME = GetGateName,
                    JSON_RESPONSE = GetJSONResponse,
                    STATUS_NAME = GetStatusName
                });
            }
            reader.Close();
            dbManager.CloseConnection(connection);
            return aed;


        }

    }
}
