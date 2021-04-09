using Intelektika_projektasML.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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

namespace Intelektika_projektas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Load();
        }
        private string path = "localdata.txt";
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PredictAndSave();
        }
        private void Load()
        {
            datagridas.Columns.Clear();
            datagridas.DataContext = null;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Review", typeof(string)));
            dt.Columns.Add(new DataColumn("+", typeof(string)));
            dt.Columns.Add(new DataColumn("-", typeof(string)));
            using (StreamReader file = new StreamReader(path))
            {
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    string[] line = ln.Split(';');
                    var row = dt.NewRow();
                    row["Review"] = line[0];
                    row["+"] = line[1];
                    row["-"] = line[2];
                    dt.Rows.Add(row);
                }
                file.Close();
            }
            
            datagridas.DataContext = dt;
        }
        private void PredictAndSave()
        {
            ModelInput sampleData = new ModelInput()
            {
                Review = txtInput.Text
            };

            var predictionResult = ConsumeModel.Predict(sampleData);
            decimal d1 = Convert.ToDecimal(predictionResult.Score[0]);
            decimal d2 = Convert.ToDecimal(predictionResult.Score[1]);
            if (predictionResult.Prediction.ToString() == "+")
            {
                Console.WriteLine(predictionResult.Prediction.ToString() + " " + d1.ToString());
                using (var tw = new StreamWriter(path, true))
                {
                    if(d1 > d2)
                        tw.WriteLine(String.Format("{0};{1};{2}", txtInput.Text, d1.ToString(CultureInfo.InvariantCulture), d2.ToString(CultureInfo.InvariantCulture)));
                    else
                        tw.WriteLine(String.Format("{0};{1};{2}", txtInput.Text, d2.ToString(CultureInfo.InvariantCulture), d1.ToString(CultureInfo.InvariantCulture)));
                }
            }
            else if (predictionResult.Prediction.ToString() == "-")
            {
                Console.WriteLine(predictionResult.Prediction.ToString() + " " + d2.ToString());
                using (var tw = new StreamWriter(path, true))
                {
                    if (d1 < d2)
                        tw.WriteLine(String.Format("{0};{1};{2}", txtInput.Text, d1.ToString(CultureInfo.InvariantCulture), d2.ToString(CultureInfo.InvariantCulture)));
                    else
                        tw.WriteLine(String.Format("{0};{1};{2}", txtInput.Text, d2.ToString(CultureInfo.InvariantCulture), d1.ToString(CultureInfo.InvariantCulture)));
                }
            }
            Load();
        }
    }
}
