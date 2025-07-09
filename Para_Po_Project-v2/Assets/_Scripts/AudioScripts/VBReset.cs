using UnityEngine;
using UnityEngine.UI;

public class VBReset : MonoBehaviour
{
    public Slider targetSlider;
    public Slider[] otherSliders; // assign all other sliders in the inspector

    private float originalValue;
    private float[] otherOriginalValues;
    private bool isZero = false;

    void Start()
    {
        if (targetSlider != null)
        {
            originalValue = targetSlider.value;
        }

        if (otherSliders != null && otherSliders.Length > 0)
        {
            otherOriginalValues = new float[otherSliders.Length];
            for (int i = 0; i < otherSliders.Length; i++)
            {
                if (otherSliders[i] != null)
                {
                    otherOriginalValues[i] = otherSliders[i].value;
                }
            }
        }
    }

    public void ResetSliderValue()
    {
        if (targetSlider == null) return;

        bool isMaster = targetSlider.name == "MasterVolumeSlider";

        if (isZero)
        {
            targetSlider.value = originalValue;
            if (isMaster && otherSliders != null)
            {
                for (int i = 0; i < otherSliders.Length; i++)
                {
                    if (otherSliders[i] != null)
                        otherSliders[i].value = otherOriginalValues[i];
                }
            }
            isZero = false;
        }
        else
        {
            targetSlider.value = 0f;
            if (isMaster && otherSliders != null)
            {
                for (int i = 0; i < otherSliders.Length; i++)
                {
                    if (otherSliders[i] != null)
                        otherSliders[i].value = 0f;
                }
            }
            isZero = true;
        }
    }
}
