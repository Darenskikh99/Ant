using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Ant
{
    internal class AntAI
    {
        List<Station> StationList = new List<Station>(); 
        List<Path> PathList = new List<Path>();
        List<double> DistanceTraveled = new List<double>();
        double alpha;
        double beta;

        public AntAI(double alpha, double beta)
        {
            PathList = new List<Path>();
            this.alpha = alpha;
            this.beta = beta;
            StationList = new List<Station>();
        }
        /// <summary>
        /// Основная исполняющая функция
        /// </summary>
        /// <param name="firstStation"> Станция с которой начинаем движение </param>
        /// <param name="stations"> Список всех станций </param>
        /// <param name="numberInteration"> Количество итераций </param>
        /// <param name="pathList"> Список путей </param>
        /// <returns> Минимальный путь </returns>
        public List<Station> AntsAI(Station firstStation, List<Station> stations, int numberInteration, List<Path> pathList)
        {
            List<List<Station>> list = new List<List<Station>>();
            var iteration = 0;
            List<Station> tabuList = new List<Station>(); // табу лист - пройденный путь

            while (iteration < numberInteration)
            {
                tabuList.Add(firstStation); // добавляем начальную станцию в список обхода
                while (CheckRoute(tabuList, stations) != true) // проверяем все ли станции обошли
                {
                    AntRun(stations, tabuList); //проходим все станции, оставляем ферамоны

                }
                while (tabuList[tabuList.Count - 1].NumberOfStation != firstStation.NumberOfStation) // пытаемся вернуться в начальную станцию
                {
                    foreach (var station in tabuList[tabuList.Count - 1].nearStation)
                    {
                        if (station.end.NumberOfStation == firstStation.NumberOfStation)
                        {
                            tabuList.Add(station.end); break;
                        }
                    }
                    //tabuList.Add(NextStationToHome(tabuList[tabuList.Count - 1]));
                }
                PheromoneDecrement(pathList); // уменьшаем след ферамонов
                PheromoneIncrement(tabuList, pathList); // усиливаем ферамоны на нашем пути
                List<Station> answer = new List<Station>();
                foreach (var station in tabuList)
                {
                    answer.Add(station);
                }
                list.Add(answer); //добавляем пройденный путь
                iteration++;
                tabuList.Clear(); //забываем пройденный путь
            }
            return list[list.Count - 1]; // возвращаем итоговый результат
        }
        /// <summary>
        /// Уменьшение следа ферамонов
        /// </summary>
        /// <param name="pathList"> список всех путей </param>
        public void PheromoneDecrement(List<Path> pathList)
        {
            foreach (var path in pathList)
            {
                path.pheromones *= 0.99;
            }
        }
        /// <summary>
        /// Увеличивание следа ферамонов
        /// </summary>
        /// <param name="stations"> станции в порядке прохождения </param>
        public void PheromoneIncrement(List<Station> stations, List<Path> pathList)
        {
            double PathLenth = PathLentCalculate(stations);
            for (var i = 0; i < stations.Count - 1; i++)
            {
                var path = stations[i].GetPath(stations[i + 1]);
                foreach (var road in pathList)
                {
                    if(path.start.NumberOfStation ==  road.start.NumberOfStation && path.end.NumberOfStation == road.end.NumberOfStation)
                    {
                        road.pheromones += 100.0 / PathLenth;
                    }
                }   
            }
        }
        /// <summary>
        /// Подсчет пройденной дистанции
        /// </summary>
        /// <param name="stations"></param>
        /// <returns> станции в порядке прохождения </returns>
        public int PathLentCalculate(List<Station> stations)
        {
            int answer = 0;
            for (var i = 1; i < stations.Count; i++)
            {
                answer += stations[i - 1].GetPath(stations[i]).distance;
            }
            return answer;
        }


        public Station NextStationToHome(Station firstStation)
        {
            var max = 0.0;
            Station answer = new Station();
            foreach (var station in firstStation.nearStation)
            {
                if (firstStation.GetPath(station.end).pheromones >= max)
                {
                    answer = station.end;
                }
            }
            return answer;
        }

        /// <summary>
        /// строим маршрут
        /// </summary>
        /// <param name="stations"> список всех станций </param>
        /// <param name="tabuList"> табу лист </param>
        private void AntRun(List<Station> stations, List<Station> tabuList)
        {
            List<Station> potentialNextStation = new List<Station>();
            foreach (var station in stations) // из всех станций которые есть
            {
                if (CheckHave(tabuList, station) &&
                    tabuList[tabuList.Count - 1].HaveRoad(station) && CheckHave(potentialNextStation, station)) //если станцию не посещали и есть дорога
                {
                    potentialNextStation.Add(station);
                }
            }
            foreach (var station in potentialNextStation)
            {
                var newPath = tabuList[tabuList.Count - 1].GetPath(station); // получаем эту дорогу
                double probability = Math.Pow(newPath.pheromones, alpha) / Math.Pow(newPath.distance, beta)   // считаем вероятность прохождения по ней
                                    / ProbabilituCalculate(tabuList[tabuList.Count - 1], potentialNextStation);
                if(double.IsNaN(probability))
                {
                    probability = 0.000001;
                }
                station.Probability = probability;
            }

            tabuList.Add(NextStationRun(potentialNextStation));
            potentialNextStation.Clear();
        }
        /// <summary>
        /// Проверка на нахождение станции в списке станций
        /// </summary>
        /// <param name="stations"> список доступных станций </param>
        /// <param name="station"> Проверяемая станция </param>
        /// <returns> да/нет </returns>
        public bool CheckHave(List<Station> stations, Station station) // есть ли станция в табу листе?
        {
            var answer = true;
            foreach (var stat in stations)
            {
                if (stat.NumberOfStation == station.NumberOfStation)
                {
                    answer = false; break;
                }
            }
            return answer;
        }

        /// <summary>
        /// Проверка на прохождение всех станций
        /// </summary>
        /// <param name="tabuList"> список пройденных станций </param>
        /// <param name="allStation"> все доступные станции </param>
        /// <returns> да/нет </returns>
        public bool CheckRoute(List<Station> tabuList, List<Station> allStation)
        {
            var answer = false;
            if (tabuList.Count == allStation.Count)
            {
                answer = true;
            }
            return answer;
        }
        /// <summary>
        /// Выбор следующей станции исходя из вероятности
        /// </summary>
        /// <param name="station"> Список станций для проверки </param>
        /// <returns> Самая выгодная станция </returns>
        public Station NextStationRun(List<Station> station)
        {
            var probability = 0.0;
            var nextStation = new Station();
            foreach (var stat in station)
            {
                if (probability < stat.Probability)
                {
                    probability = stat.Probability;
                    nextStation = stat;
                }
            }
            return nextStation;
        }
        /// <summary>
        /// Вычисление вероятности
        /// </summary>
        /// <param name="yourPlace"> Станция на которой находимся </param>
        /// <param name="newPaths"> Возможные следующие станции </param>
        /// <returns></returns>
        public double ProbabilituCalculate(Station yourPlace, List<Station> newPaths)
        {
            double probability = 0.0;
            foreach (var path in newPaths)
            {
                probability += Math.Pow(yourPlace.GetPath(path).pheromones, alpha)
                        / Math.Pow(yourPlace.GetPath(path).distance, beta);
            }
            return probability;
        }
    }
}