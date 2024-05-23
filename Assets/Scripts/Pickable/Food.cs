using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class FoodMesh
{
    public GameObject prefab;
    public float value;
}

public class Food : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _delayColliderEnable = 1f;
    [SerializeField] private List<FoodMesh> _food = new();

    private float _value;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        EnableColliderUpdate();
    }

    public void Eat(CreatureHealth creature)
    {
        creature.TakeDamage(-_value);
    }

    private void Init()
    {
        if (_collider.enabled)
        {
            Disable();
        }

        int index = Random.Range(0, _food.Count);
        _meshFilter.mesh = _food[index].prefab.GetComponent<MeshFilter>().sharedMesh;
        _meshFilter.transform.localScale = _food[index].prefab.transform.localScale;
        _meshFilter.transform.localRotation = _food[index].prefab.transform.localRotation;

        _value = _food[index].value;
    }

    private void EnableColliderUpdate()
    {
        if (_delayColliderEnable > 0f)
        {
            _delayColliderEnable -= Time.deltaTime;
            return;
        }

        _collider.enabled = true;
        Disable();
    }

    private void Disable()
    {
        _delayColliderEnable = 0f;
        enabled = false;
    }
}
