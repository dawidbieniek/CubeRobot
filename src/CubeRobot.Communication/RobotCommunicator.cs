using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Events;
using CubeRobot.Robot.Helpers;

namespace CubeRobot.Robot;

internal class RobotCommunicator : IDisposable
{
    private readonly CommunicationChannelBase _communication;
    private readonly Queue<RobotMove> _commandQueue = new();
    private Queue<MutablePair<CubeMove, int>> _movesLeftQueue = [];

    public RobotCommunicator(CommunicationChannelBase communication)
    {
        _communication = communication;
        _communication.SetReceiveCallback(OnDataRecieved);
    }

    public event CommandQueueChangedEventHandler CommandQueueChanged = delegate { };
    public event MoveQueueChangedEventHandler MoveQueueChanged = delegate { };

    public Queue<MutablePair<CubeMove, int>> MovesLeftQueue
    {
        get => _movesLeftQueue;
        set
        {
            if (value != _movesLeftQueue)
            {
                _movesLeftQueue = value;

                MoveQueueChanged.Invoke(this, new(null, _movesLeftQueue.Select((pair) => pair.Item1).ToArray()));
            }
        }
    }

    public void SendMovesToRobot(params RobotMove[] moves)
    {
        _commandQueue.EnqueueRange(moves);
        CommandQueueChanged.Invoke(this, new([], [.. _commandQueue]));

        _communication.SendMovesToRobot(moves);
    }

    public void Dispose()
    {
        if (_communication is not null && _communication is IDisposable disposableCommunication)
            disposableCommunication.Dispose();

        GC.SuppressFinalize(this);
    }

    private void OnDataRecieved(string data)
    {
        int dotCount = data.Count(c => c == '.');

        for (int i = 0; i < dotCount; i++)
        {
            DequeueDoneCommands();
            UpdateMoveQueue();
        }
    }

    private void DequeueDoneCommands()
    {
        List<RobotMove> commands = _commandQueue.DequeueUntilEncountered(RobotMove.Separator);
        CommandQueueChanged.Invoke(this, new([.. commands], [.. _commandQueue]));
    }

    private void UpdateMoveQueue()
    {
        if (MovesLeftQueue.Count == 0)
            return;

        // TODO: DEBUG check if mutability is working
        MutablePair<CubeMove, int> head = MovesLeftQueue.Peek();
        if (head.Item2 > 1)         // Decrement amount of commands left for first move
            head.Item2--;
        else
            DequeueDoneMove(head);  // Remove head from queue
    }

    private void DequeueDoneMove(MutablePair<CubeMove, int> head)
    {
        MovesLeftQueue.Dequeue();
        MoveQueueChanged.Invoke(this, new(head.Item1, MovesLeftQueue.Select((pair) => pair.Item1).ToArray()));
    }
}