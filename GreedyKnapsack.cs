namespace HeuristicAlgFinalProject;

public class GreedyKnapsack : IKnapsackAlgorithm
    {
        private List<(int value, int weight)> _items;
        private int _knapsackCapacity;
        private int _numAgents;
        private int _maxIterations;
        private List<int> _bestSolution;

        public GreedyKnapsack(
            List<(int value, int weight)> items,
            int knapsackCapacity,
            int numAgents,
            int maxIterations)
        {
            _items = items;
            _knapsackCapacity = knapsackCapacity;
            _numAgents = numAgents;
            _maxIterations = maxIterations;
            _bestSolution = new List<int>();

            Console.WriteLine($"Knapsack Dimensions: Items = {_items.Count}, Capacity = {_knapsackCapacity}");
        }

        private int EvaluateFitness(List<int> solution)
        {
            int totalValue = 0;
            int totalWeight = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (solution[i] == 1)
                {
                    totalValue += _items[i].value;
                    totalWeight += _items[i].weight;
                }
            }

            if (totalWeight > _knapsackCapacity)
            {
                return 0; // Penalize overweight solutions
            }

            return totalValue;
        }

        public (List<int>, int) Run()
        {
            // Sort items by value-to-weight ratio in descending order
            var sortedItemsWithIndices = _items
                .Select((item, index) => (
                    item,
                    ratio: (double)item.value / item.weight,
                    index))
                .OrderByDescending(x => x.ratio)
                .ToList();

            // Initialize solution with zeros
            _bestSolution = Enumerable.Repeat(0, _items.Count).ToList();

            int currentWeight = 0;
            foreach (var (item, ratio, index) in sortedItemsWithIndices)
            {
                // If adding this item doesn't exceed capacity, add it
                if (currentWeight + item.weight <= _knapsackCapacity)
                {
                    _bestSolution[index] = 1;
                    currentWeight += item.weight;
                }
            }

            int bestFitness = EvaluateFitness(_bestSolution);

            Console.WriteLine("Greedy Solution: " + string.Join(",", _bestSolution) + $" Fitness = {bestFitness}");

            return (_bestSolution, bestFitness);
        }
    }