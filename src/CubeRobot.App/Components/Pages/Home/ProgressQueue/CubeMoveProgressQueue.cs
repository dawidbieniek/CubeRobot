using System.Text;

using CubeRobot.Models.RubiksCube;
using CubeRobot.Util;

namespace CubeRobot.App.Components.Pages.Home.ProgressQueue;

public class CubeMoveProgressQueue : ProgressQueueBase<CubeMove>
{
    protected override void UpdateText()
    {
        StringBuilder sb = new();

        foreach (CubeMove item in QueueValue)
        {
            sb.Append(item.GetDescriptor());
            sb.Append(' ');
        }

        Text = sb.ToString();
    }
}