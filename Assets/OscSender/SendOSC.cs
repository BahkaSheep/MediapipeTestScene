using UnityEngine;
using extOSC;

public class SendOSC : MonoBehaviour
{
    [Header("ESP8266 Settings")]
    public string esp8266IPAddress = "192.168.1.18"; // Replace with the IP address of your ESP8266
    public int oscPort = 7000; // Port on which the ESP8266 is listening for OSC messages

    [Header("Motor and Valve Control")]
    [Range(0, 1024)] public int motor1Value = 128;
    [Range(0, 1024)] public int motor2Value = 128;

    public bool valve1 = false;
    public bool valve2 = false;

    private int valve1State = 0;
    private int valve2State = 0;

    private OSCTransmitter transmitter;
    private int previousMotor1Value;
    private int previousMotor2Value;
    private int previousValve1State;
    private int previousValve2State;

    private void Start()
    {
        // Create an OSC transmitter
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        transmitter.RemoteHost = esp8266IPAddress;
        transmitter.RemotePort = oscPort;

        // Initialize previous values
        previousMotor1Value = motor1Value;
        previousMotor2Value = motor2Value;
        previousValve1State = valve1State;
        previousValve2State = valve2State;
    }

    private void Update()
    {

        valve1State = valve1 ? 1 : 0;
        valve2State = valve2 ? 1 : 0;


        // Check for changes and send OSC messages only when values change
        if (motor1Value != previousMotor1Value)
        {
            SendMotor1OSCMessage(motor1Value);
            previousMotor1Value = motor1Value;
        }

        if (motor2Value != previousMotor2Value)
        {
            SendMotor2OSCMessage(motor2Value);
            previousMotor2Value = motor2Value;
        }

        if (valve1State != previousValve1State)
        {
            SendValve1OSCMessage(valve1State);
            previousValve1State = valve1State;
        }

        if (valve2State != previousValve2State)
        {
            SendValve2OSCMessage(valve2State);
            previousValve2State = valve2State;
        }
    }

    private void SendMotor1OSCMessage(int value)
    {
        var message = new OSCMessage("/motor1");
        message.AddValue(OSCValue.Int(value));
        transmitter.Send(message);
    }

    private void SendMotor2OSCMessage(int value)
    {
        var message = new OSCMessage("/motor2");
        message.AddValue(OSCValue.Int(value));
        transmitter.Send(message);
    }

    private void SendValve1OSCMessage(int state)
    {
        var message = new OSCMessage("/valve1");
        message.AddValue(OSCValue.Int(state));
        transmitter.Send(message);
    }

    private void SendValve2OSCMessage(int state)
    {
        var message = new OSCMessage("/valve2");
        message.AddValue(OSCValue.Int(state));
        transmitter.Send(message);
    }
}