using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;

namespace Musify.Models {
    public class Album {

        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "album_id", "AlbumId" },
            { "type", "Type" },
            { "name", "Name" },
            { "launch_year", "LaunchYear" },
            { "discography", "Discography" },
            { "image_location", "ImageLocation" }
        };

        /// <summary>
        /// Explains how to pass JSON image location data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_IMAGE_EQUIVALENT = new Dictionary<string, string>() {
            { "image_location", "ImageLocation" },
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

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Album() {
        }

        /// <summary>
        /// Creates this album with its songs.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void Save(Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            string[] filesRoutes = new string[songs.Count];
            for (int i = 0; i < songs.Count; i++) {
                filesRoutes[i] = songs.ElementAt(i).SongLocation;
            }
            RestSharpTools.PostMultimediaAsync<Song>("album/songs", null, filesRoutes, Song.JSON_MIN_EQUIVALENTS, (responseSongs) => {
                filesRoutes = new string[] { imageLocation };
                RestSharpTools.PostMultimediaAsync<Album>("album/image", null, filesRoutes, JSON_IMAGE_EQUIVALENT, (responseImage) => {
                    List<object> artists_id = new List<object>();
                    foreach (Artist artist in artists) {
                        artists_id.Add(new {
                            artist_id = artist.ArtistId
                        });
                    }
                    List<object> new_songs = new List<object>();
                    int i = 0;
                    foreach (Song song in songs) {
                        List<object> song_artists_id = new List<object>();
                        foreach (Artist artist in song.Artists) {
                            song_artists_id.Add(new {
                                artist_id = artist.ArtistId
                            });
                        }
                        new_songs.Add(new {
                            genre_id = song.GenreId,
                            title = song.Title,
                            duration = responseSongs.Model.ElementAt(i).Duration,
                            song_location = responseSongs.Model.ElementAt(i).SongLocation,
                            artists_id = song_artists_id
                        });
                        i++;
                    }
                    var albumData = new {
                        type,
                        name,
                        launch_year = launchYear,
                        discography,
                        image_location = responseImage.Model.ElementAt(0).ImageLocation,
                        artists_id,
                        new_songs
                    };
                    RestSharpTools.PostAsync("/album", albumData, (responseAlbum) => {
                        onSuccess();
                    }, onFailure, () => {
                        Console.WriteLine("Exception@Album->Save()");
                        onError?.Invoke();
                    });
                }, onFailure, () => {
                    Console.WriteLine("Exception@Album->Save()");
                    onError?.Invoke();
                });
            }, onFailure, () => {
                Console.WriteLine("Exception@Album->Save()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches an album by its ID.
        /// </summary>
        /// <param name="albumId">Album ID</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchById(int albumId, Action<Album> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsync<Album>("/album/" + albumId, null, JSON_EQUIVALENTS, (response) => {
                response.Model.FetchArtists(() => {
                    onSuccess(response.Model);
                }, (errorResponse) => {
                    onFailure?.Invoke(errorResponse);
                }, () => {
                    onError?.Invoke();
                });
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
            }, () => {
                Console.WriteLine("Exception@Album->FetchById()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches an album by its starting name.
        /// </summary>
        /// <param name="name">Album name</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchByNameCoincidences(
            string name, Action<List<Album>> onSuccess, Action<NetworkResponse> onFailure, Action onError
        ) {
            RestSharpTools.GetAsyncMultiple<Album>("/album/search/" + name, null, JSON_EQUIVALENTS, (response) => {
                if (response.Model.Count == 0) {
                    onSuccess(response.Model);
                    return;
                }
                foreach (var album in response.Model) {
                    album.FetchArtists(() => {
                        onSuccess(response.Model);
                    }, null, null);
                }
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
            }, () => {
                Console.WriteLine("Exception@Album->FetchByNameCoincidences()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches all artists attached to this album.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void FetchArtists(Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<Artist>(
                "/album/" + albumId + "/artists", null, Artist.JSON_EQUIVALENTS, 
                (response) => {
                    this.artists = response.Model;
                    onSuccess();
                }, (errorResponse) => {
                    onFailure?.Invoke(errorResponse);
                }, () => {
                    Console.WriteLine("Exception@Album->FetchArtists()");
                    onError?.Invoke();
                }
            );
        }

        /// <summary>
        /// Returns all artistic names of each artist attached to this album.
        /// </summary>
        /// <returns>Artistic names of each artist attached to this album</returns>
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

        /// <summary>
        /// Fetches this album image.
        /// </summary>
        /// <returns>Album image</returns>
        public BitmapImage FetchImage() {
            WebClient webClient = new WebClient();
            webClient.Headers["Authorization"] = Session.AccessToken ?? "";
            var image = webClient.DownloadData(Core.SERVER_API_URL + "/album/" + albumId + "/image");
            using (MemoryStream memoryStream = new MemoryStream(image)) {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        /// <summary>
        /// Fetches all this album songs.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void FetchSongs(Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<Song>(
                "/album/" + albumId + "/songs", null, Song.JSON_EQUIVALENTS, 
                (response) => {
                    this.songs = response.Model;
                    foreach (var song in songs) {
                        song.Album = this;
                        Genre.FetchById(song.GenreId, (genre) => {
                            song.Genre = genre;
                            song.FetchArtists(() => {
                                onSuccess();
                            }, null, null);
                        }, null, null);
                    }
                }, (errorResponse) => {
                    onFailure?.Invoke(errorResponse);
                }, () => {
                    Console.WriteLine("Exception@Album->FetchSongs()");
                    onError?.Invoke();
                }
            );
        }

        /// <summary>
        /// Returns this album name.
        /// </summary>
        /// <returns>Album name</returns>
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
