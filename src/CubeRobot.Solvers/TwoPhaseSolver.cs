using System.Diagnostics;

using CubeRobot.Models.RubiksCube;

namespace CubeRobot.Solvers;

public class TwoPhaseSolver : ICubeSolver
{
    private static readonly Task CreateTablesTask = new(static () => Kociemba.SearchRunTime.solution(Kociemba.Tools.randomCube(), out _, maxDepth: 30, buildTables: true));

    public static void Init()
    {
        CreateTablesTask.Start();
    }

    public IEnumerable<CubeMove> SolveCube(Cube cube)
    {
        if (!CreateTablesTask.IsCompleted)
            CreateTablesTask.Wait();

        string config = cube.ToString();
        var moves = CubeMoveParser.ParseMoves(Kociemba.Search.solution(config, out string info, maxDepth: 30));// + "U2 F2 R2 L2 F2 U2 R2 L2";   // HACK:!!!!!!!!! Zly ostateczny wzor, ten hack nie moze zostac na koneic
#if DEBUG
        Debug.WriteLine(info);
#endif

        return moves;
    }
}