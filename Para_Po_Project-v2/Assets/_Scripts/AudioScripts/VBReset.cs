using UnityEngine;
using UnityEngine.UI;

public class VBReset : MonoBehaviour
{
    public Slider targetSlider;
    public Slider[] otherSliders;
    public GameObject muteImage;

    private float lastNonZeroValue = 0.5f;
    private float[] otherLastNonZeroValues;
    private bool isZero = false;
    private bool suppressUpdate = false;

    void Start()
    {
        if (targetSlider != null)
        {
            // Ensure fallback is not zero at start
            lastNonZeroValue = (targetSlider.value == 0f) ? 0.5f : targetSlider.value;
            targetSlider.onValueChanged.AddListener(OnTargetSliderChanged);
        }

        if (otherSliders != null && otherSliders.Length > 0)
        {
            otherLastNonZeroValues = new float[otherSliders.Length];
            for (int i = 0; i < otherSliders.Length; i++)
            {
                if (otherSliders[i] != null)
                {
                    otherLastNonZeroValues[i] = (otherSliders[i].value == 0f) ? 0.5f : otherSliders[i].value;
                    int index = i;
                    otherSliders[i].onValueChanged.AddListener((val) => OnOtherSliderChanged(index, val));
                }
            }
        }

        UpdateMuteImage();
    }

    private void OnTargetSliderChanged(float value)
    {
        if (suppressUpdate) return;

        // If manually set to 0, ignore storing, fallback remains at 0.5
        if (value > 0f)
        {
            lastNonZeroValue = value;
        }
        else
        {
            lastNonZeroValue = Mathf.Max(lastNonZeroValue, 0.5f);
        }

        CheckIfMuted();
    }

    private void OnOtherSliderChanged(int index, float value)
    {
        if (suppressUpdate) return;

        if (value > 0f)
        {
            otherLastNonZeroValues[index] = value;
        }
        else
        {
            otherLastNonZeroValues[index] = Mathf.Max(otherLastNonZeroValues[index], 0.5f);
        }

        CheckIfMuted();
    }

    public void ResetSliderValue()
    {
        if (targetSlider == null) return;

        bool isMaster = targetSlider.name == "MasterVolumeSlider";
        suppressUpdate = true;

        if (isZero)
        {
            // Restore last known non-zero or fallback
            targetSlider.value = Mathf.Max(lastNonZeroValue, 0.5f);

            if (isMaster && otherSliders != null)
            {
                for (int i = 0; i < otherSliders.Length; i++)
                {
                    if (otherSliders[i] != null)
                        otherSliders[i].value = Mathf.Max(otherLastNonZeroValues[i], 0.5f);
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

        suppressUpdate = false;
        UpdateMuteImage();
    }

    private void CheckIfMuted()
    {
        isZero = targetSlider.value == 0f;
        UpdateMuteImage();
    }

    private void UpdateMuteImage()
    {
        if (muteImage != null)
        {
            muteImage.SetActive(isZero);
        }
    }
}