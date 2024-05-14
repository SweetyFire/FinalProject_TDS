using System.Collections.Generic;
using UnityEngine;

public class CameraHiddingObjects : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _distanceOffset = 1f;
    [SerializeField] private Vector3 _localCastPosition = Vector3.up;
    [SerializeField] private float _sphereRadius = 0.5f;

    public Vector3 DirectionToZero => (_localCastPosition - transform.localPosition).normalized;
    public float DistanceToZero => Vector3.Distance(transform.localPosition, _localCastPosition) - _distanceOffset;

    private List<MeshRenderer> _hiddenObjectsKeys = new();
    private List<bool> _hiddenObjectsValues = new();
    private RaycastHit[] hits = new RaycastHit[50];

    private void FixedUpdate()
    {
        HideObjects();
    }

    private void HideObjects()
    {
        for (int i = _hiddenObjectsValues.Count - 1; i >= 0; i--)
        {
            _hiddenObjectsValues[i] = false;
        }

        if (Physics.SphereCastNonAlloc(transform.position, _sphereRadius, DirectionToZero, hits, DistanceToZero, _groundMask) > 0)
        {
            for (int i = hits.Length - 1; i >= 0; i--)
            {
                if (hits[i].collider == null) continue;
                if (!hits[i].collider.TryGetComponent(out MeshRenderer renderer)) return;
                int index = _hiddenObjectsKeys.IndexOf(renderer);
                if (index >= 0)
                {
                    _hiddenObjectsValues[index] = true;
                }
                else
                {
                    _hiddenObjectsKeys.Add(renderer);
                    _hiddenObjectsValues.Add(true);
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                }
            }
        }

        for (int i = _hiddenObjectsValues.Count - 1; i >= 0; i--)
        {
            if (_hiddenObjectsValues[i]) continue;

            _hiddenObjectsKeys[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            _hiddenObjectsKeys.RemoveAt(i);
            _hiddenObjectsValues.RemoveAt(i);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = DirectionToZero * DistanceToZero;
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.DrawWireSphere(direction + transform.position, _sphereRadius);
    }
#endif
}
