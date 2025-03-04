using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Events;
using CubeRobot.Robot.Helpers;

namespace CubeRobot.Robot;

internal class CubeMoveProcessor
{
    private RotorState[] _rotors = [RotorState.Vertical, RotorState.Horizontal, RotorState.Horizontal, RotorState.Vertical];
    private MoverState[] _movers = Enumerable.Range(0, 4).Select(_ => MoverState.Far).ToArray();

    private bool _cubeTitled = false;

    public event RobotEffectorsStateChangedEventHandler RobotEffectorsStateChanged = delegate { };

    public void SetMovers(MoverState left, MoverState back, MoverState right, MoverState front)
    {
        _movers[(int)Direction.Left] = left;
        _movers[(int)Direction.Back] = back;
        _movers[(int)Direction.Right] = right;
        _movers[(int)Direction.Front] = front;

        RobotEffectorsStateChanged?.Invoke(this, new(_rotors, _movers));
    }

    public List<RobotMove> ProcessMoves(IEnumerable<CubeMove> moves, out Queue<MutablePair<CubeMove, int>> movesLeftQueue)
    {
        // HACK: CHeck if I even need to clone this -> maybe just use original in code
        RotorState[] rotors = (RotorState[])_rotors.Clone();
        MoverState[] movers = (MoverState[])_movers.Clone();

        List<RobotMove> commands = [];
        movesLeftQueue = [];

        foreach (CubeMove move in moves)
        {
            List<RobotMove> newCommands = ProcessMove(rotors, movers, move);

            movesLeftQueue.Enqueue(new(move, newCommands.Where(m => m == RobotMove.Separator).Count()));
            commands.AddRange(newCommands);
        }
        _rotors = rotors;
        _movers = movers;

        RobotEffectorsStateChanged?.Invoke(this, new(_rotors, _movers));

        return commands;
    }

    private List<RobotMove> ProcessMove(RotorState[] rotors, MoverState[] movers, CubeMove move)
    {
        List<RobotMove> newCommands = [];

        switch (move)
        {
            case CubeMove.F:
                if (_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Front, rotors, movers));
                newCommands.Add(RobotMove.RotateFrontClockwise);
                rotors[(int)Direction.Front].Switch();
                break;

            case CubeMove.FPrime:
                if (_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Front, rotors, movers));
                newCommands.Add(RobotMove.RotateFrontCounterClockwise);
                rotors[(int)Direction.Front].Switch();
                break;

            case CubeMove.F2:
                if (_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Front, rotors, movers));
                newCommands.AddRange(HighLevelRobotMove.RotateFrontDouble);
                break;

            case CubeMove.R:
                newCommands.AddRange(PrepareForRotation(Direction.Right, rotors, movers));
                newCommands.Add(RobotMove.RotateRightClockwise);
                rotors[(int)Direction.Right].Switch();
                break;

            case CubeMove.RPrime:
                newCommands.AddRange(PrepareForRotation(Direction.Right, rotors, movers));
                newCommands.Add(RobotMove.RotateRightCounterClockwise);
                rotors[(int)Direction.Right].Switch();
                break;

            case CubeMove.R2:
                newCommands.AddRange(PrepareForRotation(Direction.Right, rotors, movers));
                newCommands.AddRange(HighLevelRobotMove.RotateRightDouble);
                break;

            case CubeMove.U:
                if (!_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Back, rotors, movers));
                newCommands.Add(RobotMove.RotateBackClockwise);
                rotors[(int)Direction.Back].Switch();
                break;

            case CubeMove.UPrime:
                if (!_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Back, rotors, movers));
                newCommands.Add(RobotMove.RotateBackCounterClockwise);
                rotors[(int)Direction.Back].Switch();
                break;

            case CubeMove.U2:
                if (!_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Back, rotors, movers));
                newCommands.AddRange(HighLevelRobotMove.RotateBackDouble);
                break;

            case CubeMove.B:
                if (_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Back, rotors, movers));
                newCommands.Add(RobotMove.RotateBackClockwise);
                rotors[(int)Direction.Back].Switch();
                break;

            case CubeMove.BPrime:
                if (_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Back, rotors, movers));
                newCommands.Add(RobotMove.RotateBackCounterClockwise);
                rotors[(int)Direction.Back].Switch();
                break;

            case CubeMove.B2:
                if (_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Back, rotors, movers));
                newCommands.AddRange(HighLevelRobotMove.RotateBackDouble);
                break;

            case CubeMove.L:
                newCommands.AddRange(PrepareForRotation(Direction.Left, rotors, movers));
                newCommands.Add(RobotMove.RotateLeftClockwise);
                rotors[(int)Direction.Left].Switch();
                break;

            case CubeMove.LPrime:
                newCommands.AddRange(PrepareForRotation(Direction.Left, rotors, movers));
                newCommands.Add(RobotMove.RotateLeftCounterClockwise);
                rotors[(int)Direction.Left].Switch();
                break;

            case CubeMove.L2:
                newCommands.AddRange(PrepareForRotation(Direction.Left, rotors, movers));
                newCommands.AddRange(HighLevelRobotMove.RotateLeftDouble);
                break;

            case CubeMove.D:
                if (!_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Front, rotors, movers));
                newCommands.Add(RobotMove.RotateFrontClockwise);
                rotors[(int)Direction.Front].Switch();
                break;

            case CubeMove.DPrime:
                if (!_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Front, rotors, movers));
                newCommands.Add(RobotMove.RotateFrontCounterClockwise);
                rotors[(int)Direction.Front].Switch();
                break;

            case CubeMove.D2:
                if (!_cubeTitled)
                    newCommands.AddRange(TiltCube(rotors, movers));
                newCommands.AddRange(PrepareForRotation(Direction.Front, rotors, movers));
                newCommands.AddRange(HighLevelRobotMove.RotateFrontDouble);
                break;
        }

        newCommands.Add(RobotMove.Separator);
        return newCommands;
    }

    private List<RobotMove> TiltCube(RotorState[] rotors, MoverState[] movers)
    {
        List<RobotMove> moves = [];

        // Move front and back arm out of the way
        moves.Add(RobotMove.MoveFrontBackward);
        moves.Add(RobotMove.MoveBackBackward);
        moves.Add(RobotMove.Separator);

        // Rotate cube
        if (_cubeTitled)
            moves.AddRange(HighLevelRobotMove.RotateCubeXPrime);
        else
            moves.AddRange(HighLevelRobotMove.RotateCubeX);
        moves.Add(RobotMove.Separator);
        _cubeTitled = !_cubeTitled;

        rotors[(int)Direction.Right].Switch();
        rotors[(int)Direction.Left].Switch();

        // Move front and back arm close to cube
        moves.Add(RobotMove.MoveFrontForward);
        moves.Add(RobotMove.MoveBackForward);
        moves.Add(RobotMove.Separator);

        return moves;
    }

    //TODO: Some actions can be done in parallel
    private List<RobotMove> PrepareForRotation(Direction dir, RotorState[] rotors, MoverState[] movers)
    {
        List<RobotMove> moves = [];

        moves.AddRange(EnsureArmsAreClose(movers));

        moves.AddRange(EnsureRotorIsVertical(dir.Left(), rotors, movers));
        moves.AddRange(EnsureRotorIsVertical(dir.Right(), rotors, movers));

        return moves;
    }

    private List<RobotMove> EnsureArmsAreClose(MoverState[] movers)
    {
        List<RobotMove> moves = [];

        bool moved = false;
        // All arms must be close
        foreach (Direction d in Enum.GetValues(typeof(Direction)))
        {
            if (movers[(int)d] == MoverState.Far)
            {
                moves.Add(d.MoveArmCloseCommand());
                movers[(int)d] = MoverState.Close;
                moved = true;
            }
        }

        if (moved)
            moves.Add(RobotMove.Separator);

        return moves;
    }

    private List<RobotMove> EnsureRotorIsVertical(Direction rotorIndex, RotorState[] rotors, MoverState[] movers)
    {
        List<RobotMove> moves = [];

        if (rotors[(int)rotorIndex] == RotorState.Horizontal)
        {
            moves.AddRange(rotorIndex.FixArmCommand());
            moves.Add(RobotMove.Separator);
            rotors[(int)rotorIndex] = RotorState.Vertical;
        }

        return moves;
    }
}