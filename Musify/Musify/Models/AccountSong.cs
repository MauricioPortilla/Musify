using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musify.Models {
    public class AccountSong {

        /// <summary>
        /// Explains how to pass JSON data to an object of this type.
        /// </summary>
        public static readonly Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "account_song_id", "AccountSongId" },
            { "account_id", "AccountId" },
            { "title", "Title" },
            { "duration", "Duration" },
            { "song_location", "SongLocation" },
            { "upload_date", "UploadDate" }
        };

        private int accountSongId;
        public int AccountSongId {
            get => accountSongId;
            set => accountSongId = value;
        }
        private int accountId;
        public int AccountId {
            get => accountId;
            set => accountId = value;
        }
        private string title;
        public string Title {
            get => title;
            set => title = value;
        }
        private string duration;
        public string Duration {
            get => duration;
            set => duration = value;
        }
        private string songLocation;
        public string SongLocation {
            get => songLocation;
            set => songLocation = value;
        }
        private DateTime uploadDate;
        public DateTime UploadDate {
            get => uploadDate;
            set => uploadDate = value;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AccountSong() {
        }

        /// <summary>
        /// Represents an account song in a table.
        /// </summary>
        public struct AccountSongTable {
            public AccountSong AccountSong { get; set; }
            public string Title { get; set; }
            public string Duration { get; set; }
        }
    }
}
