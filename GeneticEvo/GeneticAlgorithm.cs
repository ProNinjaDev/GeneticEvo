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

        public int[] Mutation(int[] individual)
        {
            Random rnd = new Random();

            int[] mutant = (int[])individual.Clone();
            int length = mutant.Length;

            int start = rnd.Next(length);
            int end = rnd.Next(start, length);

            while (start < end)
            {
                (mutant[start], mutant[end]) = (mutant[end], mutant[start]);

                start++;
                end--;
            }

            return mutant;
        }

        public (int[], int[]) Crossover(int[] parent1, int[] parent2)
        {
            int length = parent1.Length;
            Random rnd = new Random();

            int point = rnd.Next(1, length);

            int[] child1 = new int[length];
            int[] child2 = new int[length];

            for (int i = 0; i < point; i++)
            {
                child1[i] = parent1[i];
                child2[i] = parent2[i];
            }

            for (int i = point; i < length; i++)
            {
                child1[i] = parent2[i];
                child2[i] = parent1[i];
            }

            return (child1, child2);
        }
    }
}