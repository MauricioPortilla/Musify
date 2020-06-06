using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musify {
    class Core {
        private static readonly string API_VERSION = "v1";
        public static readonly string SERVER_API_URL = "http://localhost:5000/api/" + API_VERSION;

        public static readonly int MAX_ACCOUNT_SONGS_PER_ACCOUNT = 250;
    }
}
