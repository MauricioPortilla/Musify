using System;
using System.Collections.Generic;
using Musify.Models;
using Musify.Pages;
using RestSharp;
using SpeedTest.Net;

namespace Musify {
    class Session {
        public static readonly RestClient REST_CLIENT = new RestClient(Core.SERVER_API_URL);
        public static Account Account;
        public static System.Windows.Controls.Frame MainFrame;
        public static MainWindow MainWindow;
        public static PlayerPage PlayerPage;
        public static List<int> SongsIdPlayQueue = new List<int>();
        public static List<int> SongsIdPlayHistory = new List<int>();
        public static List<int> GenresIdRadioStations = new List<int>();
        public static List<int> SongsIdSongList= new List<int>();
        public static int historyIndex;
        public static string SongStreamingQuality = "highquality";
        public static string SongStreamingQualitySelected = "lowquality";
        public static string AccessToken = null;

        /// <summary>
        /// Gets download speed in megabits and sets song streaming quality.
        /// </summary>
        public static async void SetSongStreamingQualityAutomatically() {
            try {
                var speedMb = await SpeedTestClient.GetDownloadSpeed(unit: SpeedTest.Net.Enums.SpeedTestUnit.MegaBitsPerSecond);
                if (speedMb.Speed >= 20 && speedMb.Speed <= 40) {
                    SongStreamingQualitySelected = "mediumquality";
                } else if (speedMb.Speed > 40) {
                    SongStreamingQualitySelected = "highquality";
                }
            } catch (Exception exception) {
                Console.WriteLine("Exception@SetSongStreamingQualityAutomatically -> " + exception);
            }
        }
    }
}
