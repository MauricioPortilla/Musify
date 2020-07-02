using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;

namespace Musify.Models {
    public class Album {
        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS { get; } = new Dictionary<string, string>() {
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
        public static Dictionary<string, string> JSON_IMAGE_EQUIVALENT { get; } = new Dictionary<string, string>() {
            { "image_location", "ImageLocation" },
        };

        public int AlbumId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int LaunchYear { get; set; }
        public string Discography { get; set; }
        public string ImageLocation { get; set; }
        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Song> Songs { get; set; } = new List<Song>();

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
            string[] filesRoutes = new string[Songs.Count];
            for (int i = 0; i < Songs.Count; i++) {
                filesRoutes[i] = Songs.ElementAt(i).SongLocation;
            }
            RestSharpTools.PostMultimediaAsync<Song>("album/songs", null, filesRoutes, Song.JSON_MIN_EQUIVALENTS, (responseSongs) => {
                filesRoutes = new string[] { ImageLocation };
                RestSharpTools.PostMultimediaAsync<Album>("album/image", null, filesRoutes, JSON_IMAGE_EQUIVALENT, (responseImage) => {
                    List<object> artistsId = new List<object>();
                    foreach (Artist artist in Artists) {
                        artistsId.Add(new {
                            artist_id = artist.ArtistId
                        });
                    }
                    List<object> newSongs = new List<object>();
                    int i = 0;
                    foreach (Song song in Songs) {
                        List<object> songArtistsId = new List<object>();
                        foreach (Artist artist in song.Artists) {
                            songArtistsId.Add(new {
                                artist_id = artist.ArtistId
                            });
                        }
                        newSongs.Add(new {
                            genre_id = song.GenreId,
                            title = song.Title,
                            duration = responseSongs.Model.ElementAt(i).Duration,
                            song_location = responseSongs.Model.ElementAt(i).SongLocation,
                            artists_id = songArtistsId
                        });
                        i++;
                    }
                    var albumData = new {
                        type = Type,
                        name = Name,
                        launch_year = LaunchYear,
                        discography = Discography,
                        image_location = responseImage.Model.ElementAt(0).ImageLocation,
                        artistsId,
                        newSongs
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
                "/album/" + AlbumId + "/artists", null, Artist.JSON_EQUIVALENTS, 
                (response) => {
                    this.Artists = response.Model;
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
            StringBuilder names = new StringBuilder();
            foreach (Artist artist in Artists) {
                if (!string.IsNullOrEmpty(names.ToString())) {
                    names.Append(", ");
                }
                names.Append(artist.ArtisticName);
            }
            return names.ToString();
        }

        /// <summary>
        /// Fetches this album image.
        /// </summary>
        /// <returns>Album image</returns>
        public BitmapImage FetchImage() {
            WebClient webClient = new WebClient();
            webClient.Headers["Authorization"] = Session.AccessToken ?? "";
            try {
                var image = webClient.DownloadData(Core.SERVER_API_URL + "/album/" + AlbumId + "/image");
                webClient.Dispose();
                using (MemoryStream memoryStream = new MemoryStream(image)) {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            } catch (Exception) {
                webClient.Dispose();
                return null;
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
                "/album/" + AlbumId + "/songs", null, Song.JSON_EQUIVALENTS, 
                (response) => {
                    this.Songs = response.Model;
                    foreach (var song in Songs) {
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
            return Name;
        }

        /// <summary>
        /// Represents an Album in a table.
        /// </summary>
        public struct AlbumTable {
            public Album Album { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
            public string Artist { get; set; }
            public int LaunchYear { get; set; }
        }
    }
}
