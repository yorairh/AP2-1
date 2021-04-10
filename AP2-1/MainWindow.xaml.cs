using System;
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
        private string pathToXML;

        private static readonly double JOYSTICK_RATIO = 3; // Joystick size / JoystickHandleSize
        private void SetJoystick()
        {
            JoystickHandle.Height = Joystick.Height / JOYSTICK_RATIO;
            JoystickHandle.Width = Joystick.Width / JOYSTICK_RATIO;
            Canvas.SetTop(JoystickHandle, Canvas.GetTop(Joystick) + (Joystick.Height) / JOYSTICK_RATIO);
            Canvas.SetLeft(JoystickHandle, Canvas.GetLeft(Joystick) + (Joystick.Width) / JOYSTICK_RATIO);
        }

        private void SetRudder()
        {
            RudderTracker.Height = RudderLayout.Height + 4;
            RudderTracker.Width = RudderLayout.Height + 4;
            Canvas.SetTop(RudderTracker, Canvas.GetTop(RudderLayout) - 2);
            Canvas.SetLeft(RudderTracker, Canvas.GetLeft(RudderLayout) + RudderLayout.Width / 2 - RudderTracker.Width / 2);
        }

        private void SetThrottle()
        {
            ThrottleTracker.Height = ThrottleLayout.Width + 4;
            ThrottleTracker.Width = ThrottleLayout.Width + 4;
            Canvas.SetTop(ThrottleTracker, Canvas.GetTop(ThrottleLayout) + ThrottleLayout.Height / 2 - ThrottleTracker.Height / 2);
            Canvas.SetLeft(ThrottleTracker, Canvas.GetLeft(ThrottleLayout) - 2);
        }

        public MainWindow()
        {
            InitializeComponent();
            SetJoystick();
            SetRudder();
            SetThrottle();

            vm = new FlightSimulatorViewModel(new FlightSimulatorModel());

            vm.notifyPropertyChanged += (object sender, EventArgs e) => {
                if (e as CSVFileUploadEventArgs != null)
                {
                    CSVFileUploadEventArgs args = e as CSVFileUploadEventArgs;
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
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate () {
                            // the last 2 in every formula can be changed to JOYSTICK_RATIO in order to get the sensitivity fit the size of the joystick
                            int leftJoystick = (int)(Canvas.GetLeft(Joystick) + (Joystick.Width - JoystickHandle.Width) / 2 + (Joystick.Width - JoystickHandle.Width) * (args.Aileron) / 2),
                                topJoystick = (int)(Canvas.GetTop(Joystick) + (Joystick.Height - JoystickHandle.Height) / 2 + (Joystick.Height - JoystickHandle.Height) * (args.Elevator) / 2);
                            Canvas.SetLeft(JoystickHandle, leftJoystick);
                            Canvas.SetTop(JoystickHandle, topJoystick);
                            int leftRudder = (int)(Canvas.GetLeft(RudderLayout) + (RudderLayout.Width - RudderTracker.Width) / 2 + (RudderLayout.Width - RudderTracker.Width) * (args.Rudder) / 2);
                            Canvas.SetLeft(RudderTracker, leftRudder);
                            int topThrottle = (int)(Canvas.GetTop(ThrottleLayout) + (ThrottleLayout.Height - ThrottleTracker.Height) / 2 - (ThrottleLayout.Height - ThrottleTracker.Height) * (args.Throttle) / 2);
                            Canvas.SetTop(ThrottleTracker, topThrottle);
                            tbHeight.Text = args.Altimeter.ToString();
                            if (args.Roll < 0.0)
                            {
                                recRoll.Height = (int)((360 - args.Roll) / 3);
                            } else
                            {
                                recRoll.Height = (int)(args.Roll / 3);
                            }
                            if (args.Pitch < 0.0)
                            {
                                recPitch.Height = (int)((360 - args.Pitch) / 3);
                            }
                            else
                            {
                                recPitch.Height = (int)(args.Pitch / 3);
                            }
                            if (args.Yaw < 0.0)
                            {
                                recYaw.Height = (int)((360 - args.Yaw) / 3);
                            }
                            else
                            {
                                recYaw.Height = (int)(args.Yaw / 3);
                            }
                            
                        });
                    }
                }
                if (e as XMLFileUploadEventArgs != null)
                {
                    XMLFileUploadEventArgs args = e as XMLFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        foreach (string c in args.Categories)
                        {
                            propertyMenu.Items.Add(c);
                        }
                    }
                }
                // more....
            };
        }

        private void CSV_Click(object sender, RoutedEventArgs e)
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
                    gridControl.Visibility = Visibility.Visible;
                    LayoutInfo.Visibility = Visibility.Visible;
                } else { 
                    tbPath.Text = "The file you entered is not a .csv file. Please Enter a .csv file.";
                    tbSuccess.Text = "";
                    btnUpload.Visibility = Visibility.Hidden;
                    gridControl.Visibility = Visibility.Hidden;
                    LayoutInfo.Visibility = Visibility.Hidden;
                }
            }
        }

        private void XML_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            bool? result = fd.ShowDialog();
            if (result == true)
            {
                string path = fd.FileName;
                if (path.EndsWith(".xml"))
                {
                    pathToXML = fd.FileName;
                    tbPath.Text = "Please Enter a .csv file";
                    btnCSV.Visibility = Visibility.Visible;
                    btnXML.Visibility = Visibility.Hidden;
                }
                else
                {
                    tbPath.Text = "The file you entered is not a .xml file. Please Enter a .xml file.";
                }
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            // upload the file
            vm.UploadFile(pathToFile, pathToXML);
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


        private void SldrTime_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            vm.SetPause(true);
        }

        private void SldrTime_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            int val = (int) ((Slider)sender).Value;
            vm.SetTime(val);
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

        private void propertyMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender as MenuItem != null)
            {
                MenuItem chosenItem = sender as MenuItem;
                vm.SetCurrentCategory(chosenItem.Header.ToString());
            }
        }
    }  
}
