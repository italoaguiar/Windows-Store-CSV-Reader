using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace Windows_8_Store_CSV_Reader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fillChart();
        }
        public void fillChart()
        {
            ObservableCollection<ChartSeries> series = new ObservableCollection<ChartSeries>();
            series.Add(new ChartSeries() { caption = "Segunda", value = 10 });
            series.Add(new ChartSeries() { caption = "Terça", value = 20 });
            series.Add(new ChartSeries() { caption = "Quarta", value = 30 });
            series.Add(new ChartSeries() { caption = "Quinta", value = 25 });
            series.Add(new ChartSeries() { caption = "Sexta", value = 15 });

            downloads.DataContext = series;
            
        }
        public void ReadDownloadFile(string filename)
        {
            FileStream fs = File.OpenRead(filename);
            StreamReader sr = new StreamReader(fs);

            //captions = new List<DownloadsSeries>();
            String[] line;

            line = sr.ReadLine().Replace("\"", "").Split(new char[] { ',' });

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Data", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Idade", typeof(string)));
            dt.Columns.Add(new DataColumn("Sexo", typeof(string)));
            dt.Columns.Add(new DataColumn("Mercado", typeof(string)));
            dt.Columns.Add(new DataColumn("Sistema Operacional"));
            dt.Columns.Add(new DataColumn("Downloads", typeof(int)));
            dt.Columns.Add(new DataColumn("Downloads Pagos"));

            //Ler arestas
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine().Replace("\"", "").Split(new char[] { ',' });

                DataRow row = dt.NewRow();
                row["Data"] = DateTime.Parse(line[0]);
                row["Idade"] = line[1];
                row["Sexo"] = line[2];
                row["Mercado"] = line[3];
                row["Sistema Operacional"] = line[4];
                row["Downloads"] = int.Parse(line[5] == "" ? "0" : line[5]);
                row["Downloads Pagos"] = int.Parse(line[6] == "" ? "0" : line[6]);

                dt.Rows.Add(row);
            }
            var results2 = from row in dt.AsEnumerable()
                           group row by new { Idade = row.Field<string>("Idade").ToString() } into rowgroup
                           select new
                           {
                               Idade = rowgroup.Key.Idade,
                               Downloads = rowgroup.Sum(r => r.Field<int>("Downloads")),
                           };
            var results22 = from row in dt.AsEnumerable()
                            group row by new { Sexo = row.Field<string>("Sexo").ToString() } into rowgroup
                            select new
                            {
                                Idade = rowgroup.Key.Sexo,
                                Downloads = rowgroup.Sum(r => r.Field<int>("Downloads"))
                            };


            var results = from row in dt.AsEnumerable()
                          group row by new { Data = row.Field<DateTime>("Data").Date } into rowgroup
                          select new
                          {
                              Data = rowgroup.Key.Data,
                              Downloads = rowgroup.Sum(r => r.Field<int>("Downloads"))
                          };
            var results3 = from row in dt.AsEnumerable()
                           group row by new { Mercado = row.Field<string>("Mercado").ToString() } into rowgroup
                           select new
                           {
                               Mercado = rowgroup.Key.Mercado,
                               Downloads = rowgroup.Sum(r => r.Field<int>("Downloads"))
                           };

            DataContext = results;
            

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ReadDownloadFile(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Formato Inválido" + ex.Message + ex.Source + ex.StackTrace);
                }
            }
        }
    }
    public class ChartSeries
    {
        public int value;
        public string caption;
    }

}
