using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticEvo
{
    internal class GeneticAlgorithm
    {
        private int numMachines;
        private int numRequests;
        private int[,] timeWork;
        private int[] deadlines;
        private int[] penalties;
        private Random rnd = new Random();
        private double c = 10.0;

        public GeneticAlgorithm(int numMachines, int numRequests, int[,] timeWork, int[] deadlines, int[] penalties)
        {
            this.numMachines = numMachines;
            this.numRequests = numRequests;
            this.timeWork = timeWork;
            this.deadlines = deadlines;
            this.penalties = penalties;
        }

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

        public List<int> Start(int populationSize, int numGenerations, double mutationProbability)
        {
            List<List<int>> population = InitializePopulation(populationSize);

            List<int> bestSequenceRequests = new List<int>();
            int bestFitness = int.MaxValue;

            for(int generation = 0; generation < numGenerations; generation++)
            {
                List<int> fitnesses = new List<int>();
                foreach (var individual in population)
                {
                    fitnesses.Add(EvaluateOptimalityCriterion(individual));
                }

                List<int> selectedIndices = Selection(fitnesses);

                List<List<int>> newPopulation = new List<List<int>>();

                for (int i = 0; i < selectedIndices.Count; i += 2)
                {
                    int[] parent1 = population[selectedIndices[i]].ToArray();
                    int[] parent2 = population[selectedIndices[i + 1]].ToArray();

                    int[] child1, child2;
                    (child1, child2) = Crossover(parent1, parent2);

                    if(rnd.NextDouble() < mutationProbability)
                    {
                        child1 = Mutation(child1);
                    }
                    newPopulation.Add(child1.ToList());

                    if (rnd.NextDouble() < mutationProbability)
                    {
                        child2 = Mutation(child2);
                    }
                    newPopulation.Add(child2.ToList());

                }

                population = newPopulation;

                fitnesses.Clear();
                foreach (var individual in population)
                {
                    fitnesses.Add(EvaluateOptimalityCriterion(individual));
                }

                int currentBestFitness = fitnesses.Min();
                int currentBestIndex = fitnesses.IndexOf(currentBestFitness);
                if (currentBestFitness < bestFitness)
                {
                    bestFitness = currentBestFitness;
                    bestSequenceRequests = population[currentBestIndex];
                }
            }
            return bestSequenceRequests;
        }

        private List<List<int>> InitializePopulation(int populationSize)
        {
            var population = new List<List<int>>();
            for (int i = 0; i < populationSize; i++)
            {
                population.Add(GenerateSequenceRequests(numRequests));
            }
            return population;
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

        public int EvaluateOptimalityCriterion(List<int> sequenceRequests)
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