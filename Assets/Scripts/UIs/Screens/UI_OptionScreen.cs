using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_OptionScreen : UI_ScreenBase
{
    [SerializeField] RectTransform myTransform;

    public void OnEnable()
    {
        //myTransform.SetAsLastSibling();
    }
}
