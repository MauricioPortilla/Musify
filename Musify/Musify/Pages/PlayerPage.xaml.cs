using Musify.Models;
using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para PlayerPage.xaml
    /// </summary>
    public partial class PlayerPage : Page {

        /// <summary>
        /// Manages song stream data.
        /// </summary>
        private WaveOut playerWaveOut;
        /// <summary>
        /// Stores song data.
        /// </summary>
        private IWaveProvider reader;
        /// <summary>
        /// Stores the latest song played.
        /// </summary>
        private Song latestSongPlayed;
        public Song LatestSongPlayed {
            get => latestSongPlayed;
            set => latestSongPlayed = value;
        }
        /// <summary>
        /// Stores the latest stream retrieved from server.
        /// </summary>
        private List<byte> latestStream;
        /// <summary>
        /// Stores the latest account song played.
        /// </summary>
        private AccountSong latestAccountSongPlayed;
        /// <summary>
        /// Checks if player was stopped or not.
        /// </summary>
        private bool isPlayerStopped = false;
        /// <summary>
        /// True if there's no song requests; false if a new one is asking to be played.
        /// </summary>
        private bool isStreamSongLocked = false;
        /// <summary>
        /// True if player is playing a song; false if not.
        /// </summary>
        private bool isPlayerWaveOutAvailable = true;

        /// <summary>
        /// Creates a new player instance.
        /// </summary>
        public PlayerPage() {
            InitializeComponent();
        }

        /// <summary>
        /// Plays a Song.
        /// </summary>
        /// <param name="song">Song to play</param>
        public void PlaySong(Song song) {
            if (latestSongPlayed != null) {
                Session.SongsIdPlayHistory.Add(latestSongPlayed.SongId);
            }
            latestAccountSongPlayed = null;
            if (latestSongPlayed != null && latestSongPlayed.SongId == song.SongId && latestStream != null) {
                PlayMemoryStream(new MemoryStream(latestStream.ToArray()));
                return;
            }
            latestSongPlayed = song;
            songNameTextBlock.Text = song.Title;
            artistNameTextBlock.Text = song.Album.GetArtistsNames();
            if (song.IsDownloaded()) {
                Task.Run(() => {
                    PlayMemoryStream(song.CreateDownloadedFileStream());
                });
            } else {
                MakeRequestStreamSong(Core.SERVER_API_URL + "/stream/song/" + song.SongId + "/" + Session.SongStreamingQuality);
            }
            likeButton.Visibility = Visibility.Visible;
            dislikeButton.Visibility = Visibility.Visible;
            Session.Account.HasLikedSong(song, () => {
                dislikeButton.IsEnabled = false;
                likeButton.IsEnabled = true;
            }, () => {
                Session.Account.HasDislikedSong(song, () => {
                    likeButton.IsEnabled = false;
                    dislikeButton.IsEnabled = true;
                }, () => {
                    likeButton.IsEnabled = true;
                    dislikeButton.IsEnabled = true;
                });
            });
        }

        /// <summary>
        /// Plays an AccountSong.
        /// </summary>
        /// <param name="accountSong">Account song to play</param>
        public void PlayAccountSong(AccountSong accountSong) {
            latestSongPlayed = null;
            if (latestAccountSongPlayed != null && latestAccountSongPlayed.AccountSongId == accountSong.AccountSongId) {
                PlayMemoryStream(new MemoryStream(latestStream.ToArray()));
                return;
            }
            latestAccountSongPlayed = accountSong;
            songNameTextBlock.Text = accountSong.Title;
            artistNameTextBlock.Text = "";
            MakeRequestStreamSong(Core.SERVER_API_URL + "/stream/accountsong/" + accountSong.AccountSongId);
            likeButton.Visibility = Visibility.Hidden;
            dislikeButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Waits until player is available and then creates a new
        /// request to server to stream and store the song data. Once the
        /// song it's been stored completely, the song will be played.
        /// </summary>
        /// <param name="streamUrl">Stream URL</param>
        private void MakeRequestStreamSong(string streamUrl) {
            isStreamSongLocked = false;
            Task.Run(() => {
                while (!isPlayerWaveOutAvailable) {
                    continue;
                }
                try {
                    latestStream = new List<byte>();
                    using (Stream memoryStream = new MemoryStream()) {
                        WebRequest webRequest = WebRequest.Create(streamUrl);
                        webRequest.Headers["Authorization"] = Session.AccessToken ?? "";
                        using (Stream stream = webRequest.GetResponse().GetResponseStream()) {
                            byte[] buffer = new byte[1024 * 1024];
                            int read;
                            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                                memoryStream.Write(buffer, 0, read);
                                latestStream.AddRange(buffer.ToList().GetRange(0, read).ToArray());
                            }
                        }
                        PlayMemoryStream(memoryStream);
                    }
                } catch (Exception exception) {
                    Console.WriteLine("Exception@PlayerPage->MakeRequestStreamSong() -> " + exception);
                    MessageBox.Show("Error al reproducir la canción.");
                    isPlayerWaveOutAvailable = true;
                }
            });
        }

        /// <summary>
        /// Attempts to play an audio stream.
        /// </summary>
        /// <param name="memoryStream">Audio stream to play</param>
        private void PlayMemoryStream(Stream memoryStream) {
            try {
                isPlayerWaveOutAvailable = false;
                isStreamSongLocked = true;
                isPlayerStopped = false;
                memoryStream.Position = 0;
                if (memoryStream.Length != latestStream.Count) {
                    latestStream = (memoryStream as MemoryStream).ToArray().ToList();
                }
                reader = new WaveFileReader(memoryStream);
                SetPlayerData(reader);
                Application.Current.Dispatcher.Invoke(delegate {
                    songDurationTimeTextBlock.Text = ((WaveFileReader) reader).TotalTime.ToString("mm\\:ss");
                });
                PlayStreamSong(reader);
            } catch (Exception) {
                Console.WriteLine("Exception@PlayerPage->PlayMemoryStream() -> Audio file is not .wav, trying with .mp3");
                try {
                    reader = new Mp3FileReader(memoryStream);
                    SetPlayerData(reader);
                    Application.Current.Dispatcher.Invoke(delegate {
                        songDurationTimeTextBlock.Text = ((Mp3FileReader) reader).TotalTime.ToString("mm\\:ss");
                    });
                    PlayStreamSong(reader);
                } catch (Exception exception) {
                    Console.WriteLine("Exception@PlayerPage->PlayMemoryStream() -> " + exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Stops and disposes the current song being played to play the new one
        /// and refreshes the player data every 1 second. If the song has ended, or
        /// a new song was requested to be played, the player data will be reset.
        /// </summary>
        /// <param name="reader">Reader that stores the song data</param>
        private void PlayStreamSong(IWaveProvider reader) {
            if (playerWaveOut != null && playerWaveOut.PlaybackState == PlaybackState.Playing) {
                playerWaveOut.Stop();
                playerWaveOut.Dispose();
                Application.Current.Dispatcher.Invoke(delegate {
                    playButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                    songSlider.IsEnabled = false;
                });
            }
            using (playerWaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback())) {
                playerWaveOut.Init(reader);
                playerWaveOut.Play();
                Application.Current.Dispatcher.Invoke(delegate {
                    playButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                    songSlider.IsEnabled = true;
                });
                while (playerWaveOut.PlaybackState == PlaybackState.Playing || isPlayerStopped) {
                    System.Threading.Thread.Sleep(1000);
                    if (!isStreamSongLocked) {
                        break;
                    }
                    if (!isPlayerStopped) {
                        SetPlayerData(reader);
                    }
                }
                Application.Current.Dispatcher.Invoke(delegate {
                    playButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                    songCurrentTimeTextBlock.Text = "00:00";
                    songDurationTimeTextBlock.Text = "00:00";
                    songSlider.Value = 0;
                    songSlider.IsEnabled = false;
                });
                isPlayerWaveOutAvailable = true;
            }
        }

        /// <summary>
        /// Updates the player data with the reader received.
        /// </summary>
        /// <param name="reader">Reader that stores the song data</param>
        private void SetPlayerData(IWaveProvider reader) {
            Application.Current.Dispatcher.Invoke(delegate {
                if (reader.GetType() == typeof(WaveFileReader)) {
                    var waveFileReader = (WaveFileReader) reader;
                    songCurrentTimeTextBlock.Text = waveFileReader.CurrentTime.ToString("mm\\:ss");
                    songSlider.Value = waveFileReader.CurrentTime.Ticks * songSlider.Maximum / waveFileReader.TotalTime.Ticks;
                } else if (reader.GetType() == typeof(Mp3FileReader)) {
                    var mp3FileReader = (Mp3FileReader) reader;
                    songCurrentTimeTextBlock.Text = mp3FileReader.CurrentTime.ToString("mm\\:ss");
                    songSlider.Value = mp3FileReader.CurrentTime.Ticks * songSlider.Maximum / mp3FileReader.TotalTime.Ticks;
                }
            });
        }

        /// <summary>
        /// If a song is being played and its current time is higher than 2 seconds,
        /// then it will rewind to the beginning. If there's no song being played, then
        /// the previous song played will be played.
        /// </summary>
        /// <param name="sender">Rewind button</param>
        /// <param name="e">Button event</param>
        private void RewindButton_Click(object sender, RoutedEventArgs e) {
            if (reader != null && !isPlayerWaveOutAvailable) {
                if (reader.GetType() == typeof(WaveFileReader)) {
                    var waveFileReader = (WaveFileReader) reader;
                    if (waveFileReader.CurrentTime.TotalSeconds > 2) {
                        waveFileReader.CurrentTime = TimeSpan.FromMilliseconds(0);
                        SetPlayerData(reader);
                    }
                } else if (reader.GetType() == typeof(Mp3FileReader)) {
                    var mp3FileReader = (Mp3FileReader) reader;
                    if (mp3FileReader.CurrentTime.TotalSeconds > 2) {
                        mp3FileReader.CurrentTime = TimeSpan.FromMilliseconds(0);
                        SetPlayerData(reader);
                    }
                }
            } else {
                List<int> songsIdPlayHistory = Session.SongsIdPlayHistory;
                if (songsIdPlayHistory.Count > 0) {
                    try {
                        // TODO: Check if next song in queue is an account song and play it.
                        Song.FetchById(songsIdPlayHistory.Last(), (song) => {
                            Session.PlayerPage.PlaySong(song);
                            if (Session.MainFrame.ToString().Split('/').Last().Equals("PlayHistoryPage.xaml")) {
                                PlayHistoryPage currentPage = Session.MainFrame.Content as PlayHistoryPage;
                                currentPage.LoadPlayHistory();
                            }
                        }, () => {
                            MessageBox.Show("Ocurrió un error al cargar la canción.");
                        });
                    } catch (Exception) {
                        MessageBox.Show("Ocurrió un error al cargar la canción.");
                    }
                }
            }
        }

        /// <summary>
        /// If the song is being played, then it will be paused. If the song was
        /// paused, then it will be resumed. If the song was stopped, then it will
        /// be played again from the beginning. This applies to a Song and to an AccountSong.
        /// </summary>
        /// <param name="sender">Play button</param>
        /// <param name="e">Button event</param>
        private void PlayButton_Click(object sender, RoutedEventArgs e) {
            if (playerWaveOut != null) {
                if (playerWaveOut.PlaybackState == PlaybackState.Playing) {
                    isPlayerStopped = true;
                    playerWaveOut.Pause();
                    playButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                } else if (playerWaveOut.PlaybackState == PlaybackState.Paused) {
                    isPlayerStopped = false;
                    playerWaveOut.Play();
                    playButtonIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                } else {
                    if (latestStream != null) {
                        Task.Run(() => {
                            PlayMemoryStream(new MemoryStream(latestStream.ToArray()));
                        });
                    }
                }
            }
        }

        /// <summary>
        /// If there are songs in play queue then the next song in play queue will be played
        /// and will be removed from this.
        /// </summary>
        /// <param name="sender">Forward button</param>
        /// <param name="e">Button event</param>
        private void ForwardButton_Click(object sender, RoutedEventArgs e) {
            List<int> songsIdPlayQueue = Session.SongsIdPlayQueue;
            if (songsIdPlayQueue.Count > 0) {
                try {
                    // TODO: Check if next song in queue is an account song and play it.
                    Song.FetchById(songsIdPlayQueue.ElementAt(0), (song) => {
                        Session.PlayerPage.PlaySong(song);
                        Session.SongsIdPlayQueue.RemoveAt(0);
                        if (Session.MainFrame.ToString().Split('/').Last().Equals("PlayHistoryPage.xaml")) {
                            PlayHistoryPage currentPage = Session.MainFrame.Content as PlayHistoryPage;
                            currentPage.LoadPlayHistory();
                        } else {
                            if (Session.MainFrame.ToString().Split('/').Last().Equals("PlayQueuePage.xaml")) {
                                PlayQueuePage currentPage = Session.MainFrame.Content as PlayQueuePage;
                                currentPage.LoadPlayQueue();
                            }
                        }
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar la canción.");
                    });
                } catch (Exception) {
                    MessageBox.Show("Ocurrió un error al cargar la canción.");
                }
            }
        }

        /// <summary>
        /// If there's a song being played and the player slider was dragged,
        /// then the song will play from the point where dragging finished.
        /// </summary>
        /// <param name="sender">Player slider</param>
        /// <param name="e">Slider event</param>
        private void SongSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) {
            if (reader != null && !isPlayerWaveOutAvailable) {
                if (reader.GetType() == typeof(WaveFileReader)) {
                    var waveFileReader = (WaveFileReader) reader;
                    waveFileReader.CurrentTime = TimeSpan.FromMilliseconds(((songSlider.Value * waveFileReader.TotalTime.Ticks / songSlider.Maximum) / TimeSpan.TicksPerSecond) * 1000);
                } else if (reader.GetType() == typeof(Mp3FileReader)) {
                    var mp3FileReader = (Mp3FileReader) reader;
                    mp3FileReader.CurrentTime = TimeSpan.FromMilliseconds(((songSlider.Value * mp3FileReader.TotalTime.Ticks / songSlider.Maximum) / TimeSpan.TicksPerSecond) * 1000);
                }
            }
        }

        /// <summary>
        /// Shows up a consult artist page with the current artist.
        /// </summary>
        /// <param name="sender">TextBlock</param>
        /// <param name="e">Event</param>
        private void ArtistNameTextBlock_MouseDown(object sender, MouseButtonEventArgs e) {
            if (string.IsNullOrEmpty(artistNameTextBlock.Text)) {
                return;
            }
            Session.MainWindow.mainFrame.Navigate(new ConsultArtistPage(latestSongPlayed.Album.Artists[0]));
            Session.MainWindow.TitleBar.Text = latestSongPlayed.Album.Artists[0].ArtisticName;
        }

        /// <summary>
        /// Likes or unlikes the current song.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void LikeButton_Click(object sender, RoutedEventArgs e) {
            try {
                if (dislikeButton.IsEnabled) {
                    Session.Account.LikeSong(latestSongPlayed, () => {
                        dislikeButton.IsEnabled = false;
                    }, () => {
                        MessageBox.Show("Ocurrió un error al procesar tu solicitud.");
                    });
                } else {
                    Session.Account.UnlikeSong(latestSongPlayed, () => {
                        dislikeButton.IsEnabled = true;
                    }, () => {
                        MessageBox.Show("Ocurrió un error al procesar tu solicitud.");
                    });
                }
            } catch (Exception) {
                MessageBox.Show("Ocurrió un error al procesar tu solicitud.");
            }
        }

        /// <summary>
        /// Dislikes or undislikes the current song.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void DislikeButton_Click(object sender, RoutedEventArgs e) {
            try {
                if (likeButton.IsEnabled) {
                    Session.Account.DislikeSong(latestSongPlayed, () => {
                        likeButton.IsEnabled = false;
                    }, () => {
                        MessageBox.Show("Ocurrió un error al procesar tu solicitud.");
                    });
                } else {
                    Session.Account.UndislikeSong(latestSongPlayed, () => {
                        likeButton.IsEnabled = true;
                    }, () => {
                        MessageBox.Show("Ocurrió un error al procesar tu solicitud.");
                    });
                }
            } catch (Exception) {
                MessageBox.Show("Ocurrió un error al procesar tu solicitud.");
            }
        }
    }
}
