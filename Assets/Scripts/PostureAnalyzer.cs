using UnityEngine;

public class PostureAnalyzer : MonoBehaviour
{
    [Header("Thresholds")]
    public float maxFlexion = 45f;
    public float maxExtension = 30f;
    public float maxRadial = 15f;
    public float maxUlnar = 15f;

    [Header("Input Angles")]
    public float flexionAngle;
    public float extensionAngle;
    public float radialDeviation;
    public float ulnarDeviation;

    public bool isInBadPosture;
    private float badPostureTimer = 0f;

    void Update()
    {
        isInBadPosture =
            flexionAngle > maxFlexion ||
            extensionAngle > maxExtension ||
            radialDeviation > maxRadial ||
            ulnarDeviation > maxUlnar;

        if (isInBadPosture)
        {
            badPostureTimer += Time.deltaTime;
            Debug.Log("⚠ Bad posture detected for " + badPostureTimer.ToString("F1") + "s");
        }
        else
        {
            badPostureTimer = 0f;
        }
    }
}
