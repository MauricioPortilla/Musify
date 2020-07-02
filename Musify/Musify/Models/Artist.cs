using System;
using System.Collections.Generic;

namespace Musify.Models {
    public class Artist {
        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS { get; set; } = new Dictionary<string, string>() {
            { "artist_id", "ArtistId" },
            { "account_id", "AccountId" },
            { "artistic_name", "ArtisticName" }
        };

        public int ArtistId { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public string ArtisticName { get; set; }
        public List<Album> Albums { get; set; } = new List<Album>();

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Artist() {
        }

        /// <summary>
        /// Fetches an artist by its ID.
        /// </summary>
        /// <param name="artistId">Artist ID</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchById(int artistId, Action<Artist> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsync<Artist>("/artist/" + artistId, null, JSON_EQUIVALENTS, (response) => {
                onSuccess(response.Model);
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
            }, () => {
                Console.WriteLine("Exception@Artist->FetchById()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches an artist by its starting artistic name.
        /// </summary>
        /// <param name="artisticName">Artist artistic name</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchByArtisticNameCoincidences(
            string artisticName, Action<List<Artist>> onSuccess, Action<NetworkResponse> onFailure, Action onError
        ) {
            RestSharpTools.GetAsyncMultiple<Artist>("/artist/search/" + artisticName, null, JSON_EQUIVALENTS, (response) => {
                onSuccess(response.Model);
            }, onFailure, () => {
                Console.WriteLine("Exception@Artist->FetchByArtisticNameCoincidences()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches all this artist albums.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void FetchAlbums(Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<Album>(
                "/artist/" + ArtistId + "/albums", null, Album.JSON_EQUIVALENTS, (response) => {
                this.Albums = response.Model;
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Artist->FetchAlbums()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Returns this artist artistic name.
        /// </summary>
        /// <returns>Artist artistic name</returns>
        public override string ToString() {
            return ArtisticName;
        }

        /// <summary>
        /// Represents an Artist in a table.
        /// </summary>
        public struct ArtistTable {
            public Artist Artist { get; set; }
            public string ArtisticName { get; set; }
        }
    }
}
