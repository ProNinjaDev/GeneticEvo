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

            int start = rnd.Next(0, length);
            int end = rnd.Next(start, length);

            int[] child1 = new int[length];
            int[] child2 = new int[length];

            for(int i = 0; i < length; i++)
            {
                child1[i] = -1;
                child2[i] = -1;
            }

            for (int i = start; i <= end; i++)
            {
                child1[i] = parent1[i];
                child2[i] = parent2[i];
            }

            FillRemainingGenes(child1, parent2, end + 1);
            FillRemainingGenes(child1, parent2, 0);

            FillRemainingGenes(child2, parent1, end + 1);
            FillRemainingGenes(child2, parent1, 0);

            return (child1, child2);
        }

        private void FillRemainingGenes(int[] child, int[] parent, int start)
        {
            int length = child.Length;
            int current = start;

            for (int i = 0; i < length; i++)
            {
                int gene = parent[i];
                if (!child.Contains(gene))
                {
                    child[current % length] = gene;
                    current++;
                }
            }
        }

        public List<int> Start(int populationSize, int numGenerations, double mutationProbability, int stagnationLimit, double fitnessThreshold)
        {
            List<List<int>> population = InitializePopulation(populationSize);

            List<int> bestIndividual = new List<int>();
            int bestFitness = int.MaxValue;

            int stagnationCounter = 0;
            int lastBestFitness = int.MaxValue;

            for (int generation = 0; generation < numGenerations; generation++)
            {
                List<int> fitnesses = new List<int>();
                foreach (var individual in population)
                {
                    fitnesses.Add(EvaluateOptimalityCriterion(individual).Item1);
                }

                List<int> selectedIndices = Selection(fitnesses);

                for (int i = 0; i < selectedIndices.Count; i += 2)
                {
                    int[] parent1 = population[selectedIndices[i]].ToArray();
                    int[] parent2 = population[selectedIndices[i + 1]].ToArray();

                    int[] child1, child2;
                    (child1, child2) = Crossover(parent1, parent2);

                    if (rnd.NextDouble() < mutationProbability)
                    {
                        child1 = Mutation(child1);
                    }

                    if (rnd.NextDouble() < mutationProbability)
                    {
                        child2 = Mutation(child2);
                    }

                    population.Add(child1.ToList());
                    population.Add(child2.ToList());

                }

                population = PerformReplacement(population, fitnesses, populationSize);

                fitnesses.Clear();
                foreach (var individual in population)
                {
                    fitnesses.Add(EvaluateOptimalityCriterion(individual).Item1);
                }

                int currentBestFitness = fitnesses.Min();
                int currentBestIndex = fitnesses.IndexOf(currentBestFitness);

                Console.WriteLine($"Популяция {generation}: Лучшая приспособленность = {currentBestFitness}, Особь = {string.Join(",", population[currentBestIndex])}");
                double avgFitness = fitnesses.Average();
                double stdDevFitness = Math.Sqrt(fitnesses.Select(f => Math.Pow(f - avgFitness, 2)).Average());
                Console.WriteLine($"Средняя приспособленность = {avgFitness}, Стандартное отклонение = {stdDevFitness}");
                Console.WriteLine("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");

                if (Math.Abs(currentBestFitness - lastBestFitness) <= fitnessThreshold)
                {
                    stagnationCounter++;
                }
                else
                {
                    stagnationCounter = 0;
                }
                lastBestFitness = currentBestFitness;

                if (currentBestFitness < bestFitness)
                {
                    bestFitness = currentBestFitness;
                    bestIndividual = population[currentBestIndex];
                }

                if (stagnationCounter >= stagnationLimit)
                {
                    Console.WriteLine($"Остановка алгоритма на {generation} поколении.\n Лучшая приспособленность: {bestFitness}");
                    break;
                }

            }
            return bestIndividual;
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

        private List<int> GenerateSequenceRequests(int numRequests)
        {
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

        public (int, Dictionary<int, List<(int Start, int End)>>) EvaluateOptimalityCriterion(List<int> sequenceRequests)
        {
            int optimalityCriterion = 0;

            Dictionary<int, List<(int Start, int End)>> schedule = new Dictionary<int, List<(int Start, int End)>>();

            int[] startTimeForMachines = new int[numMachines];

            foreach (int request in sequenceRequests)
            {
                int requestIndex = request - 1;

                int[] startTimes = new int[numMachines];
                int[] endTimes = new int[numMachines];

                for (int machine = 0; machine < numMachines; machine++)
                {
                    if (machine == 0)
                    {
                        startTimes[machine] = startTimeForMachines[machine];
                    }
                    else
                    {
                        startTimes[machine] = Math.Max(startTimeForMachines[machine], endTimes[machine - 1]);
                    }

                    endTimes[machine] = startTimes[machine] + timeWork[machine, requestIndex];
                    startTimeForMachines[machine] = endTimes[machine];
                }

                int finishTime = endTimes[numMachines - 1];
                int delay = Math.Max(0, finishTime - deadlines[requestIndex]);
                optimalityCriterion += penalties[requestIndex] * delay;

                List<(int Start, int End)> machineSchedule = new List<(int Start, int End)>();
                for (int machine = 0; machine < numMachines; machine++)
                {
                    machineSchedule.Add((startTimes[machine], endTimes[machine]));
                }
                schedule[request] = machineSchedule;
            }

            return (optimalityCriterion, schedule);
        }

        private List<List<int>> PerformReplacement(List<List<int>> population, List<int> fitnesses, int targetSize)
        {
            List<(int Index, int Fitness)> indexedFitnesses = new List<(int, int)>();
            for (int i = 0; i < fitnesses.Count; i++)
            {
                indexedFitnesses.Add((i, fitnesses[i]));
            }

            for (int i = 0; i < indexedFitnesses.Count - 1; i++)
            {
                for (int j = i + 1; j < indexedFitnesses.Count; j++)
                {
                    if (indexedFitnesses[i].Fitness > indexedFitnesses[j].Fitness)
                    {
                        (indexedFitnesses[i], indexedFitnesses[j]) = (indexedFitnesses[j], indexedFitnesses[i]);
                    }
                }
            }

            List<List<int>> newPopulation = new List<List<int>>();
            for (int i = 0; i < targetSize; i++)
            {
                int index = indexedFitnesses[i].Index;
                newPopulation.Add(population[index]);
            }

            return newPopulation;
        }

    }
}