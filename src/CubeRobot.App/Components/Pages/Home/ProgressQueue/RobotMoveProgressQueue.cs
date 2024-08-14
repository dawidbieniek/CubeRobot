using System.Text;

using CubeRobot.Robot;

namespace CubeRobot.App.Components.Pages.Home.ProgressQueue;

public class RobotMoveProgressQueue : ProgressQueueBase<RobotMove>
{
    protected override void UpdateText()
    {
        StringBuilder sb = new();

        foreach (RobotMove move in QueueValue)
        {
            sb.Append(move.ToProtocolString());
        }

        Text = sb.ToString();
    }
}