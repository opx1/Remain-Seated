using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [Header("Volume Reference")]
    [SerializeField] private Volume globalVolume;

    [Header("UI Slider References")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider saturationSlider;

    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        // Make sure the Volume and sliders are assigned
        if (globalVolume == null)
        {
            Debug.LogError("Global Volume not assigned in BrightnessController.");
            return;
        }

        // Try to grab the ColorAdjustments override from the Volume
        if (!globalVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.LogError("No ColorAdjustments override found in Volume Profile!");
            return;
        }

        // Initialize slider listeners (only if sliders exist)
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            // Optional: sync current value
            brightnessSlider.value = colorAdjustments.postExposure.value;
        }

        if (saturationSlider != null)
        {
            saturationSlider.onValueChanged.AddListener(SetSaturation);
            saturationSlider.value = colorAdjustments.saturation.value;
        }
    }

    public void SetBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = value;
        }
    }

    public void SetSaturation(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = value;
        }
    }
}
