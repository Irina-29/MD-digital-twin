using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class PostureAnalyzer : MonoBehaviour
{
    public GameObject warningBorder;
    private Image warningImage;
    private Coroutine blinkCoroutine;
    public TMP_Text nerveStatusLabel;
    public TMP_Text liveStatsText;
    public Image warningSign;
    public TMP_Text warningText;
    private Coroutine warningBlinkCoroutine;
    private AudioSource alarmAudio;

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

    public ArduinoCommunication arduinoComm;

    private bool isVibrating = false;

    private bool pulseSent = false;
    private float lastPulseTime = 0f;
    private float pulseCooldown = 1f; // seconds between pulses

    [Header("Posture State")]
    public bool isInBadPosture;
    private float badPostureTimer = 0f;

    private void GiveFeedback()
    {
        Debug.Log("🔔 Feedback: You've been in a bad posture for too long. Please adjust your wrist position.");

        if (warningImage != null && blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(BlinkWarning());

        if (!alarmAudio.isPlaying)
            alarmAudio.Play();
        
        if (arduinoComm != null && !isVibrating)
        {
            arduinoComm.SendVibrationCommand();
            isVibrating = true;
        }
    }

    private void StopFeedback()
    {
        if (arduinoComm != null && isVibrating)
        {
            arduinoComm.SendStopVibrationCommand();
            isVibrating = false;
        }
    }

    private IEnumerator BlinkWarning()
    {
        float duration = 0.5f;
        bool on = false;

        while (true)
        {
            on = !on;
            if (warningImage != null)
            {
                float alpha = on ? 0.6f : 0.0f;
                warningImage.color = new Color(1f, 0f, 0f, alpha);
            }

            yield return new WaitForSeconds(duration);
        }
    }

    private IEnumerator BlinkWarningSign()
    {
        while (true)
        {
            if (warningSign != null)
                warningSign.color = new Color(1f, 1f, 1f, 1f); // show

            yield return new WaitForSeconds(0.5f);

            if (warningSign != null)
                warningSign.color = new Color(1f, 1f, 1f, 0f); // hide

            yield return new WaitForSeconds(0.5f);
        }
    }

    void Start()
    {
        alarmAudio = GetComponent<AudioSource>();
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

        if (warningImage == null && warningBorder != null)
        {
            warningImage = warningBorder.GetComponent<Image>();
        }

        float pressure = pressureDatabase.GetPressure(flexionExtension, radialUlnar);
        isInBadPosture = pressure > highPressureThreshold;

        string colorCode;

        if (pressure >= 3.1f)
        {
            colorCode = "red";
            if (warningBlinkCoroutine == null && warningSign != null)
                warningBlinkCoroutine = StartCoroutine(BlinkWarningSign());

            if (warningText != null)
                warningText.alpha = 1f; // show warning text

            if (!feedbackGiven && !pulseSent && Time.time - lastPulseTime > pulseCooldown)
            {
                arduinoComm?.SendPulseVibrationCommand();
                pulseSent = true;
                lastPulseTime = Time.time;
            }
        }
        else if (pressure >= 2.5f)
        {
            pulseSent = false;
            colorCode = "yellow";
            if (warningBlinkCoroutine != null)
            {
                StopCoroutine(warningBlinkCoroutine);
                warningBlinkCoroutine = null;
            }

            if (warningSign != null)
                warningSign.color = new Color(1f, 1f, 1f, 0f); // fully hidden

            if (warningText != null)
                warningText.alpha = 0f; // hide text
        }
        else
        {
            pulseSent = false;
            colorCode = "green";
            if (warningBlinkCoroutine != null)
            {
                StopCoroutine(warningBlinkCoroutine);
                warningBlinkCoroutine = null;
            }

            if (warningSign != null)
                warningSign.color = new Color(1f, 1f, 1f, 0f); // fully hidden

            if (warningText != null)
                warningText.alpha = 0f; // hide text
        }

        liveStatsText.text =
            $"<b>Pressure:</b> <color={colorCode}>{pressure:F2} kPa</color>\n\n" +
            $"<b>Flex/Ext:</b> {flexionExtension:F1}°\n" +
            $"<b>Radial/Ulnar:</b> {radialUlnar:F1}°";

        // Debug.Log($"Pressure at angles V:{flexionExtension} H:{radialUlnar} = {pressure}");
        // Debug.Log($"Pressure = {pressure}, Threshold = {highPressureThreshold}, isBadPosture = {isInBadPosture}");

        if (isInBadPosture)
        {
            badPostureTimer += Time.deltaTime;
            Debug.Log("⚠ Bad posture detected for " + badPostureTimer.ToString("F1") + "s");
            nerveStatusLabel.text = "<b>Nerve compression:</b> <color=red>HIGH</color>";
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
            StopFeedback();
            if (alarmAudio.isPlaying)
                alarmAudio.Stop();
            nerveStatusLabel.text = "<b>Nerve compression:</b> <color=green>NORMAL</color>";
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }

            if (warningImage != null)
                warningImage.color = new Color(1f, 0f, 0f, 0f); // transparent
        }
    }
}
