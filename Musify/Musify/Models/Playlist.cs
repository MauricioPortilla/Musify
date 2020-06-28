using System;
using System.Collections.Generic;

namespace Musify.Models {
    public class Playlist {

        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static readonly Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "playlist_id", "PlaylistId" },
            { "account_id", "AccountId" },
            { "name", "Name" }
        };

        private int playlistId = 0;
        public int PlaylistId {
            get => playlistId;
            set => playlistId = value;
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
        private List<Song> songs = new List<Song>();
        public List<Song> Songs {
            get => songs;
            set => songs = value;
        }
        private string name;
        public string Name {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Playlist() {
        }

        /// <summary>
        /// Fetches all account playlists.
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public static void FetchByAccountId(int accountId, Action<List<Playlist>> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<Playlist>("/account/" + accountId + "/playlists", null, JSON_EQUIVALENTS, (response) => {
                onSuccess(response.Model);
            }, onFailure, () => {
                Console.WriteLine("Exception@Playlist->FetchByAccountId()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Fetches all this playlist songs.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void FetchSongs(Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.GetAsyncMultiple<Song>("/playlist/" + playlistId + "/songs", null, Song.JSON_EQUIVALENTS, (response) => {
                songs = response.Model;
                foreach (var song in songs) {
                    Album.FetchById(song.AlbumId, (album) => {
                        song.Album = album;
                        Genre.FetchById(song.GenreId, (genre) => {
                            song.Genre = genre;
                            song.FetchArtists(() => {
                                onSuccess();
                            }, null, null);
                        }, null, null);
                    }, null, null);
                }
            }, onFailure, () => {
                Console.WriteLine("Exception@Playlist->FetchSongs()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Saves this playlist data. If it does not exist, it will be created; otherwise,
        /// it will be updated.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void Save(Action<Playlist> onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var playlistData = new {
                playlist_id = playlistId,
                account_id = accountId,
                name
            };
            if (playlistId == 0) {
                RestSharpTools.PostAsync<Playlist>("/playlist", playlistData, JSON_EQUIVALENTS, (response) => {
                    onSuccess(response.Model);
                }, onFailure, () => {
                    Console.WriteLine("Exception@Playlist->Save()");
                    onError?.Invoke();
                });
            } else {
                RestSharpTools.PutAsync<Playlist>("/playlist/" + playlistId, playlistData, JSON_EQUIVALENTS, (response) => {
                    onSuccess(response.Model);
                }, onFailure, () => {
                    Console.WriteLine("Exception@Playlist->Save()");
                    onError?.Invoke();
                });
            }
        }

        /// <summary>
        /// Adds a song to this playlist.
        /// </summary>
        /// <param name="song">Song to add</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void AddSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var data = new {
                song_id = song.SongId
            };
            RestSharpTools.PostAsync("/playlist/" + playlistId + "/song", data, (response) => {
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Playlist->AddSong()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Checks if the song is in this playlist.
        /// </summary>
        /// <param name="song">Song to check</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void ContainsSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            var data = new {
                song_id = song.SongId
            };
            RestSharpTools.GetAsync<Song>(
                "/playlist/" + playlistId + "/songs/" + song.SongId, 
                null, Song.JSON_EQUIVALENTS, 
                (response) => {
                    onSuccess();
                }, (errorResponse) => {
                    onFailure?.Invoke(errorResponse);
                }, () => {
                    Console.WriteLine("Exception@Playlist->ContainsSong()");
                    onError();
                }
            );
        }

        /// <summary>
        /// Deletes this playlist.
        /// </summary>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void Delete(Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.DeleteAsync("/playlist/" + playlistId, null, (response) => {
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Playlist->Delete()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Deletes the song from this playlist.
        /// </summary>
        /// <param name="song">Song to delete</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        /// <param name="onError">On error</param>
        public void DeleteSong(Song song, Action onSuccess, Action<NetworkResponse> onFailure, Action onError) {
            RestSharpTools.DeleteAsync("/playlist/" + playlistId + "/songs/" + song.SongId, null, (response) => {
                onSuccess();
            }, onFailure, () => {
                Console.WriteLine("Exception@Playlist->DeleteSong()");
                onError?.Invoke();
            });
        }

        /// <summary>
        /// Returns the name of this playlist.
        /// </summary>
        /// <returns>Playlist name</returns>
        public override string ToString() {
            return name;
        }
    }
}
