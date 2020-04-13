using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musify.Models {
    public class Genre {

        public static Dictionary<string, string> JSON_EQUIVALENTS = new Dictionary<string, string>() {
            { "genre_id", "GenreId" },
            { "name", "Name" }
        };

        private int genreId;
        public int GenreId {
            get => genreId;
            set => genreId = value;
        }
        private string name;
        public string Name {
            get => name;
            set => name = value;
        }

        public Genre() {
        }

        public static void FetchById(int genreId, Action<Genre> onSuccess, Action onFailure) {
            RestSharpTools.GetAsync<Genre>("/genre/" + genreId, null, JSON_EQUIVALENTS, (response) => {
                if (response.IsSuccessful) {
                    onSuccess(response.Data);
                } else {
                    onFailure();
                }
            });
        }
    }
}
