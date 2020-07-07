using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Musify.Models;

namespace MusifyTests {
    [TestClass]
    public class AccountSongTests {
        /// <summary>
        /// Test to prove that is possible fetch an owned account song.
        /// </summary>
        [TestMethod]
        public void FetchOwnedByIdTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                AccountSong.FetchById(1, (accountSong) => {
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
        /// Test to prove that is not possible fetch a not owned account song.
        /// </summary>
        [TestMethod]
        public void FetchNotOwnedByIdTest() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            bool pass = false;
            Account.Login("freya@arkanapp.com", "1230", (account) => {
                AccountSong.FetchById(1000, (accountSong) => {
                    autoResetEvent.Set();
                }, (errorResponse) => {
                    pass = true;
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
