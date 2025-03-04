namespace CubeRobot.Robot.Events;

public delegate void CommunicationChannelDataRecievedEventHandler(object sender, CommunicationChannelDataEventArgs e);

public class CommunicationChannelDataEventArgs(string recievedData) : EventArgs
{
    public string RecievedData { get; set; } = recievedData;
}