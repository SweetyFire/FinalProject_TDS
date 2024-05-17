using UnityEngine;

public static class AudioSourceExttensions
{
    public static bool IsReversePitch(this AudioSource source)
    {
        return source.pitch < 0f;
    }

    public static float GetClipRemainingTime(this AudioSource source)
    {
        float remainingTime = (source.clip.length - source.time) / source.pitch;
        return source.IsReversePitch() ? (source.clip.length + remainingTime) : remainingTime;
    }

    public static float GetClipDuration(this AudioSource source)
    {
        return source.clip.length / source.pitch;
    }

    public static float GetClipDuration(this AudioSource source, AudioClip clip)
    {
        return clip.length / source.pitch;
    }

    public static void SetRandomPitch(this AudioSource source, float minPitch, float maxPitch)
    {
        SetRandomPitchAndVolume(source, minPitch, maxPitch, source.volume, source.volume);
    }

    public static void SetRandomVolume(this AudioSource source, float minVolume, float maxVolume)
    {
        SetRandomPitchAndVolume(source, source.pitch, source.pitch, minVolume, maxVolume);
    }

    public static void SetRandomPitchAndVolume(this AudioSource source, float minPitch, float maxPitch, float minVolume, float maxVolume)
    {
        source.pitch = Random.Range(minPitch, maxPitch);
        source.volume = Random.Range(minVolume, maxVolume);
    }

    public static bool IsPlaying(this AudioSource source)
    {
        return source.isPlaying || (source.clip != null && source.GetClipRemainingTime() <= 0f);
    }

    public static float GetTimePercent(this AudioSource source)
    {
        return source.time * Mathf.Abs(source.pitch) / source.GetClipDuration();
    }
}
