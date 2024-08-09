using CubeRobot.Robot.Events;

namespace CubeRobot.Robot.Communication;

public abstract class CommunicationChannelBase
{
    public event CommunicationChannelDataRecievedEventHandler DataRecieved = delegate { };

    public abstract void SendMovesToRobot(params RobotMove[] moves);

    protected virtual void OnDataRecieved(CommunicationChannelDataEventArgs args)
    {
        DataRecieved?.Invoke(this, args);
    }
}