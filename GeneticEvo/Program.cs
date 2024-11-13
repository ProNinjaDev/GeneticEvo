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

            string[] lines = File.ReadAllLines(filePath);

            var parameters = lines[1].Split(new string[] {"   "}, StringSplitOptions.RemoveEmptyEntries);

            int numMachines = int.Parse(parameters[0]);
            int numRequests = int.Parse(parameters[1]);

            int[,] timeWork = new int[numMachines, numRequests];

            for (int i = 0; i < numMachines; i++) 
            {
                var times = lines[i + 3].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < numRequests; j++)
                {
                    timeWork[i, j] = int.Parse(times[j]);
                }
            }

            var deadlinesStr = lines[11].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            int[] deadlines = new int[deadlinesStr.Length - 1];
            for(int i = 0; i < deadlinesStr.Length - 1; i++)
            {
                deadlines[i] = int.Parse(deadlinesStr[i].Trim());
            }

        }
    }
}
