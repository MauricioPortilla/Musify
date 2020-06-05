using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Musify {
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application {
        public static readonly string DATA_SONGS_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory + "/data/songs";
        public static readonly string DATA_PLAYER_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory + "/data/player";
        public static readonly string DATA_STATIONS_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory + "/data/stations";

        public App() {
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
        }
    }
}
