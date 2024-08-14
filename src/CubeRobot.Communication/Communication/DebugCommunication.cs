using System.Diagnostics;

namespace CubeRobot.Robot.Communication;

public class DebugCommunication : CommunicationChannelBase
{
    public override void SendMovesToRobot(IEnumerable<RobotMove> moves)
    {
        Debug.WriteLine(moves.ToProtocolString());
    }
}