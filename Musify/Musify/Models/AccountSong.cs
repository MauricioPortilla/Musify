using System;
using System.Collections.Generic;

namespace Musify.Models {
    public class AccountSong {

        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS { get; set; } = new Dictionary<string, string>() {
            { "account_song_id", "AccountSongId" },
            { "account_id", "AccountId" },
            { "title", "Title" },
            { "duration", "Duration" },
            { "song_location", "SongLocation" },
            { "upload_date", "UploadDate" }
        };

        public int AccountSongId { get; set; }
        public int AccountId { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public string SongLocation { get; set; }
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AccountSong() {
        }

        /// <summary>
        /// Represents an account song in a table.
        /// </summary>
        public struct AccountSongTable {
            public AccountSong AccountSong { get; set; }
            public string Title { get; set; }
            public string Duration { get; set; }
        }

        /// <summary>
        /// Fetches a account song by its ID.
        /// </summary>
        /// <param name="SongId">Song ID</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchById(int accountSongId, Action<AccountSong> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsync<AccountSong>(
                "/account/" + Session.Account.AccountId + "/accountsong/" + accountSongId, 
                null, JSON_EQUIVALENTS, 
                (response) => {
                    onSuccess(response.Model);
                }, (errorResponse) => {
                    onFailure?.Invoke(errorResponse);
                }, () => {
                    Console.WriteLine("Exception@Song->FetchById()");
                    onError?.Invoke();
                }
            );
        }
    }
}
