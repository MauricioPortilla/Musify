using System;
using System.Collections.Generic;
using Musify.Models;
using Musify.Pages;
using RestSharp;
using SpeedTest.Net;

namespace Musify {
    static class Session {
        public static readonly RestClient REST_CLIENT = new RestClient(Core.SERVER_API_URL);
        public static Account Account { get; set; }
        public static MainWindow MainWindow { get; set; }
        public static PlayerPage PlayerPage { get; set; } = new PlayerPage();
        public static List<int> SongsIdPlayQueue { get; set; } = new List<int>();
        public static List<int> SongsIdPlayHistory { get; set; } = new List<int>();
        public static List<int> GenresIdRadioStations { get; set; } = new List<int>();
        public static List<int> SongsIdSongList { get; set; } = new List<int>();
        public static int HistoryIndex { get; set; }
        public static string SongStreamingQuality { get; set; } = "highquality";
        public static string SongStreamingQualitySelected { get; set; } = "lowquality";
        public static string AccessToken { get; set; } = null;

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
