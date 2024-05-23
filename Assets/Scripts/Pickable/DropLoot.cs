using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Loot
{
    public GameObject prefab;
    public float chance;
}

public class DropLoot : MonoBehaviour
{
    [SerializeField] private Transform _createAtParent;
    [SerializeField] private float _randomizePosition;
    [SerializeField] private List<Loot> _loot = new();

    public void Drop()
    {
        for (int i = 0; i < _loot.Count; i++)
        {
            float chance = Random.Range(0f, 100f);
            if (chance > _loot[i].chance) continue;

            CreateLoot(i);
        }
    }

    private GameObject CreateLoot(int index)
    {
        Vector3 position = Random.onUnitSphere * _randomizePosition;
        position += transform.position;
        position.y = transform.position.y;
        return Instantiate(_loot[index].prefab, position, transform.rotation, _createAtParent);
    }
}
