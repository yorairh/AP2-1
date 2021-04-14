using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using OxyPlot.Series;
using System.IO;
using System.Windows.Data;

namespace AP2_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMainViewModel mainVM;
        private IFlightDataViewModel flightDataVM;
        private ITimeManagerViewModel timeManagerVM;
        private IGraphViewModel graphVM;

        private string pathToAnomalyFile;
        private string pathToLearningFile;
        private string pathToXML;

        private bool hasXML = false, hasCSVAnomaly = false, hasCSVLearning = false;

        private readonly string PLUGINS_DIR = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\plugins";

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

        private void SetPluginsDir()
        {
            foreach (string file in Directory.GetFiles(PLUGINS_DIR))
            {
                var f = Path.GetFileName(file);
                if (f != "InnerCircle.dll" && f != "LinearRegression.dll" && f != "StringWrapper.dll")
                {
                    File.Delete(file);
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            SetJoystick();
            SetRudder();
            SetThrottle();
            SetPluginsDir();

            // vm = new FlightSimulatorViewModel(new FlightSimulatorModel());

            // create vm's

            MainModel mainModel = new MainModel();
            GraphModel gModel = new GraphModel(mainModel);
            graphVM = new GraphViewModel(gModel);
            FlightDataModel fdModel = new FlightDataModel(mainModel);
            flightDataVM = new FlightDataViewModel(fdModel);
            TimeManagerModel tmModel = new TimeManagerModel(mainModel);
            timeManagerVM = new TimeManagerViewModel(tmModel);
            mainVM = new MainViewModel(mainModel);
            mainModel.SetFlightDataModel(fdModel);
            mainModel.SetGraphModel(gModel);
            mainModel.SetTimeManagerModel(tmModel);

            // data contexts
            tbTime.DataContext = timeManagerVM;
            sldrTime.DataContext = mainVM;
            tbHeight.DataContext = flightDataVM;
            tbAirSpeed.DataContext = flightDataVM;

            CurrCategoryPlot.DataContext = gModel;
            CurrCorrelatedCategoryPlot.DataContext = gModel;
            CorrelatedAsFuncOfCurrent.DataContext = gModel;
            AnomaliesTable.DataContext = gModel;

            File.Copy(PLUGINS_DIR + "/LinearRegression.dll", LibraryManager.LIBRARY_PATH, true);
            File.Copy(PLUGINS_DIR + "/LinearRegression.dll", LibraryManager.LINEAR_LIBRARY_PATH, true);
            File.Copy(PLUGINS_DIR + "/StringWrapper.dll", StringWrapper.STRING_LIBRARY_PATH, true);

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            timeManagerVM.notifyPropertyChanged += (object sender, EventArgs e) =>
            {
                if (e as TimeChangedEventArgs != null)
                {
                    TimeChangedEventArgs args = e as TimeChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.TimeChanged)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart) delegate ()
                        {
                            sldrTime.Value = args.Seconds;
                            tbTime.Text = args.NewTime;
                        });
                    }
                }
            };

            flightDataVM.notifyPropertyChanged += (object sender, EventArgs e) =>
            {
                if (e as InformationChangedEventArgs != null)
                {
                    InformationChangedEventArgs args = e as InformationChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.InfoChanged)
                    {
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                        {
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
                            tbAirSpeed.Text = args.AirSpeed.ToString();
                            angleOfRoll.Angle = args.Roll;
                            angleOfPitch.Angle = args.Pitch;
                            angleOfYaw.Angle = args.Yaw;
                            orientation.Angle = args.Orientation;
                        });
                    }
                }
            };

            mainVM.notifyPropertyChanged += (object sender, EventArgs e) =>
            {
                if (e as XMLFileUploadEventArgs != null)
                {
                    XMLFileUploadEventArgs args = e as XMLFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        foreach (string c in args.Categories)
                        {
                            MenuItem item = new MenuItem
                            {
                                Header = c
                            };
                            item.Click += MenuItem_Click;
                            // mainVM.CategoriesMenu.Add(item);
                            Props.Items.Add(item);
                        }
                    }
                }
                if (e as CSVAnomaliesFileUploadEventArgs != null)
                {
                    CSVAnomaliesFileUploadEventArgs args = e as CSVAnomaliesFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        sldrTime.Maximum = args.Length;
                    }
                }
                // more....
            };
            MessageBox.Show("Welcome to our Flight Tracker\nTo start open flightgear (Make sure you already put your xml inside" +
                " the protocols folder in the flightgear folder) now click on fly\nNow in the window that will open after you press ok upload all the three files - click on settings" +
                " and then choose the files to upload (one by one to their category) then click on upload and you are good to go!", "Welcome!");

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mi = sender as MenuItem;
            if (mi != null)
            {
                mainVM.SetCurrentCategory(mi.Header as string);
                var items = AnomaliesTable.ItemsSource;
                AnomaliesTable.ItemsSource = null;
                AnomaliesTable.ItemsSource = items;
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            graphVM?.UpdateGraph();
            if (CurrCategoryPlot?.Model?.Series != null)
            {
                CurrCategoryPlot.InvalidatePlot(true);
            }
            if (CurrCorrelatedCategoryPlot?.Model?.Series != null)
            {
                CurrCorrelatedCategoryPlot.InvalidatePlot(true);
            }
            if (CorrelatedAsFuncOfCurrent?.Model?.Series != null)
            {
                CorrelatedAsFuncOfCurrent.InvalidatePlot(true);
            }
        }

        private void CSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            bool? result = fd.ShowDialog();
            if (result == true)
            {
                string path = fd.FileName;
                if (path.EndsWith(".csv"))
                {
                    pathToAnomalyFile = fd.FileName;
                    hasCSVAnomaly = true;
                    SetUploadButton();
                }
                else
                {
                    // tbPath.Text = "The file you entered is not a .csv file. Please Enter a .csv file.";
                    MessageBox.Show("The file you entered is not a .csv file. Please Enter a .csv file.");
                    hasCSVAnomaly = false;
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
                    hasXML = true;
                    SetUploadButton();
                }
                else
                {
                    MessageBox.Show("The file you entered is not a .xml file. Please Enter a .xml file.");
                    hasXML = false;
                }
            }
        }

        private void SetUploadButton()
        {
            if (hasXML && hasCSVAnomaly && hasCSVLearning)
            {
                btnUpload.Visibility = Visibility.Visible;
            } else
            {
                btnUpload.Visibility = Visibility.Hidden;
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            // upload the file
            mainVM.UploadFile(pathToAnomalyFile, pathToXML, pathToLearningFile);
            
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            timeManagerVM.SetPause(false);
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            timeManagerVM.SetPause(true);
        }

        private void BtnFastBackward_Click(object sender, RoutedEventArgs e)
        {
            timeManagerVM.Jump(-50);
        }

        private void BtnFastForward_Click(object sender, RoutedEventArgs e)
        {
            timeManagerVM.Jump(50);
        }


        private void SldrTime_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            timeManagerVM.SetPause(true);
        }

        private void SldrTime_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            int val = (int)((Slider)sender).Value;
            timeManagerVM.SetTime(val);
            timeManagerVM.SetPause(false);
        }

        private void BtnSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(tbSpeed.Text, out double speed) || speed > 10 || speed <= 0)
            {
                tbSpeed.Text = "1.0";
            }
            else
            {
                tbSpeed.Text = Math.Round(speed, 1, MidpointRounding.AwayFromZero).ToString();
                timeManagerVM.SetSpeed(Math.Round(speed, 1, MidpointRounding.AwayFromZero));
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
            mainVM.Exit();
        }

        private void propertyMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            mainVM?.SetCurrentCategory((cb.SelectedItem as ComboBoxItem).Content.ToString());
        }

        private void CSVLearningMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            bool? result = fd.ShowDialog();
            if (result == true)
            {
                string path = fd.FileName;
                if (path.EndsWith(".csv"))
                {
                    pathToLearningFile = fd.FileName;
                    hasCSVLearning = true;
                    SetUploadButton();
                }
                else
                {
                    MessageBox.Show("The file you entered is not a .csv file. Please Enter a .csv file.");
                    hasCSVLearning = false;
                }
            }
        }

        private void AddDLL_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            bool? result = fd.ShowDialog();
            if (result == true)
            {
                string path = fd.FileName;
                if (path.EndsWith(".dll"))
                {
                    string withoutExt = Path.GetFileNameWithoutExtension(path);
                    string name = Path.GetFileName(path);
                    File.Copy(path, Path.Combine(PLUGINS_DIR, name));
                    MenuItem item = new MenuItem
                    {
                        Header = withoutExt
                    };
                    item.Click += DllChoosed_Click;
                    DLLMenu.Items.Insert(DLLMenu.Items.Count - 2, item);
                }
                else
                {
                    MessageBox.Show("The file you entered is not a .dll file. Please try again.");
                }
            }
        }

        private void DllChoosed_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            LibraryManager.SetLibrary(item.Header as string);
        }
    }
}
