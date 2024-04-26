using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

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

    private const string MUSIC_VOLUME_NAME = "Music";
    private const string MASTER_VOLUME_NAME = "Master";
    private const string SOUNDFX_VOLUME_NAME = "SoundFX";

    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        InitSliders();
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

    private float GetClampedSliderValue(float volume)
    {
        return Mathf.Clamp(volume, 0.0001f, 1f);
    }

    private void InitSliders()
    {
        float volume = GetVolume(SOUNDFX_VOLUME_NAME);
        _masterVolumeSlider.value = volume;
        SetSliderTextPercent(_soundFXText, volume);

        volume = GetVolume(MUSIC_VOLUME_NAME);
        _musicVolumeSlider.value = volume;
        SetSliderTextPercent(_musicVolumeText, volume);

        volume = GetVolume(MASTER_VOLUME_NAME);
        _masterVolumeSlider.value = volume;
        SetSliderTextPercent(_masterVolumeText, volume);
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
