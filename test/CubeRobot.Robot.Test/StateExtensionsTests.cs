namespace CubeRobot.Robot.Test;

[TestClass]
public class StateExtensionsTests
{
    [TestMethod]
    public void RotorState_Switch_TogglesBetweenHorizontalAndVertical()
    {
        RotorState state = RotorState.Horizontal;
        state.Switch();

        Assert.AreEqual(RotorState.Vertical, state);

        state.Switch();

        Assert.AreEqual(RotorState.Horizontal, state);
    }

    [TestMethod]
    public void MoverState_Switch_TogglesBetweenCloseAndFar()
    {
        MoverState state = MoverState.Close;
        state.Switch();

        Assert.AreEqual(MoverState.Far, state);

        state.Switch();

        Assert.AreEqual(MoverState.Close, state);
    }
}