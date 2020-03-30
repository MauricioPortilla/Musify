using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Musify {
    public class Account {

        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static readonly Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "account_id", "AccountId" },
            { "email", "Email" },
            { "password", "Password" },
            { "name", "Name" },
            { "last_name", "LastName" },
            { "second_last_name", "SecondLastName" },
            { "creation_date", "CreationDate" }
        };

        private int accountId;
        public int AccountId {
            get => accountId;
            set => accountId = value;
        }
        private string email;
        public string Email {
            get => email;
            set => email = value;
        }
        private string password;
        public string Password {
            get => password;
            set => password = value;
        }
        private string name;
        public string Name {
            get => name;
            set => name = value;
        }
        private string lastName;
        public string LastName {
            get => lastName;
            set => lastName = value;
        }
        private string secondLastName;
        public string SecondLastName {
            get => secondLastName;
            set => secondLastName = value;
        }
        private DateTime creationDate;
        public DateTime CreationDate {
            get => creationDate;
            set => creationDate = value;
        }

        public Account() {
        }

        public Account(string email, string password, string name, string lastName, string secondLastName) {
            this.email = email;
            this.password = password;
            this.name = name;
            this.lastName = lastName;
            this.secondLastName = secondLastName;
        }

        public static void Login(string email, string password, Action<Account> onSuccess, Action onFailure) {
            var accountData = new {
                request_type = "login",
                email,
                password
            };
            RestSharpTools.PostAsync<Account>("/Account", accountData, JSON_EQUIVALENTS, (response) => {
                if (response.IsSuccessful) {
                    onSuccess(response.Data);
                } else {
                    onFailure();
                }
            });
        }

        public void Register(bool isArtist, Action<dynamic> onSuccess, Action<dynamic> onFailure, string artisticName = null) {
            var accountData = new {
                request_type = "register",
                email,
                password,
                name,
                last_name = lastName,
                second_last_name = secondLastName,
                is_artist = isArtist,
                artistic_name = artisticName
            };
            RestSharpTools.PostAsync("/Account", accountData, (response) => {
                if (response.IsSuccessful) {
                    onSuccess(JObject.Parse(response.Content));
                } else {
                    onFailure(JObject.Parse(response.Content));
                }
            });
        }
    }
}
