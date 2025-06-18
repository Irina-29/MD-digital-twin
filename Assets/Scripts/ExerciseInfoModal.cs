using UnityEngine;
using UnityEngine.UI;

public class ExerciseInfoModal : MonoBehaviour
{
    public GameObject modalPanel;
    public Button openButton;
    public Button closeButton;

    void Start()
    {
        modalPanel.SetActive(false);
        openButton.onClick.AddListener(() => modalPanel.SetActive(true));
        closeButton.onClick.AddListener(() => modalPanel.SetActive(false));
    }
}
