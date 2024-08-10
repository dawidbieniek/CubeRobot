using System.Reflection;

using CubeRobot.Models.RubiksCube;
using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Helpers;

namespace CubeRobot.Robot.Test;

[TestClass]
public class RobotCommunicatorTests
{
    private RobotCommunicator _robotCommunicator = null!;
    private MockCommunicationChannel _communicationChannel = null!;

    [TestInitialize]
    public void Initialize()
    {
        _communicationChannel = new();
        _robotCommunicator = new(_communicationChannel);
    }

    [TestMethod]
    public void SendMovesToRobot_RasisesEventsWithCorrectArgs_ForQueueingToEmptyQueues()
    {
        List<MutablePair<CubeMove, int>> cubeMoves = [new(CubeMove.F, 2), new(CubeMove.R, 1)];
        List<RobotMove> robotMoves = [RobotMove.MoveFrontForward, RobotMove.Separator, RobotMove.RotateFrontClockwise, RobotMove.Separator, RobotMove.RotateRightClockwise, RobotMove.Separator];

        List<RobotMove> queuedRobotMoves = [];
        _robotCommunicator.CommandQueueChanged += (s, e) =>
        {
            queuedRobotMoves = e.RemainingCommands.ToList();
        };

        List<CubeMove> queuedCubeMoves = [];
        _robotCommunicator.MoveQueueChanged += (s, e) =>
        {
            queuedCubeMoves = e.RemainingMoves.ToList();
        };

        _robotCommunicator.SendMovesToRobot(robotMoves, cubeMoves);

        CollectionAssert.AreEqual(robotMoves, queuedRobotMoves);
        CollectionAssert.AreEqual(cubeMoves.Select(p => p.Item1).ToList(), queuedCubeMoves);
    }

    [TestMethod]
    public void SendMovesToRobot_RasisesEventsWithCorrectArgs_ForQueueingToAlreadyPopulatedQueues()
    {
        // Arrange
        List<MutablePair<CubeMove, int>> initialCubeMoves = [new(CubeMove.F, 2), new(CubeMove.R, 1)];
        List<MutablePair<CubeMove, int>> addedCubeMoves = [new(CubeMove.L, 2)];
        List<RobotMove> initialRobotMoves = [RobotMove.MoveFrontForward, RobotMove.Separator, RobotMove.RotateFrontClockwise, RobotMove.Separator, RobotMove.RotateRightClockwise, RobotMove.Separator];
        List<RobotMove> addedRobotMoves = [RobotMove.MoveLeftForward, RobotMove.Separator, RobotMove.RotateLeftClockwise, RobotMove.Separator];

        _robotCommunicator.SendMovesToRobot(initialRobotMoves, initialCubeMoves);

        List<RobotMove> remainingRobotMoves = [];
        _robotCommunicator.CommandQueueChanged += (s, e) =>
        {
            remainingRobotMoves = e.RemainingCommands.ToList();
        };

        List<CubeMove> remainingCubeMoves = [];
        _robotCommunicator.MoveQueueChanged += (s, e) =>
        {
            remainingCubeMoves = e.RemainingMoves.ToList();
        };

        // Act
        _robotCommunicator.SendMovesToRobot(addedRobotMoves, addedCubeMoves);

        // Assert
        List<MutablePair<CubeMove, int>> allCubeMoves = [.. initialCubeMoves, .. addedCubeMoves];
        List<RobotMove> allRobotMoves = [.. initialRobotMoves, .. addedRobotMoves];
        CollectionAssert.AreEqual(allRobotMoves, remainingRobotMoves);
        CollectionAssert.AreEqual(allCubeMoves.Select(p => p.Item1).ToList(), remainingCubeMoves);
    }

    [TestMethod]
    public void SendMovesToRobot_SendsOnlyNewMovesToCommunicationChannel_ForSendingMovesToAlreadyPopulatedQueues()
    {
        // Arrange
        List<RobotMove> initialRobotMoves = [RobotMove.MoveFrontForward, RobotMove.Separator, RobotMove.RotateFrontClockwise, RobotMove.Separator, RobotMove.RotateRightClockwise, RobotMove.Separator];
        List<RobotMove> addedRobotMoves = [RobotMove.MoveLeftForward, RobotMove.RotateLeftClockwise];

        _robotCommunicator.SendMovesToRobot(initialRobotMoves);

        // Act
        _robotCommunicator.SendMovesToRobot(addedRobotMoves);

        // Assert
        CollectionAssert.AreEqual(addedRobotMoves, _communicationChannel.LastMovesSent);
        Assert.Equals(0, _communicationChannel.LastMovesSent.Intersect(initialRobotMoves).Count());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public void OnDataReceived_DecrementsFirstMoveCounter_ForDotCharsRecieved(int dotCount)
    {
        // Arrange
        List<MutablePair<CubeMove, int>> initialCubeMoves = [new(CubeMove.F, dotCount + 1), new(CubeMove.R, 1)];
        _robotCommunicator.SendMovesToRobot([], initialCubeMoves);

        // Act
        _communicationChannel.SimulateRecieveData(new string('.', dotCount));

        // Assert
        FieldInfo moveQueueField = typeof(RobotCommunicator).GetField("_cubeMovesLeft", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception();
        Queue<MutablePair<CubeMove, int>> moveQueue = moveQueueField.GetValue(_robotCommunicator) as Queue<MutablePair<CubeMove, int>> ?? throw new Exception(); ;
        MutablePair<CubeMove, int> firstMove = moveQueue.Peek();
        Assert.Equals(1, firstMove.Item2);
    }

    [TestMethod]
    public void OnDataReceived_DoesNothing_ForDifferentCharsRecieved()
    {
        // Arrange
        List<MutablePair<CubeMove, int>> initialCubeMoves = [new(CubeMove.F, 2), new(CubeMove.R, 1)];
        _robotCommunicator.SendMovesToRobot([], initialCubeMoves);

        // Act
        _communicationChannel.SimulateRecieveData("abcdefg");

        // Assert
        FieldInfo moveQueueField = typeof(RobotCommunicator).GetField("_cubeMovesLeft", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception();
        Queue<MutablePair<CubeMove, int>> moveQueue = moveQueueField.GetValue(_robotCommunicator) as Queue<MutablePair<CubeMove, int>> ?? throw new Exception(); ;
        MutablePair<CubeMove, int> firstMove = moveQueue.Peek();
        Assert.Equals(2, firstMove.Item2);
    }

    [TestMethod]
    public void OnDataReceived_RemovesFirstMoveFromQueue_ForDotCharRecievedWhenFirstMoveCounterIs1()
    {
        // Arrange
        List<MutablePair<CubeMove, int>> initialCubeMoves = [new(CubeMove.F, 2), new(CubeMove.R, 1)];
        _robotCommunicator.SendMovesToRobot([], initialCubeMoves);

        // Act
        _communicationChannel.SimulateRecieveData("..");

        // Assert
        FieldInfo moveQueueField = typeof(RobotCommunicator).GetField("_cubeMovesLeft", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception();
        Queue<MutablePair<CubeMove, int>> moveQueue = moveQueueField.GetValue(_robotCommunicator) as Queue<MutablePair<CubeMove, int>> ?? throw new Exception(); ;
        MutablePair<CubeMove, int> firstMove = moveQueue.Peek();
        Assert.Equals(initialCubeMoves[1].Item1, firstMove.Item1);
        Assert.Equals(initialCubeMoves[1].Item2, firstMove.Item2);
    }

    [TestMethod]
    public void Dispose_DisposesCommunicationChannel()
    {
        _robotCommunicator.Dispose();

        Assert.IsTrue(_communicationChannel.IsDisposed);
    }

    // Helper classes for mocking
    public sealed class MockCommunicationChannel : CommunicationChannelBase, IDisposable
    {
        public List<RobotMove> LastMovesSent { get; private set; } = [];
        public bool IsDisposed { get; private set; }

        public override void SendMovesToRobot(IEnumerable<RobotMove> moves)
        {
            LastMovesSent = [.. moves];
        }

        public void SimulateRecieveData(string data)
        {
            OnDataRecieved(new(data));
        }

        public void Dispose() => IsDisposed = true;
    }
}