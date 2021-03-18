using RestSharp;
using Newtonsoft.Json;
using System.Net;

namespace KMBL.StepupAuthentication.CoreComponents
{
    public class AzureFaceAPIs
    {
        private string FaceIDEndpoint, PersonGroupId, subscriptionKey, ProxyServerUrl, ProxyFlag;

        // properties.
        public int StatusId { get; set; }
        public string Message { get; set; }
        public int Score { get; set; }

        //constructor
        public AzureFaceAPIs() { }

        public AzureFaceAPIs(string FaceIDEndpoint,string PersonGroupId, string subscriptionKey,string ProxyServerUrl,string ProxyFlag)
        {
            this.FaceIDEndpoint= FaceIDEndpoint;
            this.PersonGroupId = PersonGroupId;
            this.subscriptionKey = subscriptionKey;
            this.ProxyServerUrl = ProxyServerUrl;
            this.ProxyFlag = ProxyFlag;
        }

        // Add a New Person
        /*StatusId 200 for success, StatusId 400 for Fail  and StatusId 500 for Not able to access*/
        public AzureFaceAPIs AddPerson(string name)
        {
            var client = new RestClient(FaceIDEndpoint + "/face/v1.0/persongroups/" + PersonGroupId + "/persons");
            if(ProxyFlag=="1")
                client.Proxy = new WebProxy(ProxyServerUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("ocp-apim-subscription-key", subscriptionKey);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"name\": \"" + name + "\"\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if((int)response.StatusCode == 200)
            {
            dynamic result = JsonConvert.DeserializeObject(response.Content);
            return new AzureFaceAPIs { StatusId = 200, Message = result.personId };
            }
            else if((int)response.StatusCode == 400 || (int)response.StatusCode == 401 || (int)response.StatusCode == 403 || (int)response.StatusCode == 404 || (int)response.StatusCode == 409 || (int)response.StatusCode == 415 || (int)response.StatusCode == 429)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 400, Message = result.error.message };
            }       
            else
                return new AzureFaceAPIs { StatusId=500, Message= "Unable to access the URL" };             
        }


        // AddFace Function
        /*StatusId 200 for success, StatusId 400 for Fail  and StatusId 500 for Not able to access*/
        public AzureFaceAPIs AddFace(byte[] imageBytes, string PersonID)
        {
            var client = new RestClient(FaceIDEndpoint + "/face/v1.0/persongroups/" + PersonGroupId + "/persons/" + PersonID + "/persistedFaces");
            if (ProxyFlag == "1")
                client.Proxy = new WebProxy(ProxyServerUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("ocp-apim-subscription-key", subscriptionKey);
            request.AddHeader("content-type", "application/octet-stream");
            request.AddParameter("undefined", imageBytes, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if ((int)response.StatusCode == 200)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 200, Message = result.persistedFaceId };
            }
            else if ((int)response.StatusCode == 400 || (int)response.StatusCode == 401 || (int)response.StatusCode == 403 || (int)response.StatusCode == 404 || (int)response.StatusCode == 408 || (int)response.StatusCode == 409 || (int)response.StatusCode == 415 || (int)response.StatusCode == 429)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 400, Message = result.error.message };
            }
            else
                return new AzureFaceAPIs { StatusId = 500, Message = "Unable to access the URL" };
        }



        // TrainPersonGroup Function
        /*StatusId 200 for success, StatusId 400 for Fail  and StatusId 500 for Not able to access*/
        public AzureFaceAPIs TrainPersonGroup()
        {
            var client = new RestClient(FaceIDEndpoint + "/face/v1.0/persongroups/" + PersonGroupId + "/train");
            if (ProxyFlag == "1")
                client.Proxy = new WebProxy(ProxyServerUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("ocp-apim-subscription-key", subscriptionKey);
            request.AddHeader("content-type", "application/json");

            IRestResponse response = client.Execute(request);
            if ((int)response.StatusCode == 202)          
                return new AzureFaceAPIs { StatusId = 200, Message = "Person Group Training Success" };
            else if ((int)response.StatusCode == 401 || (int)response.StatusCode == 403 || (int)response.StatusCode == 404 || (int)response.StatusCode == 409 || (int)response.StatusCode == 429)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 400, Message = result.error.message };
            }
            else
                return new AzureFaceAPIs { StatusId = 500, Message = "Unable to access the URL" };
        }



        // Face Detection Function
        /*StatusId 200 for success, 201 for No Face Found, 202 for Multiple Face Found, 400 for Fail  and 500 for Not able to access*/
        public AzureFaceAPIs DetectFace(byte[] imageBytes)
        {
            var client = new RestClient(FaceIDEndpoint + "/face/v1.0/detect?returnFaceId=true&returnFaceLandmarks=false&recognitionModel=recognition_01&returnRecognitionModel=false");
            if (ProxyFlag == "1")
                client.Proxy = new WebProxy(ProxyServerUrl);
            var request = new RestRequest(Method.POST);
            request.AddParameter("undefined", imageBytes, ParameterType.RequestBody);
            request.AddHeader("ocp-apim-subscription-key", subscriptionKey);
            request.AddHeader("content-type", "application/octet-stream");

            IRestResponse response = client.Execute(request);
            if ((int)response.StatusCode == 200)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);                
                if(result.Count == 1)
                    return new AzureFaceAPIs { StatusId = 200, Message = result[0].faceId };
                else if (result.Count == 0)
                    return new AzureFaceAPIs { StatusId = 201, Message = "No Face Fond" };
                else
                    return new AzureFaceAPIs { StatusId = 202, Message = "Multiple Face Fond" };
            }
            else if ((int)response.StatusCode == 400 || (int)response.StatusCode == 401 || (int)response.StatusCode == 403 || (int)response.StatusCode == 408 || (int)response.StatusCode == 415 || (int)response.StatusCode == 429)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 400, Message = result.error.message };
            }
            else
                return new AzureFaceAPIs { StatusId = 500, Message = "Unable to access the URL" };
        }

        public AzureFaceAPIs VerifyFace(string FaceID, string PersonId)
        {
            var client = new RestClient(FaceIDEndpoint + "/face/v1.0/verify");
            if (ProxyFlag == "1")
                client.Proxy = new WebProxy(ProxyServerUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("ocp-apim-subscription-key", subscriptionKey);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\n    \"faceId\": \"" + FaceID + "\",\n    \"personId\": \"" + PersonId + "\",\n    \"personGroupId\": \"" + PersonGroupId + "\"\n}\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if ((int)response.StatusCode == 200)  //{"isIdentical": true,"confidence": 0.9}
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                Score = result.confidence;
                if (result.isIdentical == true)
                    return new AzureFaceAPIs { StatusId = 200, Message = "Verification Success" }; 
                else
                    return new AzureFaceAPIs { StatusId = 201, Message = "Verification Failed" };
            }
            else if ((int)response.StatusCode == 400 || (int)response.StatusCode == 401 || (int)response.StatusCode == 403 || (int)response.StatusCode == 404 || (int)response.StatusCode == 415 || (int)response.StatusCode == 429)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 400, Message = result.error.message };
            }
            else
                return new AzureFaceAPIs { StatusId = 500, Message = "Unable to access the URL" };
        }



        // Face Detection Function
        /*StatusId 200 for success, 201 for Unauthorized Person, 400 for Fail  and 500 for Not able to access*/
        public AzureFaceAPIs IdentifyPerson(string FaceID)
        {
            var client = new RestClient(FaceIDEndpoint + "/face/v1.0/identify");
            if (ProxyFlag == "1")
                client.Proxy = new WebProxy(ProxyServerUrl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("ocp-apim-subscription-key", subscriptionKey);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\r\n    \"personGroupId\": \"" + PersonGroupId + "\",\r\n    \"faceIds\": [\r\n        \"" + FaceID + "\"\r\n    ],\r\n    \"maxNumOfCandidatesReturned\": 1,\r\n    \"confidenceThreshold\": 0.6\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if ((int)response.StatusCode == 200)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                if(result.Count==1 && result[0].candidates.Count==1)
                    return new AzureFaceAPIs { StatusId = 200, Message = result[0].candidates[0].personId, Score = (int)(result[0].candidates[0].confidence *100) };                
                else
                    return new AzureFaceAPIs { StatusId = 201, Message = "Unauthorized Person" };
            }
            else if ((int)response.StatusCode == 400 || (int)response.StatusCode == 401 || (int)response.StatusCode == 403 || (int)response.StatusCode == 409 || (int)response.StatusCode == 415 || (int)response.StatusCode == 429)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 400, Message = result.error.message };
            }
            else
                return new AzureFaceAPIs { StatusId = 500, Message = "Unable to access the URL" };
        }



        // Get a Person Info using PersonId
        /*StatusId 200 for success, StatusId 400 for Fail  and StatusId 500 for Not able to access*/
        public AzureFaceAPIs GetPersonInfo(string personId)
        {
            var client = new RestClient(FaceIDEndpoint + "/face/v1.0/persongroups/" + PersonGroupId + "/persons/" + personId);
            if (ProxyFlag == "1")
                client.Proxy = new WebProxy(ProxyServerUrl);
            var request = new RestRequest(Method.GET);
            request.AddHeader("ocp-apim-subscription-key", subscriptionKey);

            IRestResponse response = client.Execute(request);
            if ((int)response.StatusCode == 200)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 200, Message = result.name };
            }
            else if ((int)response.StatusCode == 401 || (int)response.StatusCode == 403 || (int)response.StatusCode == 404 || (int)response.StatusCode == 409 || (int)response.StatusCode == 429)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 400, Message = result.error.message };
            }
            else
                return new AzureFaceAPIs { StatusId = 500, Message = "Unable to access the URL" };
        }



        // Delete a Person using PersonId
        /*StatusId 200 for success, StatusId 400 for Fail  and StatusId 500 for Not able to access*/
        public AzureFaceAPIs DeletePerson(string personId)
        {
            var client = new RestClient(FaceIDEndpoint + "/face/v1.0/persongroups/" + PersonGroupId + "/persons/" + personId);
            if (ProxyFlag == "1")
                client.Proxy = new WebProxy(ProxyServerUrl);
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("ocp-apim-subscription-key", subscriptionKey);

            IRestResponse response = client.Execute(request);
            if ((int)response.StatusCode == 200)
                return new AzureFaceAPIs { StatusId = 200, Message = "Person Group Deleted Successfully" };
            else if ((int)response.StatusCode == 401 || (int)response.StatusCode == 403 || (int)response.StatusCode == 404 || (int)response.StatusCode == 409 || (int)response.StatusCode == 429)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                return new AzureFaceAPIs { StatusId = 400, Message = result.error.message };
            }
            else
                return new AzureFaceAPIs { StatusId = 500, Message = "Unable to access the URL" };
        }
    }
}
