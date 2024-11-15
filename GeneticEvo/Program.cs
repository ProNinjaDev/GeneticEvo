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

            int numMachines, numRequests;
            int[,] timeWork;
            int[] deadlines;
            int[] penalties;
            int optimalityCriterion;

            LoadData(filePath, out numMachines, out numRequests, out timeWork, out deadlines, out penalties);
            List<int> sequenceRequests = GenerateSequenceRequests(numRequests);
            List<List<int>> population = new List<List<int>>();

            //optimalityCriterion = EvaluateOptimalityCriterion(sequenceRequests, deadlines, penalties, timeWork, numMachines, numRequests);
            //Console.WriteLine(optimalityCriterion);
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

        static List<int> GenerateSequenceRequests(int numRequests)
        {
            Random rnd = new Random();
            List<int> sequenceRequests = new List<int>();

            for (int i = 0; i < numRequests; i++) 
            {
                int request;

                do request = rnd.Next(1, numRequests + 1);
                while (sequenceRequests.Contains(request));

                sequenceRequests.Add(request);
            }

            return sequenceRequests;
        }

        static int EvaluateOptimalityCriterion(List<int> sequenceRequests, int[] deadlines, int[] penalties, int[,] timeWork, int numMachines, int numRequests)
        {
            int optimalityCriterion = 0;

            int[] endTimeForMachines = new int[numMachines];

            foreach (int request in sequenceRequests) 
            {
                int requestIndex = request - 1;

                for (int machine = 0; machine < numMachines; machine++)
                {
                    if (machine == 0)
                    {
                        endTimeForMachines[machine] += timeWork[machine, requestIndex];
                    }
                    else
                    {
                        endTimeForMachines[machine] = Math.Max(endTimeForMachines[machine], endTimeForMachines[machine - 1]) + timeWork[machine, requestIndex];
                    }

                }

                int finishTime = endTimeForMachines[numMachines - 1];

                int delay = Math.Max(0, finishTime - deadlines[requestIndex]);
                optimalityCriterion += penalties[requestIndex] * delay;
            }

            return optimalityCriterion; // 1230
        }

    }
}
