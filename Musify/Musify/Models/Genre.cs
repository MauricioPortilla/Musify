using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musify.Models {
    public class Genre {

        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "genre_id", "GenreId" },
            { "name", "Name" }
        };

        private int genreId;
        public int GenreId {
            get => genreId;
            set => genreId = value;
        }
        private string name;
        public string Name {
            get => name;
            set => name = value;
        }

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
        public static void FetchById(int genreId, Action<Genre> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsync<Genre>("/genre/" + genreId, null, JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess(response.Data);
                    } else {
                        onFailure();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Genre->FetchById() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        /// <summary>
        /// Fetches all genres.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public static void FetchAll(Action<List<Genre>> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Genre>("/genres", null, JSON_EQUIVALENTS, (response, genres) => {
                    if (response.IsSuccessful) {
                        onSuccess(genres);
                    } else {
                        onFailure();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Genre->FetchAll() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        /// <summary>
        /// Fetches all songs attached to this genre.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public void FetchSongs(Action<List<Song>> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Song>("/genre/" + genreId + "/songs", null, Song.JSON_EQUIVALENTS, (response, objects) => {
                    if (response.IsSuccessful) {
                        List<Song> songs = objects;
                        foreach (var song in songs) {
                            Album.FetchById(song.AlbumId, (album) => {
                                song.Album = album;
                                Genre.FetchById(song.GenreId, (genre) => {
                                    song.Genre = genre;
                                    song.FetchArtists(() => {
                                        onSuccess(songs);
                                    }, null);
                                }, null);
                            }, null);
                        }
                    } else {
                        onFailure?.Invoke();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Playlist->FetchSongs() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        /// <summary>
        /// Returns this genre name.
        /// </summary>
        /// <returns>Genre name</returns>
        public override string ToString() {
            return name;
        }
    }
}
