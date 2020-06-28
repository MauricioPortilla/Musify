using System;
using System.Collections.Generic;
using System.Linq;

namespace Musify.Models {
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
        private Artist artist = new Artist();
        public Artist Artist {
            get => artist;
            set => artist = value;
        }
        private List<AccountSong> accountSongs = new List<AccountSong>();
        public List<AccountSong> AccountSongs {
            get => accountSongs;
            set => accountSongs = value;
        }
        private List<Song> likedSongs = new List<Song>();
        public List<Song> LikedSongs {
            get => likedSongs;
            set => likedSongs = value;
        }
        private Subscription subscription;
        public Subscription Subscription {
            get => subscription;
            set => subscription = value;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Account() {
        }

        /// <summary>
        /// Creates a new instance with given data.
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <param name="name">Name</param>
        /// <param name="lastName">Last name</param>
        public Account(string email, string password, string name, string lastName) {
            this.email = email;
            this.password = password;
            this.name = name;
            this.lastName = lastName;
        }

        /// <summary>
        /// Attempts to log in with given credentials.
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Hashed password</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void Login(
            string email, string password, Action<Account> onSuccess, 
            Action<NetworkResponse> onFailure, Action onError
        ) {
            var accountData = new {
                email,
                password
            };
            RestSharpTools.PostAsync<Account>("/auth/login", accountData, JSON_EQUIVALENTS, (response) => {
                Session.AccessToken = response.Json["access_token"];
                Session.Account = response.Model;
                onSuccess(response.Model);
            }, onFailure, () => {
                Console.WriteLine("Exception@Account->Login()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Registers this new account.
        /// </summary>
        /// <param name="isArtist">Register as an artist too</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        /// <param name="artisticName">Artistic name if should be registered as an artist too</param>
        public void Register(bool isArtist, Action onSuccess, Action<NetworkResponse> onFailure, Action onError, string artisticName = null) {
            var accountData = new {
                email,
                password,
                name,
                last_name = lastName,
                is_artist = isArtist,
                artistic_name = artisticName
            };
            RestSharpTools.PostAsync("/auth/register", accountData, (response) => {
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Account->Register()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches artist data attached to this account.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        /// <param name="onFinish">It's executed at the end of every case</param>
        public void FetchArtist(Action onSuccess, Action<NetworkResponse> onFailure, Action onError, Action onFinish = null) {
            RestSharpTools.GetAsync<Artist>("/account/" + accountId + "/artist", null, Artist.JSON_EQUIVALENTS, (response) => {
                this.artist = response.Model;
                onSuccess();
                onFinish?.Invoke();
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
                onFinish?.Invoke();
            }, () => {
                Console.WriteLine("Exception@Account->FetchArtist()");
                onError();
                onFinish?.Invoke();
            });
        }

        /// <summary>
        /// Fetches all account songs of this account.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void FetchAccountSongs(Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<AccountSong>(
                "/account/" + accountId + "/accountsongs", null, 
                AccountSong.JSON_EQUIVALENTS, 
                (response) => {
                    this.accountSongs = response.Model;
                    onSuccess?.Invoke();
                }, (errorResponse) => {
                    onFailure?.Invoke(errorResponse);
                }, () => {
                    Console.WriteLine("Exception@Account->FetchAccountSongs()");
                    onError?.Invoke();
                }
            );
        }

        /// <summary>
        /// Adds new account songs.
        /// </summary>
        /// <param name="fileRoutes">Files to add</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void AddAccountSongs(string[] fileRoutes, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.PostMultimediaAsync<AccountSong>(
                "/account/" + accountId + "/accountsongs", null, fileRoutes,
                AccountSong.JSON_EQUIVALENTS, 
                (response) => {
                    this.accountSongs = this.accountSongs.Union(response.Model).ToList();
                    onSuccess?.Invoke();
                }, onFailure, () => {
                    Console.WriteLine("Exception@Account->AddAccountSongs()");
                    onError?.Invoke();
                }
            );
        }

        /// <summary>
        /// Deletes an account song.
        /// </summary>
        /// <param name="accountSong">Account song to delete</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void DeleteAccountSong(AccountSong accountSong, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.DeleteAsync("/account/" + accountId + "/accountsong/" + accountSong.AccountSongId, null, (response) => {
                accountSongs.Remove(accountSong);
                onSuccess?.Invoke();
            }, onFailure, () => {
                Console.WriteLine("Exception@Account->DeleteAccountSong() -> ");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Likes a song.
        /// </summary>
        /// <param name="song">Song to like</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void LikeSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var data = new {
                account_id = accountId
            };
            RestSharpTools.PostAsync("/song/" + song.SongId + "/songlike", data, (response) => {
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Account->LikeSong()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Dislikes a song.
        /// </summary>
        /// <param name="song">Song to dislike</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void DislikeSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var data = new {
                account_id = accountId
            };
            RestSharpTools.PostAsync("/song/" + song.SongId + "/songdislike", data, (response) => {
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Account->DislikeSong()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Checks if the song was liked before.
        /// </summary>
        /// <param name="song">Song</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void HasLikedSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var data = new {
                account_id = accountId
            };
            RestSharpTools.GetAsync("/song/" + song.SongId + "/songlike", data, (response) => {
                onSuccess();
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
            }, () => {
                Console.WriteLine("Exception@Account->HasLikedSong()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Checks if song was disliked before.
        /// </summary>
        /// <param name="song">Song</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void HasDislikedSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var data = new {
                account_id = accountId
            };
            RestSharpTools.GetAsync("/song/" + song.SongId + "/songdislike", data, (response) => {
                onSuccess();
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
            }, () => {
                Console.WriteLine("Exception@Account->HasDislikedSong()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Unlikes a song.
        /// </summary>
        /// <param name="song">Song to unlike</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void UnlikeSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var data = new {
                account_id = accountId
            };
            RestSharpTools.DeleteAsync("/song/" + song.SongId + "/songlike", data, (response) => {
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Account->LikeSong()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Undislikes a song.
        /// </summary>
        /// <param name="song">Song to undislike</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void UndislikeSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var data = new {
                account_id = accountId
            };
            RestSharpTools.DeleteAsync("/song/" + song.SongId + "/songdislike", data, (response) => {
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Account->DislikeSong()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches an active subscription.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        /// <param name="onFinish">It's executed at the end of every case</param>
        public void FetchSubscription(Action<Subscription> onSuccess, Action<NetworkResponse> onFailure, Action onError, Action onFinish = null) {
            RestSharpTools.GetAsync<Subscription>("/subscription", null, Subscription.JSON_EQUIVALENTS, (response) => {
                onSuccess(response.Model);
                onFinish?.Invoke();
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
                onFinish?.Invoke();
            }, () => {
                Console.WriteLine("Exception@Account->FetchSubscription()");
                onError?.Invoke();
                onFinish?.Invoke();
            });
        }

        /// <summary>
        /// Gets a new subscription.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void Subscribe(Action<Subscription> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.PostAsync<Subscription>("/subscription", null, Subscription.JSON_EQUIVALENTS, (response) => {
                subscription = response.Model;
                onSuccess(response.Model);
            }, onFailure, () => {
                Console.WriteLine("Exception@Account->Subscribe()");
                onError?.Invoke();
            });
        }
    }
}
