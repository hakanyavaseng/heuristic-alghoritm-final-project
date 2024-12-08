﻿namespace HeuristicAlgFinalProject
{
    public class ArtificialBeeColonyKnapsack : IKnapsackAlgorithm
    {
        private List<(int, int)> _items; // (value, weight)
        private int _knapsackCapacity;
        private int _numBees;
        private int _maxIterations;
        private List<List<int>> _currentPopulation;
        private List<int> _bestSolution;

        public ArtificialBeeColonyKnapsack(List<(int, int)> items, int knapsackCapacity, int numBees, int maxIterations)
        {
            _items = items;
            _knapsackCapacity = knapsackCapacity;
            _numBees = numBees;
            _maxIterations = maxIterations;
            _currentPopulation = new List<List<int>>();
            _bestSolution = new List<int>();

            // Initialize population
            for (int i = 0; i < numBees; i++)
            {
                _currentPopulation.Add(GenerateSolution());
            }

            _bestSolution = _currentPopulation[0];
        }

        private List<int> GenerateSolution()
        {
            Random rand = new Random();
            return _items.Select(item => rand.Next(2)).ToList();
        }

        // Updated fitness evaluation that incorporates capacity usage
        private int EvaluateFitness(List<int> solution)
        {
            int totalValue = 0;
            int totalWeight = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (solution[i] == 1)
                {
                    totalValue += _items[i].Item1;
                    totalWeight += _items[i].Item2;
                }
            }

            // Apply penalty if weight exceeds capacity
            if (totalWeight > _knapsackCapacity)
            {
                return 0; // Penalize overweight solutions
            }

            // Reward for using the knapsack capacity efficiently
            double capacityUsage = (double)totalWeight / _knapsackCapacity;
            double fitness = totalValue * capacityUsage; // Fitness increases with better capacity usage

            return (int)fitness;
        }

        private List<int> ApplyNeighborhoodStructure(List<int> solution)
        {
            Random rand = new Random();
            List<int> newSolution = new List<int>(solution);
            int index = rand.Next(solution.Count);
            newSolution[index] = 1 - newSolution[index]; // Flip the bit
            return newSolution;
        }

        public (List<int>, int) Run()
        {
            for (int iteration = 0; iteration < _maxIterations; iteration++)
            {
                // Employed Bee Phase
                for (int i = 0; i < _numBees / 2; i++)
                {
                    List<int> currentSolution = _currentPopulation[i];
                    List<int> newSolution = ApplyNeighborhoodStructure(currentSolution);
                    if (EvaluateFitness(newSolution) > EvaluateFitness(currentSolution))
                    {
                        _currentPopulation[i] = newSolution;
                    }
                }

                // Onlooker Bee Phase
                List<double> probabilities = _currentPopulation
                    .Select(sol => (double)EvaluateFitness(sol) / _currentPopulation.Sum(s => EvaluateFitness(s)))
                    .ToList();

                Random rand = new Random();
                for (int i = _numBees / 2; i < _numBees; i++)
                {
                    List<int> selectedSolution = _currentPopulation[WeightedRandomSelection(probabilities)];
                    List<int> newSolution = ApplyNeighborhoodStructure(selectedSolution);
                    if (EvaluateFitness(newSolution) > EvaluateFitness(selectedSolution))
                    {
                        _currentPopulation[i] = newSolution;
                    }
                }

                // Update Best Solution
                foreach (var solution in _currentPopulation)
                {
                    if (EvaluateFitness(solution) > EvaluateFitness(_bestSolution))
                    {
                        _bestSolution = solution;
                    }
                }
            }

            return (_bestSolution, EvaluateFitness(_bestSolution));
        }

        private int WeightedRandomSelection(List<double> probabilities)
        {
            Random rand = new Random();
            double total = probabilities.Sum();
            double randomValue = rand.NextDouble() * total;
            double cumulativeSum = 0;

            for (int i = 0; i < probabilities.Count; i++)
            {
                cumulativeSum += probabilities[i];
                if (cumulativeSum >= randomValue)
                {
                    return i;
                }
            }

            return probabilities.Count - 1; // Should never reach here if probabilities are normalized
        }
    }
}