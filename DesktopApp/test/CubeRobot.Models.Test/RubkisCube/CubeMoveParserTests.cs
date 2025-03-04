using CubeRobot.Models.RubiksCube;

namespace CubeRobot.Models.Test.RubkisCube;

[TestClass]
public class CubeMoveParserTests
{
    [TestMethod]
    [DataRow("FRULBDX", new CubeMove[] { CubeMove.F, CubeMove.R, CubeMove.U, CubeMove.L, CubeMove.B, CubeMove.D, CubeMove.X })]
    [DataRow("F'RU2L'BDX'", new CubeMove[] { CubeMove.FPrime, CubeMove.R, CubeMove.U2, CubeMove.LPrime, CubeMove.B, CubeMove.D, CubeMove.XPrime })]
    public void ParseMoves_ContainsCorrectMoves_ForValidNotation(string movesInNotation, CubeMove[] expectedMoves)
    {
        CubeMove[] moves = CubeMoveParser.ParseMoves(movesInNotation);

        CollectionAssert.AreEqual(expectedMoves, moves);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    [DataRow("")]
    [DataRow("RFA")]    // Move 'A' is invalid
    [DataRow("RfL")]    // Move 'f' is invalid
    public void ParseMoves_Throws_ForEmptyOrInvalidMoves(string movesInNotation)
    {
        CubeMoveParser.ParseMoves(movesInNotation);
    }
}