namespace CubeRobot.Robot.Helpers;

public static class QueueExtensions
{
    /// <summary>
    /// Adds an array of elements to the end of the queue.
    /// </summary>
    /// <param name="queue"> The queue to which the elements will be added. </param>
    /// <param name="items"> An array of elements to add to the queue. </param>
    public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
    {
        foreach (T item in items)
            queue.Enqueue(item);
    }

    /// <summary>
    /// Removes elements from the front of the queue until the specified element is encountered,
    /// then returns the dequeued elements as a list. <b> The specified element will also be
    /// removed, but not added to the list </b>
    /// </summary>
    /// <param name="queue"> The queue from which elements will be removed. </param>
    /// <param name="target"> The target element to stop dequeuing at. </param>
    /// <returns>
    /// A list of elements that were dequeued from the queue up to and including the target element.
    /// </returns>
    public static List<T> DequeueUntilEncountered<T>(this Queue<T> queue, T target) where T : notnull
    {
        List<T> items = [];

        while (queue.Count > 0)
        {
            T item = queue.Dequeue();
            items.Add(item);

            if (item.Equals(target))
                return items;
        }

        return items;
    }
}