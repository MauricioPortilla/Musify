using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musify.Models {
    public class Song {

        public static Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "song_id", "SongId" },
            { "album_id", "AlbumId" },
            { "genre_id", "GenreId" },
            { "title", "Title" },
            { "song_location", "SongLocation" }
        };

        private int songId;
        public int SongId {
            get => songId;
            set => songId = value;
        }
        private int albumId;
        public int AlbumId {
            get => albumId;
            set => albumId = value;
        }
        private Album album;
        public Album Album {
            get => album;
            set => album = value;
        }
        private int genreId;
        public int GenreId {
            get => genreId;
            set => genreId = value;
        }
        private Genre genre;
        public Genre Genre {
            get => genre;
            set => genre = value;
        }
        private string title;
        public string Title {
            get => title;
            set => title = value;
        }
        private string songLocation;
        public string SongLocation {
            get => songLocation;
            set => songLocation = value;
        }
        private string duration = "0:00";
        public string Duration {
            get => duration;
            set => duration = value;
        }

        public Song() {
        }

        public struct SongTable {
            public Song Song;
            public string Title { get; set; }
            public string ArtistsNames { get; set; }
            public Album Album { get; set; }
            public string Duration { get; set; }
        }

        /// <summary>
        /// Fetches all songs whose name starts with the given title.
        /// </summary>
        /// <param name="title">Song title</param>
        /// <param name="onSuccess">On success</param>
        /// <param name="onFailure">On failure</param>
        public static void FetchByTitleCoincidences(string title, Action<List<Song>> onSuccess, Action onFailure) {
            try {
                RestSharpTools.GetAsyncMultiple<Song>("/song/search/" + title, null, JSON_EQUIVALENTS, (response, songs) => {
                    if (response.IsSuccessful) {
                        if (songs.Count == 0) {
                            onSuccess(songs);
                            return;
                        }
                        foreach (var song in songs) {
                            Album.FetchById(song.albumId, (album) => {
                                song.album = album;
                                Genre.FetchById(song.genreId, (genre) => {
                                    song.genre = genre;
                                    onSuccess(songs);
                                }, null);
                            }, null);
                        }
                    } else {
                        onFailure();
                    }
                });
            } catch (Exception exception) {
                Console.WriteLine("Exception@Playlist->FetchByTitleCoincidences() -> " + exception.Message);
                onFailure?.Invoke();
            }
        }

        public override string ToString() {
            return title;
        }
    }
}
