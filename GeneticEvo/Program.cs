using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticEvo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "Z15_5_9.DAT";
            //string filePath = "TEST2.txt";

            int numMachines, numRequests;
            int[,] timeWork;
            int[] deadlines;
            int[] penalties;

            LoadData(filePath, out numMachines, out numRequests, out timeWork, out deadlines, out penalties);
            GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(numMachines, numRequests, timeWork, deadlines, penalties);

            int populationSize = 70;
            int numGenerations = 200;
            double mutationProbability = 0.05;
            int stagnationLimit = populationSize;
            int fitnessThreshold = 1;

            List<int> bestSequenceRequests = geneticAlgorithm.Start(populationSize, numGenerations, mutationProbability, stagnationLimit, fitnessThreshold);

            Console.WriteLine("Лучшее решение: ");

            foreach (var request in bestSequenceRequests)
            {
                Console.Write($"{request}   ");
            }

            Console.WriteLine("\n");
            var result = geneticAlgorithm.EvaluateOptimalityCriterion(bestSequenceRequests);

            int optimalityCriterion = result.Item1;
            Dictionary<int, List<(int Start, int End)>> schedule = result.Item2;

            foreach (var entry in schedule)
            {
                Console.WriteLine($"Заявка {entry.Key}:");
                for (int machine = 0; machine < entry.Value.Count; machine++)
                {
                    Console.WriteLine($"  Станок {machine + 1}: Начало {entry.Value[machine].Start}, Конец {entry.Value[machine].End}");
                }
            }
            Console.WriteLine($"Критерий оптимальности: {optimalityCriterion}");
        }

        static void LoadData(string filePath, out int numMachines, out int numRequests, out int[,] timeWork, out int[] deadlines, out int[] penalties)
        {
            string[] lines = File.ReadAllLines(filePath);

            var parameters = lines[1].Split(new string[] { "   " }, StringSplitOptions.RemoveEmptyEntries);
            numMachines = int.Parse(parameters[0]);
            numRequests = int.Parse(parameters[1]);

            timeWork = new int[numMachines, numRequests];
            for (int i = 0; i < numMachines; i++)
            {
                var times = lines[i + 3].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < numRequests; j++)
                {
                    timeWork[i, j] = int.Parse(times[j]);
                }
            }

            var deadlinesStr = lines[11].Split(new string[] { "   " }, StringSplitOptions.RemoveEmptyEntries);
            deadlines = new int[deadlinesStr.Length];
            for (int i = 0; i < deadlinesStr.Length; i++)
            {
                deadlines[i] = int.Parse(deadlinesStr[i].Trim());
            }

            var penaltiesStr = lines[13].Split(new string[] { "   " }, StringSplitOptions.RemoveEmptyEntries);
            penalties = new int[penaltiesStr.Length];
            for (int i = 0; i < penaltiesStr.Length; i++)
            {
                penalties[i] = int.Parse(penaltiesStr[i].Trim());
            }
        }
    }
}
