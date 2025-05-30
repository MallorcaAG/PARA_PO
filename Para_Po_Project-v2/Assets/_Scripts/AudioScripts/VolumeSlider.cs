using UnityEngine;
using UnityEngine.UI;

enum AudioType
{
    Master, Music, SFX
};

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider mySlider;
    [SerializeField] private AudioType myAudioType;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (DataManager.Instance == null)
        {
            mySlider.value = 0.5f;
            return;
        }

        switch(myAudioType)
        {
            case AudioType.Master:
                mySlider.value = DataManager.Instance.MasterVolumeSliderValue;
                break;

            case AudioType.Music:
                mySlider.value = DataManager.Instance.MusicVolumeSliderValue;
                break;

            case AudioType.SFX:
                mySlider.value = DataManager.Instance.SFXVolumeSliderValue;
                break;
        }
    }

}
