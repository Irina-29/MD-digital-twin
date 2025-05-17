using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Finger
{
    public Transform bone01; // base knuckle
    public Transform bone02; // middle joint
    public Transform bone03; // fingertip
}

public class TypingSimulator : MonoBehaviour
{
    public Finger[] fingers;
    public SensorSimulator sensorInput; // reference to the sensor input script

    [Header("Typing speed settings")]
    public float pressAngle = 35f; // curl of fingers while typing
    public float pressDuration = 0.25f; // one finger press duration
    public float minDelay = 0.1f; // random delay between presses)
    public float maxDelay = 0.25f;

    [Header("Wrist movement")]
    public Transform wristBoneR, wristBoneL;
    private Quaternion baseWristRotationR, baseWristRotationL; // original wrist rotation
    private Quaternion[] baseRotations; // initial local rotations of all finger bones (to preserve base pose)
    private float[] fingerCooldowns; // ensure each finger can't be typed again too quickly

    void Start()
    {
        baseRotations = new Quaternion[fingers.Length * 3];
        fingerCooldowns = new float[fingers.Length];

        for (int i = 0; i < fingers.Length; i++)
        {
            int b = i * 3;
            baseRotations[b]     = fingers[i].bone01 ? fingers[i].bone01.localRotation : Quaternion.identity;
            baseRotations[b + 1] = fingers[i].bone02 ? fingers[i].bone02.localRotation : Quaternion.identity;
            baseRotations[b + 2] = fingers[i].bone03 ? fingers[i].bone03.localRotation : Quaternion.identity;
        }

        if (wristBoneR != null)
            baseWristRotationR = wristBoneR.localRotation;
        if (wristBoneL != null)
            baseWristRotationL = wristBoneL.localRotation;

        StartCoroutine(TypingLoop());
    }

    // Runs every frame to animate the wrist movement
    void Update()
    {
        if (sensorInput == null) return;

        if (wristBoneR != null)
        {
            wristBoneR.localRotation = baseWristRotationR * Quaternion.Euler(
                sensorInput.wristVerticalR,
                0f,
                sensorInput.wristHorizontalR
            );
        }

        if (wristBoneL != null)
        {
            wristBoneL.localRotation = baseWristRotationL * Quaternion.Euler(
                sensorInput.wristVerticalL,
                0f,
                -sensorInput.wristHorizontalL // mirrored for left hand
            );
        }
    }

    // Coroutine to simulate typing by randomly pressing fingers
    IEnumerator TypingLoop()
    {
        while (true)
        {
            List<int> available = new List<int>();

            // check which fingers are available to press (not on cooldown)
            for (int i = 0; i < fingers.Length; i++)
            {
                if (Time.time >= fingerCooldowns[i])
                {
                    available.Add(i);
                }
            }

            // If there are available fingers, randomly choose one to press
            if (available.Count > 0)
            {
                int chosen = available[Random.Range(0, available.Count)];
                StartCoroutine(PressFinger(fingers[chosen], chosen));
                fingerCooldowns[chosen] = Time.time + 0.3f;
            }

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay)); // wait for a random short delay
        }
    }

    // Coroutine to animate one full key press
    IEnumerator PressFinger(Finger finger, int index)
    {
        // A key press consists of two parts: pressing down and releasing the key
        float half = pressDuration / 2f;
        float t = 0f;

        // Press down
        while (t < half)
        {
            float angle = Mathf.Lerp(0f, pressAngle, t / half);
            RotateFinger(finger, angle, index);
            t += Time.deltaTime;
            yield return null;
        }

        // Release up
        t = 0f;
        while (t < half)
        {
            float angle = Mathf.Lerp(pressAngle, 0f, t / half);
            RotateFinger(finger, angle, index);
            t += Time.deltaTime;
            yield return null;
        }
    }

    // Rotate the finger bones based on the press angle and index
    void RotateFinger(Finger finger, float angle, int index)
    {
        int i = index * 3;

        if (finger.bone01 != null)
            finger.bone01.localRotation = baseRotations[i] * Quaternion.Euler(angle * 0.4f, 0, 0);
        if (finger.bone02 != null)
            finger.bone02.localRotation = baseRotations[i + 1] * Quaternion.Euler(angle * 0.3f, 0, 0);
        if (finger.bone03 != null)
            finger.bone03.localRotation = baseRotations[i + 2] * Quaternion.Euler(angle * 0.3f, 0, 0);
    }
}