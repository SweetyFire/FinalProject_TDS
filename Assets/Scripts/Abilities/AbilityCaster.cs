using System.Collections.Generic;
using UnityEngine;

public class AbilityCaster : MonoBehaviour
{
    [SerializeField] private List<Ability> _abilitiyPrefabs;

    private List<Ability> _abilities;

    private void Awake()
    {
        foreach (Ability ability in _abilitiyPrefabs)
        {
            Ability abil = Instantiate(ability, transform);
            abil.Init(this);
        }
    }

    public void ActivateAbility(int index)
    {
        _abilities[index].Activate();
    }
}
