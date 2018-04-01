using Myo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class guitar : Page, INotifyPropertyChanged
    {
        MediaCapture capture;
        InMemoryRandomAccessStream buffer;
        bool record;
        string filename;
        string audioFile = ".MP3";

        private readonly global::Myo.Myo _myo;

        public guitar()
        {
            this.InitializeComponent();

            _myo = new global::Myo.Myo();
            _myo.Connect();

            _myo.OnPoseDetected += _myo_OnPoseDetected;
            _myo.OnEMGAvailable += _myo_OnEMGAvailable;
            _myo.DataAvailable += _myo_DataAvailable;

        }

        private async void _myo_DataAvailable(object sender, MyoDataEventArgs e)
        {
            double x, y, z;
            x = e.Acceletometer.X;
            y = e.Acceletometer.Y;
            z = e.Acceletometer.Z;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                { });


        }

        private void _myo_OnEMGAvailable(object sender, Myo.MyoEMGEventArgs e)
        {
            Debug.WriteLine("EMG");
            Debug.WriteLine("Channel #1: " + e.Channel1);
            Debug.WriteLine("Channel #4: " + e.Channel4);
            Debug.WriteLine("Channel #6: " + e.Channel6);
        }


        private async void playPDS()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    g1.Play();
                }
                );

        }
        private async void playPD()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    g2.Play();
                }
                );

        }
        private async void playPGS()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    g3.Play();
                }
                );

        }
        private async void playPG()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    g4.Play();
                }
                );

        }
        private async void playPB()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    g5.Play();
                }
                );

        }
        private void _myo_OnPoseDetected(object sender, Myo.MyoPoseEventArgs e)
        {
            if (e.Pose == MyoPoseEventArgs.PoseType.Fist)
            {

                playPDS();
            }

            if (e.Pose == MyoPoseEventArgs.PoseType.DoubleTap)
            {

                playPD();
            }

            if (e.Pose == MyoPoseEventArgs.PoseType.WaveIn)
            {

                playPG();
            }

            if (e.Pose == MyoPoseEventArgs.PoseType.WaveOut)
            {

                playPGS();
            }

            if (e.Pose == MyoPoseEventArgs.PoseType.FingersSpread)
            {

                playPB();
            }


            switch (e.Pose)
            {
                case MyoPoseEventArgs.PoseType.Rest:
                    Debug.WriteLine("Rest");

                    break;
                case MyoPoseEventArgs.PoseType.Fist:
                    Debug.WriteLine("Fist");

                    break;
                case MyoPoseEventArgs.PoseType.WaveIn:
                    Debug.WriteLine("Wave In");

                    break;
                case MyoPoseEventArgs.PoseType.WaveOut:
                    Debug.WriteLine("Wave Out");

                    break;
                case MyoPoseEventArgs.PoseType.DoubleTap:
                    Debug.WriteLine("Double Tap");

                    break;
                case MyoPoseEventArgs.PoseType.FingersSpread:
                    Debug.WriteLine("Fingers Spread");

                    break;
                default:
                    break;
            }


        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void MenuButton1_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(piano));
        }

        private void MenuButton2_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void MenuButton3_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(drums));
        }

        private void MenuButton4_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(trumpet));
        }


        private async Task<bool> RecordProcess()
        {
            if (buffer != null)
            {
                buffer.Dispose();
            }
            buffer = new InMemoryRandomAccessStream();
            if (capture != null)
            {
                capture.Dispose();
            }
            try
            {
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Audio
                };
                capture = new MediaCapture();
                await capture.InitializeAsync(settings);
                capture.RecordLimitationExceeded += (MediaCapture sender) =>
                {
                    record = false;
                    throw new Exception("Record Limitation Exceeded ");
                };
                capture.Failed += (MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs) =>
                {
                    record = false;
                    throw new Exception(string.Format("Code: {0}. {1}", errorEventArgs.Code, errorEventArgs.Message));
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == typeof(UnauthorizedAccessException))
                {
                    throw ex.InnerException;
                }
                throw;
            }
            return true;
        }
        public async Task PlayRecordedAudio(CoreDispatcher UiDispatcher)
        {
            MediaElement playback = new MediaElement();
            IRandomAccessStream audio = buffer.CloneStream();

            if (audio == null)
                throw new ArgumentNullException("buffer");
            StorageFolder storageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            if (!string.IsNullOrEmpty(filename))
            {
                StorageFile original = await storageFolder.GetFileAsync(filename);
                await original.DeleteAsync();
            }
            await UiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                StorageFile storageFile = await storageFolder.CreateFileAsync(audioFile, CreationCollisionOption.GenerateUniqueName);
                filename = storageFile.Name;
                using (IRandomAccessStream fileStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await RandomAccessStream.CopyAndCloseAsync(audio.GetInputStreamAt(0), fileStream.GetOutputStreamAt(0));
                    await audio.FlushAsync();
                    audio.Dispose();
                }
                IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.Read);
                playback.SetSource(stream, storageFile.FileType);
                playback.Play();
            });
        }
        private async void recordBtn_Click(object sender, RoutedEventArgs e)
        {
            if (record)
            {
                //already recored process
            }
            else
            {
                await RecordProcess();
                await capture.StartRecordToStreamAsync(MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Auto), buffer);
                if (record)
                {
                    throw new InvalidOperationException();
                }
                record = true;
            }

        }

        private async void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            await capture.StopRecordAsync();
            record = false;
        }

        private async void playBtn_Click(object sender, RoutedEventArgs e)
        {
            await PlayRecordedAudio(Dispatcher);
        }

    }
}
