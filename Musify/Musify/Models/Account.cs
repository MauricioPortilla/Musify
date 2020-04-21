using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Musify.Models;
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
        private DateTime creationDate;
        public DateTime CreationDate {
            get => creationDate;
            set => creationDate = value;
        }
        private List<AccountSong> accountSongs;
        public List<AccountSong> AccountSongs {
            get => accountSongs;
            set => accountSongs = value;
        }

        public Account() {
        }

        public Account(string email, string password, string name, string lastName) {
            this.email = email;
            this.password = password;
            this.name = name;
            this.lastName = lastName;
        }

        public static void Login(string email, string password, Action<Account> onSuccess, Action onFailure) {
            var accountData = new {
                email,
                password
            };
            try {
                RestSharpTools.PostAsync<Account>("/auth/login", accountData, JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        Session.AccessToken = ((dynamic) JObject.Parse(response.Content))["access_token"];
                        onSuccess(response.Data);
                    } else {
                        onFailure();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->Login() -> " + exception.Message);
                onFailure();
            }
            
        }

        public void Register(bool isArtist, Action onSuccess, Action onFailure, string artisticName = null) {
            var accountData = new {
                email,
                password,
                name,
                last_name = lastName,
                is_artist = isArtist,
                artistic_name = artisticName
            };
            try {
                RestSharpTools.PostAsync("/auth/register", accountData, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                    } else {
                        onFailure();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->Register() -> " + exception.Message);
                onFailure();
            }
            
        }

        public void FetchAccountSongs(Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<AccountSong>("/account/" + accountId + "/accountsongs", null, AccountSong.JSON_EQUIVALENTS, (response, accountSongs) => {
                    if (response.IsSuccessful) {
                        this.accountSongs = accountSongs;
                        onSuccess?.Invoke();
                        return;
                    }
                    onFailure?.Invoke();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->FetchAccountSongs() -> " + exception.Message);
                onFailure();
            }
        }

        public void AddAccountSongs(string[] fileRoutes, Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.PostMultimediaAsync<AccountSong>(
                "/account/" + accountId + "/accountsongs", null, fileRoutes,
                AccountSong.JSON_EQUIVALENTS, (response, accountSongs) => {
                    if (response.IsSuccessful) {
                        this.accountSongs = this.accountSongs.Union(accountSongs).ToList();
                        onSuccess?.Invoke();
                        return;
                    }
                    onFailure?.Invoke();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->AddAccountSongs() -> " + exception.Message);
                onFailure();
            }
        }

        public void DeleteAccountSong(AccountSong accountSong, Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.DeleteAsync("/account/" + accountId + "/accountsong/" + accountSong.AccountSongId, null, (response) => {
                    if (response.IsSuccessful) {
                        accountSongs.Remove(accountSong);
                        onSuccess?.Invoke();
                        return;
                    }
                    onFailure?.Invoke();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->DeleteAccountSong() -> " + exception.Message);
                onFailure();
            }
        }
    }
}
