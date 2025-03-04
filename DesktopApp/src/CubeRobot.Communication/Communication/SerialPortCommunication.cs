using System.Diagnostics;
using System.IO.Ports;

namespace CubeRobot.Robot.Communication;

public sealed class SerialPortCommunication : CommunicationChannelBase, IDisposable
{
    private readonly SerialPort _port;

    public SerialPortCommunication(string portName, int baudRate = 9600)
    {
        _port = new(portName, baudRate);
        _port.DataReceived += OnPortDataRecieved;
        _port.Open();
    }

    public static string[] ValidPorts => SerialPort.GetPortNames();

    public void Dispose()
    {
        _port.Close();
    }

    public override void SendTextToRobot(string text)
    {
        _port.Write(text);

#if DEBUG
        Debug.WriteLine($"Sending '{text}'");
#endif
    }

    private void OnPortDataRecieved(object sender, SerialDataReceivedEventArgs e)
    {
        string data = _port.ReadExisting();
        OnDataRecieved(new(data));

#if DEBUG
        Debug.WriteLine($"Recieved '{data}'");
#endif
    }
}