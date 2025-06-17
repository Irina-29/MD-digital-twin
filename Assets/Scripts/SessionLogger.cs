using System.IO;
using UnityEngine;

public static class SessionLogger
{
    private static string filePath = Application.persistentDataPath + "/cts_session.csv";
    private static bool headerWritten = false;

    private static float finalTotalTime = 0f;
    private static float finalBadPostureTime = 0f;

    public static void LogEntry(float time, float flexion, float ulnar, float pressure, float badPostureDuration, float totalTime)
    {
        if (!headerWritten)
        {
            File.AppendAllText(filePath, "Time,FlexionExtension,RadialUlnar,PressureTyping,BadPostureDuration,TotalTime\n");
            headerWritten = true;
        }

        string line = $"{time:F2},{flexion:F2},{ulnar:F2},{pressure:F2},{badPostureDuration:F2},{totalTime:F2}\n";
        File.AppendAllText(filePath, line);

        // store latest durations
        finalTotalTime = totalTime;
        finalBadPostureTime = badPostureDuration;
    }

    public static void WriteSummary(float badPostureDuration, float totalTime, float maxPressure, float avgPressure)
    {
        string path = Path.Combine(Application.persistentDataPath, "PostureSessionSummary.txt");

        float badPosturePercent = (totalTime > 0f) ? (badPostureDuration / totalTime) * 100f : 0f;

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            writer.WriteLine("Session Summary");
            writer.WriteLine("---------------------");
            writer.WriteLine($"Total Time: {totalTime:F2} seconds");
            writer.WriteLine($"Bad Posture Duration: {badPostureDuration:F2} seconds");
            writer.WriteLine($"Bad Posture Percentage: {badPosturePercent:F1}%");
            writer.WriteLine($"Max Pressure: {maxPressure:F2} kPa");
            writer.WriteLine($"Average Pressure: {avgPressure:F2} kPa");
        }

        Debug.Log("✅ Summary written to: " + path);
    }

}
