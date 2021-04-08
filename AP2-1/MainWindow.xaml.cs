﻿using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Controls;

namespace AP2_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        IViewModel vm;
        private string pathToFile;

        private void SetJoystick()
        {
            double ratio = 2; // Joystick size / JoystickHandleSize
            JoystickHandle.Height = Joystick.Height / ratio;
            JoystickHandle.Width = Joystick.Width / ratio;
            Canvas.SetTop(JoystickHandle, Canvas.GetTop(Joystick) + (Joystick.Height - JoystickHandle.Height) / 2);
            Canvas.SetLeft(JoystickHandle, Canvas.GetLeft(Joystick) + (Joystick.Width - JoystickHandle.Width) / 2);
        }

        public MainWindow()
        {
            InitializeComponent();
            SetJoystick();

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
                if (e as InformationChangedEventArgs != null)
                {
                    InformationChangedEventArgs args = e as InformationChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.InfoChanged)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate () {
                            int left = (int)(Canvas.GetLeft(Joystick) + JoystickHandle.Width * (1 + args.Aileron) / 2);
                            Canvas.SetLeft(JoystickHandle, left);
                            int top = (int)(Canvas.GetTop(Joystick) + JoystickHandle.Height  * (1 + args.Elevator) / 2);
                            Canvas.SetTop(JoystickHandle, top);
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

        private bool dragStarted = false;

        private void SldrTime_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStarted = true;
        }

        private void SldrTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStarted)
            {
                vm.SetTime((int)e.NewValue);
            }
        }

        private void SldrTime_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            vm.SetPause(true);
            int val = (int) ((Slider)sender).Value;
            vm.SetTime(val);
            dragStarted = false;
            vm.SetPause(false);
        }

        private void BtnSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(tbSpeed.Text, out double speed) || speed > 10 || speed <= 0)
            {
                tbSpeed.Text = "1.0";
            }
            else
            {
                if (vm != null)
                {
                    tbSpeed.Text = Math.Round(speed, 1, MidpointRounding.AwayFromZero).ToString();
                    vm.SetSpeed(Math.Round(speed, 1, MidpointRounding.AwayFromZero));
                }
            }
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(tbSpeed.Text, out double speed))
            {
                tbSpeed.Text = "1.0";
            }
            else
            {
                if (speed - Math.Round(speed, 1, MidpointRounding.AwayFromZero) < 0.05 && speed - Math.Round(speed, 1, MidpointRounding.AwayFromZero) != 0)
                {
                    if (Math.Round(speed, 1, MidpointRounding.AwayFromZero) < speed)
                    {
                        speed = Math.Round(speed + 0.1, 1, MidpointRounding.AwayFromZero);
                    } 
                    else
                    {
                        speed = Math.Round(speed, 1, MidpointRounding.AwayFromZero);
                    }
                } 
                else
                {
                    speed = Math.Round(speed + 0.1, 1, MidpointRounding.AwayFromZero);
                }
                if (speed <= 10)
                {
                    tbSpeed.Text = speed.ToString();
                }
            }
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(tbSpeed.Text, out double speed))
            {
                tbSpeed.Text = "1.0";
            }
            else
            {
                if (speed - Math.Round(speed, 1, MidpointRounding.AwayFromZero) < 0.05 && speed - Math.Round(speed, 1, MidpointRounding.AwayFromZero) > 0)
                {
                    if (Math.Round(speed, 1, MidpointRounding.AwayFromZero) > speed)
                    {
                        speed = Math.Round(speed - 0.1, 1, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        speed = Math.Round(speed, 1, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    speed = Math.Round(speed - 0.1, 1, MidpointRounding.AwayFromZero);
                }
                if (speed > 0)
                {
                    tbSpeed.Text = speed.ToString();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.Exit();
        }
    }  
}
