using System.Collections.Generic;

namespace HeuristicAlgFinalProject;

public interface IKnapsackAlgorithm
{
    (List<int>, int) Run();
}