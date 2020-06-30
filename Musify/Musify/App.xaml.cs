using System;
using System.IO;
using System.Windows;

namespace Musify {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static readonly string DATA_SONGS_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory + "/data/songs";
        public static readonly string DATA_PLAYER_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory + "/data/player";
        public static readonly string DATA_STATIONS_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory + "/data/stations";
        public static readonly string DATA_DOWNLOADS_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory + "/data/downloads";

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public App() {
            var downloadedPlaylists = Musify.Properties.Settings.Default.DownloadedPlaylists;
            if (downloadedPlaylists == null) {
                Musify.Properties.Settings.Default.DownloadedPlaylists = new System.Collections.Specialized.StringCollection();
            }
        }

        /// <summary>
        /// Verifies if the essential directories exists; if false, they will
        /// be created with their essential account files, so it requires an account
        /// in session.
        /// </summary>
        public static void CreateDirectories() {
            if (Session.Account == null) {
                return;
            }
            if (!Directory.Exists(DATA_SONGS_DIRECTORY)) {
                Directory.CreateDirectory(DATA_SONGS_DIRECTORY);
                File.Create(DATA_SONGS_DIRECTORY + "/playQueue" + Session.Account.AccountId).Close();
                File.Create(DATA_SONGS_DIRECTORY + "/playHistory" + Session.Account.AccountId).Close();
            }
            if (!Directory.Exists(DATA_PLAYER_DIRECTORY)) {
                Directory.CreateDirectory(DATA_PLAYER_DIRECTORY);
                File.Create(DATA_PLAYER_DIRECTORY + "/player" + Session.Account.AccountId).Close();
            }
            if (!Directory.Exists(DATA_STATIONS_DIRECTORY)) {
                Directory.CreateDirectory(DATA_STATIONS_DIRECTORY);
                File.Create(DATA_STATIONS_DIRECTORY + "/stations" + Session.Account.AccountId).Close();
            }
            if (!Directory.Exists(DATA_DOWNLOADS_DIRECTORY)) {
                Directory.CreateDirectory(DATA_DOWNLOADS_DIRECTORY);
            }
        }
    }
}
