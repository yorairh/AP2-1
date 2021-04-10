using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP2_1
{
    class FlightSimulatorViewModel : IViewModel
    {
        private IModel model;

        private PlotModel currCategoryPM;

        public event propertyChanged notifyPropertyChanged;

        public FlightSimulatorViewModel(IModel model)
        {
            this.model = model;
            this.currCategoryPM = new PlotModel();
            

            model.notifyPropertyChanged += (object sender, EventArgs e) => {
                if (e as CSVFileUploadEventArgs != null)
                {
                    CSVFileUploadEventArgs args = e as CSVFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                if (e as TimeChangedEventArgs != null)
                {
                    TimeChangedEventArgs args = e as TimeChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.TimeChanged)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                if (e as InformationChangedEventArgs != null)
                {
                    InformationChangedEventArgs args = e as InformationChangedEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.InfoChanged)
                    {
                        notifyPropertyChanged(this, args);
                        UpdateGraph();
                    }
                }
                if (e as XMLFileUploadEventArgs != null)
                {
                    XMLFileUploadEventArgs args = e as XMLFileUploadEventArgs;
                    if (args.Info == PropertyChangedEventArgs.InfoVal.FileUpdated)
                    {
                        notifyPropertyChanged(this, args);
                    }
                }
                // more....
            };
        }

        public void UpdateGraph()
        {
            var data = model.GetRelevantData();
            currCategoryPM.Axes.Clear();
            var xAxes = new LinearAxis
            {
                Title = "Time",
                Minimum = -5,
                Maximum = 35,
                Position = AxisPosition.Bottom
            };
            currCategoryPM.Axes.Add(xAxes);
            var yAxes = new LinearAxis()
            {
                Title = model.GetCurrentCategory(),
                Minimum = model.GetCurrentCategoryMinimum() - 5,
                Maximum = model.GetCurrentCategoryMaximum() + 5,
                Position = AxisPosition.Left
            };
            currCategoryPM.Axes.Add(yAxes);
            ScatterSeries series = new ScatterSeries
            {
                MarkerType = MarkerType.Circle
            };
            List<ScatterPoint> points = new List<ScatterPoint>();
            for (int i = 0; i < data.Count; ++i)
            {
                points.Add(new ScatterPoint(i, data.ElementAt(i)));
            }
            series.ItemsSource = points;
            currCategoryPM.Series.Add(series);
        } 

        public void UploadFile(string pathCSV, string pathXML)
        {
            model.UploadFile(pathCSV, pathXML);
        }

        public void SetPause(bool pause)
        {
            model.SetPause(pause);
        }

        public void Jump(int val)
        {
            model.Jump(val);
        }

        public void SetTime(int time)
        {
            model.SetTime(time);
        }

        public void SetSpeed(double speed)
        {
            model.SetSpeed(speed);
        }

        public void SetCurrentCategory(string category)
        {
            model.SetCurrentCategory(category);
        }

        public void Exit()
        {
            model.Exit();
        }
    }
}
