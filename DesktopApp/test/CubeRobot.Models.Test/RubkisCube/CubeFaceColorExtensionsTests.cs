using CubeRobot.Models.RubiksCube;

namespace CubeRobot.Models.Test.RubkisCube;

[TestClass]
public class CubeFaceColorExtensionsTests
{
    [TestMethod]
    public void ToFaceChar_ReturnsCorrectChar_ForValidColors()
    {
        Assert.AreEqual('F', CubeFaceColor.White.ToFaceChar());
        Assert.AreEqual('L', CubeFaceColor.Red.ToFaceChar());
        Assert.AreEqual('U', CubeFaceColor.Green.ToFaceChar());
        Assert.AreEqual('B', CubeFaceColor.Yellow.ToFaceChar());
        Assert.AreEqual('R', CubeFaceColor.Orange.ToFaceChar());
        Assert.AreEqual('D', CubeFaceColor.Blue.ToFaceChar());
        Assert.AreEqual('X', CubeFaceColor.None.ToFaceChar());
    }

    [TestMethod]
    public void ToFaceChar_ThrowsException_ForInvalidColor()
    {
        // Attempt to convert an invalid enum value to char
        var invalidColor = (CubeFaceColor)(-2); // Cast an invalid value to CubeFaceColor

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => invalidColor.ToFaceChar());
    }
}