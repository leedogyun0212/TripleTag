using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_OptionMusic : UIBase
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI Sound;
    [SerializeField] Image MainImage;
    [SerializeField] Sprite OffImage;
    [SerializeField] Sprite OnImage;
    private void OnEnable()
    {
        SoundText(slider.value);
        slider.onValueChanged.AddListener(SoundText);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(SoundText);
    }

    public void SoundText(float value)
    {
        Sound?.SetText($"{value * 100.0f:0}%");
        if(value * 100.0f<1.0f)
        {
            MainImage.sprite = OffImage;
        }
        else
        {
            MainImage.sprite = OnImage;
        }
    }
}
