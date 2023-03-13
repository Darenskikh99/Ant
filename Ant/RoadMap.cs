using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Ant
{
    internal class RoadMap : INotifyPropertyChanged
    {
        public List<Path> roadMap = new List<Path>();
        public List<Station> stationsMap = new List<Station>();
        private int _numberStation;
        
        public int NumberStation
        {
            get { return _numberStation; }
            set { _numberStation = value; }
        }

        public RoadMap(int numberStation)
        {     
            Random randomDistance = new Random();
            Random randomWeight = new Random();
            for (var i = 0; i < numberStation; i++)
            {
                stationsMap.Add(new Station(i + 1));
                for (var j = 0; j < i; j++)
                {
                    var distance = randomDistance.Next(1, 100);
                    var weight = randomWeight.NextDouble();
                    roadMap.Add(new Path(stationsMap[i], stationsMap[j], distance, weight));

                    stationsMap[i].NewPath(stationsMap[i], stationsMap[j], distance, weight);
                    stationsMap[j].NewPath(stationsMap[j], stationsMap[i], distance, weight);
                }
            }
        }

        public RoadMap(Cell[,] distance, Cell[,] weight)
        {
            for (var i = 1; i < distance.GetLength(0); i++)
            {
                stationsMap.Add(new Station(i));
                for (var j = 1; j < i; j++)
                {
                    if (i != j)
                    {
                        roadMap.Add(new Path(stationsMap[i - 1], stationsMap[j - 1], int.Parse(distance[i, j].CellText.Text), double.Parse(weight[i, j].CellText.Text)));
                        roadMap.Add(new Path(stationsMap[j - 1], stationsMap[i - 1], int.Parse(distance[i, j].CellText.Text), double.Parse(weight[i, j].CellText.Text)));

                        stationsMap[i - 1].NewPath(stationsMap[i - 1], stationsMap[j - 1], int.Parse(distance[i, j].CellText.Text), double.Parse(weight[i, j].CellText.Text));
                        stationsMap[j - 1].NewPath(stationsMap[j - 1], stationsMap[i - 1], int.Parse(distance[i, j].CellText.Text), double.Parse(weight[i, j].CellText.Text));
                    }
                }
            }
        }

        public void UpdateRoadMap(Cell[,] distance, Cell[,] weight)
        {
            for(var i = 1; i < distance.GetLength(0); i++)
            {
                stationsMap.Add(new Station(i));
                for (var j = 1; j < i; j++)
                {
                    if(i!=j)
                    {
                        roadMap.Add(new Path(stationsMap[i - 1], stationsMap[j - 1], int.Parse(distance[i, j].CellText.Text), double.Parse(weight[i,j].CellText.Text)));
                        roadMap.Add(new Path(stationsMap[j - 1], stationsMap[i - 1], int.Parse(distance[i, j].CellText.Text), double.Parse(weight[i, j].CellText.Text)));

                        stationsMap[i - 1].NewPath(stationsMap[i - 1], stationsMap[j - 1], int.Parse(distance[i, j].CellText.Text), double.Parse(weight[i,j].CellText.Text));
                        stationsMap[j - 1].NewPath(stationsMap[j - 1], stationsMap[i - 1], int.Parse(distance[i, j].CellText.Text), double.Parse(weight[i, j].CellText.Text));
                    }
                }
            }
        }

        public int FindDistance(int i, int j)
        {
            int distance = 0;
            foreach (var K in roadMap)
            {
                if (K.HaveRoad(stationsMap[i], stationsMap[j]))
                {
                    distance = K.GetDistance(stationsMap[i], stationsMap[j]);
                }
            }
            return distance;
        }

        public double FindPheromone(int i, int j)
        {
            double pheromone = 0;
            foreach (var K in roadMap)
            {
                if (K.HaveRoad(stationsMap[i], stationsMap[j]))
                {
                    pheromone = K.pheromones;
                }
            }
            return pheromone;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
