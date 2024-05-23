using System.Collections.Generic;
using UnityEngine;

public class OpeningDoor : MonoBehaviour
{
    [SerializeField] private GameObject _doorObject;
    [SerializeField] private float _openingSpeed = 0.5f;
    [SerializeField] private float _rotationAngle = 90f;
    [SerializeField] private LeanTweenType _animationType;
    [SerializeField] private bool _isLocked;

    private List<CreatureHealth> _overlappedCreatures = new();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CreatureHealth creature)) return;
        if (!creature.IsAlive) return;

        _overlappedCreatures.Add(creature);

        Vector3 direction = (other.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > 90f)
        {
            Open(true);
        }
        else
        {
            Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out CreatureHealth creature)) return;
        int findIndex = _overlappedCreatures.IndexOf(creature);
        if (findIndex >= 0)
        {
            _overlappedCreatures.RemoveAt(findIndex);

            for (int i = _overlappedCreatures.Count - 1; i >= 0; i--)
            {
                if (_overlappedCreatures[i] != null && _overlappedCreatures[i].IsAlive) continue;
                _overlappedCreatures.RemoveAt(i);
            }
        }

        if (_overlappedCreatures.Count > 0) return;
        Close();
    }

    public void Lock()
    {
        _isLocked = true;
    }

    public void Unlock()
    {
        _isLocked = false;
    }

    private void Open(bool mirror = false)
    {
        if (_isLocked) return;

        LeanTween.cancel(_doorObject);
        float angle = mirror ? _rotationAngle : -_rotationAngle;
        LeanTween.rotateLocal(_doorObject, new(0f, angle, 0f), _openingSpeed).setEase(_animationType);
    }

    private void Close()
    {
        LeanTween.cancel(_doorObject);
        LeanTween.rotateLocal(_doorObject, Vector3.zero, _openingSpeed).setEase(_animationType);
    }
}
