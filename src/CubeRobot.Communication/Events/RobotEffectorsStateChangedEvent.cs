namespace CubeRobot.Robot.Events;

public delegate void RobotEffectorsStateChangedEventHandler(object sender, RobotEffectorsEventArgs e);

public class RobotEffectorsEventArgs(RotorState[] rotorStates, MoverState[] moverStates) : EventArgs
{
    public RotorState[] RotorStates { get; private init; } = rotorStates;
    public MoverState[] MoverStates { get; private init; } = moverStates;
}