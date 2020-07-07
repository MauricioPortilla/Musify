using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Musify.Models;

namespace MusifyTests {
    [TestClass]
    public class PlaylistTests {
        /// <summary>
        /// Test to prove that is possible fetch all account playlists.
        /// </summary>
        [TestMethod]
        public void FetchAccountPlaylistsTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya2@arkanapp.com", "1230", (account) => {
                Playlist.FetchByAccountId(account.AccountId, (playlists) => {
                    pass = true;
                    autoResetEvent.Set();
                }, (errorResponse) => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, (errorResponse) => {
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Test to prove that is possible fetch all playlist songs.
        /// </summary>
        [TestMethod]
        public void FetchPlaylistSongsTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya2@arkanapp.com", "1230", (account) => {
                Playlist.FetchByAccountId(account.AccountId, (playlists) => {
                    if (playlists.Count == 0) {
                        autoResetEvent.Set();
                        return;
                    }
                    playlists.FirstOrDefault().FetchSongs(() => {
                        pass = true;
                        autoResetEvent.Set();
                    }, (errorMessage) => {
                        autoResetEvent.Set();
                    }, () => {
                        autoResetEvent.Set();
                    });
                }, (errorResponse) => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, (errorResponse) => {
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Test to prove that is possible create a new playlist.
        /// </summary>
        [TestMethod]
        public void CreatePlaylistTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya2@arkanapp.com", "1230", (account) => {
                Playlist playlist = new Playlist {
                    AccountId = account.AccountId,
                    Name = "Playlist Test"
                };
                playlist.Save((createdPlaylist) => {
                    pass = true;
                    autoResetEvent.Set();
                }, (errorResponse) => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, (errorResponse) => {
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Test to prove that is possible rename an existent playlist.
        /// </summary>
        [TestMethod]
        public void RenamePlaylistTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya2@arkanapp.com", "1230", (account) => {
                Playlist.FetchByAccountId(account.AccountId, (playlists) => {
                    if (playlists.Count == 0) {
                        autoResetEvent.Set();
                        return;
                    }
                    Playlist playlist = playlists.FirstOrDefault();
                    playlist.Name = "Renamed playlist test";
                    playlist.Save((renamedPlaylist) => {
                        pass = true;
                        autoResetEvent.Set();
                    }, (errorResponse) => {
                        autoResetEvent.Set();
                    }, () => {
                        autoResetEvent.Set();
                    });
                }, (errorResponse) => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, (errorResponse) => {
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Test to prove that is possible add a song to an existent playlist.
        /// </summary>
        [TestMethod]
        public void AddSongTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya2@arkanapp.com", "1230", (account) => {
                Playlist.FetchByAccountId(account.AccountId, (playlists) => {
                    if (playlists.Count == 0) {
                        autoResetEvent.Set();
                        return;
                    }
                    Playlist playlist = playlists.FirstOrDefault();
                    Song song = new Song {
                        SongId = 1
                    };
                    playlist.AddSong(song, () => {
                        pass = true;
                        autoResetEvent.Set();
                    }, (errorResponse) => {
                        autoResetEvent.Set();
                    }, () => {
                        autoResetEvent.Set();
                    });
                }, (errorResponse) => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, (errorResponse) => {
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Test to prove that is possible delete a song from an existent playlist.
        /// </summary>
        [TestMethod]
        public void DeleteSongTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya2@arkanapp.com", "1230", (account) => {
                Playlist.FetchByAccountId(account.AccountId, (playlists) => {
                    if (playlists.Count == 0) {
                        autoResetEvent.Set();
                        return;
                    }
                    Playlist playlist = playlists.FirstOrDefault();
                    Song song = new Song {
                        SongId = 1
                    };
                    playlist.DeleteSong(song, () => {
                        pass = true;
                        autoResetEvent.Set();
                    }, (errorResponse) => {
                        autoResetEvent.Set();
                    }, () => {
                        autoResetEvent.Set();
                    });
                }, (errorResponse) => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, (errorResponse) => {
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Test to prove that is possible delete an existent playlist.
        /// </summary>
        [TestMethod]
        public void DeletePlaylistTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya2@arkanapp.com", "1230", (account) => {
                Playlist.FetchByAccountId(account.AccountId, (playlists) => {
                    if (playlists.Count == 0) {
                        autoResetEvent.Set();
                        return;
                    }
                    Playlist playlist = playlists.FirstOrDefault();
                    playlist.Delete(() => {
                        pass = true;
                        autoResetEvent.Set();
                    }, (errorResponse) => {
                        autoResetEvent.Set();
                    }, () => {
                        autoResetEvent.Set();
                    });
                }, (errorResponse) => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, (errorResponse) => {
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }
    }
}
