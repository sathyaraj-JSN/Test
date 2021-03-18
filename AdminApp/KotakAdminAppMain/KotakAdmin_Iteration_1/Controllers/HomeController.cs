using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Web;
using System.Web.Mvc;
using KMBL.StepupAuthentication.CoreComponents;
using Oracle.ManagedDataAccess.Client;

namespace KotakAdmin_Iteration_1.Controllers
{
    public class NoClientCache : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
    }

    public class HomeController : Controller
    {

        // MongoDB Database ConnectionString & Database Name
        private static string MongoDBConnectionString = ConfigurationManager.AppSettings["MongoDBConnectionstring"];
        private static string MongoDB_Verification_Databse = ConfigurationManager.AppSettings["MongoDB_Verification_Databse"];
        private static string MongoDB_Registration_Databse = ConfigurationManager.AppSettings["MongoDB_Registration_Databse"];

        //Subscription Key, Face Endpoint and Group ID
        private string subscriptionKey = ConfigurationManager.AppSettings["FaceAPIKey"], FaceIDEndpoint = ConfigurationManager.AppSettings["FaceAPIEndPoint"], PersonGroupId = ConfigurationManager.AppSettings["PersonGroupId"];

        // Proxy Server Url & Flag
        private static string ProxyServerUrl = ConfigurationManager.AppSettings["ProxyServerUrl"];
        private static string ProxyFlag = ConfigurationManager.AppSettings["ProxyFlag"];

        private string ConnectionString = ConfigurationManager.AppSettings["OracleConnectionString"];
               

        public ActionResult Dashboard_Failure(string f = null, string t = null)
        {  
            DataAccessGallery dataaccess = new DataAccessGallery();
            List<string> dcv1 = dataaccess.DashboardFailureCount(f, t);
            ViewBag.total_reg = dcv1[0];
            ViewBag.mobile_reg_Android = dcv1[1];
            ViewBag.mobile_reg_IOS = dcv1[2];
            ViewBag.netbanking_reg = dcv1[3];
            ViewBag.branch_reg = dcv1[4];
            ViewBag.total_ver = dcv1[5];
            ViewBag.mobile_ver_Android = dcv1[6];
            ViewBag.mobile_ver_IOS = dcv1[7];
            ViewBag.netbanking_ver = dcv1[8];
            ViewBag.branch_ver = dcv1[9];
            ViewBag.latest_update = dcv1[10];            
            ViewBag.f = (f == null) ? DateTime.Parse(dcv1[11].ToString()).ToString("yyyy-MM-dd") : f;
            ViewBag.t = t;
            List<string> dcv2 = dataaccess.DashboardGateDropOffsFailure(f, t);
            ViewBag.reg_drop_offs = dcv2[0];
            ViewBag.veri_drop_offs = dcv2[1];
            ViewBag.gate_1_failure = dcv2[2];
            ViewBag.gate_2_failure = dcv2[3];
            ViewBag.gate_3_failure = dcv2[4];
            return View();            
        }

        public JsonResult PostDashboardFailure(string f = null, string t = null)
        {
            try
            {
                DataAccessGallery dataaccess = new DataAccessGallery();
                List<string> dcv1 = dataaccess.DashboardFailureCount(f, t);
                List<string> dcv2 = dataaccess.DashboardGateDropOffsFailure(f, t);
                return Json(new { StatusCode = 200, TotalReg = dcv1[0], MobileRegAndroid = dcv1[1], MobileRegIOS = dcv1[2], NetbankingReg = dcv1[3], BranchReg = dcv1[4], TotalVer = dcv1[5], MobileVerAndroid = dcv1[6], MobileVerIOS = dcv1[7], NetbankingVer = dcv1[8], BranchVer = dcv1[9], LatestUpdate = dcv1[10], RegDropOffs = dcv2[0], VeriDropOffs = dcv2[1], Gate1Failure = dcv2[2], Gate2Failure = dcv2[3], Gate3Failure = dcv2[4] });
            }
            catch (Exception e)
            {
                return Json(new { StatusCode = 400, Message = e.Message });
            }
        }

        public ActionResult DashboardSpecificResult(int event_id=0, int resultsid=0, int channelid=0, string f = null, string t = null)        
        {
            ViewBag.Event = (event_id == 1)?"Registration":"Verification";
            ViewBag.Status= (resultsid == 1) ? "Success" : "Failure";
            ViewBag.Chennal= (channelid == 0) ? "" : (channelid == 1) ? "Mobile-Android" : (channelid == 2) ? "Mobile-IOS" : (channelid == 3) ? "Net Banking" : "Branch";
            DataAccessGallery dataaccess = new DataAccessGallery();
            dynamic mymodel = new ExpandoObject();
            mymodel.DashData = dataaccess.DashboardSPecificResult(event_id,resultsid,channelid,f,t);
            return View(mymodel);            
        }

        public JsonResult DoVerification(string ObjectId,string CRN,string Event)
        {
            try
            {
                if (InputValidate(ObjectId) || InputValidate(CRN) || InputValidate(Event))
                {
                    return Json(new { StatusCode = 400, Message = "ObjectId, CRN or Event is empty" });
                }
                else
                {                   
                    
                    byte[] imagear = (Event == "Verification")? new ImageGallery(MongoDBConnectionString, MongoDB_Verification_Databse).Get(ObjectId): new ImageGallery(MongoDBConnectionString, MongoDB_Registration_Databse).Get(ObjectId);// Getting Image from MongoDB
                    AzureFaceAPIs AzureAPI = new AzureFaceAPIs(FaceIDEndpoint, PersonGroupId, subscriptionKey, ProxyServerUrl, ProxyFlag);
                    AzureFaceAPIs result = AzureAPI.DetectFace(imagear);
                    if (result.StatusId == 200)
                    {
                        //result = AzureAPI.IdentifyPerson(result.Message);
                        string personid = new DataAccessGallery().SP_GET_PERSON_ID(CRN);
                        if (personid == null)
                            return Json(new { StatusCode = 200, Message = "Face detection success, But could not find this CRN (" + CRN + ") in registered list", Image = "data:image/jpeg;base64," + Convert.ToBase64String(imagear) });

                        result = AzureAPI.VerifyFace(result.Message, personid);
                        if (result.StatusId == 200 || result.StatusId == 201)
                        {
                            return Json(new { StatusCode = 200, Message = result.Message,Image= "data:image/jpeg;base64," + Convert.ToBase64String(imagear) });
                        }                        
                        else
                        {
                            return Json(new { StatusCode = 200, Message = result.Message, Image = "data:image/jpeg;base64," + Convert.ToBase64String(imagear) });
                        }
                    }
                    else if (result.StatusId == 201 || result.StatusId == 202)
                    {
                        return Json(new { StatusCode = 200, Message = result.Message, Image = "data:image/jpeg;base64," + Convert.ToBase64String(imagear) });
                    }
                    else
                    {
                        return Json(new { StatusCode = 200, Message = result.Message, Image = "data:image/jpeg;base64," + Convert.ToBase64String(imagear) });
                    }
                }
                            
            }
            catch (Exception e)
            {
                return Json(new { StatusCode = 400, Message = e.Message });
            }
        }

        public ActionResult Dashboard(string f = null, string t = null)
        {          
            
            DataAccessGallery dataaccess = new DataAccessGallery();
            List<string> dcv1 = dataaccess.DashboardCountV1(f,t);            
            ViewBag.total_reg = dcv1[0];
            ViewBag.mobile_reg_Android = dcv1[1];
            ViewBag.mobile_reg_IOS = dcv1[2];
            ViewBag.netbanking_reg = dcv1[3];
            ViewBag.branch_reg = dcv1[4];            
            ViewBag.total_ver = dcv1[5];
            ViewBag.mobile_ver_Android = dcv1[6];
            ViewBag.mobile_ver_IOS = dcv1[7];
            ViewBag.netbanking_ver = dcv1[8];
            ViewBag.branch_ver = dcv1[9];
            ViewBag.latest_update = dcv1[10];
            ViewBag.f = (f == null) ? DateTime.Parse(dcv1[11].ToString()).ToString("yyyy-MM-dd") : f;
            ViewBag.t = t;
            List<string> dcv2 = dataaccess.DashboardGateDropOffsFailure(f, t);
            ViewBag.reg_drop_offs = dcv2[0];
            ViewBag.veri_drop_offs = dcv2[1];
            ViewBag.gate_1_failure = dcv2[2];
            ViewBag.gate_2_failure = dcv2[3];
            ViewBag.gate_3_failure = dcv2[4];
            return View();            
        }



        public JsonResult PostDashboardSuccess(string f = null, string t = null)
        {
            try
            {                
                DataAccessGallery dataaccess = new DataAccessGallery();
                List<string> dcv1 = dataaccess.DashboardCountV1(f, t);                                
                List<string> dcv2 = dataaccess.DashboardGateDropOffsFailure(f, t);               
                return Json(new { StatusCode = 200, TotalReg = dcv1[0], MobileRegAndroid = dcv1[1], MobileRegIOS = dcv1[2], NetbankingReg = dcv1[3], BranchReg = dcv1[4], TotalVer = dcv1[5], MobileVerAndroid = dcv1[6], MobileVerIOS = dcv1[7], NetbankingVer = dcv1[8], BranchVer = dcv1[9], LatestUpdate = dcv1[10], RegDropOffs = dcv2[0], VeriDropOffs = dcv2[1], Gate1Failure = dcv2[2], Gate2Failure = dcv2[3], Gate3Failure = dcv2[4]});
            }
            catch (Exception e)
            {
                return Json(new { StatusCode = 400, Message = e.Message });
            }
        }

        // Clear CRN
        public JsonResult Clear_CRN(string crn_no)
        {
            try
            {
                new DataAccessGallery().SP_ClearCRN(crn_no);
                return Json(new { Result = "{\"ClearCrnResult\":\"Your CRN has been deleted\"}" });
            }
            catch (Exception e)
            {
               return Json(new { Result = e.Message });
            }
        }

        // Index ActionResult
        public ActionResult Index(int v = 0, string cf = null, string ef = null, string sf = null, string gf = null, string f = null, string t = null, string version = null)
        {
            ViewBag.v = v;
            ViewBag.cf = cf;
            ViewBag.ef = ef;
            ViewBag.sf = sf;
            ViewBag.gf = gf;
            ViewBag.f = f;
            ViewBag.t = t;
            ViewBag.version = version;

            DataAccessGallery dg = new DataAccessGallery();

            dynamic mymodel = new ExpandoObject();

            mymodel.AuditLogFetch = dg.Log(v, cf, ef, sf, gf, f, t, version);

            mymodel.MasterEventsFetch = dg.MasterEventsFetchlog();

            mymodel.MasterStatusFetch = dg.MasterStatusFetchlog();

            mymodel.MasterGateFetch = dg.MasterGateFetchlog();

            mymodel.Versions = dg.GetVersions();

            ViewBag.RestOfRecords = dg.RestOfRecords;

            return View(mymodel);

        }

        // ClearCRN ActionResult
        public ActionResult Clearcrn()
        {
            return View();
        }

        //AuditLogExpandFetch function
        public JsonResult AuditLogExpandFetch(string audit_id, string event_name)
        {
            try
            {
                DataAccessGallery dg = new DataAccessGallery();
                List<KMBLNetNbanking.AuditLogExpandFetch> aed = dg.AuditLogExpandFetch(audit_id, event_name);
                return new JsonResult()
                {
                    Data = aed,
                    MaxJsonLength = 86753090
                };
            }
            catch (Exception e) //handling runtime errors
            {
                return Json(new { Result = e.Message }); //returning runtime errors as a json
            }
        }

        //TruncateAuditLog function
        public string TruncateAuditLog()
        {
            string TruncateOut = "";
            try
            {
                using (OracleConnection objConn = new OracleConnection(ConnectionString))
                {
                    OracleCommand objCmd = new OracleCommand();

                    objCmd.Connection = objConn;

                    objCmd.CommandText = "SP_Truncate_Audit_Log";

                    objCmd.CommandType = CommandType.StoredProcedure;

                    objCmd.Parameters.Add("v_result", OracleDbType.NVarchar2).Direction = ParameterDirection.Output;

                    // Execute command
                    objConn.Open();
                    OracleDataReader reader = objCmd.ExecuteReader();
                    TruncateOut = objCmd.Parameters["v_result"].Value.ToString();
                    objConn.Close();
                }

                return TruncateOut;
            }
            catch (Exception e) //handling runtime errors
            {
                return e.Message;
            }
        }
        private bool InputValidate(string SrtIn)
        {
            return (SrtIn == string.Empty || SrtIn.Trim().Length == 0 || SrtIn is null) ? true : false;
        }
    }
}