using UnityEngine;

public class WristPressureDatabase : MonoBehaviour
{
    public TextAsset jsonFile;
    private WristPressureDataset dataset;

    void Awake()
    {
        if (jsonFile == null)
        {
            jsonFile = Resources.Load<TextAsset>("WristPressureData");
        }

        dataset = JsonUtility.FromJson<WristPressureDataset>(jsonFile.text);
    }

    public float GetPressure(float flexionExtension, float radialUlnar, bool typing = true)
    {
        float bestScore = float.MaxValue;
        WristPressureEntry closest = null;

        foreach (var entry in dataset.entries)
        {
            float score = Mathf.Pow(entry.flexionExtension - flexionExtension, 2) +
                          Mathf.Pow(entry.radialUlnar - radialUlnar, 2);

            if (score < bestScore)
            {
                bestScore = score;
                closest = entry;
            }
        }

        if (closest == null) return 0f;
        return typing ? closest.pressureTyping : closest.pressureStatic;
    }
}
