using CubeRobot.Robot.Helpers;

namespace CubeRobot.Robot.Test;

[TestClass]
public class MutablePairTests
{
    [TestMethod]
    public void Properties_CanBeModified_AfterConstruction()
    {
        char firstItem = 'a';
        MutablePair<char, int> pair = new(firstItem, 2);

        pair.Item2 += 2;

        Assert.AreEqual(firstItem, pair.Item1);
        Assert.AreEqual(4, pair.Item2);
    }
}