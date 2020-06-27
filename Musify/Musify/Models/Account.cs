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

        /// <summary>
        /// Registers this new account.
        /// </summary>
        /// <param name="isArtist">Register as an artist too</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="artisticName">Artistic name if should be registered as an artist too</param>
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

        /// <summary>
        /// Fetches artist data attached to this account.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void FetchArtist(Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsync<Artist>("/account/" + accountId + "/artist", null, Artist.JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        this.artist = response.Data;
                    }
                    onSuccess();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->FetchArtist() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        /// <summary>
        /// Fetches all account songs of this account.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
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

        /// <summary>
        /// Adds new account songs.
        /// </summary>
        /// <param name="fileRoutes">Files to add</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
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

        /// <summary>
        /// Deletes an account song.
        /// </summary>
        /// <param name="accountSong">Account song to delete</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
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

        /// <summary>
        /// Likes a song.
        /// </summary>
        /// <param name="song">Song to like</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void LikeSong(Song song, Action onSuccess, Action onFailure) {
            try {
                var data = new {
                    account_id = accountId
                };
                RestSharpTools.PostAsync("/song/" + song.SongId + "/songlike", data, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                        return;
                    }
                    onFailure();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->LikeSong() -> " + exception.Message);
                onFailure();
            }
        }

        /// <summary>
        /// Dislikes a song.
        /// </summary>
        /// <param name="song">Song to dislike</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void DislikeSong(Song song, Action onSuccess, Action onFailure) {
            try {
                var data = new {
                    account_id = accountId
                };
                RestSharpTools.PostAsync("/song/" + song.SongId + "/songdislike", data, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                        return;
                    }
                    onFailure();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->DislikeSong() -> " + exception.Message);
                onFailure();
            }
        }

        /// <summary>
        /// Checks if the song was liked before.
        /// </summary>
        /// <param name="song">Song</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void HasLikedSong(Song song, Action onSuccess, Action onFailure) {
            try {
                var data = new {
                    account_id = accountId
                };
                RestSharpTools.GetAsync("/song/" + song.SongId + "/songlike", data, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                        return;
                    }
                    onFailure();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->HasLikedSong() -> " + exception.Message);
                onFailure();
            }
        }

        /// <summary>
        /// Checks if song was disliked before.
        /// </summary>
        /// <param name="song">Song</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void HasDislikedSong(Song song, Action onSuccess, Action onFailure) {
            try {
                var data = new {
                    account_id = accountId
                };
                RestSharpTools.GetAsync("/song/" + song.SongId + "/songdislike", data, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                        return;
                    }
                    onFailure();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->HasDislikedSong() -> " + exception.Message);
                onFailure();
            }
        }

        /// <summary>
        /// Unlikes a song.
        /// </summary>
        /// <param name="song">Song to unlike</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void UnlikeSong(Song song, Action onSuccess, Action onFailure) {
            try {
                var data = new {
                    account_id = accountId
                };
                RestSharpTools.DeleteAsync("/song/" + song.SongId + "/songlike", data, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                        return;
                    }
                    onFailure();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->LikeSong() -> " + exception.Message);
                onFailure();
            }
        }

        /// <summary>
        /// Undislikes a song.
        /// </summary>
        /// <param name="song">Song to undislike</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void UndislikeSong(Song song, Action onSuccess, Action onFailure) {
            try {
                var data = new {
                    account_id = accountId
                };
                RestSharpTools.DeleteAsync("/song/" + song.SongId + "/songdislike", data, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                        return;
                    }
                    onFailure();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->DislikeSong() -> " + exception.Message);
                onFailure();
            }
        }

        /// <summary>
        /// Fetches an active subscription.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void FetchSubscription(Action<Subscription> onSuccess, Action onFailure, Action onError) {
            try {
                RestSharpTools.GetAsync<Subscription>("/subscription", null, Subscription.JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess(response.Data);
                        return;
                    }
                    onFailure?.Invoke();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->FetchSubscription() -> " + exception.Message);
                onError?.Invoke();
            }
        }

        /// <summary>
        /// Gets a new subscription.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void Subscribe(Action<Subscription> onSuccess, Action onFailure, Action onError) {
            try {
                RestSharpTools.PostAsync<Subscription>("/subscription", null, Subscription.JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        subscription = response.Data;
                        onSuccess(response.Data);
                        return;
                    }
                    onFailure?.Invoke();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Account->Subscribe() -> " + exception.Message);
                onError?.Invoke();
            }
        }
    }
}
