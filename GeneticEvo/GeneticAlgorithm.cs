using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticEvo
{
    internal class GeneticAlgorithm
    {
        private double c = 10.0;

        public List<int> Selection(List<int> fitnesses)
        {
            double meanFitness = fitnesses.Average();

            List<double> adjustedFitnesses = new List<double>();
            for (int i = 0; i < fitnesses.Count; i++)
            {
                adjustedFitnesses.Add(fitnesses[i] + c * meanFitness);
            }

            double totalFitness = 0.0;
            for (int i = 0; i < adjustedFitnesses.Count; i++)
            {
                totalFitness += adjustedFitnesses[i];
            }

            List<double> expectedCopies = new List<double>();
            for (int i = 0; i < fitnesses.Count; i++)
            {
                double probability = adjustedFitnesses[i] / totalFitness;
                double expectedCopyCount = probability * fitnesses.Count;
                expectedCopies.Add(expectedCopyCount);
            }

            List<double> cumulativeCopies = new List<double>();
            double cumulativeSum = 0;
            foreach (double copies in expectedCopies)
            {
                cumulativeSum += copies;
                cumulativeCopies.Add(cumulativeSum);
            }

            List<int> selectedIndices = new List<int>();
            Random rnd = new Random();

            for (int i = 0; i < fitnesses.Count; i++)
            {
                double spin = rnd.NextDouble() * cumulativeSum;

                for (int j = 0; j < cumulativeCopies.Count; j++)
                {
                    if (spin <= cumulativeCopies[j])
                    {
                        selectedIndices.Add(j);
                        break;
                    }
                }
            }

            return selectedIndices;
        }
    }
}