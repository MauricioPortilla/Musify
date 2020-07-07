using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Musify.Models;

namespace MusifyTests {
    [TestClass]
    public class AlbumTest {
        /// <summary>
        /// Test to prove that is possible save a album.
        /// </summary>
        [TestMethod]
        public void SaveTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                account.FetchArtist(() => {
                    Album album = new Album {
                        Type = "Sencillo",
                        Name = "Album Test",
                        LaunchYear = 2020,
                        Discography = "Discography Test",
                        ImageLocation = "C:\\testImage.png",
                        Artists = { account.Artist },
                        Songs = { new Song {
                           GenreId = 1,
                           Title = "Song Test",
                           SongLocation = "C:\\testSong.mp3",
                           Artists = { account.Artist }
                        } }
                    };
                    album.Save(() => {
                        pass = true;
                        autoResetEvent.Set();
                    }, (errorResponse) => {
                        autoResetEvent.Set();
                    }, () => {
                        autoResetEvent.Set();
                    });
                }, (errorResponse) => {
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
        /// Test to prove that is possible fetch a album by its ID.
        /// </summary>
        [TestMethod]
        public void FetchByIdTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Album.FetchById(1, (album) => {
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
        /// Test to prove that is possible fetch albums by its starting title.
        /// </summary>
        [TestMethod]
        public void FetchByTitleCoincidencesTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Album.FetchByNameCoincidences("Alb", (albums) => {
                    pass = albums.Count > 0;
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
        /// Test to prove that is possible fetch artists from a album.
        /// </summary>
        [TestMethod]
        public void FetchAlbumArtistsTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Album.FetchById(1, (album) => {
                    album.FetchArtists(() => {
                        pass = album.Artists.Count > 0;
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
        /// Test to prove that is possible fetch all album songs.
        /// </summary>
        [TestMethod]
        public void FetchAlbumSongsTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Album.FetchById(1, (album) => {
                    album.FetchSongs(() => {
                        pass = album.Songs.Count > 0;
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
