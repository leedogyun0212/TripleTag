using TMPro;
using UnityEngine;

public class UI_Version : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI targetText;

    private void Start()
    {
        targetText.SetText($"V.{Application.version}");
    }
}

