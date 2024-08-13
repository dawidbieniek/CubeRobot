using System.ComponentModel;

namespace CubeRobot.Solvers;

public enum CubeSolvingAlgorithm
{
    TwoPhase = 0,
    [Description("LBL (not implemented)")]
    LBL = 1,
    [Description("CFOP (not implemented)")]
    CFOP = 2
}

public static class SolvingAlgorithmExtensions
{
    public static ICubeSolver Solver(this CubeSolvingAlgorithm algorithm)
    {
        return algorithm switch
        {
            CubeSolvingAlgorithm.TwoPhase => new TwoPhaseSolver(),
            CubeSolvingAlgorithm.LBL => throw new NotImplementedException(),
            CubeSolvingAlgorithm.CFOP => throw new NotImplementedException(),
            _ => throw new ArgumentException($"No solver for {algorithm}")
        };
    }
}