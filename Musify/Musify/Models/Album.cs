using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Musify.Models {
    public class Album {

        public static Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "album_id", "AlbumId" },
            { "type", "Type" },
            { "name", "Name" },
            { "launch_year", "LaunchYear" },
            { "discography", "Discography" },
            { "image_location", "ImageLocation" }
        };

        private int albumId;
        public int AlbumId {
            get => albumId;
            set => albumId = value;
        }
        private string type;
        public string Type {
            get => type;
            set => type = value;
        }
        private string name;
        public string Name {
            get => name;
            set => name = value;
        }
        private int launchYear;
        public int LaunchYear {
            get => launchYear;
            set => launchYear = value;
        }
        private string discography;
        public string Discography {
            get => discography;
            set => discography = value;
        }
        private string imageLocation;
        public string ImageLocation {
            get => imageLocation;
            set => imageLocation = value;
        }
        private List<Artist> artists = new List<Artist>();
        public List<Artist> Artists {
            get => artists;
            set => artists = value;
        }
        private List<Song> songs = new List<Song>();
        public List<Song> Songs {
            get => songs;
            set => songs = value;
        }

        public Album() {
        }

        public static void FetchById(int albumId, Action<Album> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsync<Album>("/album/" + albumId, null, JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        response.Data.FetchArtists(() => {
                            onSuccess(response.Data);
                        }, () => { });
                    } else {
                        onFailure?.Invoke();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Album->FetchById() -> " + exception.Message);
                onFailure();
            }
        }

        public static void FetchByNameCoincidences(string name, Action<List<Album>> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Album>("/album/search/" + name, null, JSON_EQUIVALENTS, (response, albums) => {
                    if (response.IsSuccessful) {
                        if (albums.Count == 0) {
                            onSuccess(albums);
                            return;
                        }
                        foreach (var album in albums) {
                            album.FetchArtists(() => {
                                onSuccess(albums);
                            }, null);
                        }
                    } else {
                        onFailure();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Album->FetchByNameCoincidences() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public void FetchArtists(Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Artist>("/album/" + albumId + "/artists", null, Artist.JSON_EQUIVALENTS, (response, artists) => {
                    if (response.IsSuccessful) {
                        this.artists = artists;
                        onSuccess();
                        return;
                    }
                    onFailure?.Invoke();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Album->FetchArtists() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public string GetArtistsNames() {
            string names = "";
            foreach (Artist artist in artists) {
                if (!string.IsNullOrEmpty(names)) {
                    names += ", ";
                }
                names += artist.ArtisticName;
            }
            return names;
        }

        public BitmapImage FetchImage() {
            return new BitmapImage(new Uri(Core.SERVER_API_URL + "/album/" + albumId + "/image", UriKind.RelativeOrAbsolute));
        }

        public void FetchSongs(Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Song>("/album/" + albumId + "/songs", null, Song.JSON_EQUIVALENTS, (response, songs) => {
                    if (response.IsSuccessful) {
                        this.songs = songs;
                        foreach (var song in songs) {
                            song.Album = this;
                        }
                        onSuccess?.Invoke();
                        return;
                    }
                    onFailure?.Invoke();
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Album->FetchSongs() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public override string ToString() {
            return name;
        }

        /// <summary>
        /// Represents an Album in a table.
        /// </summary>
        public struct AlbumTable {
            public Album Album;
            public string Type { get; set; }
            public string Name { get; set; }
            public string Artist { get; set; }
            public int LaunchYear { get; set; }
        }
    }
}
