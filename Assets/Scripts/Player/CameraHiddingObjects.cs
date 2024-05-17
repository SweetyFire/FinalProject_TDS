using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshWithCollider
{
    public MeshRenderer meshRenderer;
    public int defaultLayer;
    public bool enabled;
}

public class CameraHiddingObjects : MonoBehaviour
{
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _distanceOffset = 1f;
    [SerializeField] private Vector3 _localCastPosition = Vector3.up;
    [SerializeField] private float _sphereRadius = 0.5f;

    public Vector3 DirectionToZero => (_localCastPosition - transform.localPosition).normalized;
    public float DistanceToZero => Vector3.Distance(transform.localPosition, _localCastPosition) - _distanceOffset;

    private List<MeshWithCollider> _hiddenObjects = new();
    private Collider[] _overlapColliders = new Collider[50];

    private void FixedUpdate()
    {
        HideObjects();
    }

    private void HideObjects()
    {
        foreach (MeshWithCollider mesh in _hiddenObjects)
        {
            mesh.enabled = false;
        }

        Vector3 point2 = DirectionToZero * DistanceToZero + transform.position;
        if (Physics.OverlapCapsuleNonAlloc(transform.position, point2, _sphereRadius, _overlapColliders, _groundMask) <= 0)
        {
            for (int i = 0; i < _overlapColliders.Length; i++)
            {
                if (_overlapColliders[i] == null) continue;
                _overlapColliders[i] = null;
            }
        }

        for (int i = _overlapColliders.Length - 1; i >= 0; i--)
        {
            if (_overlapColliders[i] == null) continue;
            if (!_overlapColliders[i].TryGetComponent(out MeshRenderer renderer)) continue;
            int index = _hiddenObjects.FindIndex(m => m.meshRenderer == renderer);
            if (index >= 0)
            {
                _hiddenObjects[index].enabled = true;
            }
            else
            {
                index = _hiddenObjects.FindIndex(e => e.meshRenderer == null);
                if (index < 0)
                {
                    if (_hiddenObjects.Count < _overlapColliders.Length)
                    {
                        index = _hiddenObjects.Count;
                        _hiddenObjects.Add(new MeshWithCollider());
                    }
                    else
                    {
                        Debug.LogError("List of hidden objects is crowded!");
                        continue;
                    }
                }

                _hiddenObjects[index].meshRenderer = renderer;
                _hiddenObjects[index].defaultLayer = renderer.gameObject.layer;
                _hiddenObjects[index].enabled = true;

                _hiddenObjects[index].meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                if (_overlapColliders[i].isTrigger) continue;
                _hiddenObjects[index].meshRenderer.gameObject.layer = Physics.IgnoreRaycastLayer;

            }
        }

        foreach (MeshWithCollider mesh in _hiddenObjects)
        {
            if (mesh.meshRenderer == null) continue;
            if (mesh.enabled) continue;

            mesh.meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            mesh.meshRenderer.gameObject.layer = mesh.defaultLayer;

            mesh.meshRenderer = null;
            mesh.defaultLayer = 0;
            mesh.enabled = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = DirectionToZero * DistanceToZero;
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.DrawWireSphere(direction + transform.position, _sphereRadius);
        Gizmos.DrawWireSphere(transform.position, _sphereRadius);
    }
#endif
}
