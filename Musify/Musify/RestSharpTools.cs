using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

namespace Musify {
    class RestSharpTools {
        public static async void GetAsync<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents,
            Action<IRestResponse<TModel>> callback
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                if (parameters != null) {
                    request.AddParameter("data", JsonConvert.SerializeObject(parameters));
                }
                var response = await Session.REST_CLIENT.ExecuteGetAsync<TModel>(request, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                if (json["status"] != null && json["status"] == "success") {
                    response.Data = FromJsonToObject<TModel>(response.Content, jsonEquivalents);
                }
                callback(response);
            } catch (Exception) {
                throw;
            }
        }

        public static async void GetAsyncMultiple<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents,
            Action<IRestResponse, List<TModel>> callback
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                if (parameters != null) {
                    request.AddParameter("data", JsonConvert.SerializeObject(parameters));
                }
                var response = await Session.REST_CLIENT.ExecuteGetAsync(request, new CancellationTokenSource().Token);
                List<TModel> objects = new List<TModel>();
                if (response.IsSuccessful) {
                    objects = FromJsonToObjectsList<TModel>(response.Content, jsonEquivalents);
                }
                callback(response, objects);
            } catch (Exception) {
                throw;
            }
        }

        public static async void PostAsync<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents, 
            Action<IRestResponse<TModel>> callback
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                request.AddJsonBody(parameters);
                var response = await Session.REST_CLIENT.ExecutePostAsync<TModel>(request, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                if (json["status"] != null && json["status"] == "success") {
                    response.Data = FromJsonToObject<TModel>(response.Content, jsonEquivalents);
                }
                callback(response);
            } catch (Exception) {
                throw;
            }
        }

        public static async void PostAsync(string resource, dynamic parameters, Action<IRestResponse> callback) {
            try {
                var request = new RestRequest(resource);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(parameters);
                var response = await Session.REST_CLIENT.ExecutePostAsync(request, new CancellationTokenSource().Token);
                callback(response);
            } catch (Exception) {
                throw;
            }
        }

        public static async void PostMultimediaAsync<TModel>(
            string resource, dynamic parameters, string[] fileRoutes, 
            Dictionary<string, string> jsonEquivalents, Action<IRestResponse, List<TModel>> callback
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource);
                request.AddJsonBody(parameters);
                request.AlwaysMultipartFormData = true;
                request.AddHeader("Content-Type", "multipart/form-data");
                int counter = 0;
                foreach (var fileRoute in fileRoutes) {
                    if (File.Exists(fileRoute)) {
                        request.AddFile("files[" + counter + "]", fileRoute);
                    }
                }
                var response = await Session.REST_CLIENT.ExecutePostAsync<TModel>(request, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                List<TModel> objects = new List<TModel>();
                if (json["status"] != null && json["status"] == "success") {
                    objects = FromJsonToObjectsList<TModel>(response.Content, jsonEquivalents);
                }
                callback(response, objects);
            } catch (Exception) {
                throw;
            }
        }

        public static async void PutAsync<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents, 
            Action<IRestResponse<TModel>> callback
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                request.AddJsonBody(parameters);
                var response = await Session.REST_CLIENT.ExecuteAsync<TModel>(request, Method.PUT, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                if (json["status"] != null && json["status"] == "success") {
                    response.Data = FromJsonToObject<TModel>(response.Content, jsonEquivalents);
                }
                callback(response);
            } catch (Exception) {
                throw;
            }
        }

        public static async void DeleteAsync(string resource, dynamic parameters, Action<IRestResponse> callback) {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                request.AddJsonBody(parameters);
                var response = await Session.REST_CLIENT.ExecuteAsync(request, Method.DELETE, new CancellationTokenSource().Token);
                callback(response);
            } catch (Exception) {
                throw;
            }
        }

        public static TModel FromJsonToObject<TModel>(string json, Dictionary<string, string> jsonEquivalents) where TModel : new() {
            dynamic jsonObject = JObject.Parse(json);
            TModel model = new TModel();
            foreach (var jsonEquivalent in jsonEquivalents) {
                PropertyInfo property = model.GetType().GetProperty(jsonEquivalent.Value);
                var finalValue = Convert.ChangeType(jsonObject["data"][jsonEquivalent.Key], property.PropertyType);
                property.SetValue(model, finalValue);
            }
            return model;
        }

        public static List<TModel> FromJsonToObjectsList<TModel>(string json, Dictionary<string, string> jsonEquivalents) where TModel : new() {
            dynamic jsonObject = JObject.Parse(json);
            List<TModel> models = new List<TModel>();
            foreach (var dataObject in jsonObject["data"]) {
                TModel model = new TModel();
                foreach (var jsonEquivalent in jsonEquivalents) {
                    PropertyInfo property = model.GetType().GetProperty(jsonEquivalent.Value);
                    var finalValue = Convert.ChangeType(dataObject[jsonEquivalent.Key], property.PropertyType);
                    property.SetValue(model, finalValue);
                }
                models.Add(model);
            }
            return models;
        }
    }
}
