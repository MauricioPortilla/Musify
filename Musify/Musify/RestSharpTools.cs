using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

namespace Musify {
    static class RestSharpTools {
        /// <summary>
        /// Sends a GET request.
        /// </summary>
        /// <param name="resource">API Resource</param>
        /// <param name="parameters">Query parameters</param>
        /// <param name="onSuccess">On response code is success</param>
        /// <param name="onFailure">On response code is failure</param>
        /// <param name="onError">On raised exception</param>
        public static async void GetAsync(
            string resource, dynamic parameters, 
            Action<NetworkResponse> onSuccess,
            Action<NetworkResponse> onFailure,
            Action onError
        ) {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                if (parameters != null) {
                    request.AddParameter("data", JsonConvert.SerializeObject(parameters));
                }
                request.AddHeader("Authorization", Session.AccessToken ?? "");
                request.AddHeader("Accept-Language", "es");
                var response = await Session.REST_CLIENT.ExecuteGetAsync(request, new CancellationTokenSource().Token);
                if (response.IsSuccessful) {
                    onSuccess(new NetworkResponse(JObject.Parse(response.Content)));
                } else {
                    onFailure(new NetworkResponse(JObject.Parse(response.Content)));
                }
            } catch (Exception) {
                onError();
            }
        }

        /// <summary>
        /// Sends a GET request to receive a new model with fetched data.
        /// </summary>
        /// <typeparam name="TModel">Model to create with fetched data</typeparam>
        /// <param name="resource">API Resource</param>
        /// <param name="parameters">Query parameters</param>
        /// <param name="jsonEquivalents">JSON attributes equivalent to Model attributes</param>
        /// <param name="onSuccess">On response code is success</param>
        /// <param name="onFailure">On response code is failure</param>
        /// <param name="onError">On raised exception</param>
        public static async void GetAsync<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents,
            Action<NetworkResponse<TModel>> onSuccess,
            Action<NetworkResponse> onFailure,
            Action onError
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                if (parameters != null) {
                    request.AddParameter("data", JsonConvert.SerializeObject(parameters));
                }
                request.AddHeader("Authorization", Session.AccessToken ?? "");
                request.AddHeader("Accept-Language", "es");
                var response = await Session.REST_CLIENT.ExecuteGetAsync<TModel>(request, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                if (json["status"] != null && json["status"] == "success") {
                    response.Data = FromJsonToObject<TModel>(response.Content, jsonEquivalents);
                    if (response.IsSuccessful) {
                        onSuccess(new NetworkResponse<TModel>(json, response.Data));
                    } else {
                        onFailure(new NetworkResponse(json));
                    }
                } else {
                    onFailure(new NetworkResponse(json));
                }
            } catch (Exception) {
                onError();
            }
        }

        /// <summary>
        /// Sends a GET request to receive a new model collection with fetched data.
        /// </summary>
        /// <typeparam name="TModel">Model to create with fetched data</typeparam>
        /// <param name="resource">API Resource</param>
        /// <param name="parameters">Query parameters</param>
        /// <param name="jsonEquivalents">JSON attributes equivalent to Model attributes</param>
        /// <param name="onSuccess">On response code is success</param>
        /// <param name="onFailure">On response code is failure</param>
        /// <param name="onError">On raised exception</param>
        public static async void GetAsyncMultiple<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents,
            Action<NetworkResponse<List<TModel>>> onSuccess,
            Action<NetworkResponse> onFailure,
            Action onError
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                if (parameters != null) {
                    request.AddParameter("data", JsonConvert.SerializeObject(parameters));
                }
                request.AddHeader("Authorization", Session.AccessToken ?? "");
                request.AddHeader("Accept-Language", "es");
                var response = await Session.REST_CLIENT.ExecuteGetAsync(request, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                List<TModel> objects;
                if (response.IsSuccessful) {
                    objects = FromJsonToObjectsList<TModel>(response.Content, jsonEquivalents);
                    onSuccess(new NetworkResponse<List<TModel>>(json, objects));
                } else {
                    onFailure(new NetworkResponse(json));
                }
            } catch (Exception) {
                onError();
            }
        }

        /// <summary>
        /// Sends a POST request to receive a new model with fetched data.
        /// </summary>
        /// <typeparam name="TModel">Model to create with fetched data</typeparam>
        /// <param name="resource">API Resource</param>
        /// <param name="parameters">JSON body struct</param>
        /// <param name="jsonEquivalents">JSON attributes equivalent to Model attributes</param>
        /// <param name="onSuccess">On response code is success</param>
        /// <param name="onFailure">On response code is failure</param>
        /// <param name="onError">On raised exception</param>
        public static async void PostAsync<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents,
            Action<NetworkResponse<TModel>> onSuccess,
            Action<NetworkResponse> onFailure,
            Action onError
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                request.AddJsonBody(parameters);
                request.AddHeader("Authorization", Session.AccessToken ?? "");
                request.AddHeader("Accept-Language", "es");
                var response = await Session.REST_CLIENT.ExecutePostAsync<TModel>(request, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                if (json["status"] != null && json["status"] == "success") {
                    response.Data = FromJsonToObject<TModel>(response.Content, jsonEquivalents);
                    if (response.IsSuccessful) {
                        onSuccess(new NetworkResponse<TModel>(json, response.Data));
                    } else {
                        onFailure(new NetworkResponse(json));
                    }
                } else {
                    onFailure(new NetworkResponse(json));
                }
            } catch (Exception) {
                onError();
            }
        }

        /// <summary>
        /// Sends a POST request.
        /// </summary>
        /// <param name="resource">API Resource</param>
        /// <param name="parameters">JSON body struct</param>
        /// <param name="onSuccess">On response code is success</param>
        /// <param name="onFailure">On response code is failure</param>
        /// <param name="onError">On raised exception</param>
        public static async void PostAsync(
            string resource, dynamic parameters,
            Action<NetworkResponse> onSuccess,
            Action<NetworkResponse> onFailure,
            Action onError
        ) {
            try {
                var request = new RestRequest(resource);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(parameters);
                request.AddHeader("Authorization", Session.AccessToken ?? "");
                request.AddHeader("Accept-Language", "es");
                var response = await Session.REST_CLIENT.ExecutePostAsync(request, new CancellationTokenSource().Token);
                if (response.IsSuccessful) {
                    onSuccess(new NetworkResponse(JObject.Parse(response.Content)));
                } else {
                    onFailure(new NetworkResponse(JObject.Parse(response.Content)));
                }
            } catch (Exception) {
                onError();
            }
        }

        /// <summary>
        /// Sends a POST request with multimedia to receive a new model with fetched data.
        /// </summary>
        /// <typeparam name="TModel">Model to create with fetched data</typeparam>
        /// <param name="resource">API Resource</param>
        /// <param name="parameters">JSON body struct</param>
        /// <param name="fileRoutes">File routes to include in request</param>
        /// <param name="jsonEquivalents">JSON attributes equivalent to Model attributes</param>
        /// <param name="onSuccess">On response code is success</param>
        /// <param name="onFailure">On response code is failure</param>
        /// <param name="onError">On raised exception</param>
        public static async void PostMultimediaAsync<TModel>(
            string resource, dynamic parameters, string[] fileRoutes, 
            Dictionary<string, string> jsonEquivalents, 
            Action<NetworkResponse<List<TModel>>> onSuccess,
            Action<NetworkResponse> onFailure,
            Action onError
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource);
                request.AddJsonBody(parameters);
                request.AlwaysMultipartFormData = true;
                request.AddHeader("Content-Type", "multipart/form-data");
                request.AddHeader("Authorization", Session.AccessToken ?? "");
                request.AddHeader("Accept-Language", "es");
                int counter = 0;
                foreach (var fileRoute in fileRoutes) {
                    if (File.Exists(fileRoute)) {
                        request.AddFile(counter.ToString(), fileRoute);
                        counter++;
                    }
                }
                var response = await Session.REST_CLIENT.ExecutePostAsync<TModel>(request, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                List<TModel> objects;
                if (response.IsSuccessful) {
                    objects = FromJsonToObjectsList<TModel>(response.Content, jsonEquivalents);
                    onSuccess(new NetworkResponse<List<TModel>>(json, objects));
                } else {
                    onFailure(new NetworkResponse(json));
                }
            } catch (Exception) {
                onError();
            }
        }

        /// <summary>
        /// Sends a PUT request to receive a new model with fetched data.
        /// </summary>
        /// <typeparam name="TModel">Model to create with fetched data</typeparam>
        /// <param name="resource">API Resource</param>
        /// <param name="parameters">JSON body struct</param>
        /// <param name="jsonEquivalents">JSON attributes equivalent to Model attributes</param>
        /// <param name="onSuccess">On response code is success</param>
        /// <param name="onFailure">On response code is failure</param>
        /// <param name="onError">On raised exception</param>
        public static async void PutAsync<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents,
            Action<NetworkResponse<TModel>> onSuccess,
            Action<NetworkResponse> onFailure,
            Action onError
        ) where TModel : new() {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                request.AddJsonBody(parameters);
                request.AddHeader("Authorization", Session.AccessToken ?? "");
                request.AddHeader("Accept-Language", "es");
                var response = await Session.REST_CLIENT.ExecuteAsync<TModel>(request, Method.PUT, new CancellationTokenSource().Token);
                dynamic json = JObject.Parse(response.Content);
                if (json["status"] != null && json["status"] == "success") {
                    response.Data = FromJsonToObject<TModel>(response.Content, jsonEquivalents);
                    if (response.IsSuccessful) {
                        onSuccess(new NetworkResponse<TModel>(json, response.Data));
                    } else {
                        onFailure(new NetworkResponse(json));
                    }
                } else {
                    onFailure(new NetworkResponse(json));
                }
            } catch (Exception) {
                onError();
            }
        }

        /// <summary>
        /// Sends a DELETE request.
        /// </summary>
        /// <param name="resource">API Resource</param>
        /// <param name="parameters">JSON body struct</param>
        /// <param name="onSuccess">On response code is success</param>
        /// <param name="onFailure">On response code is failure</param>
        /// <param name="onError">On raised exception</param>
        public static async void DeleteAsync(
            string resource, dynamic parameters,
            Action<NetworkResponse> onSuccess,
            Action<NetworkResponse> onFailure,
            Action onError
        ) {
            try {
                var request = new RestRequest(resource, DataFormat.Json);
                request.AddJsonBody(parameters);
                request.AddHeader("Authorization", Session.AccessToken ?? "");
                request.AddHeader("Accept-Language", "es");
                var response = await Session.REST_CLIENT.ExecuteAsync(request, Method.DELETE, new CancellationTokenSource().Token);
                if (response.IsSuccessful) {
                    onSuccess(new NetworkResponse(JObject.Parse(response.Content)));
                } else {
                    onFailure(new NetworkResponse(JObject.Parse(response.Content)));
                }
            } catch (Exception) {
                onError();
            }
        }

        /// <summary>
        /// Creates a new model with given JSON string.
        /// </summary>
        /// <typeparam name="TModel">Model to create</typeparam>
        /// <param name="json">JSON string</param>
        /// <param name="jsonEquivalents">JSON attributes equivalent to Model attributes</param>
        /// <returns>New model instance</returns>
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

        /// <summary>
        /// Creates a collection of models with given JSON string.
        /// </summary>
        /// <typeparam name="TModel">Model to create</typeparam>
        /// <param name="json">JSON string</param>
        /// <param name="jsonEquivalents">JSON attributes equivalent to Model attributes</param>
        /// <returns>List of model instances</returns>
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
