using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musify.Models {
    public class Artist {

        public static Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "artist_id", "ArtistId" },
            { "account_id", "AccountId" },
            { "artistic_name", "ArtisticName" }
        };

        private int artistId;
        public int ArtistId {
            get => artistId;
            set => artistId = value;
        }
        private int accountId;
        public int AccountId {
            get => accountId;
            set => accountId = value;
        }
        private Account account;
        public Account Account {
            get => account;
            set => account = value;
        }
        private string artisticName;
        public string ArtisticName {
            get => artisticName;
            set => artisticName = value;
        }
        private List<Album> albums = new List<Album>();
        public List<Album> Albums {
            get => albums;
            set => albums = value;
        }

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
        public static void FetchById(int artistId, Action<Artist> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsync<Artist>("/artist/" + artistId, null, JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess(response.Data);
                    } else {
                        onFailure();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Artist->FetchById() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        /// <summary>
        /// Fetches an artist by its starting artistic name.
        /// </summary>
        /// <param name="artisticName">Artist artistic name</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public static void FetchByArtisticNameCoincidences(string artisticName, Action<List<Artist>> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Artist>("/artist/search/" + artisticName, null, JSON_EQUIVALENTS, (response, artists) => {
                    if (response.IsSuccessful) {
                        onSuccess(artists);
                    } else {
                        onFailure();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Artist->FetchByArtisticNameCoincidences() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        /// <summary>
        /// Fetches all this artist albums.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void FetchAlbums(Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Album>("/artist/" + artistId + "/albums", null, Album.JSON_EQUIVALENTS, (response, albums) => {
                    if (response.IsSuccessful) {
                        this.albums = albums;
                        onSuccess();
                        return;
                    }
                    onFailure?.Invoke();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Artist->FetchAlbums() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        /// <summary>
        /// Returns this artist artistic name.
        /// </summary>
        /// <returns>Artist artistic name</returns>
        public override string ToString() {
            return artisticName;
        }

        /// <summary>
        /// Represents an Artist in a table.
        /// </summary>
        public struct ArtistTable {
            public Artist Artist;
            public string ArtisticName { get; set; }
        }
    }
}
