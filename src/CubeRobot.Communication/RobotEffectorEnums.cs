﻿namespace CubeRobot.Robot;

internal enum RotorState
{
    Horizontal,
    Vertical
}

internal enum MoverState
{
    Close,
    Far
}

internal static class RotorStateExtensions
{
    /// <summary>
    /// Toggles the <see cref="RotorState"/> between <see cref="RotorState.Horizontal"/> and <see cref="RotorState.Vertical"/>.
    /// </summary>
    public static void Switch(ref this RotorState state)
    {
        state = state == RotorState.Horizontal ? RotorState.Vertical : RotorState.Horizontal;
    }
}

internal static class MoverStateExtensions
{
    /// <summary>
    /// Toggles the <see cref="MoverState"/> between <see cref="MoverState.Close"/> and <see cref="MoverState.Far"/>.
    /// </summary>
    public static void Switch(ref this MoverState state)
    {
        state = state == MoverState.Close ? MoverState.Far : MoverState.Close;
    }
}