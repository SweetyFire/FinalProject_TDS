using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    private const string SAVED_QUALITY_NAME = "graphicsQuality";
    private const string SAVED_MASTER_VOLUME_NAME = "masterVolume";
    private const string SAVED_SOUNDFX_VOLUME_NAME = "soundFXVolume";
    private const string SAVED_MUSIC_VOLUME_NAME = "musicVolume";

    private const string MUSIC_VOLUME_NAME = "Music";
    private const string MASTER_VOLUME_NAME = "Master";
    private const string SOUNDFX_VOLUME_NAME = "SoundFX";

    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown _qualityDropdown;

    [Header("Sound")]
    [SerializeField] private AudioMixer _mixer;
    [Header("Master")]
    [SerializeField] private TextMeshProUGUI _masterVolumeText;
    [SerializeField] private Slider _masterVolumeSlider;
    [Header("Music")]
    [SerializeField] private TextMeshProUGUI _musicVolumeText;
    [SerializeField] private Slider _musicVolumeSlider;
    [Header("SoundFX")]
    [SerializeField] private TextMeshProUGUI _soundFXText;
    [SerializeField] private Slider _soundFXSlider;

    private void Start()
    {
        InitSettings();
    }

    public void SetQualityLevel(int level)
    {
        int newValue = Mathf.Clamp(level, 0, QualitySettings.count);
        QualitySettings.SetQualityLevel(newValue);
        
    }

    public void SetMasterVolume(float volume)
    {
        _mixer.SetFloat(MASTER_VOLUME_NAME, Mathf.Log10(GetClampedSliderValue(volume)) * 20f);
        SetSliderTextPercent(_masterVolumeText, volume);
        
    }

    public void SetMusicVolume(float volume)
    {
        _mixer.SetFloat(MUSIC_VOLUME_NAME, Mathf.Log10(GetClampedSliderValue(volume)) * 20f);
        SetSliderTextPercent(_musicVolumeText, volume);
    }

    public void SetSoundFXVolume(float volume)
    {
        _mixer.SetFloat(SOUNDFX_VOLUME_NAME, Mathf.Log10(GetClampedSliderValue(volume)) * 20f);
        SetSliderTextPercent(_soundFXText, volume);
    }

    public void SaveSettings()
    {
        SaveSystem.SetValue(SAVED_QUALITY_NAME, _qualityDropdown.value);
        SaveSystem.SetValue(SAVED_MASTER_VOLUME_NAME, _masterVolumeSlider.value);
        SaveSystem.SetValue(SAVED_SOUNDFX_VOLUME_NAME, _soundFXSlider.value);
        SaveSystem.SetValue(SAVED_MUSIC_VOLUME_NAME, _musicVolumeSlider.value);
        SaveSystem.Save();
    }

    private void InitSettings()
    {
        int quality;
        if (!SaveSystem.TryGetInt(SAVED_QUALITY_NAME, out quality))
            quality = QualitySettings.GetQualityLevel();

        _qualityDropdown.SetValueWithoutNotify(quality);


        float volume;
        if (!SaveSystem.TryGetFloat(SAVED_MASTER_VOLUME_NAME, out volume))
            volume = GetVolume(MASTER_VOLUME_NAME);

        _masterVolumeSlider.SetValueWithoutNotify(volume);
        SetSliderTextPercent(_masterVolumeText, volume);


        if (!SaveSystem.TryGetFloat(SAVED_SOUNDFX_VOLUME_NAME, out volume))
            volume = GetVolume(SOUNDFX_VOLUME_NAME);

        _soundFXSlider.SetValueWithoutNotify(volume);
        SetSliderTextPercent(_soundFXText, volume);


        if (!SaveSystem.TryGetFloat(SAVED_MUSIC_VOLUME_NAME, out volume))
            volume = GetVolume(MUSIC_VOLUME_NAME);

        _musicVolumeSlider.SetValueWithoutNotify(volume);
        SetSliderTextPercent(_musicVolumeText, volume);
    }

    private float GetClampedSliderValue(float volume)
    {
        return Mathf.Clamp(volume, 0.0001f, 1f);
    }

    private void SetSliderTextPercent(TextMeshProUGUI text, float value)
    {
        text.text = ((int)(value * 100f)).ToString() + "%";
    }

    private float GetVolume(string name)
    {
        if (_mixer.GetFloat(name, out float volume))
            return Mathf.Pow(10f, volume / 20f);
        else
            return 1f;
    }
}
