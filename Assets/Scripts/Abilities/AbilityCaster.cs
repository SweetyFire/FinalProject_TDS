using System.Collections.Generic;
using UnityEngine;

public class AbilityCaster : MonoBehaviour
{
    [SerializeField] private Transform _abilitiesParent;
    [SerializeField] private List<Ability> _abilitiyPrefabs;
    
    public CreatureController Controller => _controller;
    public IReadOnlyList<Ability> Abilities => _abilities;

    private List<Ability> _abilities = new();
    private CreatureController _controller;

    public void Init(CreatureController controller)
    {
        _controller = controller;

        foreach (Ability ability in _abilitiyPrefabs)
        {
            Ability abil = Instantiate(ability, _abilitiesParent);
            _abilities.Add(abil);
            abil.Init(this);
        }
    }

    public void ActivateAbility(int index)
    {
        _abilities[index].Activate();
    }
}
