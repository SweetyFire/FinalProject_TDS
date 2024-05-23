using UnityEngine;

public class SoundManager : InitializableBehavior
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource _audioSource;

    public override void Initialize()
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
    }

    public void PlaySound(AudioClip clip, bool randomizePitch = true, bool randomizeVolume = true)
    {
        if (clip == null) return;
        if (_audioSource.IsPlaying())
        {
            if (_audioSource.GetTimePercent() < 0.5f) return;
        }

        _audioSource.Stop();
        _audioSource.clip = clip;
        if (randomizePitch)
        {
            _audioSource.pitch = Random.Range(0.9f, 1.1f);
        }
        else
        {
            _audioSource.pitch = 1f;
        }

        if (randomizeVolume)
        {
            _audioSource.volume = Random.Range(0.9f, 1f);
        }
        else
        {
            _audioSource.volume = 1f;
        }
        _audioSource.Play();
    }
}
