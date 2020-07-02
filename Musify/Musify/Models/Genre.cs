using System;
using System.Collections.Generic;

namespace Musify.Models {
    public class Genre {
        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS { get; set; } = new Dictionary<string, string>() {
            { "genre_id", "GenreId" },
            { "name", "Name" }
        };

        public int GenreId { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Genre() {
        }

        /// <summary>
        /// Fetches a genre by its ID.
        /// </summary>
        /// <param name="genreId">Genre ID</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchById(int genreId, Action<Genre> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsync<Genre>("/genre/" + genreId, null, JSON_EQUIVALENTS, (response) => {
                onSuccess(response.Model);
            }, (errorResponse) => {
                onFailure?.Invoke(errorResponse);
            }, () => {
                Console.WriteLine("Exception@Genre->FetchById()");
                onError();
            });
        }

        /// <summary>
        /// Fetches all genres.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchAll(Action<List<Genre>> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<Genre>("/genres", null, JSON_EQUIVALENTS, (response) => {
                onSuccess(response.Model);
            }, onFailure, () => {
                onError?.Invoke();
                Console.WriteLine("Exception@Genre->FetchAll()");
            });
        }

        /// <summary>
        /// Fetches all songs attached to this genre.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void FetchSongs(Action<List<Song>> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<Song>("/genre/" + GenreId + "/songs", null, Song.JSON_EQUIVALENTS, (response) => {
                List<Song> songs = response.Model;
                foreach (var song in songs) {
                    Album.FetchById(song.AlbumId, (album) => {
                        song.Album = album;
                        Genre.FetchById(song.GenreId, (genre) => {
                            song.Genre = genre;
                            song.FetchArtists(() => {
                                onSuccess(songs);
                            }, null, null);
                        }, null, null);
                    }, null, null);
                }
            }, onFailure, () => {
                onError?.Invoke();
                Console.WriteLine("Exception@Playlist->FetchSongs()");
            });
        }

        /// <summary>
        /// Returns this genre name.
        /// </summary>
        /// <returns>Genre name</returns>
        public override string ToString() {
            return Name;
        }
    }
}
