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
        private List<Album> albums;
        public List<Album> Albums {
            get => albums;
            set => albums = value;
        }

        public Artist() {
        }

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

        public void FetchAlbums(Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Album>("/artist/" + artistId + "/albums", null, Album.JSON_EQUIVALENTS, (response, albums) => {
                    if (response.IsSuccessful) {
                        this.albums = albums;
                        onSuccess();
                        return;
                    }
                    onFailure();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Artist->FetchAlbums() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public override string ToString() {
            return artisticName;
        }
    }
}
