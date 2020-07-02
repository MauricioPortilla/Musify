using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Musify.Models {
    public class Song {
        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS { get; } = new Dictionary<string, string>() {
            { "song_id", "SongId" },
            { "album_id", "AlbumId" },
            { "genre_id", "GenreId" },
            { "title", "Title" },
            { "duration", "Duration" },
            { "song_location", "SongLocation" },
            { "status", "Status" }
        };

        /// <summary>
        /// Explains how to pass JSON minimum data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_MIN_EQUIVALENTS { get; } = new Dictionary<string, string>() {
            { "name", "SongLocation" },
            { "duration", "Duration" }
        };

        public int SongId { get; set; }
        public int AlbumId { get; set; }
        public Album Album { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; } = "0:00";
        public string SongLocation { get; set; }
        public string Status { get; set; }
        public List<Artist> Artists { get; set; } = new List<Artist>();

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Song() {
        }

        /// <summary>
        /// Represents a song in a table.
        /// </summary>
        public struct SongTable {
            public Song Song { get; set; }
            public string Title { get; set; }
            public string ArtistsNames { get; set; }
            public Album Album { get; set; }
            public Genre Genre { get; set; }
            public string Duration { get; set; }
        }

        /// <summary>
        /// Fetches all songs whose name starts with the given title.
        /// </summary>
        /// <param name="title">Song title</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchByTitleCoincidences(
            string title, Action<List<Song>> onSuccess, Action<NetworkResponse> onFailure, Action onError
        ) {
            RestSharpTools.GetAsyncMultiple<Song>("/song/search/" + title, null, JSON_EQUIVALENTS, (response) => {
                if (response.Model.Count == 0) {
                    onSuccess(response.Model);
                    return;
                }
                foreach (var song in response.Model) {
                    Album.FetchById(song.AlbumId, (album) => {
                        song.Album = album;
                        Genre.FetchById(song.GenreId, (genre) => {
                            song.Genre = genre;
                            song.FetchArtists(() => {
                                onSuccess(response.Model);
                            }, null, null);
                        }, null, null);
                    }, null, null);
                }
            }, onFailure, () => {
                Console.WriteLine("Exception@Song->FetchByTitleCoincidences()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches a song by its ID.
        /// </summary>
        /// <param name="SongId">Song ID</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchById(int songId, Action<Song> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsync<Song>("/song/" + songId, null, JSON_EQUIVALENTS, (response) => {
                Album.FetchById(response.Model.AlbumId, (album) => {
                    response.Model.Album = album;
                    Genre.FetchById(response.Model.GenreId, (genre) => {
                        response.Model.Genre = genre;
                        response.Model.FetchArtists(() => {
                            onSuccess(response.Model);
                        }, null, null);
                    }, null, null);
                }, null, null);
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
            }, () => {
                Console.WriteLine("Exception@Song->FetchById()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches this song artists.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void FetchArtists(Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<Artist>("/song/" + SongId + "/artists", null, Artist.JSON_EQUIVALENTS, (response) => {
                this.Artists = response.Model;
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Album->FetchArtists()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Gets a string with the artistic name of each artist attached to this song.
        /// </summary>
        /// <returns>Artistic name of each artist attached to this song</returns>
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
        /// Checks if this song has been downloaded before.
        /// </summary>
        /// <returns>true if is downloaded; false if not</returns>
        public bool IsDownloaded() {
            return File.Exists(App.DATA_DOWNLOADS_DIRECTORY + Path.AltDirectorySeparatorChar + SongId + ".bin");
        }

        /// <summary>
        /// Creates a stream from downloaded song file.
        /// </summary>
        /// <returns>Stream with song file bytes</returns>
        public Stream CreateDownloadedFileStream() {
            Stream stream = new MemoryStream(File.ReadAllBytes(App.DATA_DOWNLOADS_DIRECTORY + Path.AltDirectorySeparatorChar + SongId + ".bin"));
            return stream;
        }

        /// <summary>
        /// Returns this song title.
        /// </summary>
        /// <returns>Song title</returns>
        public override string ToString() {
            return Title;
        }
    }
}
