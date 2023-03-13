using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ant
{
    internal class Path
    {
        public Station end;
        public Station start;
        public int distance;
        public double pheromones;

        public Path()
        {

        }
        public Path(Station A, Station B, int distance, double pheromones)
        {
            start = A;
            end = B;
            this.distance = distance;
            this.pheromones = pheromones;
        }

        public int GetDistance(Station A, Station B)
        {
            if(start.NumberOfStation == A.NumberOfStation && end.NumberOfStation == B.NumberOfStation
                || start.NumberOfStation == B.NumberOfStation && end.NumberOfStation == A.NumberOfStation)
            {
                return distance;
            }
            else
            {
                return 0;
            }
        }

        public bool HaveRoad (Station A, Station B)
        {
            if (start.NumberOfStation == A.NumberOfStation && end.NumberOfStation == B.NumberOfStation
                || start.NumberOfStation == B.NumberOfStation && end.NumberOfStation == A.NumberOfStation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
