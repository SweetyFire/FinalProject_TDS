using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private float _minTimeBetweenMusic;
    [SerializeField] private float _maxTimeBetweenMusic;
    [SerializeField] private bool _isLooping = true;
    [SerializeField] private List<AudioClip> _music = new();

    private List<AudioClip> _musicQueue = new();
    private int _currentClipIndex;
    private bool _isPlaying;
    private float _timeForNextMusic;

    private void Update()
    {
        MusicUpdate();
    }

    public void StartMusic()
    {
        SetQueue();
        Play();
    }

    private void SetQueue()
    {
        _musicQueue.Clear();
        _musicQueue.AddRange(_music);
        _musicQueue.Shuffle();
    }

    private void Play()
    {
        SetTimeForNextMusic();
        _musicSource.clip = _musicQueue[_currentClipIndex];
        _isPlaying = true;
        _musicSource.Play();
    }

    private void MusicUpdate()
    {
        if (!_isPlaying) return;
        if (_musicSource.IsPlaying()) return;

        if (_timeForNextMusic > 0f)
        {
            _timeForNextMusic -= Time.deltaTime;
            return;
        }

        _currentClipIndex++;

        if (_currentClipIndex >= _musicQueue.Count)
        {
            if (_isLooping)
            {
                _musicQueue.Shuffle();
                _currentClipIndex = 0;
            }
            else
            {
                _isPlaying = false;
            }
        }

        Play();
    }

    private void SetTimeForNextMusic()
    {
        _timeForNextMusic = Random.Range(_minTimeBetweenMusic, _maxTimeBetweenMusic);
    }
}
