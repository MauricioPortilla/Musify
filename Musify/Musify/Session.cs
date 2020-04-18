using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Musify.Pages;
using RestSharp;

namespace Musify {
    class Session {
        public static readonly RestClient REST_CLIENT = new RestClient(Core.SERVER_API_URL);
        public static Account Account;
        public static System.Windows.Controls.Frame MainFrame;
        public static MainWindow MainWindow;
        public static PlayerPage PlayerPage;
        public static string SongStreamingQuality = "mediumQuality";
    }
}
