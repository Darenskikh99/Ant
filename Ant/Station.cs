using System.Collections;
using System.Collections.Generic;


namespace Ant
{
    internal class Station
    {
        private int numberOfStation;
        private double probability = 0;
        public List<Path> nearStation = new List<Path>();
        public int NumberOfStation
        {
            get { return numberOfStation; }
            set { numberOfStation = value; }
        }

        public double Probability
        {
            get { return probability; }
            set { probability = value; }
        }

        public Station()
        {
            Probability = 0;
        }

        public Station(int numberOfStation)
        {
            NumberOfStation = numberOfStation;
            Probability = 0;
        }

        public void NewPath (Station A, Station B, int distance, double pheromones)
        {
            nearStation.Add(new Path(A, B, distance, pheromones));
        }

        public bool HaveRoad(Station nextStation)
        {
            bool answer = false;
            foreach (var path in nearStation)
            {
                if(NumberOfStation == path.start.NumberOfStation && path.end.NumberOfStation == nextStation.NumberOfStation)
                {
                    answer = true;
                }
            }
            return answer;
        }

        public Path GetPath(Station A)
        {
            Path answer = new Path();
            foreach (var path in nearStation)
            {
                if(NumberOfStation == path.start.NumberOfStation && A.NumberOfStation == path.end.NumberOfStation)
                {
                    answer = path; break;
                }
            }
            return answer;
        }

    }
}
