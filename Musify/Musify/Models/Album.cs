﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musify.Models {
    public class Album {

        public static Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "album_id", "AlbumId" },
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
        private List<Artist> artists;
        public List<Artist> Artists {
            get => artists;
            set => artists = value;
        }

        public Album() {
        }

        public static void FetchById(int albumId, Action<Album> onSuccess, Action onFailure) {
            RestSharpTools.GetAsync<Album>("/album/" + albumId, null, JSON_EQUIVALENTS, (response) => {
                if (response.IsSuccessful) {
                    response.Data.FetchArtists(() => {
                        onSuccess(response.Data);
                    });
                } else {
                    onFailure?.Invoke();
                }
            });
        }

        private void FetchArtists(Action onFinish) {
            RestSharpTools.GetAsyncMultiple<Artist>("/album/" + albumId + "/artists", null, Artist.JSON_EQUIVALENTS, (response, artists) => {
                if (response.IsSuccessful) {
                    this.artists = artists;
                }
                onFinish();
            });
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

        public override string ToString() {
            return name;
        }
    }
}
