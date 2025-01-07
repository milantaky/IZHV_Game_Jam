using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Slider redSlider, greenSlider, blueSlider;
    public TextMeshProUGUI colorValueText;
    public Image colorDisplay;

    private Color currentColor;
    void Start()
    {
        // Initial
        currentColor = Color.black;
        
        redSlider.onValueChanged.AddListener(UpdateColor);
        greenSlider.onValueChanged.AddListener(UpdateColor);
        blueSlider.onValueChanged.AddListener(UpdateColor);

        // Sets initial color
        UpdateColor();
    }

    // Update is called once per frame
    void UpdateColor(float value = 0)
    {
        currentColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        
        colorDisplay.color = currentColor;

        // Shows values on screen
        colorValueText.text = Mathf.RoundToInt(currentColor.r * 255) + 
                              "\n" + Mathf.RoundToInt(currentColor.g * 255) + 
                              "\n" + Mathf.RoundToInt(currentColor.b * 255);
    }
    
    public Color GetCurrentColor()
    {
        return currentColor;
    }
}
