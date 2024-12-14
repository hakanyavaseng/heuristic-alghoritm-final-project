using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HeuristicAlgFinalProject
{
    public class KnapsackOptimizationProgram
    {
        private static readonly List<(int value, int weight)> ItemSet = new()
        {
            (10, 5), (20, 10), (30, 15), (40, 20), (50, 25),
            (15, 7), (25, 12), (35, 17), (45, 22), (55, 27),
            (12, 6), (22, 11), (32, 16), (42, 21), (52, 26),
            (8, 4), (18, 9), (28, 14), (38, 19), (48, 24),
            (11, 5), (21, 10), (31, 15), (41, 20), (51, 25),
            (14, 7), (24, 12), (34, 17), (44, 22), (54, 27),
            (13, 6), (23, 11), (33, 16), (43, 21), (53, 26),
            (9, 4), (19, 9), (29, 14), (39, 19), (49, 24)
        };

        private static readonly List<int> KnapsackCapacities = new() { 460 };
        private static readonly List<int> NumAgentsRange = new() { 20, 50, 100, 200 };
        private static readonly List<int> MaxIterationsRange = new() { 50, 100, 200, 500 };

        public static async Task Main()
        {
            Console.WriteLine("Knapsack Optimization Experiment Runner");
            Console.WriteLine("Select the algorithm to run:");
            Console.WriteLine("1. Artificial Bee Colony Knapsack");
            Console.WriteLine("2. Artificial Immune System Knapsack");
            Console.WriteLine("3. Greedy Knapsack");
            Console.Write("Enter your choice: ");

            int choice = int.Parse(Console.ReadLine() ?? "1");
            var results = await RunComprehensiveExperiments(choice);

            await SaveResultsToCsv(results);
            DisplayBestResult(results);
        }

        private static async Task<List<Dictionary<string, object>>> RunComprehensiveExperiments(
            int algorithmChoice
        )
        {
            var results = new List<Dictionary<string, object>>();

            foreach (var capacity in KnapsackCapacities)
            {
                foreach (var numAgents in NumAgentsRange)
                {
                    foreach (var maxIterations in MaxIterationsRange)
                    {
                        Console.WriteLine($"Running experiment: " +
                                          $"Capacity = {capacity}, " +
                                          $"Agents = {numAgents}, " +
                                          $"Iterations = {maxIterations}");

                        IKnapsackAlgorithm algorithm = AlgorithmFactory.Create(
                            algorithmChoice,
                            ItemSet,
                            capacity,
                            numAgents,
                            maxIterations
                        );

                        var startTime = DateTime.Now;
                        var (bestSolution, bestFitness) = algorithm.Run();
                        var execTime = (DateTime.Now - startTime).TotalSeconds;

                        results.Add(new Dictionary<string, object>
                        {
                            { "Items", ItemSet },
                            { "Capacity", capacity },
                            { "Number of Agents", numAgents },
                            { "Max Iterations", maxIterations },
                            { "Best Solution", bestSolution },
                            { "Best Fitness", bestFitness },
                            { "Execution Time (s)", execTime }
                        });
                    }
                }
            }

            return results;
        }

        private static async Task SaveResultsToCsv(
            List<Dictionary<string, object>> results
        )
        {
            var filePath = "knapsack_param_results.csv";
            var lines = new List<string>
            {
                "Combination,Items,Number of Agents,Max Iterations,Best Solution,Best Fitness,Execution Time (s)"
            };

            foreach (var result in results)
            {
                var line = string.Join(",",
                    System.Linq.Enumerable.Select(result, kv =>
                        kv.Value is List<int>
                            ? $"[{string.Join(",", (List<int>)kv.Value)}]"
                            : kv.Value is List<(int, int)>
                                ? $"[{string.Join("|", (List<(int, int)>)kv.Value)}]"
                                : kv.Value.ToString()));
                lines.Add(line);
            }

            await File.WriteAllLinesAsync(filePath, lines);
            Console.WriteLine($"Results saved to {filePath}");
        }

        private static void DisplayBestResult(
            List<Dictionary<string, object>> results
        )
        {
            var bestResult = results.OrderByDescending(r => (int)r["Best Fitness"]).First();

            Console.WriteLine("\nBest Experiment Result:");
            Console.WriteLine($"Knapsack Capacity: {bestResult["Capacity"]}");
            Console.WriteLine($"Number of Agents: {bestResult["Number of Agents"]}");
            Console.WriteLine($"Max Iterations: {bestResult["Max Iterations"]}");

            var bestSolution = (List<int>)bestResult["Best Solution"];
            var items = (List<(int value, int weight)>)bestResult["Items"];

            Console.WriteLine("Selected Items:");
            int totalValue = 0;
            int totalWeight = 0;

            for (int i = 0; i < bestSolution.Count; i++)
            {
                if (bestSolution[i] == 1)
                {
                    Console.WriteLine($"Item {i + 1}: Value = {items[i].value}, Weight = {items[i].weight}");
                    totalValue += items[i].value;
                    totalWeight += items[i].weight;
                }
            }

            Console.WriteLine($"\nTotal Selected Items: {bestSolution.Count(x => x == 1)}");
            Console.WriteLine($"Total Value: {totalValue}");
            Console.WriteLine($"Total Weight: {totalWeight}");

            Console.WriteLine($"\nBest Solution: {string.Join(", ", bestSolution)}");
            Console.WriteLine($"Best Fitness: {bestResult["Best Fitness"]}");
            Console.WriteLine($"Execution Time: {bestResult["Execution Time (s)"]:F2} seconds");
        }

        public static class AlgorithmFactory
        {
            public static IKnapsackAlgorithm Create(
                int choice,
                List<(int value, int weight)> items,
                int capacity,
                int numAntibodiesOrBees,
                int maxIterations
            )
            {
                return choice switch
                {
                    1 => new ArtificialBeeColonyKnapsack(items, capacity, numAntibodiesOrBees, maxIterations),
                    2 => new ArtificialImmuneSystemKnapsack(items, capacity, numAntibodiesOrBees, maxIterations),
                    3 => new GreedyKnapsack(items, capacity, numAntibodiesOrBees, maxIterations),
                    _ => throw new ArgumentException("Invalid algorithm choice")
                };
            }
        }
    }
}