using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PostureReportLogger : MonoBehaviour
{
    [System.Serializable]
    public class PostureEntry
    {
        public string timestamp;
        public float flexionExtension;
        public float radialUlnar;
        public float pressure;
        public float badPostureDuration;
        public string status;  // "Yellow" or "Red"
    }

    private List<PostureEntry> entries = new List<PostureEntry>();
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "CTS_Report.html");
        // ClearReport(); // Start with fresh file
    }

    public void LogEntry(float flexionExtension, float radialUlnar, float pressure, float badPostureDuration, string status)
    {
        var newEntry = new PostureEntry
        {
            // Include full date + time in timestamp
            timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            flexionExtension = flexionExtension,
            radialUlnar = radialUlnar,
            pressure = pressure,
            badPostureDuration = badPostureDuration,
            status = status
        };

        entries.Add(newEntry);
        UnityEngine.Debug.Log($"📋 Logged Entry: {newEntry.timestamp} | Status: {status} | Pressure: {pressure:F2} | Duration: {badPostureDuration:F1}");
    }

    public void GenerateAndOpenHTMLReport()
    {
        GenerateHTML();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
#elif UNITY_STANDALONE_OSX
        Process.Start("open", filePath);
#elif UNITY_STANDALONE_LINUX
        Process.Start("xdg-open", filePath);
#endif
        UnityEngine.Debug.Log("✅ HTML report saved to and opened: " + filePath);
    }

    public void ClearReport()
    {
        entries.Clear();
        File.WriteAllText(filePath,
            "<html><head><meta charset=\"UTF-8\"><style>" +
            "body { font-family: 'Segoe UI', Tahoma, sans-serif; margin: 20px; background-color: #f9f9f9; color: #333; }" +
            "h2 { border-bottom: 2px solid #555; padding-bottom: 10px; }" +
            "table { width: 100%; border-collapse: collapse; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }" +
            "th, td { border: 1px solid #ccc; padding: 10px; text-align: center; }" +
            "th { background-color: #e0e0e0; }" +
            "tr:nth-child(even) { background-color: #fdfdfd; }" +
            "tr:nth-child(odd) { background-color: #ffffff; }" +
            "tr.red { background-color: #ffe5e5; }" +
            "tr.yellow { background-color: #fffbe5; }" +
            "</style></head><body><h2>Carpal Tunnel Report</h2><p>No data recorded yet.</p></body></html>");
        UnityEngine.Debug.Log("🧹 Cleared report file at: " + filePath);
    }

    private void GenerateHTML()
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("<html><head><meta charset=\"UTF-8\"><style>");
            writer.WriteLine("body { font-family: 'Segoe UI', Tahoma, sans-serif; margin: 20px; background-color: #f9f9f9; color: #333; }");
            writer.WriteLine("h2 { border-bottom: 2px solid #555; padding-bottom: 10px; }");
            writer.WriteLine("table { width: 100%; border-collapse: collapse; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }");
            writer.WriteLine("th, td { border: 1px solid #ccc; padding: 10px; text-align: center; }");
            writer.WriteLine("th { background-color: #e0e0e0; }");
            writer.WriteLine("tr:nth-child(even) { background-color: #fdfdfd; }");
            writer.WriteLine("tr:nth-child(odd) { background-color: #ffffff; }");
            writer.WriteLine("tr.red { background-color: #ffe5e5; }");
            writer.WriteLine("tr.yellow { background-color: #fffbe5; }");
            // Sticky header CSS
            writer.WriteLine("thead th { position: sticky; top: 0; background-color: #e0e0e0; z-index: 2; }");
            writer.WriteLine("</style></head><body>");
            writer.WriteLine("<h2>Carpal Tunnel Posture Report</h2>");
            writer.WriteLine("<table>");
            writer.WriteLine("<thead><tr><th>Timestamp</th><th>Flexion/Extension (°)</th><th>Radial/Ulnar (°)</th><th>Pressure (kPa)</th><th>Duration (s)</th><th>Status</th></tr></thead>");
            writer.WriteLine("<tbody>");

            foreach (var entry in entries)
            {
                string rowClass = entry.status.ToLower();
                writer.WriteLine($"<tr class='{rowClass}'>" +
                                 $"<td>{entry.timestamp}</td>" +
                                 $"<td>{entry.flexionExtension:F1}</td>" +
                                 $"<td>{entry.radialUlnar:F1}</td>" +
                                 $"<td>{entry.pressure:F2}</td>" +
                                 $"<td>{entry.badPostureDuration:F1}</td>" +
                                 $"<td>{entry.status}</td>" +
                                 $"</tr>");
            }

            writer.WriteLine("</tbody></table></body></html>");
        }
    }
}
