using CubeRobot.Robot.Events;

namespace CubeRobot.Robot.Communication;

public abstract class CommunicationChannelBase
{
    public event CommunicationChannelDataRecievedEventHandler DataRecieved = delegate { };

    public virtual void SendMovesToRobot(IEnumerable<RobotMove> moves) => SendTextToRobot(moves.ToProtocolString());
    public abstract void SendTextToRobot(string text);
    public virtual void SendTextToRobot(char text) => SendTextToRobot(text.ToString());

    protected virtual void OnDataRecieved(CommunicationChannelDataEventArgs args)
    {
        DataRecieved?.Invoke(this, args);
    }
}