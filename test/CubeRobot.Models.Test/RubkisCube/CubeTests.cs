using CubeRobot.Models.RubiksCube;

namespace CubeRobot.Models.Test.RubkisCube;

[TestClass]
public class CubeTests
{
    private Cube _cube = null!;

    [TestInitialize]
    public void Initialize()
    {
        _cube = new(3, true);
    }

    [TestMethod]
    public void Constructor_CubeContainsCorrectColors_ForInitializedCube()
    {
        Assert.AreEqual(CubeFaceColor.White, _cube[CubeFace.Front, 0, 0]);
        Assert.AreEqual(CubeFaceColor.Orange, _cube[CubeFace.Right, 0, 0]);
        Assert.AreEqual(CubeFaceColor.Green, _cube[CubeFace.Up, 0, 0]);
        Assert.AreEqual(CubeFaceColor.Yellow, _cube[CubeFace.Back, 0, 0]);
        Assert.AreEqual(CubeFaceColor.Red, _cube[CubeFace.Left, 0, 0]);
        Assert.AreEqual(CubeFaceColor.Blue, _cube[CubeFace.Down, 0, 0]);
    }

    [TestMethod]
    public void Constructor_CubeContainsOnlyNoneColors_ForUnInitializedCube()
    {
        Cube grayCube = new(3, false);

        for (int face = 0; face < Cube.NumberOfFaces; face++)
        {
            for (int i = 0; i < grayCube.Size; i++)
                for (int j = 0; j < grayCube.Size; j++)
                    Assert.AreEqual(CubeFaceColor.None, grayCube[(CubeFace)face][i, j]);
        }
    }

    [TestMethod]
    [DataRow("UUUUUULLLURRURRURRFFFFFFFFFRRRDDDDDDLLDLLDLLDBBBBBBBBB", new CubeMove[] { CubeMove.F })]
    [DataRow("RUUFUUFFFUBBRRRUBUDRRDFDRRRBBBDDDBBLDLLFLLDFDLLFUBUFLL", new CubeMove[] { CubeMove.R, CubeMove.U, CubeMove.LPrime, CubeMove.DPrime })]
    [DataRow("UUUBUBUDFLRFRRBLRFLFUFFDLDDDLFLDUDBDBUBULRBFBRFRLBLRDR", new CubeMove[] { CubeMove.D2, CubeMove.R2, CubeMove.LPrime, CubeMove.D2, CubeMove.U2, CubeMove.B2, CubeMove.R, CubeMove.DPrime, CubeMove.U2, CubeMove.L, CubeMove.B, CubeMove.FPrime, CubeMove.R, CubeMove.L, CubeMove.UPrime, CubeMove.DPrime, CubeMove.R2, CubeMove.U, CubeMove.LPrime, CubeMove.FPrime, CubeMove.LPrime, CubeMove.RPrime, CubeMove.F2, CubeMove.UPrime, CubeMove.L2 })]
    public void PerformMove_CubeIsInCorrectConfiguration_ForScrambledCube(string expectedConfiguration, CubeMove[] moves)
    {
        foreach (CubeMove move in moves)
            _cube.PerformMove(move);

        Assert.AreEqual(expectedConfiguration, _cube.ToString());
    }

    [TestMethod]
    [DataRow("UUUBUBUDFLRFRRBLRFLFUFFDLDDDLFLDUDBDBUBULRBFBRFRLBLRDR")]
    [DataRow("RUUFUUFFFUBBRRRUBUDRRDFDRRRBBBDDDBBLDLLFLLDFDLLFUBUFLL")]
    public void FromConfigurationString_ReturnsCorrectCube_ForCorrectInput(string cubeConfig)
    {
        Cube cube = Cube.FromConfigurationString(cubeConfig);
        string createdCubeConfig = cube.ToString();

        Assert.AreEqual(cubeConfig, createdCubeConfig);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    [DataRow("UUUABUBUDFLRFRRBLRFLFUFFDLDDDLFLDUDBDBUBULRFBRFRLBLRDR")] // Unrecognized char
    [DataRow("UUUUUULLLURRURRURRFFFFFFFFFRRRDDDDDDLLDLLDLLDBBBBBBB")]   // Wrong length
    public void FromConfigurationString_ThrowsException_ForInvalidConfig(string cubeConfig)
    {
        Cube.FromConfigurationString(cubeConfig);
    }
}