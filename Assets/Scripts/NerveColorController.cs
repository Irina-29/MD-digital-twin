using UnityEngine;

public class NerveColorController : MonoBehaviour
{
     [Header("Material Reference")]
    public Renderer nerveRenderer; 
    private Material nerveMaterial;

    [Header("Shader Pressure Property")]
    [Range(0, 1)]
    public float targetPressure = 0f; // Value from 0 (normal) to 1 (max compression)

    [Header("Animation")]
    public float lerpSpeed = 2f; 

    private float currentPressure = 0f;

    private void Start()
    {
        if (nerveRenderer != null)
        {
            // Copy the material instance so it doesn't affect other objects
            nerveMaterial = nerveRenderer.material;
        }
        else
        {
            Debug.LogError("Nerve Renderer not assigned!");
        }
    }

    private void Update()
    {
        if (nerveMaterial == null) return;

        currentPressure = Mathf.Lerp(currentPressure, targetPressure, Time.deltaTime * lerpSpeed);

        // Update shader property
        // Debug.Log($"Current Pressure: {currentPressure}");
        nerveMaterial.SetFloat("_PressureLevel", currentPressure);

        // Debug
        // float testValue = Mathf.PingPong(Time.time, 1f);
        // nerveMaterial.SetFloat("_PressureLevel", testValue);
    }

    // Gets called from wrist movement scripts
    public void SetPressureValue(float newPressure)
    {
        targetPressure = Mathf.Clamp01(newPressure); 
    }
}
