using UnityEngine;

public class PostureAnalyzer : MonoBehaviour
{
    // [Header("Thresholds")]
    // public float maxFlexion = 45f;
    // public float maxExtension = 30f;
    // public float maxRadial = 15f;
    // public float maxUlnar = 15f;

    [Header("Pressure Threshold")]
    public float highPressureThreshold = 3f;

    // [Header("Input Angles")]
    // public float flexionAngle;
    // public float extensionAngle;
    // public float radialDeviation;
    // public float ulnarDeviation;

    [Header("Feedback")]
    public float badPostureDurationThreshold = 60f; // 1 minute
    private bool feedbackGiven = false;

    [Header("Sensor Input")]
    public float flexionExtension;   // Positive = Flexion, Negative = Extension
    public float radialUlnar;        // Positive = Radial, Negative = Ulnar

    public WristPressureDatabase pressureDatabase;

    [Header("Posture State")]
    public bool isInBadPosture;
    private float badPostureTimer = 0f;

    private void GiveFeedback()
    {
        Debug.Log("🔔 Feedback: You've been in a bad posture for too long. Please adjust your wrist position.");
        // TODO: Trigger a UI warning, vibration, sound, or log to file
    }

    void Update()
    {
        // isInBadPosture =
        //     flexionAngle > maxFlexion ||
        //     extensionAngle > maxExtension ||
        //     radialDeviation > maxRadial ||
        //     ulnarDeviation > maxUlnar;

        // float flexionExtension = flexionAngle - extensionAngle;
        // float radialUlnar = radialDeviation - ulnarDeviation;

        float pressure = pressureDatabase.GetPressure(flexionExtension, radialUlnar);
        isInBadPosture = pressure > highPressureThreshold;

        Debug.Log($"Pressure at angles V:{flexionExtension} H:{radialUlnar} = {pressure}");
        Debug.Log($"Pressure = {pressure}, Threshold = {highPressureThreshold}, isBadPosture = {isInBadPosture}");

        if (isInBadPosture)
        {
            badPostureTimer += Time.deltaTime;
            Debug.Log("⚠ Bad posture detected for " + badPostureTimer.ToString("F1") + "s");
            if (badPostureTimer >= badPostureDurationThreshold && !feedbackGiven)
            {
                GiveFeedback();
                feedbackGiven = true;
            }
        }
        else
        {
            badPostureTimer = 0f;
            feedbackGiven = false;
        }
    }
}
