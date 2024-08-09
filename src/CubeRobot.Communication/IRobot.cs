using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Events;

namespace CubeRobot.Robot;

public interface IRobot
{
    public event RobotStateChangedEventHandler RobotStateChanged;
    public event CommandQueueChangedEventHandler CommandQueueChanged;
    public event MoveQueueChangedEventHandler MoveQueueChanged;

    public RobotState CurrentState { get; }

    public void ConfigureCommunicationChannel(CommunicationChannelBase channel);    // TODO: Use DI instead

    public void GrabCube();

    public void PresentCube(CubeFace face);

    public void StopPresenting();

    void SolveCube(params CubeMove[] moves);

    public void ReleaseCube();
}