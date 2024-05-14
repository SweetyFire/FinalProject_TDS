using UnityEngine;

public class OpeningDoor : MonoBehaviour
{
    [SerializeField] private GameObject _doorObject;
    [SerializeField] private float _openingSpeed = 0.5f;
    [SerializeField] private float _rotationAngle = 90f;
    [SerializeField] private LeanTweenType _animationType;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CreatureController _)) return;

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
        if (!other.TryGetComponent(out CreatureController _)) return;
        Close();
    }

    private void Open(bool mirror = false)
    {
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
