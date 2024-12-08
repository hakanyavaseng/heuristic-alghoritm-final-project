using HeuristicAlgFinalProject;

Console.WriteLine("Select the algorithm to run:");
Console.WriteLine("1. Artificial Bee Colony Knapsack");
Console.WriteLine("2. Artificial Immune System Knapsack");
Console.Write("Enter your choice: ");
int choice = int.Parse(Console.ReadLine() ?? "1");

var itemsCombinations = new List<List<(int, int)>>()
{
    new() { (10, 5), (20, 10), (30, 15) },
    new() { (15, 7), (25, 12), (35, 17) },
    new() { (12, 6), (22, 11), (32, 16) },
    new() { (50, 25), (60, 30), (70, 35), (80, 40) },
    new() { (100, 50), (200, 100), (300, 150), (400, 200) },
    new() { (5, 2), (8, 3), (12, 6), (18, 9), (25, 12) },
    new() { (30, 15), (45, 20), (60, 25), (75, 30) },
    new() { (10, 4), (25, 8), (40, 12), (55, 16), (70, 20) },
    new() { (5, 1), (10, 2), (20, 5), (50, 10), (100, 15) },
    new() { (25, 7), (40, 15), (60, 22), (80, 30), (100, 40) }
};

int knapsackCapacity = 59;
var numAgentsRange = new List<int> { 20, 50, 100, 200 };
var maxIterationsRange = new List<int> { 50, 100, 200, 500, 1000 };

var results = new List<Dictionary<string, object>>();

foreach (var (items, index) in itemsCombinations.Select((
             items,
             index
         ) => (items, index)))
{
    foreach (var numAgents in numAgentsRange)
    {
        foreach (var maxIterations in maxIterationsRange)
        {
            Console.WriteLine(
                $"Processing combination {index + 1}, Agents: {numAgents}, Iterations: {maxIterations}...");

            IKnapsackAlgorithm algorithm =
                AlgorithmFactory.Create(choice, items, knapsackCapacity, numAgents, maxIterations);

            var startTime = DateTime.Now;
            var (bestSolution, bestFitness) = algorithm.Run();
            var endTime = DateTime.Now;
            var execTime = (endTime - startTime).TotalSeconds;

            results.Add(new Dictionary<string, object>
            {
                { "Combination", index + 1 },
                { "Items", items },
                { "Number of Agents", numAgents },
                { "Max Iterations", maxIterations },
                { "Best Solution", bestSolution },
                { "Best Fitness", bestFitness },
                { "Execution Time (s)", execTime }
            });
        }
    }
}

await SaveResultsToCsv(results);

var bestResult = results.OrderByDescending(r => (int)r["Best Fitness"]).First();
Console.WriteLine("\nBest Combination:");
Console.WriteLine($"Combination: {bestResult["Combination"]}");
Console.WriteLine($"Items: {string.Join(", ", (List<(int, int)>)bestResult["Items"])}");
Console.WriteLine($"Number of Agents: {bestResult["Number of Agents"]}");
Console.WriteLine($"Max Iterations: {bestResult["Max Iterations"]}");
Console.WriteLine($"Best Solution: {string.Join(", ", (List<int>)bestResult["Best Solution"])}");
Console.WriteLine($"Best Fitness: {bestResult["Best Fitness"]}");
Console.WriteLine($"Execution Time: {bestResult["Execution Time (s)"]} seconds");

Console.WriteLine("Results saved to knapsack_param_results.csv");



static async Task SaveResultsToCsv(List<Dictionary<string, object>> results)
{
    var filePath = "knapsack_param_results.csv";
    var lines = new List<string>
    {
        "Combination,Items,Number of Agents,Max Iterations,Best Solution,Best Fitness,Execution Time (s)"
    };

    foreach (var result in results)
    {
        var line = string.Join(",",
            result.Select(kv =>
                kv.Value is List<int> ? $"[{string.Join(",", (List<int>)kv.Value)}]" : kv.Value.ToString()));
        lines.Add(line);
    }

    await File.WriteAllLinesAsync(filePath, lines);
}

public static class AlgorithmFactory
{
    public static IKnapsackAlgorithm Create(
        int choice,
        List<(int, int)> items,
        int capacity,
        int numAntibodiesOrBees,
        int maxIterations
    )
    {
        return choice switch
        {
            1 => new ArtificialBeeColonyKnapsack(items, capacity, numAntibodiesOrBees, maxIterations),
            2 => new ArtificialImmuneSystemKnapsack(items, capacity, numAntibodiesOrBees, maxIterations),
            _ => throw new ArgumentException("Invalid algorithm choice")
        };
    }
}