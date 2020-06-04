﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Playlist() {
        }

        public static void FetchByAccountId(int accountId, Action<List<Playlist>> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Playlist>("/account/" + accountId + "/playlists", null, JSON_EQUIVALENTS, (response, objects) => {
                    if (response.IsSuccessful) {
                        onSuccess(objects);
                    } else {
                        onFailure?.Invoke();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Playlist->FetchByAccountId() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public void FetchSongs(Action onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Song>("/playlist/" + playlistId + "/songs", null, Song.JSON_EQUIVALENTS, (response, objects) => {
                    if (response.IsSuccessful) {
                        songs = objects;
                        foreach (var song in songs) {
                            Album.FetchById(song.AlbumId, (album) => {
                                song.Album = album;
                                Genre.FetchById(song.GenreId, (genre) => {
                                    song.Genre = genre;
                                    song.FetchArtists(() => {
                                        onSuccess();
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

        public void Save(Action<Playlist> onSuccess, Action onFailure) {
            var playlistData = new {
                playlist_id = playlistId,
                account_id = accountId,
                name
            };
            try {
                if (playlistId == 0) {
                    RestSharpTools.PostAsync<Playlist>("/playlist", playlistData, JSON_EQUIVALENTS, (response) => {
                        if (response.IsSuccessful) {
                            onSuccess(response.Data);
                        } else {
                            onFailure();
                        }
                    });
                } else {
                    RestSharpTools.PutAsync<Playlist>("/playlist", playlistData, JSON_EQUIVALENTS, (response) => {
                        if (response.IsSuccessful) {
                            onSuccess(response.Data);
                        } else {
                            onFailure();
                        }
                    });
                }
            } catch (Exception exception) {
                Console.WriteLine("Exception@Playlist->Save() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public void AddSong(Song song, Action onSuccess, Action onFailure) {
            var data = new {
                song_id = song.SongId
            };
            try {
                RestSharpTools.PostAsync("/playlist/" + playlistId + "/song", data, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                    } else {
                        onFailure?.Invoke();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Playlist->AddSong() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public void ContainsSong(Song song, Action onSuccess, Action onFailure) {
            var data = new {
                song_id = song.SongId
            };
            try {
                RestSharpTools.GetAsync<Song>("/playlist/" + playlistId + "/songs/" + song.SongId, null, Song.JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess();
                    } else {
                        onFailure?.Invoke();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Playlist->ContainsSong() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public override string ToString() {
            return name;
        }
    }
}
