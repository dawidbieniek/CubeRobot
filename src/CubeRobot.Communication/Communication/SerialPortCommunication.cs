using System.Diagnostics;
using System.IO.Ports;

namespace CubeRobot.Robot.Communication;

public class SerialPortCommunication : CommunicationChannelBase, IDisposable
{
    private readonly SerialPort _port;

    public SerialPortCommunication(string portName, int baudRate)
    {
        _port = new(portName, baudRate);
        _port.DataReceived += OnPortDataRecieved;
        _port.Open();
    }

    public static string[] ValidPorts => SerialPort.GetPortNames();

    public void Dispose()
    {
        _port.Close();
        GC.SuppressFinalize(this);
    }

    public override void SendMovesToRobot(IEnumerable<RobotMove> moves)
    {
        _port.Write(moves.ToProtocolString());

#if DEBUG
        Debug.Write("Out ");
        Debug.WriteLine(moves.ToProtocolString());
#endif
    }

    private void OnPortDataRecieved(object sender, SerialDataReceivedEventArgs e)
    {
        string data = _port.ReadExisting().ToString();
        OnDataRecieved(new(data));

#if DEBUG
        Debug.Write("In  ");
        Debug.WriteLine(data);
#endif
    }
}