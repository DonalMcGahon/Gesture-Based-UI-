using Myo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MyoApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
   
    {
        private readonly global::Myo.Myo _myo;

        public MainPage()
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
                {});


        }

        private void _myo_OnEMGAvailable(object sender, Myo.MyoEMGEventArgs e)
        {
            Debug.WriteLine("EMG");
            Debug.WriteLine("Channel #1: " + e.Channel1);
            Debug.WriteLine("Channel #4: " + e.Channel4);
            Debug.WriteLine("Channel #6: " + e.Channel6);
        }

        private void _myo_OnPoseDetected(object sender, Myo.MyoPoseEventArgs e)
        {
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
            this.Frame.Navigate(typeof(guitar));
        }

        private void MenuButton3_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(drums));
        }

        private void MenuButton4_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(trumpet));
        }
    }
}
