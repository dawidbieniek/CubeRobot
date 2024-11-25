using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Events;

namespace CubeRobot.Robot;

public interface IRobot
{
    public event RobotStateChangedEventHandler RobotStateChanged;
    public event CommandQueueChangedEventHandler CommandQueueChanged;
    public event MoveQueueChangedEventHandler MoveQueueChanged;
    public event RobotEffectorsStateChangedEventHandler RobotEffectorsStateChanged;
    public event EventHandler ConnectionEstablished;
    public event EventHandler ConnectionEstablishmentFailed;

    public RobotState CurrentState { get; }

    public void ConfigureCommunicationChannel(CommunicationChannelBase channel);

    public void GrabCube();

    public void PresentCube(CubeFace face);

    public void StopPresenting();

    public void SolveCube(IEnumerable<CubeMove> moves);

    public void ReleaseCube();
}