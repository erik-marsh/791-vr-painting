using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Color currentColor;
    public Fingerpainting fingerpainting;

    public Slider[] sliderRGBA = new Slider[3];
    public TMPro.TextMeshProUGUI[] textRGBA = new TMPro.TextMeshProUGUI[3];
    public Image colorView;

    private void Start()
    {
        UpdateColor();
    }

    public void UpdateColor()
    {
        currentColor.r = sliderRGBA[0].value;
        currentColor.g = sliderRGBA[1].value;
        currentColor.b = sliderRGBA[2].value;
        currentColor.a = 1.0f;

        colorView.color = currentColor;
        if (fingerpainting)
            fingerpainting.ChangeColor(currentColor);

        for (int i = 0; i < 3; i++)
        {
            textRGBA[i].text = sliderRGBA[i].value.ToString();
        }
    }
}
