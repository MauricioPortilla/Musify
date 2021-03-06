﻿using DotNetEnv;

namespace Musify {
    static class Core {
        private static readonly string API_VERSION = Env.GetString("API_VERSION", "1");
        public static readonly string SERVER_API_URL = Env.GetString("SERVER_URL", "http://localhost:5000") + "/api/v" + API_VERSION;

        public static readonly int MAX_ACCOUNT_SONGS_PER_ACCOUNT = 250;
        public static readonly int MAX_SONGS_IN_HISTORY = 50;

        public static readonly string REGEX_EMAIL = @"^\w+@\w+\.[a-zA-Z]+$";
        public static readonly string REGEX_ONLY_LETTERS = "^[a-zA-Z ]+$";
        public static readonly string REGEX_ONLY_LETTERS_NUMBERS = "^[a-zA-Z0-9 ]+$";
    }
}
