using Newtonsoft.Json.Linq;

namespace Musify {
    public class NetworkResponse {
        private readonly string status;
        public string Status {
            get => status;
        }
        private readonly string message;
        public string Message {
            get => message;
        }
        private readonly dynamic data;
        public dynamic Data {
            get => data;
        }
        private readonly dynamic json;
        public dynamic Json {
            get => json;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="json">Json</param>
        public NetworkResponse(JObject json) {
            dynamic _json = json;
            status = _json["status"];
            message = _json["message"] ?? null;
            data = _json["data"];
            this.json = _json;
        }
    }

    public class NetworkResponse<TModel> where TModel : new() {
        private readonly string status;
        public string Status {
            get => status;
        }
        private readonly string message;
        public string Message {
            get => message;
        }
        private readonly dynamic data;
        public dynamic Data {
            get => data;
        }
        private readonly dynamic json;
        public dynamic Json {
            get => json;
        }
        private TModel model;
        public TModel Model {
            get => model;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="json">Json</param>
        /// <param name="model">Model to associate</param>
        public NetworkResponse(JObject json, TModel model) {
            dynamic _json = json;
            status = _json["status"];
            message = _json["message"] ?? null;
            data = _json["data"];
            this.model = model;
            this.json = _json;
        }
    }
}
