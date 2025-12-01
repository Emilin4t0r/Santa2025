using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsControls : MonoBehaviour
{
    public AudioMixer mixer;
    public TextMeshProUGUI volValueText, sensValueText;
    public Slider volSlider, sensSlider;
    public Toggle toggleTT, toggleADRoll, toggleADYaw;

    void Start()
    {
        toggleADRoll.onValueChanged.AddListener(OnRollToggleChanged);
        toggleADYaw.onValueChanged.AddListener(OnYawToggleChanged);
    }

    private void OnEnable()
    {
        volSlider.value = Settings.audioVolume;
        volValueText.text = (volSlider.value * 100).ToString("0") + "%";

        sensSlider.value = Settings.mouseSensitivity;
        sensValueText.text = (sensSlider.value * 100).ToString("0") + "%";

        toggleTT.isOn = Settings.showTooltips;

        toggleADRoll.isOn = Settings.useADForRoll;
        toggleADYaw.isOn = !Settings.useADForRoll;
    }

    public void SetMixerVol(float sliderValue)
    {
        // Slider 0–1 -> dB
        // -80 dB = silent, 0 dB = normal max volume
        float dB;

        if (sliderValue <= 0.0001f)
            dB = -80f; // hard mute
        else
            dB = Mathf.Log10(sliderValue) * 20f;

        mixer.SetFloat("AudioVol", dB);

        volValueText.text = (sliderValue * 100).ToString("0") + "%";

        Settings.audioVolume = sliderValue; // store 0–1 cleanly
    }

    public void SetSensitivity(float sens)
    {
        sensValueText.text = (sens * 100).ToString("0") + "%";
        Settings.mouseSensitivity = sens;
    }

    public void SetTooltipsVisible(bool val)
    {
        Settings.showTooltips = val;
    }

    private void OnRollToggleChanged(bool isOn)
    {
        if (isOn)
        {
            Settings.useADForRoll = true;
            print("Setting to Roll");
        }
    }

    private void OnYawToggleChanged(bool isOn)
    {
        if (isOn)
        {
            Settings.useADForRoll = false;
            print("Setting to Yaw");
        }
    }
}
