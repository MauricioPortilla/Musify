using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Musify;
using Musify.Models;

namespace MusifyTests {
    [TestClass]
    public class AccountTests {
        /// <summary>
        /// Attempts to log in with an existent account and correct credentials.
        /// </summary>
        [TestMethod]
        public void LoginWithExistentAccountTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("frey@arkanapp.com", "1230", (account) => {
                pass = true;
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to log in with a non-existent account.
        /// </summary>
        [TestMethod]
        public void LoginWithNonExistentAccountTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("mlum@arkanapp.com", "123123", (account) => {
                autoResetEvent.Set();
            }, () => {
                pass = true;
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to register a new account with no artist data.
        /// </summary>
        [TestMethod]
        public void RegisterNewAccountWithNoArtistDataTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account account = new Account {
                Email = "frey@arkanapp.com",
                Password = "1230",
                Name = "Frey",
                LastName = "Stroud"
            };
            account.Register(false, () => {
                pass = true;
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to register an existent account with no artist data.
        /// </summary>
        [TestMethod]
        public void RegisterExistentAccountWithNoArtistDataTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account account = new Account {
                Email = "frey@arkanapp.com",
                Password = "5678",
                Name = "Frey",
                LastName = "Stroud"
            };
            account.Register(false, () => {
                autoResetEvent.Set();
            }, () => {
                pass = true;
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to register a new account with artist data.
        /// </summary>
        [TestMethod]
        public void RegisterNewAccountWithArtistDataTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account account = new Account {
                Email = "freya@arkanapp.com",
                Password = "1230",
                Name = "Freya",
                LastName = "Stroud"
            };
            account.Register(true, () => {
                pass = true;
                autoResetEvent.Set();
            }, () => {
                autoResetEvent.Set();
            }, "Freya Stroud");
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to register a new account with artist data.
        /// </summary>
        [TestMethod]
        public void RegisterNewAccountWithExistentArtistTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account account = new Account {
                Email = "freyastroud@arkanapp.com",
                Password = "1230",
                Name = "Freya",
                LastName = "Stroud"
            };
            account.Register(true, () => {
                autoResetEvent.Set();
            }, () => {
                pass = true;
                autoResetEvent.Set();
            }, "Freya Stroud");
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to fetch artist data from account.
        /// </summary>
        [TestMethod]
        public void FetchAccountArtistTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                account.FetchArtist(() => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to fetch account songs data from account.
        /// </summary>
        [TestMethod]
        public void FetchAccountSongsTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                account.FetchAccountSongs(() => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to add account songs to account.
        /// </summary>
        [TestMethod]
        public void AddAccountSongsTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            string[] fileSongsPath = {
                ""
            };
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                account.AddAccountSongs(fileSongsPath, () => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to delete the first account song from account.
        /// </summary>
        [TestMethod]
        public void DeleteAccountSongTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                account.FetchAccountSongs(() => {
                    account.DeleteAccountSong(account.AccountSongs.FirstOrDefault(), () => {
                        pass = true;
                        autoResetEvent.Set();
                    }, () => {
                        autoResetEvent.Set();
                    });
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to like an existent song. An existent rate must not exist.
        /// </summary>
        [TestMethod]
        public void LikeSongTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Song song = new Song {
                    SongId = 1
                };
                account.LikeSong(song, () => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to dislike an existent song. An existent rate must not exist.
        /// </summary>
        [TestMethod]
        public void DislikeSongTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Song song = new Song {
                    SongId = 1
                };
                account.DislikeSong(song, () => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to unlike an existent song.
        /// </summary>
        [TestMethod]
        public void UnlikeSongTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Song song = new Song {
                    SongId = 1
                };
                account.UnlikeSong(song, () => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to undislike an existent song.
        /// </summary>
        [TestMethod]
        public void UndislikeSongTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Song song = new Song {
                    SongId = 1
                };
                account.UndislikeSong(song, () => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to fetch active subscription from account.
        /// </summary>
        [TestMethod]
        public void FetchSubscriptionTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                account.FetchSubscription((subscription) => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Attempts to subscribe.
        /// </summary>
        [TestMethod]
        public void SubscribeTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                account.Subscribe((subscription) => {
                    pass = true;
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                }, () => {
                    autoResetEvent.Set();
                });
            }, () => {
                autoResetEvent.Set();
            });
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

    }
}
