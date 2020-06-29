using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Musify.Models;

namespace MusifyTests {
    [TestClass]
    public class GenreTest {
        /// <summary>
        /// Test to prove that is possible fetch a genre by its ID.
        /// </summary>
        [TestMethod]
        public void FetchByIdTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Genre.FetchById(1, (genre) => {
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
        /// Test to prove that is possible fetch all genres.
        /// </summary>
        [TestMethod]
        public void FetchAllTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Genre.FetchAll((genres) => {
                    pass = genres.Count > 0;
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
        /// Test to prove that is possible fetch all a genre songs.
        /// </summary>
        [TestMethod]
        public void FetchGenreSongsTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Genre.FetchById(1, (genre) => {
                    genre.FetchSongs((songs) => {
                        pass = songs.Count > 0;
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
