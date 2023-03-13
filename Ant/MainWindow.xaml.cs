using System.Data;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.Generic;

namespace Ant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {
        RoadMap myRoadMap;
        Cell[,] tableDistance;
        Cell[,] tableWeight;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateTable_Click(object sender, RoutedEventArgs e)
        {
            myRoadMap = new RoadMap(int.Parse(NumberStation.Text));

            tableDistance = new Cell[int.Parse(NumberStation.Text) + 1, int.Parse(NumberStation.Text) + 1];
            tableWeight = new Cell[int.Parse(NumberStation.Text) + 1, int.Parse(NumberStation.Text) + 1];

            for (var i = 0; i < tableDistance.GetLength(0); i++)
            {
                for (var j = 0; j < tableDistance.GetLength(1); j++)
                {
                    var cell = new Cell
                    {
                        Height = WPDistance.ActualHeight / (int.Parse(NumberStation.Text) + 1),
                        Width = WPDistance.ActualWidth / (int.Parse(NumberStation.Text) + 1),
                        FontSize = WPDistance.ActualWidth / int.Parse(NumberStation.Text) / 2
                    };
                    tableDistance[i, j] = cell;

                    if(i == 0 && j > 0)
                    {
                        tableDistance[i, j].CellText.Text = myRoadMap.stationsMap[j - 1].NumberOfStation.ToString();
                        tableDistance[i, j].CellText.IsReadOnly = true;
                    }
                    if(j == 0 && i > 0)
                    {
                        tableDistance[i, j].CellText.Text = myRoadMap.stationsMap[i-1].NumberOfStation.ToString();
                        tableDistance[i, j].CellText.IsReadOnly = true;
                    }
                    if(i > 0 && j > 0 && i != j)
                    {
                        tableDistance[i, j].CellText.Text = myRoadMap.FindDistance(i-1, j-1).ToString();
                    }
                }
            }

            for (var i = 0; i < tableWeight.GetLength(0); i++)
            {
                for (var j = 0; j < tableWeight.GetLength(1); j++)
                {
                    var cell = new Cell
                    {
                        Height = WPWeight.ActualHeight / (int.Parse(NumberStation.Text) + 1),
                        Width = WPWeight.ActualWidth / (int.Parse(NumberStation.Text) + 1),
                        FontSize = WPWeight.ActualWidth / int.Parse(NumberStation.Text) / 2
                    };
                    tableWeight[i, j] = cell;

                    if (i == 0 && j > 0)
                    {
                        tableWeight[i, j].CellText.Text = myRoadMap.stationsMap[j - 1].NumberOfStation.ToString();
                        tableWeight[i, j].CellText.IsReadOnly = true;
                    }
                    if (j == 0 && i > 0)
                    {
                        tableWeight[i, j].CellText.Text = myRoadMap.stationsMap[i - 1].NumberOfStation.ToString();
                        tableWeight[i, j].CellText.IsReadOnly = true;
                    }
                    if (i > 0 && j > 0 && i != j)
                    {
                        tableWeight[i, j].CellText.Text = myRoadMap.FindPheromone(i - 1, j - 1).ToString();
                    }
                }
            }

            for (var i = 0; i < tableDistance.GetLength(0); i++)
            {
                for (var j = 0; j < tableDistance.GetLength(1); j++)
                {
                    WPDistance.Children.Add(tableDistance[i, j]);
                    WPWeight.Children.Add(tableWeight[i, j]);
                }
            }
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            myRoadMap.roadMap.Clear();
            myRoadMap.stationsMap.Clear();
            myRoadMap.UpdateRoadMap(tableDistance, tableWeight);
            
            AntAI antAI = new AntAI(myRoadMap.roadMap, myRoadMap.stationsMap,
                double.Parse(DegreeAlpha.Text), double.Parse(DegreeBeta.Text), int.Parse(NumberIterartion.Text));

            var myPath = antAI.AntsAI(myRoadMap.stationsMap[int.Parse(FirstStation.Text) - 1],
                myRoadMap.stationsMap, int.Parse(NumberIterartion.Text), myRoadMap.roadMap);

            Answer.Text = myPath[0].NumberOfStation.ToString();
            for (var i = 1; i < myPath.Count; i++)
            {
                if(i % 5 == 0)
                {
                    Answer.Text += "\n";
                }
                Answer.Text += " - " + myPath[i].NumberOfStation.ToString();
            }
            Answer.Text += " = " + antAI.PathLentCalculate(myPath).ToString();
            int count = 0;
            for(var i = 1; i < tableWeight.GetLength(0); i++)
            {
                for (var j = i; j < tableWeight.GetLength(1); j++)
                {
                    if(i != j)
                    {
                        tableWeight[i, j].CellText.Text = myRoadMap.roadMap[count].pheromones.ToString();
                        tableWeight[j, i].CellText.Text = myRoadMap.roadMap[count].pheromones.ToString();
                        count++;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
