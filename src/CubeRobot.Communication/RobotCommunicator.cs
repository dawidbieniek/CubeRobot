using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Events;
using CubeRobot.Robot.Helpers;

namespace CubeRobot.Robot;

internal class RobotCommunicator : IDisposable
{
    private readonly CommunicationChannelBase _communication;
    private readonly ConnectionEstablisher _connectionEstablisher;
    private readonly Queue<RobotMove> _commandQueue = [];
    /// <summary>
    /// Queue holding moves to be performed along counters indicating number of commands to perform
    /// for each move
    /// </summary>
    private readonly Queue<MutablePair<CubeMove, int>> _cubeMovesLeft = [];

    public RobotCommunicator(CommunicationChannelBase communication)
    {
        _communication = communication;
        _connectionEstablisher = new(communication);

        _connectionEstablisher.CommunicationEstablished += (s, e) => { _communication.DataRecieved += OnDataRecieved; CommunicationEstablished?.Invoke(this, EventArgs.Empty); };
        _connectionEstablisher.CommunicationEstablishmentFailed += (s, e) => CommunicationEstablishmentFailed?.Invoke(this, EventArgs.Empty);
        _connectionEstablisher.StartConnection();
    }

    public event CommandQueueChangedEventHandler CommandQueueChanged = delegate { };
    public event MoveQueueChangedEventHandler MoveQueueChanged = delegate { };
    public event EventHandler CommunicationEstablished = delegate { };
    public event EventHandler CommunicationEstablishmentFailed = delegate { };

    public void SendMovesToRobot(IEnumerable<RobotMove> robotMoves, IEnumerable<MutablePair<CubeMove, int>>? cubeMoves = null)
    {
        if (cubeMoves is not null)
        {
            _cubeMovesLeft.EnqueueRange(cubeMoves);
            MoveQueueChanged.Invoke(this, new(null, _cubeMovesLeft.Select((pair) => pair.Item1)));
        }

        if (!robotMoves.Any())
            return;

        _commandQueue.EnqueueRange(robotMoves);
        CommandQueueChanged.Invoke(this, new([], [.. _commandQueue]));

        _communication.SendMovesToRobot(robotMoves);
    }

    public void Dispose()
    {
        if (_communication is not null && _communication is IDisposable disposableCommunication)
            disposableCommunication.Dispose();
    }

    private void OnDataRecieved(object sender, CommunicationChannelDataEventArgs e)
    {
        int dotCount = e.RecievedData.Count(c => c == '.');

        for (int i = 0; i < dotCount; i++)
        {
            DequeueDoneCommands();
            DecrementCommandFromMoveQueue();
        }
    }

    private void DequeueDoneCommands()
    {
        if (_commandQueue.Count == 0)
            return;

        List<RobotMove> commands = _commandQueue.DequeueUntilEncountered(RobotMove.Separator);
        CommandQueueChanged.Invoke(this, new([.. commands], [.. _commandQueue]));
    }

    private void DecrementCommandFromMoveQueue()
    {
        if (_cubeMovesLeft.Count == 0)
            return;

        MutablePair<CubeMove, int> head = _cubeMovesLeft.Peek();
        if (head.Item2 > 1)         // Decrement amount of commands left for first move
            head.Item2--;
        else
            DequeueDoneMove(head);  // Remove head from queue
    }

    private void DequeueDoneMove(MutablePair<CubeMove, int> head)
    {
        _cubeMovesLeft.Dequeue();
        MoveQueueChanged.Invoke(this, new(head.Item1, _cubeMovesLeft.Select((pair) => pair.Item1)));
    }
}