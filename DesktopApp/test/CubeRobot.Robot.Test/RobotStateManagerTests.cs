using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeRobot.Robot.Test;

[TestClass]
public class RobotStateManagerTests
{
    private readonly RobotState _initialState = RobotState.Disconnected;
    private RobotStateManager _stateManager = null!;

    [TestInitialize]
    public void Intitialize()
    {
        _stateManager = new()
        {
            CurrentState = _initialState
        };
    }

    [TestMethod]
    public void CurrentState_RobotStateEventTriggered_ForStateChange()
    {
        bool wasInvoked = false;

        _stateManager.RobotStateChanged += (sender, args) =>
        {
            wasInvoked = true;
        };

        // Change the state
        _stateManager.CurrentState = RobotState.Resetting;

        Assert.IsTrue(wasInvoked);
    }

    [TestMethod]
    public void CurrentState_RobotStateEventNotTriggered_ForSettingTheSameState()
    {
        bool wasInvoked = false;

        _stateManager.RobotStateChanged += (sender, args) =>
        {
            wasInvoked = true;
        };

        // Change the state
        _stateManager.CurrentState = _initialState;

        Assert.IsFalse(wasInvoked);
    }
}