using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 5f;

    private Vector3 _offset;

    private void Awake()
    {
        InitAxis();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position + _offset, _smoothSpeed * Time.fixedDeltaTime);
    }

    private void InitAxis()
    {
        if (transform.parent == null) return;

        _offset = transform.localPosition;
        transform.SetParent(null);
    }
}
