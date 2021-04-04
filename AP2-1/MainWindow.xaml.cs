﻿using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Threading;

namespace AP2_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        IViewModel vm;
        private string pathToFile;

        public MainWindow()
        {
            InitializeComponent();
            vm = new FlightSimulatorViewModel(new FlightSimulatorModel());

            vm.notifyPropertyChanged += (object sender, EventArgs e) => {
                if (e as FileUploadEventArgs != null)
                {
                    FileUploadEventArgs args = e as FileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        tbSuccess.Text = "File uploaded successfully";
                        sldrTime.Maximum = args.Length;
                        gridControl.Visibility = Visibility.Visible;
                    }
                }
                if (e as TimeChangedEventArgs != null)
                {
                    TimeChangedEventArgs args = e as TimeChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.TimeChanged)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate () {
                            sldrTime.Value = args.Seconds;
                            tbTime.Text = args.NewTime;
                        });
                    }
                }
                // more....
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            bool? result = fd.ShowDialog();
            if (result == true)
            {
                string path = fd.FileName;
                if (path.EndsWith(".csv")) {
                    pathToFile = fd.FileName;
                    tbPath.Text = "Path to .csv file: " + pathToFile;
                    btnUpload.Visibility = Visibility.Visible;
                } else { 
                    tbPath.Text = "The file you entered is not a .csv file. Please Enter a .csv file.";
                    tbSuccess.Text = "";
                    btnUpload.Visibility = Visibility.Hidden;
                    gridControl.Visibility = Visibility.Hidden;
                }
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            // upload the file
            vm.UploadFile(pathToFile);
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            vm.SetPause(false);
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            vm.SetPause(true);
        }
        
        private void BtnFastBackward_Click(object sender, RoutedEventArgs e)
        {
            vm.Jump(-50);
        }

        private void BtnFastForward_Click(object sender, RoutedEventArgs e)
        {
            vm.Jump(50);
        }

        private void SldrTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            vm.SetTime((int) e.NewValue);
        }
    }
}
