using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DesiredOrderPuzzle : SavedObject
{
    private const string IS_CORRECT_NAME = "isCorrect";

    public UnityEvent onCorrect;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _correctSounds = new();

    private List<ToggleObject> _toggleObjects = new();
    private bool _isCorrect;

    public override void Save(int id, int sceneId)
    {
        string saveName = GetSaveValueName(id, sceneId, IS_CORRECT_NAME);
        SaveSystem.SetValue(saveName, _isCorrect);
    }

    public override void Load(int id, int sceneId)
    {
        string saveName = GetSaveValueName(id, sceneId, IS_CORRECT_NAME);
        if (!SaveSystem.TryGetBool(saveName, out bool value)) return;

        _isCorrect = value;
        if (_isCorrect)
        {
            onCorrect?.Invoke();
            DeactivateToggleObjects();
        }
    }

    protected override void Init()
    {
        _toggleObjects.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent(out ToggleObject obj)) continue;

            obj.onToggle.RemoveListener(OnObjectToggle);
            _toggleObjects.Add(obj);
            obj.onToggle.AddListener(OnObjectToggle);
            obj.Init();
        }
    }

    public void OnObjectToggle(ToggleObject toggleObject)
    {
        if (_isCorrect) return;

        for (int i = 0; i < _toggleObjects.Count; i++)
        {
            if (_toggleObjects[i].Enabled != _toggleObjects[i].CorrectValue) return;
        }

        _isCorrect = true;
        onCorrect?.Invoke();
        DeactivateToggleObjects();
        PlaySound();

        Saver.Save(this);
        GameLoader.Instance.SaveWithoutInvoke();
    }

    private void PlaySound()
    {
        _audioSource.Stop();
        _audioSource.clip = _correctSounds.GetRandom();
        _audioSource.SetRandomPitchAndVolume(0.9f, 1.1f, 0.6f, 0.7f);
        _audioSource.Play();
    }

    private void DeactivateToggleObjects()
    {
        for (int i = 0; i < _toggleObjects.Count; i++)
        {
            _toggleObjects[i].Toggle(_toggleObjects[i].CorrectValue);
            _toggleObjects[i].Deactivate();
        }
    }
}