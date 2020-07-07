using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Musify.Models;

namespace MusifyTests {
    [TestClass]
    public class SongTests {
        /// <summary>
        /// Test to prove that is possible fetch songs by its starting title.
        /// </summary>
        [TestMethod]
        public void FetchByTitleCoincidencesTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Song.FetchByTitleCoincidences("song", (songs) => {
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
            autoResetEvent.WaitOne();
            Assert.AreEqual(true, pass);
        }

        /// <summary>
        /// Test to prove that is possible fetch a song by its ID.
        /// </summary>
        [TestMethod]
        public void FetchByIdTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Song.FetchById(1, (song) => {
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
        /// Test to prove that is possible fetch artists from a song.
        /// </summary>
        [TestMethod]
        public void FetchSongArtistsTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                Song.FetchById(1, (song) => {
                    song.FetchArtists(() => {
                        pass = song.Artists.Count > 0;
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
