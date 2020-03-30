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
        private List<Song> songs;
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

        public static void Fetch(int accountId, Action<List<Playlist>> onSuccess, Action onFailure) {
            var data = new {
                request_type = "accountPlaylists",
                account_id = accountId
            };
            RestSharpTools.GetAsyncMultiple<Playlist>("/Playlist", data, JSON_EQUIVALENTS, (response, objects) => {
                if (response.IsSuccessful) {
                    onSuccess(objects);
                } else {
                    onFailure();
                }
            });
        }

        public void Save(Action<Playlist> onSuccess, Action onFailure) {
            var playlistData = new {
                playlist_id = playlistId,
                account_id = accountId,
                name
            };
            if (playlistId == 0) {
                RestSharpTools.PostAsync<Playlist>("/Playlist", playlistData, JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess(response.Data);
                    } else {
                        onFailure();
                    }
                });
            } else {
                RestSharpTools.PutAsync<Playlist>("/Playlist", playlistData, JSON_EQUIVALENTS, (response) => {
                    if (response.IsSuccessful) {
                        onSuccess(response.Data);
                    } else {
                        onFailure();
                    }
                });
            }
        }

        public override string ToString() {
            return name;
        }
    }
}
