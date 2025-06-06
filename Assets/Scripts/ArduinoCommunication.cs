using UnityEngine;
using System.IO.Ports;
using UnityEngine.InputSystem;

public class ArduinoCommunication : MonoBehaviour
{
    public string portName = "COM5";
    public int baudRate = 19200;

    private SerialPort serialPort;
    public SensorSimulator sensorSimulator;

    private InputControl controls;

    void Awake()
    {
        controls = new InputControl();
        controls.Player.Calibrate.performed += ctx => SendCalibrationCommand();
    }

    void OnEnable()
    {
        controls.Enable(); // Enable input
    }

    void OnDisable()
    {
        controls.Disable(); // Disable input
    }

    void Start()
    {
        Debug.Log("Serial port name: " + portName);
        serialPort = new SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            serialPort.ReadTimeout = 10;
            // serialPort.NewLine = "\n";
            serialPort.DtrEnable = true; 
        }
        catch (System.Exception e)
        {
            Debug.Log("Serial port error: " + e.Message);
        }
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     SendCalibrationCommand();
        // }


        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string line = serialPort.ReadLine();
                string[] values = line.Split(',');

                if (values.Length >= 2)
                {

                    float vertical = float.Parse(values[0]);
                    float horizontal = float.Parse(values[1]);
                //     // bool typing = values[2] == "1";
                    Debug.Log("vertical: " + vertical + " horizontal: " + horizontal);

                    // sensorSimulator.simulate = false;
                    sensorSimulator.wristVerticalR = vertical * 1.5f;
                    sensorSimulator.wristHorizontalR = horizontal;
                }
                else
                {
                    Debug.LogWarning("Invalid serial data: " + line);
                }
            }
            catch (System.TimeoutException) { }
            catch (System.Exception e)
            {
                Debug.LogWarning("Serial read failed: " + e.Message);
            }
        }
    }

    public void SendCalibrationCommand()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine("CAL");
            Debug.Log("Sent calibration command to Arduino");
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }   
}