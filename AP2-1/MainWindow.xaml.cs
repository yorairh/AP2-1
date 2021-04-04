using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Win32;
using System.IO;

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
            // fileData = File.ReadAllLines(pathToFile);
        }
    }
}
