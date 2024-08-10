using CubeRobot.Robot.Helpers;

namespace CubeRobot.Robot.Test;

[TestClass]
public class QueueExtensionsTests
{
    private Queue<int> _queue = null!;

    [TestInitialize]
    public void Initialize()
    {
        _queue = new();
    }

    [TestMethod]
    public void EnqueueRange_AddsElements_ForEmptyQueue()
    {
        int[] itemsToAdd = [1, 2, 3];

        _queue.EnqueueRange(itemsToAdd);

        Assert.AreEqual(3, _queue.Count);
        Assert.AreEqual(1, _queue.Dequeue());
        Assert.AreEqual(2, _queue.Dequeue());
        Assert.AreEqual(3, _queue.Dequeue());
    }

    [TestMethod]
    public void EnqueueRange_AddsElementsToEnd_ForNonEmptyQueue()
    {
        _queue.Enqueue(0);
        int[] itemsToAdd = [1, 2, 3];

        _queue.EnqueueRange(itemsToAdd);

        Assert.AreEqual(4, _queue.Count);
        Assert.AreEqual(0, _queue.Dequeue());
        Assert.AreEqual(1, _queue.Dequeue());
        Assert.AreEqual(2, _queue.Dequeue());
        Assert.AreEqual(3, _queue.Dequeue());
    }

    [TestMethod]
    public void DequeueUntilEncountered_RemovesElementsUntilTargetFound_ForQueueContainingTheTarget()
    {
        int target = 3;
        int[] initialItems = [1, 2, target, 4];
        foreach (int item in initialItems)
            _queue.Enqueue(item);

        List<int> dequeuedItems = _queue.DequeueUntilEncountered(target);

        Assert.AreEqual([1, 2], dequeuedItems); // Returned list shouldn't contain the target element
        Assert.AreEqual(1, _queue.Count); // Only '4' should remain in the queue
    }

    [TestMethod]
    public void DequeueUntilEncountered_RemovesAllElements_ForQueueWithoutTarget()
    {
        int target = 4;
        int[] initialItems = [1, 2, 3];
        foreach (int item in initialItems)
            _queue.Enqueue(item);

        List<int> dequeuedItems = _queue.DequeueUntilEncountered(target);

        Assert.AreEqual([1, 2, 3], dequeuedItems);
        Assert.AreEqual(0, _queue.Count); // Queue should be empty
    }

    [TestMethod]
    public void DequeueUntilEncountered_ReturnsEmptyList_ForEmptyQueue()
    {
        List<int> dequeuedItems = _queue.DequeueUntilEncountered(1);

        Assert.AreEqual(0, dequeuedItems.Count);
    }
}