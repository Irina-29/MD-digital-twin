using UnityEngine;
using System.IO.Ports;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ArduinoCommunication : MonoBehaviour
{
    public string portName = "COM5";
    public int baudRate = 115200;

    private SerialPort serialPort;
    public SensorSimulator sensorSimulator;
    public PostureAnalyzer postureAnalyzer;

    public TMP_Text calibrationStatusText;
    public Button calibrateButton;

    private InputControl controls;

    private bool isVibrating = false;

    void Awake()
    {
        controls = new InputControl();
        controls.Player.Calibrate.performed += ctx => SendCalibrationCommand();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        Debug.Log("Serial port name: " + portName);
        serialPort = new SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            serialPort.ReadTimeout = 10;
            serialPort.DtrEnable = true;
        }
        catch (System.Exception e)
        {
            Debug.Log("Serial port error: " + e.Message);
        }

        if (calibrationStatusText != null)
        {
            calibrationStatusText.text = "NOT CALIBRATED";
            calibrationStatusText.color = Color.red;
        }

        if (calibrateButton != null)
            calibrateButton.onClick.AddListener(SendCalibrationCommand);
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string line = serialPort.ReadLine().Trim();

                if (line == "CALIBRATED")
                {
                    Debug.Log("Calibration confirmed from Arduino");
                    if (calibrationStatusText != null)
                    {
                        calibrationStatusText.text = "CALIBRATED";
                        calibrationStatusText.color = Color.green;
                    }
                    return;
                }

                string[] values = line.Split(',');
                if (values.Length >= 2)
                {
                    float vertical = float.Parse(values[0]);
                    float horizontal = float.Parse(values[1]);

                    Debug.Log($"vertical: {vertical} horizontal: {horizontal}");

                    sensorSimulator.wristVerticalR = vertical;
                    sensorSimulator.wristHorizontalR = -horizontal;

                    if (postureAnalyzer != null)
                    {
                        postureAnalyzer.flexionExtension = vertical;
                        postureAnalyzer.radialUlnar = -horizontal;
                    }
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

    public void SendVibrationCommand()
    {
        if (serialPort != null && serialPort.IsOpen && !isVibrating)
        {
            serialPort.WriteLine("VIB_ON");
            Debug.Log("Sent VIB_ON to Arduino");
            isVibrating = true;
        }
    }

    public void SendStopVibrationCommand()
    {
        if (serialPort != null && serialPort.IsOpen && isVibrating)
        {
            serialPort.WriteLine("VIB_OFF");
            Debug.Log("Sent VIB_OFF to Arduino");
            isVibrating = false;
        }
    }

    public void SendPulseVibrationCommand()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine("VIB_PULSE");
            Debug.Log("Sent VIB_PULSE to Arduino");
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}
