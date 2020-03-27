using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Reflection;

namespace Musify {
    class RestSharpTools {
        public static async void PostAsync<TModel>(
            string resource, dynamic parameters, Dictionary<string, string> jsonEquivalents, 
            Action<IRestResponse<TModel>> callback
        ) where TModel : new() {
            var request = new RestRequest(resource);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(parameters);
            var response = await Session.REST_CLIENT.ExecutePostAsync<TModel>(request, new CancellationTokenSource().Token);

            response.Data = FromJsonToObject<TModel>(response.Content, jsonEquivalents);
            callback(response);
        }

        public static async void PostAsync(string resource, dynamic parameters, Action<IRestResponse> callback) {
            var request = new RestRequest(resource);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(parameters);
            var response = await Session.REST_CLIENT.ExecutePostAsync(request, new CancellationTokenSource().Token);
            callback(response);
        }

        public static TModel FromJsonToObject<TModel>(string json, Dictionary<string, string> jsonEquivalents) where TModel : new() {
            dynamic jsonObject = JObject.Parse(json);
            TModel model = new TModel();
            foreach (var jsonEquivalent in jsonEquivalents) {
                PropertyInfo property = model.GetType().GetProperty(jsonEquivalent.Value);
                var finalValue = Convert.ChangeType(jsonObject[jsonEquivalent.Key], property.PropertyType);
                property.SetValue(model, finalValue);
            }
            return model;
        }
    }
}
