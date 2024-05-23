using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool IsGrounded => _isGrounded;
    private bool _isGrounded;
    private List<Collider> _overlapColliders = new();

    private void OnTriggerEnter(Collider other)
    {
        _isGrounded = true;
        _overlapColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _overlapColliders.Remove(other);
        if (_overlapColliders.Count > 0) return;

        _isGrounded = false;
    }
}
