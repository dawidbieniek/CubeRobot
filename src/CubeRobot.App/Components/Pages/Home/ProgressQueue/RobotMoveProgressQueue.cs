using CubeRobot.Robot;

namespace CubeRobot.App.Components.Pages.Home.ProgressQueue;

public class RobotMoveProgressQueue : ProgressQueueBase<RobotMove>
{
    protected override void UpdateText()
    {
        Text = Queue?.ToProtocolString() ?? string.Empty; ;
    }
}