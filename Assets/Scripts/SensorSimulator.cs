using UnityEngine;

public class SensorSimulator : MonoBehaviour
{
    public PostureAnalyzer postureAnalyzer; // Reference to the PostureAnalyzer script

    [Header("Simulation Mode")]
    public bool simulate = true;  // Set to false when using real sensors

    [Header("Simulated Wrist Angles (degrees)")]
    public float wristVerticalR = 0f;
    public float wristHorizontalR = 0f;
    public float wristVerticalL = 0f;
    public float wristHorizontalL = 0f;

    [Header("Snap Simulation Parameters")]
    public float maxVerticalAngle = 7f;
    public float maxHorizontalAngle = 5f;
    public float snapSpeed = 4f;
    public float minSnapDelay = 0.1f;
    public float maxSnapDelay = 0.35f;

    private float targetVerticalR = 0f, targetHorizontalR = 0f;
    private float targetVerticalL = 0f, targetHorizontalL = 0f;
    private float snapTimerR = 0f, snapTimerL = 0f;

    void Update()
    {
        if (simulate)
        {
            // RIGHT wrist snapping logic
            snapTimerR -= Time.deltaTime;
            if (snapTimerR <= 0f)
            {
                targetVerticalR = Random.Range(-maxVerticalAngle, maxVerticalAngle);
                targetHorizontalR = Random.Range(-maxHorizontalAngle, maxHorizontalAngle);
                snapTimerR = Random.Range(minSnapDelay, maxSnapDelay);
            }
            wristVerticalR = Mathf.Lerp(wristVerticalR, targetVerticalR, Time.deltaTime * snapSpeed);
            wristHorizontalR = Mathf.Lerp(wristHorizontalR, targetHorizontalR, Time.deltaTime * snapSpeed);

            // LEFT wrist snapping logic
            snapTimerL -= Time.deltaTime;
            if (snapTimerL <= 0f)
            {
                targetVerticalL = Random.Range(-maxVerticalAngle, maxVerticalAngle);
                targetHorizontalL = Random.Range(-maxHorizontalAngle, maxHorizontalAngle);
                snapTimerL = Random.Range(minSnapDelay, maxSnapDelay);
            }
            wristVerticalL = Mathf.Lerp(wristVerticalL, targetVerticalL, Time.deltaTime * snapSpeed);
            wristHorizontalL = Mathf.Lerp(wristHorizontalL, targetHorizontalL, Time.deltaTime * snapSpeed);

            if (postureAnalyzer != null)
            {
                // Vertical (Flexion/Extension)
                postureAnalyzer.flexionAngle = wristVerticalR > 0 ? wristVerticalR : 0f;
                postureAnalyzer.extensionAngle = wristVerticalR < 0 ? -wristVerticalR : 0f;

                // Horizontal (Radial/Ulnar)
                postureAnalyzer.radialDeviation = wristHorizontalR > 0 ? wristHorizontalR : 0f;
                postureAnalyzer.ulnarDeviation = wristHorizontalR < 0 ? -wristHorizontalR : 0f;
            }
        }
        else
        {
            // TODO: Replace this with real sensor input logic
        }
    }
}
