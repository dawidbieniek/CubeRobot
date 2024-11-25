using System.Diagnostics;

using CubeRobot.Robot.Communication;
using CubeRobot.Robot.Events;

namespace CubeRobot.Robot;

internal class ConnectionEstablisher(CommunicationChannelBase communication)
{
    private const char CommunicationEstablishmentRequestChar = '?';
    private const char CommunicationEstablishmentAcceptedChar = '$';
    private const int ConnectionEstablishmentFailureTimeoutDelay = 5000;

    private readonly CommunicationChannelBase _communication = communication;
    private readonly CancellationTokenSource _connectionEstablishmentFailureCts = new();

    public event EventHandler CommunicationEstablished = delegate { };
    public event EventHandler CommunicationEstablishmentFailed = delegate { };

    public void StartConnection()
    {
        _communication.DataRecieved += OnDataRecievedBeforeConnectionEstablishment;

        // Send connection request to robot
        _communication.SendTextToRobot(CommunicationEstablishmentRequestChar);
        // Start connection establishment faliure countdown
        _ = ConnectionEstablishmentFailureCountdown();  // Fire and forget
    }

    private void OnDataRecievedBeforeConnectionEstablishment(object sender, CommunicationChannelDataEventArgs e)
    {
        if (!e.RecievedData.Contains(CommunicationEstablishmentAcceptedChar))
            return;

        _connectionEstablishmentFailureCts.Cancel();
        _communication.DataRecieved -= OnDataRecievedBeforeConnectionEstablishment;
        CommunicationEstablished?.Invoke(this, new());
    }

    private async Task ConnectionEstablishmentFailureCountdown()
    {
        try
        {
            await Task.Delay(ConnectionEstablishmentFailureTimeoutDelay, _connectionEstablishmentFailureCts.Token);

            // Fail connection establishment
            _communication.DataRecieved -= OnDataRecievedBeforeConnectionEstablishment;
#if DEBUG
            Debug.WriteLine("Failed to establish connection");
#endif // DEBUG
            CommunicationEstablishmentFailed?.Invoke(this, EventArgs.Empty);
        }
        catch (OperationCanceledException) { }
    }
}