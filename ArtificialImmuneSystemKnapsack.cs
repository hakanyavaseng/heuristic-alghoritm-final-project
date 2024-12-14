using System;
using System.Collections.Generic;
using System.Linq;

namespace HeuristicAlgFinalProject;

public class ArtificialImmuneSystemKnapsack : IKnapsackAlgorithm
{
    private List<(int, int)> _items; // (value, weight)
    private int _knapsackCapacity;
    private int _populationSize;
    private int _maxIterations;
    private List<List<int>> _population; // Antibody population
    private List<int> _bestSolution;

    public ArtificialImmuneSystemKnapsack(
        List<(int, int)> items,
        int knapsackCapacity,
        int populationSize,
        int maxIterations
    )
    {
        _items = items;
        _knapsackCapacity = knapsackCapacity;
        _populationSize = populationSize;
        _maxIterations = maxIterations;
        _population = new List<List<int>>();
        _bestSolution = new List<int>();

        Console.WriteLine(
            $"Knapsack Dimensions: Items = {_items.Count}, Capacity = {_knapsackCapacity}, Population Size = {_populationSize}, Max Iterations = {_maxIterations}");

        // Initialize population
        for (int i = 0; i < populationSize; i++)
        {
            _population.Add(GenerateSolution());
        }

        _bestSolution = _population[0];

        Console.WriteLine("Initial Population:");
        PrintPopulation(_population);
    }

    // Generate a random solution (antibody)
    private List<int> GenerateSolution()
    {
        Random rand = new Random();
        return _items.Select(item => rand.Next(2)).ToList();
    }

    // Evaluate the fitness (affinity) of a solution
    private int EvaluateFitness(
        List<int> solution
    )
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

        // Penalize overweight solutions
        if (totalWeight > _knapsackCapacity)
            return 0;

        return totalValue;
    }

    // Mutate an antibody by flipping a random bit
    private List<int> Mutate(
        List<int> solution
    )
    {
        Random rand = new Random();
        List<int> mutatedSolution = new List<int>(solution);
        int index = rand.Next(solution.Count);
        mutatedSolution[index] = 1 - mutatedSolution[index]; // Flip the bit
        return mutatedSolution;
    }

    // Perform the clonal selection process
    private List<List<int>> PerformClonalSelection(
        List<List<int>> population,
        int clonesPerAntibody
    )
    {
        List<List<int>> clonedPopulation = new List<List<int>>();

        foreach (var antibody in population)
        {
            for (int i = 0; i < clonesPerAntibody; i++)
            {
                clonedPopulation.Add(Mutate(antibody));
            }
        }

        return clonedPopulation;
    }

    // Print the current population
    private void PrintPopulation(
        List<List<int>> population
    )
    {
        for (int i = 0; i < population.Count; i++)
        {
            Console.WriteLine($"Antibody {i + 1}: " + string.Join(",", population[i]) +
                              $" Fitness = {EvaluateFitness(population[i])}");
        }
    }

    // Main AIS process
    public (List<int>, int) Run()
    {
        for (int iteration = 0; iteration < _maxIterations; iteration++)
        {
            Console.WriteLine($"\nIteration {iteration + 1}:");

            // Evaluate fitness for all antibodies
            List<(List<int>, int)> evaluatedPopulation = _population
                .Select(antibody => (antibody, EvaluateFitness(antibody)))
                .OrderByDescending(e => e.Item2)
                .ToList();

            Console.WriteLine("Evaluated Population:");
            PrintPopulation(_population);

            // Keep the best solutions
            List<List<int>> topAntibodies = evaluatedPopulation
                .Take(_populationSize / 2)
                .Select(e => e.Item1)
                .ToList();

            // Clone and mutate the top antibodies
            _population = PerformClonalSelection(topAntibodies, clonesPerAntibody: 5);

            // Replace the worst antibodies with new random solutions
            while (_population.Count < _populationSize)
            {
                _population.Add(GenerateSolution());
            }

            // Update the best solution
            foreach (var antibody in _population)
            {
                if (EvaluateFitness(antibody) > EvaluateFitness(_bestSolution))
                {
                    _bestSolution = antibody;
                }
            }

            Console.WriteLine("Best Solution So Far: " + string.Join(",", _bestSolution) +
                              $" Fitness = {EvaluateFitness(_bestSolution)}");
        }

        return (_bestSolution, EvaluateFitness(_bestSolution));
    }
}