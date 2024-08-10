using CubeRobot.Models.RubiksCube;

namespace CubeRobot.Solvers;

public interface ICubeSolver
{
    public IEnumerable<CubeMove> SolveCube(Cube cube);
}