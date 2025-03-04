using System.Diagnostics;

using CubeRobot.Models.RubiksCube;

using Kociemba;

namespace CubeRobot.Solvers;

public class TwoPhaseSolver : ICubeSolver
{
    private static readonly string RelativeTablesDirectory = Path.Combine("Assets", "TwoPhase", "Tables");

    public IEnumerable<CubeMove> SolveCube(Cube cube)
    {
        EnsureTableFilesArePresent();

        string cubeConfiguration = cube.ToString();
        string solution = Search.solution(cubeConfiguration, out string info);
        CubeMove[] moves = CubeMoveParser.ParseMoves(solution);

#if DEBUG
        Debug.WriteLine(info);
#endif

        return moves;
    }

    private static void EnsureTableFilesArePresent()
    {
        Tools.EnsureTableDirectory();

        string assetsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RelativeTablesDirectory);
        string[] assetFiles = Directory.GetFiles(assetsDirectory);

        foreach (string assetFilePath in assetFiles)
        {
            string appdataFilePath = Path.Combine(Tools.TableDirectory, Path.GetFileName(assetFilePath));
            if (!File.Exists(appdataFilePath))
                File.Copy(assetFilePath, appdataFilePath);
        }
    }
}