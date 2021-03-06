﻿using Musify.Models;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Musify.Pages {
    /// <summary>
    /// Interaction logic for PlayerPage.xaml
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
        public Song LatestSongPlayed { get; set; }
        /// <summary>
        /// Stores the latest stream retrieved from server.
        /// </summary>
        private List<byte> latestStream;
        /// <summary>
        /// Stores the latest account song played.
        /// </summary>
        public AccountSong LatestAccountSongPlayed { get; set; }
        /// <summary>
        /// Checks if player was stopped or not.
        /// </summary>
        private bool isPlayerStopped = false;
        /// <summary>
        /// True if there's no song requests; false if a new one is asking to be played.
        /// </summary>
        public bool IsStreamSongLocked { get; set; } = false;
        /// <summary>
        /// True if player is playing a song; false if not.
        /// </summary>
        private bool isPlayerWaveOutAvailable = true;
        /// <summary>
        /// True if should play next song in queue when a song or an account song is about to be played; false if not.
        /// </summary>
        public bool ShouldPlayNextSong { get; set; } = true;

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
            if (LatestSongPlayed != null) {
                if (Session.SongsIdPlayHistory.Count == Core.MAX_SONGS_IN_HISTORY) {
                    Session.SongsIdPlayHistory.RemoveAt(0);
                }
                Session.SongsIdPlayHistory.Add(LatestSongPlayed.SongId);
            }
            if (LatestAccountSongPlayed != null) {
                if (Session.SongsIdPlayHistory.Count == Core.MAX_SONGS_IN_HISTORY) {
                    Session.SongsIdPlayHistory.RemoveAt(0);
                }
                Session.SongsIdPlayHistory.Add(LatestAccountSongPlayed.AccountSongId * -1);
            }
            LatestAccountSongPlayed = null;
            if (LatestSongPlayed != null && LatestSongPlayed.SongId == song.SongId && latestStream != null) {
                Task.Run(() => {
                    PlayMemoryStream(new MemoryStream(latestStream.ToArray()), false);
                });
                return;
            }
            LatestSongPlayed = song;
            songNameTextBlock.Text = song.Title;
            artistNameTextBlock.Text = song.Album.GetArtistsNames();
            if (song.IsDownloaded()) {
                Task.Run(() => {
                    PlayMemoryStream(song.CreateDownloadedFileStream(), false);
                });
            } else {
                if (Session.SongStreamingQuality == "automaticquality") {
                    MakeRequestStreamSong(
                        Core.SERVER_API_URL + "/stream/song/" + song.SongId + Path.AltDirectorySeparatorChar + 
                        Session.SongStreamingQualitySelected
                    );
                } else {
                    MakeRequestStreamSong(
                        Core.SERVER_API_URL + "/stream/song/" + song.SongId + Path.AltDirectorySeparatorChar + 
                        Session.SongStreamingQuality
                    );
                }
            }
            likeButton.Visibility = Visibility.Visible;
            dislikeButton.Visibility = Visibility.Visible;
            Session.Account.HasLikedSong(song, () => {
                dislikeButton.IsEnabled = false;
                likeButton.IsEnabled = true;
            }, (errorResponse) => {
                Session.Account.HasDislikedSong(song, () => {
                    likeButton.IsEnabled = false;
                    dislikeButton.IsEnabled = true;
                }, (errorResponse2) => {
                    likeButton.IsEnabled = true;
                    dislikeButton.IsEnabled = true;
                }, null);
            }, null);
        }

        /// <summary>
        /// Plays an AccountSong.
        /// </summary>
        /// <param name="accountSong">Account song to play</param>
        public void PlayAccountSong(AccountSong accountSong) {
            if (LatestAccountSongPlayed != null) {
                if (Session.SongsIdPlayHistory.Count == Core.MAX_SONGS_IN_HISTORY) {
                    Session.SongsIdPlayHistory.RemoveAt(0);
                }
                Session.SongsIdPlayHistory.Add(LatestAccountSongPlayed.AccountSongId * -1);
            }
            if (LatestSongPlayed != null) {
                if (Session.SongsIdPlayHistory.Count == Core.MAX_SONGS_IN_HISTORY) {
                    Session.SongsIdPlayHistory.RemoveAt(0);
                }
                Session.SongsIdPlayHistory.Add(LatestSongPlayed.SongId);
            }
            LatestSongPlayed = null;
            if (LatestAccountSongPlayed != null && LatestAccountSongPlayed.AccountSongId == accountSong.AccountSongId) {
                PlayMemoryStream(new MemoryStream(latestStream.ToArray()), false);
                return;
            }
            LatestAccountSongPlayed = accountSong;
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
            IsStreamSongLocked = false;
            Task.Run(() => {
                while (!isPlayerWaveOutAvailable) {
                    Thread.Sleep(100);
                }
                isPlayerWaveOutAvailable = false;
                IsStreamSongLocked = true;
                isPlayerStopped = false;
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
                        PlayMemoryStream(memoryStream, true);
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
        private void PlayMemoryStream(Stream memoryStream, bool fromRequest) {
            try {
                if (!fromRequest) {
                    IsStreamSongLocked = false;
                    while (!isPlayerWaveOutAvailable) {
                        Thread.Sleep(100);
                    }
                }
                isPlayerWaveOutAvailable = false;
                IsStreamSongLocked = true;
                isPlayerStopped = false;
                ShouldPlayNextSong = true;
                memoryStream.Position = 0;
                if (latestStream == null || memoryStream.Length != latestStream.Count) {
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
            try {
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
                        Thread.Sleep(1000);
                        if (!IsStreamSongLocked) {
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
                    if (ShouldPlayNextSong) {
                        PlayNextSong();
                    }
                    ShouldPlayNextSong = true;
                    isPlayerWaveOutAvailable = true;
                }
            } catch (Exception) {
                MessageBox.Show("Error al reproducir la canción.");
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
                    if (waveFileReader.CurrentTime.TotalSeconds >= 3) {
                        waveFileReader.CurrentTime = TimeSpan.FromMilliseconds(0);
                        SetPlayerData(reader);
                    } else {
                        PreviousSong();
                    }
                } else if (reader.GetType() == typeof(Mp3FileReader)) {
                    var mp3FileReader = (Mp3FileReader) reader;
                    if (mp3FileReader.CurrentTime.TotalSeconds >= 3) {
                        mp3FileReader.CurrentTime = TimeSpan.FromMilliseconds(0);
                        SetPlayerData(reader);
                    } else {
                        PreviousSong();
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to play the previous played song.
        /// </summary>
        public void PreviousSong() {
            if (Session.HistoryIndex >= 0) {
                try {
                    if (Session.SongsIdPlayHistory.ElementAt(Session.HistoryIndex) > 0) {
                        Song.FetchById(Session.SongsIdPlayHistory.ElementAt(Session.HistoryIndex), (song) => {
                            Session.HistoryIndex--;
                            if (Session.SongsIdPlayHistory.Count == Core.MAX_SONGS_IN_HISTORY) {
                                Session.HistoryIndex--;
                            }
                            ShouldPlayNextSong = false;
                            Session.PlayerPage.PlaySong(song);
                            RefreshPage(false);
                        }, (errorResponse) => {
                            MessageBox.Show(errorResponse.Message);
                        }, () => {
                            MessageBox.Show("Ocurrió un error al cargar la canción.");
                        });
                    } else {
                        AccountSong.FetchById(Session.SongsIdPlayHistory.ElementAt(Session.HistoryIndex) * -1, (accountSong) => {
                            Session.HistoryIndex--;
                            if (Session.SongsIdPlayHistory.Count == Core.MAX_SONGS_IN_HISTORY) {
                                Session.HistoryIndex--;
                            }
                            ShouldPlayNextSong = false;
                            Session.PlayerPage.PlayAccountSong(accountSong);
                            RefreshPage(true);
                        }, (errorResponse) => {
                            MessageBox.Show(errorResponse.Message);
                        }, () => {
                            MessageBox.Show("Ocurrió un error al cargar la canción.");
                        });
                    }
                } catch (Exception) {
                    MessageBox.Show("Ocurrió un error al cargar la canción.");
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
                            PlayMemoryStream(new MemoryStream(latestStream.ToArray()), false);
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
            ShouldPlayNextSong = false;
            PlayNextSong();
        }

        /// <summary>
        /// Attempts to play the next song in queue.
        /// </summary>
        private void PlayNextSong() {
            Application.Current.Dispatcher.Invoke(() => {
                if (reader != null && !isPlayerWaveOutAvailable && (Session.SongsIdPlayQueue.Count > 0 || Session.SongsIdSongList.Count > 0)) {
                    int id;
                    if (Session.SongsIdPlayQueue.Count > 0) {
                        id = Session.SongsIdPlayQueue.First();
                    } else {
                        id = Session.SongsIdSongList.First();
                    }
                    Action update = () => {
                        Session.HistoryIndex = Session.SongsIdPlayHistory.Count - 1;
                        if (Session.SongsIdPlayQueue.Count > 0) {
                            Session.SongsIdPlayQueue.RemoveAt(0);
                        } else {
                            Session.SongsIdSongList.RemoveAt(0);
                        }
                        RefreshPage(true);
                    };
                    if (id > 0) {
                        Song.FetchById(id, (song) => {
                            Session.PlayerPage.PlaySong(song);
                            update();
                        }, (errorResponse) => {
                            MessageBox.Show(errorResponse.Message);
                        }, () => {
                            MessageBox.Show("Ocurrió un error al cargar la canción.");
                        });
                    } else {
                        AccountSong.FetchById(id * -1, (accountSong) => {
                            Session.PlayerPage.PlayAccountSong(accountSong);
                            update();
                        }, (errorResponse) => {
                            MessageBox.Show(errorResponse.Message);
                        }, () => {
                            MessageBox.Show("Ocurrió un error al cargar la canción.");
                        });
                    }
                }
            });
        }

        /// <summary>
        /// Refresh current page if it's history or play queue page.
        /// </summary>
        public void RefreshPage(bool nextSong) {
            if (Session.MainWindow.mainFrame.ToString().Split('/').Last().Equals("PlayHistoryPage.xaml")) {
                PlayHistoryPage currentPage = Session.MainWindow.mainFrame.Content as PlayHistoryPage;
                currentPage.LoadPlayHistory();
            } else {
                if (Session.MainWindow.mainFrame.ToString().Split('/').Last().Equals("PlayQueuePage.xaml") && nextSong) {
                    PlayQueuePage currentPage = Session.MainWindow.mainFrame.Content as PlayQueuePage;
                    currentPage.LoadPlayQueue();
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
            Session.MainWindow.mainFrame.Navigate(new ConsultArtistPage(LatestSongPlayed.Album.Artists[0]));
            Session.MainWindow.titleBarTextBlock.Text = LatestSongPlayed.Album.Artists[0].ArtisticName;
        }

        /// <summary>
        /// Likes or unlikes the current song.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void LikeButton_Click(object sender, RoutedEventArgs e) {
            try {
                if (dislikeButton.IsEnabled) {
                    Session.Account.LikeSong(LatestSongPlayed, () => {
                        dislikeButton.IsEnabled = false;
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al procesar tu solicitud.");
                    });
                } else {
                    Session.Account.UnlikeSong(LatestSongPlayed, () => {
                        dislikeButton.IsEnabled = true;
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
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
                    Session.Account.DislikeSong(LatestSongPlayed, () => {
                        likeButton.IsEnabled = false;
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al procesar tu solicitud.");
                    });
                } else {
                    Session.Account.UndislikeSong(LatestSongPlayed, () => {
                        likeButton.IsEnabled = true;
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
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
