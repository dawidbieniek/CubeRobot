using CubeRobot.Models.RubiksCube;

namespace CubeRobot.Models.Test.RubkisCube;

[TestClass]
public class CubeMoveExtensionsTests
{
    [TestMethod]
    [DataRow(CubeMove.F)]
    [DataRow(CubeMove.FPrime)]
    [DataRow(CubeMove.F2)]
    [DataRow(CubeMove.R)]
    [DataRow(CubeMove.RPrime)]
    [DataRow(CubeMove.R2)]
    public void MoveType_ReturnsCorrectType_ForFaceMoves(CubeMove move)
    {
        Assert.AreEqual(CubeMoveType.Face, move.MoveType());
    }

    [TestMethod]
    [DataRow(CubeMove.M)]
    [DataRow(CubeMove.MPrime)]
    [DataRow(CubeMove.M2)]
    [DataRow(CubeMove.E)]
    [DataRow(CubeMove.EPrime)]
    [DataRow(CubeMove.E2)]
    public void MoveType_ReturnsCorrectType_ForSliceMoves(CubeMove move)
    {
        Assert.AreEqual(CubeMoveType.Slice, move.MoveType());
    }

    [TestMethod]
    [DataRow(CubeMove.X)]
    [DataRow(CubeMove.XPrime)]
    [DataRow(CubeMove.X2)]
    [DataRow(CubeMove.Y)]
    [DataRow(CubeMove.YPrime)]
    [DataRow(CubeMove.Y2)]
    public void MoveType_ReturnsCorrectType_ForRotationMoves(CubeMove move)
    {
        Assert.AreEqual(CubeMoveType.Rotation, move.MoveType());
    }

    [TestMethod]
    [DataRow(CubeMove.F)]
    [DataRow(CubeMove.X)]
    [DataRow(CubeMove.M)]
    public void MoveDirection_ReturnsCorrectDirection_ForClockwiseMoves(CubeMove move)
    {
        Assert.AreEqual(CubeMoveDirection.Clockwise, move.MoveDirection());
    }

    [TestMethod]
    [DataRow(CubeMove.FPrime)]
    [DataRow(CubeMove.XPrime)]
    [DataRow(CubeMove.MPrime)]
    public void MoveDirection_ReturnsCorrectDirection_ForCounterClockwiseMoves(CubeMove move)
    {
        Assert.AreEqual(CubeMoveDirection.CounterClockwise, move.MoveDirection());
    }

    [TestMethod]
    [DataRow(CubeMove.F2)]
    [DataRow(CubeMove.X2)]
    [DataRow(CubeMove.M2)]
    public void MoveDirection_ReturnsCorrectDirection_ForDoubleMoves(CubeMove move)
    {
        Assert.AreEqual(CubeMoveDirection.Double, move.MoveDirection());
    }

    [TestMethod]
    [DataRow(CubeMove.F, CubeFace.Front)]
    [DataRow(CubeMove.FPrime, CubeFace.Front)]
    [DataRow(CubeMove.F2, CubeFace.Front)]
    [DataRow(CubeMove.B, CubeFace.Back)]
    [DataRow(CubeMove.BPrime, CubeFace.Back)]
    [DataRow(CubeMove.B2, CubeFace.Back)]
    public void CubeFace_ReturnsCorrectFace_ForFaceMoves(CubeMove move, CubeFace expectedFace)
    {
        Assert.AreEqual(expectedFace, move.CubeFace());
    }
}